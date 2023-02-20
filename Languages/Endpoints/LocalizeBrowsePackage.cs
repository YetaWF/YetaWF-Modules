/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.DataProvider;
using YetaWF.Modules.Languages.Modules;
using YetaWF.Modules.Languages.Support;

namespace YetaWF.Modules.Languages.Endpoints;

public class LocalizeBrowsePackageModuleEndpoints : YetaWFEndpoints {

    internal const string CreateCustomLocalization = "CreateCustomLocalization";
    internal const string CreateInstalledLocalization = "CreateInstalledLocalization";
    internal const string CreateAllInstalledLocalizations = "CreateAllInstalledLocalizations";
    internal const string LocalizePackageData = "LocalizePackageData";
    internal const string LocalizeAllPackagesData = "LocalizeAllPackagesData";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(LocalizeBrowsePackageModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LocalizeBrowsePackageModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData, string packageName) => {
            LocalizeBrowsePackageModule module = await GetModuleAsync<LocalizeBrowsePackageModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(package), gridPvData);
        });

        group.MapPost(CreateCustomLocalization, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string packageName, string language) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            await TranslatePackageAsync(packageName, language, Localization.Location.CustomResources);
            return Done(__ResStr("custGenerated", "Custom localization resources successfully generated"));
        })
            .ExcludeDemoMode();

        group.MapPost(CreateInstalledLocalization, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string packageName, string language) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            await TranslatePackageAsync(packageName, language, Localization.Location.InstalledResources);
            return Done(__ResStr("instGenerated", "Installed localization resources successfully generated"));
        })
            .ExcludeDemoMode();

        group.MapPost(CreateAllInstalledLocalizations, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string language) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage)
                    await TranslatePackageAsync(package.Name, language, Localization.Location.InstalledResources);
            }
            return Done(__ResStr("instGenerated", "Installed localization resources successfully generated"));
        })
            .ExcludeDemoMode();

        group.MapPost(LocalizePackageData, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string packageName, string language) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<Type> models = package.InstallableModels;
            foreach (Type type in models) {
                await LocalizeOneTypeAsync(type, language);
            }
            return Done(__ResStr("packDataGenerated", "Translated package data successfully generated"));
        })
            .ExcludeDemoMode();

        group.MapPost(LocalizeAllPackagesData, async (HttpContext context, [FromQuery] Guid __ModuleGuid, string language) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
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
            return Done(__ResStr("packAllDataGenerated", "Translated package data for all packages successfully generated"));
        })
            .ExcludeDemoMode();
    }

    private static async Task LocalizeOneTypeAsync(Type type, string language) {
        object instMod = Activator.CreateInstance(type)!;
        using ((IDisposable)instMod) {
            IInstallableModel model = (IInstallableModel)instMod;
            await model.LocalizeModelAsync(language, (t) => IsHtml(t), (t) => TranslateStringsAsync(language, t), (t) => TranslateComplexStringAsync(t, language, (c) => TranslateStringsAsync(language, c)));
        }
    }

    private static async Task TranslatePackageAsync(string packageName, string language, Localization.Location resourceType) {
        Package package = Package.GetPackageFromPackageName(packageName);
        if (resourceType == Localization.Location.InstalledResources && language == MultiString.DefaultLanguage)
            throw new InternalError("Can't save installed resources using the default language {0}", MultiString.DefaultLanguage);
        List<LocalizeBrowsePackageModule.LocalizeFile> files = (from s in await Localization.GetFilesAsync(package, MultiString.DefaultLanguage, false) select new LocalizeBrowsePackageModule.LocalizeFile { FileName = Path.GetFileName(s) }).ToList();

        // Extract all strings into a list
        List<string> strings = new List<string>();
        List<LocalizationData> allData = new List<LocalizationData>();
        foreach (LocalizeBrowsePackageModule.LocalizeFile file in files) {
            LocalizationData data = Localization.Load(package, file.FileName, Localization.Location.DefaultResources) ?? throw new InternalError($"Localization file {file.FileName} not found");
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

    private static async Task<List<string>> TranslateStringsAsync(string language, List<string> strings) {
        LocalizeConfigData config = await LocalizeConfigDataProvider.GetConfigAsync();
        if (config.TranslationService == LocalizeConfigData.TranslationServiceEnum.GoogleTranslate) {
            if (!string.IsNullOrWhiteSpace(config.GoogleTranslateAPIKey) && !string.IsNullOrWhiteSpace(config.GoogleTranslateAppName))
                return await TranslateStringsUsingGoogleAsync(language, config, strings);
        } else if (config.TranslationService == LocalizeConfigData.TranslationServiceEnum.MicrosoftTranslator) {
            if (!string.IsNullOrWhiteSpace(config.MSTextTranslationUrl) && !string.IsNullOrWhiteSpace(config.MSClientKey))
                return await TranslateStringsUsingMicrosoftAsync(language, config, strings);
        }
        throw new Error("No translation API available - Define a translation API using Localization Settings.");
    }
    private static async Task<List<string>> TranslateStringsUsingGoogleAsync(string language, LocalizeConfigData config, List<string> strings) {
        string from = MultiString.GetPrimaryLanguage(MultiString.DefaultLanguage);
        string to = MultiString.GetPrimaryLanguage(language);

        GoogleTranslate googleTrans = new GoogleTranslate(config.GoogleTranslateAppName!, config.GoogleTranslateAPIKey!);

        int total = strings.Count();
        int skip = 0;
        List<string> newStrings = new List<string>();
        while (total > 0) {
            List<string> returnedStrings = await googleTrans.TranslateAsync(from, to, strings.Skip(skip).Take(40).ToList());
            newStrings.AddRange(returnedStrings);
            skip += returnedStrings.Count;
            total -= returnedStrings.Count;
        }
        return newStrings;
    }
    private static async Task<List<string>> TranslateStringsUsingMicrosoftAsync(string language, LocalizeConfigData config, List<string> strings) {
        string from = MultiString.GetPrimaryLanguage(MultiString.DefaultLanguage);
        string to = MultiString.GetPrimaryLanguage(language);

        MSTranslate msTrans = new MSTranslate(config.MSTextTranslationUrl, config.MSTextTranslationRegion, config.MSClientKey!, config.MSRequestLimit);

        int total = strings.Count();
        int skip = 0;
        List<string> newStrings = new List<string>();
        while (total > 0) {
            List<string> returnedStrings = await msTrans.TranslateAsync(from, to, strings.Skip(skip).Take(40).ToList());
            newStrings.AddRange(returnedStrings);
            skip += returnedStrings.Count;
            total -= returnedStrings.Count;
        }
        return newStrings;
    }

    public class TextItem {
        public int Offset { get; set; }
        public int Length { get; set; }
        public string Text { get; set; } = null!;
    }

    private static async Task<string> TranslateComplexStringAsync(string text, string language, Func<List<string>, Task<List<string>>> translateStringsAsync) {
        List<TextItem> items = new List<TextItem>();
        int textIndex = 0;
        int index = 0;
        bool inHtml = false;
        bool inScript = false, inPre = false;
        for (; ; ) {
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

        if (items.Count == 0) return text;

        List<string> strings = (from i in items select i.Text).ToList();
        strings = await translateStringsAsync(strings);

        for (int i = strings.Count - 1; i >= 0; --i) {
            TextItem item = items[i];
            text = text.Substring(0, item.Offset) + strings[i] + text.Substring(item.Offset + item.Length);
            strings.RemoveAt(i);
        }
        return text;
    }

    private static bool IsHtml(string text) {
        text = text.Trim();
        if (text.StartsWith("<") && text.EndsWith(">")) return true; // seen <.....> so it is most likely html
        int gtIndex = text.IndexOf('>');
        if (gtIndex < 0) return false;
        int ltIndex = text.Substring(gtIndex).IndexOf('<');
        if (ltIndex < 0) return false;
        return true; // we've seen ..>....<.. so it is most likely html
    }
}
