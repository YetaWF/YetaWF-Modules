/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.Controllers.Support;
using YetaWF.Modules.Languages.DataProvider;
using YetaWF.Modules.Languages.Modules;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeBrowsePackageModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeBrowsePackageModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    LocalizeEditFileModule editMod = new LocalizeEditFileModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, PackageName, FileName), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("File Name"), Description("The name of the localization resource")]
            [UIHint("String"), ReadOnly]
            public string FileName { get; set; }

            public string PackageName { get; set; }

            private LocalizeBrowsePackageModule Module { get; set; }

            public BrowseItem(LocalizeBrowsePackageModule module, string packageName, string file) {
                Module = module;
                FileName = file;
                PackageName = packageName;
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }

            public class ExtraData {
                public string PackageName { get; set; }
            }
        }
        private GridDefinition GetGridModel(Package package) {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(LocalizeBrowsePackage_GridData)),
                ExtraData = new BrowseModel.ExtraData { PackageName = package.Name },
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    List<LocalizeFile> files = (from s in await Localization.GetFilesAsync(package, MultiString.DefaultLanguage, false) select new LocalizeFile { FileName = Path.GetFileName(s) }).ToList();
                    DataProviderGetRecords<LocalizeFile> recs = DataProviderImpl<LocalizeFile>.GetRecords(files, skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in recs.Data select new BrowseItem(Module, package.Name, s.FileName)).ToList<object>(),
                        Total = recs.Total
                    };
                },
            };
        }

        [AllowGet]
        public ActionResult LocalizeBrowsePackage(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(package)
            };
            Module.Title = this.__ResStr("modTitle", "Localization Resources - Package {0}", package.Name);
            return View(model);
        }

        public class LocalizeFile {
            public string FileName { get; set; }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> LocalizeBrowsePackage_GridData(GridPartialViewData gridPVData, string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            return await GridPartialViewAsync(GetGridModel(package), gridPVData);
        }

        [AllowPost]
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> CreateCustomLocalization(string packageName, string language) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            await TranslatePackageAsync(packageName, language, Localization.Location.CustomResources);
            return FormProcessed(null, popupText: this.__ResStr("custGenerated", "Custom localization resources successfully generated"), OnClose: OnCloseEnum.Nothing);
        }

        [AllowPost]
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> CreateInstalledLocalization(string packageName, string language) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            await TranslatePackageAsync(packageName, language, Localization.Location.InstalledResources);
            return FormProcessed(null, popupText: this.__ResStr("instGenerated", "Installed localization resources successfully generated"), OnClose: OnCloseEnum.Nothing);
        }

        [AllowPost]
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> CreateAllInstalledLocalizations(string language) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage)
                    await TranslatePackageAsync(package.Name, language, Localization.Location.InstalledResources);
            }
            return FormProcessed(null, popupText: this.__ResStr("instGenerated", "Installed localization resources successfully generated"), OnClose: OnCloseEnum.Nothing);
        }

        private async Task TranslatePackageAsync(string packageName, string language, Localization.Location resourceType) {
            Package package = Package.GetPackageFromPackageName(packageName);
            if (resourceType == Localization.Location.InstalledResources && language == MultiString.DefaultLanguage)
                throw new InternalError("Can't save installed resources using the default language {0}", MultiString.DefaultLanguage);
            List<LocalizeFile> files = (from s in await Localization.GetFilesAsync(package, MultiString.DefaultLanguage, false) select new LocalizeFile { FileName = Path.GetFileName(s) }).ToList();

            // Extract all strings into a list
            List<string> strings = new List<string>();
            List<LocalizationData> allData = new List<LocalizationData>();
            foreach (LocalizeFile file in files) {
                LocalizationData data = Localization.Load(package, file.FileName, Localization.Location.DefaultResources);
                allData.Add(data);
                foreach (LocalizationData.ClassData cd in data.Classes) {
                    if (!string.IsNullOrWhiteSpace(cd.Header))
                        strings.Add(cd.Header);
                    if (!string.IsNullOrWhiteSpace(cd.Footer))
                        strings.Add(cd.Footer);
                    if (!string.IsNullOrWhiteSpace(cd.Legend))
                        strings.Add(cd.Legend);
                    foreach (var entry in cd.Categories)
                        strings.Add(entry.Value);
                    foreach (LocalizationData.PropertyData pd in cd.Properties) {
                        if (!string.IsNullOrWhiteSpace(pd.Caption))
                            strings.Add(pd.Caption);
                        if (!string.IsNullOrWhiteSpace(pd.Description))
                            strings.Add(pd.Description);
                        if (!string.IsNullOrWhiteSpace(pd.TextAbove))
                            strings.Add(pd.TextAbove);
                        if (!string.IsNullOrWhiteSpace(pd.TextBelow))
                            strings.Add(pd.TextBelow);
                    }
                }
                foreach (LocalizationData.StringData sd in data.Strings) {
                    if (!string.IsNullOrWhiteSpace(sd.Text))
                        strings.Add(sd.Text);
                }
                foreach (LocalizationData.EnumData ed in data.Enums) {
                    foreach (LocalizationData.EnumDataEntry ede in ed.Entries) {
                        if (!string.IsNullOrWhiteSpace(ede.Caption))
                            strings.Add(ede.Caption);
                        if (!string.IsNullOrWhiteSpace(ede.Description))
                            strings.Add(ede.Description);
                    }
                }
            }
            // Translate all strings
            strings = await TranslateStringsAsync(language, strings);

            // put new strings into localization resource
            int stringIndex = 0, index = 0;
            foreach (LocalizationData locData in allData) {
                LocalizationData data = allData[index];
                data.Comment = "***WARNING*** Automated Translation - Not Usable In Its Present Form - Must Be Corrected";
                foreach (LocalizationData.ClassData cd in data.Classes) {
                    if (!string.IsNullOrWhiteSpace(cd.Header))
                        cd.Header = strings[stringIndex++];
                    if (!string.IsNullOrWhiteSpace(cd.Footer))
                        cd.Footer = strings[stringIndex++];
                    if (!string.IsNullOrWhiteSpace(cd.Legend))
                        cd.Legend = strings[stringIndex++];
                    SerializableDictionary<string, string> newCats = new SerializableDictionary<string, string>();
                    foreach (var entry in cd.Categories)
                        newCats.Add(entry.Key, strings[stringIndex++]);
                    cd.Categories = newCats;
                    foreach (LocalizationData.PropertyData pd in cd.Properties) {
                        if (!string.IsNullOrWhiteSpace(pd.Caption))
                            pd.Caption = strings[stringIndex++];
                        if (!string.IsNullOrWhiteSpace(pd.Description))
                            pd.Description = strings[stringIndex++];
                        if (!string.IsNullOrWhiteSpace(pd.TextAbove))
                            pd.TextAbove = strings[stringIndex++];
                        if (!string.IsNullOrWhiteSpace(pd.TextBelow))
                            pd.TextBelow = strings[stringIndex++];
                    }
                }
                foreach (LocalizationData.StringData sd in data.Strings) {
                    if (!string.IsNullOrWhiteSpace(sd.Text))
                        sd.Text = strings[stringIndex++];
                }
                foreach (LocalizationData.EnumData ed in data.Enums) {
                    foreach (LocalizationData.EnumDataEntry ede in ed.Entries) {
                        if (!string.IsNullOrWhiteSpace(ede.Caption))
                            ede.Caption = strings[stringIndex++];
                        if (!string.IsNullOrWhiteSpace(ede.Description))
                            ede.Description = strings[stringIndex++];
                    }
                }
                await Localization.SaveAsync(package, files[index].FileName, resourceType, locData);

                ++index;
            }
        }

        private async Task<List<string>> TranslateStringsAsync(string language, List<string> strings) {
            LocalizeConfigData config = await LocalizeConfigDataProvider.GetConfigAsync();
            if (config.TranslationService == LocalizeConfigData.TranslationServiceEnum.GoogleTranslate) {
                if (!string.IsNullOrWhiteSpace(config.GoogleTranslateAPIKey) && !string.IsNullOrWhiteSpace(config.GoogleTranslateAppName))
                    return await TranslateStringsUsingGoogleAsync(language, config.GoogleTranslateAPIKey, config.GoogleTranslateAppName, strings);
            } else if (config.TranslationService == LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator) {
                if (!string.IsNullOrWhiteSpace(config.MSClientKey))
                    return await TranslateStringsUsingMicrosoftAsync(language, config.MSClientKey, strings);
            }
            throw new InternalError("No translation API available - Define a translation API using Localization Settings");
        }
        private async Task<List<string>> TranslateStringsUsingGoogleAsync(string language, string apiKey, string appName, List<string> strings) {
            string from = MultiString.GetPrimaryLanguage(MultiString.DefaultLanguage);
            string to = MultiString.GetPrimaryLanguage(language);
            List<string> newStrings = new List<string>();
            int total = strings.Count();
            int skip = 0;
            while (total > 0) {
                TranslateService service = new TranslateService(new BaseClientService.Initializer() {
                    ApiKey = apiKey,
                    ApplicationName = appName,
                });
                TranslationsListResponse resp = await service.Translations.List(strings.Skip(skip).Take(40).ToList(), to).ExecuteAsync();
                List<string> returnedStrings = (from r in resp.Translations select r.TranslatedText).ToList();
                returnedStrings = (from r in returnedStrings select Utility.HtmlDecode(r)).ToList();
                newStrings.AddRange(returnedStrings);
                skip += returnedStrings.Count();
                total -= returnedStrings.Count();
            }
            return newStrings;
        }
        private async Task<List<string>> TranslateStringsUsingMicrosoftAsync(string language, string clientId, List<string> strings) {
            string from = MultiString.GetPrimaryLanguage(MultiString.DefaultLanguage);
            string to = MultiString.GetPrimaryLanguage(language);

            MSTranslate msTrans = new MSTranslate(clientId);

            int total = strings.Count();
            int skip = 0;
            List<string> newStrings = new List<string>();
            while (total > 0) {
                List<string> returnedStrings = await msTrans.TranslateAsync(from, to, strings.Skip(skip).Take(40).ToList());
                newStrings.AddRange(returnedStrings);
                skip += returnedStrings.Count();
                total -= returnedStrings.Count();
            }
            return newStrings;
        }

        public class TextItem {
            public int Offset { get; set; }
            public int Length { get; set; }
            public string Text { get; set; }
        }

        public async Task<string> TranslateComplexStringAsync(string text, string language, Func<List<string>, Task<List<string>>> translateStringsAsync) {
            List<TextItem> items = new List<TextItem>();
            int textIndex = 0;
            int index = 0;
            bool inHtml = false;
            bool inScript = false, inPre = false;
            for (;;) {
                if (index >= text.Length) {
                    if (!inHtml) {
                        string s = text.Substring(textIndex, index - textIndex);
                        s = s.Replace("\r", " ");
                        s = s.Replace("\n", " ");
                        s = s.Replace("\t", " ");
                        if (s.Trim().Length > 0)
                            items.Add(new TextItem { Offset = textIndex, Length = index - textIndex, Text = s });
                        break;
                    }
                }
                if (!inHtml) {
                    // in text
                    if (text[index] == '<') {
                        if (text.Substring(index).StartsWith("<script")) {
                            inScript = true;
                        } else if (text.Substring(index).StartsWith("<pre")) {
                            inPre = true;
                        } else {
                            // start of html
                            if (textIndex < index) {
                                string s = text.Substring(textIndex, index - textIndex);
                                s = s.Replace("\r", " ");
                                s = s.Replace("\n", " ");
                                s = s.Replace("\t", " ");
                                if (s.Trim().Length > 0)
                                    items.Add(new TextItem { Offset = textIndex, Length = index - textIndex, Text = s });
                            }
                        }
                        inHtml = true;
                    }
                } else if (inScript) {
                    if (text[index] == '<') {
                        if (text.Substring(index).StartsWith("</script"))
                            inScript = false;
                    }
                } else if (inPre) {
                    if (text[index] == '<') {
                        if (text.Substring(index).StartsWith("</pre"))
                            inPre = false;
                    }
                } else {
                    // in html
                    if (text[index] == '>')
                        inHtml = false;
                    textIndex = index + 1;
                }
                ++index;
            }

            if (items.Count == 0) return "?";

            List<string> strings = (from i in items select i.Text).ToList();
            strings = await translateStringsAsync(strings);

            for (int i = strings.Count - 1; i >= 0; --i) {
                TextItem item = items[i];
                text = text.Substring(0, item.Offset) + strings[i] + text.Substring(item.Offset + item.Length);
                strings.RemoveAt(i);
            }
            return text;
        }

        public bool IsHtml(string text) {
            text = text.Trim();
            if (text.StartsWith("<") && text.EndsWith(">")) return true; // seen <.....> so it is most likely html
            int gtIndex = text.IndexOf('>');
            if (gtIndex < 0) return false;
            int ltIndex = text.Substring(gtIndex).IndexOf('<');
            if (ltIndex < 0) return false;
            return true; // we've seen ..>....<.. so it is most likely html
        }

        [AllowPost]
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> LocalizePackageData(string packageName, string language) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<Type> models = package.InstallableModels;
            foreach (Type type in models) {
                await LocalizeOneTypeAsync(type, language);
            }
            return FormProcessed(null, popupText: this.__ResStr("packDataGenerated", "Translated package data successfully generated"), OnClose: OnCloseEnum.Nothing);
        }
        private async Task LocalizeOneTypeAsync(Type type, string language) {
            object instMod = Activator.CreateInstance(type);
            using ((IDisposable)instMod) {
                IInstallableModel model = (IInstallableModel)instMod;
                await model.LocalizeModelAsync(language, (t) => IsHtml(t), (t) => TranslateStringsAsync(language, t), (t) => TranslateComplexStringAsync(t, language, (c) => TranslateStringsAsync(language, c)));
            }
        }
        [AllowPost]
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> LocalizeAllPackagesData(string language) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage) {
                    List<Type> models = package.InstallableModels;
                    foreach (Type type in models) {
                        await LocalizeOneTypeAsync(type, language);
                    }
                }
            }
            return FormProcessed(null, popupText: this.__ResStr("packAllDataGenerated", "Translated package data for all packages successfully generated"), OnClose: OnCloseEnum.Nothing);
        }
    }
}