/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.IO;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeEditFileModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeEditFileModule> {

        public LocalizeEditFileModuleController() { }

        /*DO NOT [Trim]*/
        public class EditModel {

            [Caption("Current Language"), Description("Displays the current language used - Use User Settings to change the current language")]
            [UIHint("String"), ReadOnly]
            public string CurrentLanguage { get; set; } = null!;

            [Caption("Comment"), Description("An optional comment")]
            [UIHint("TextAreaSourceOnly"), StringLength(LocalizationData.MaxComment), Trim]
            public string? Comment { get; set; }

            [Caption("Classes"), Description("Shows all the localizable string fragments found in class properties and the class's Header, Footer and Legend attributes")]
            [UIHint("YetaWF_Languages_LocalizeClasses"), SuppressIf("ClassCount", 0)]
            public SerializableList<LocalizationData.ClassData> Classes { get; set; }
            [Caption("Enums"), Description("Shows all the localizable string fragments found in EnumDescription attributes")]
            [UIHint("YetaWF_Languages_LocalizeEnums"), SuppressIf("EnumCount", 0)]
            public SerializableList<LocalizationData.EnumData> Enums { get; set; }
            [Caption("Strings"), Description("Shows all the localizable string fragments found in __ResStr() calls")]
            [UIHint("YetaWF_Languages_LocalizeStrings"), SuppressIf("StringCount", 0)]
            public SerializableList<LocalizationData.StringData> Strings { get; set; }

            [Caption("Custom"), Description("Defines whether localization strings are saved as customizations (in ./LocalizationCustom) or as package installed resources (./Localization) - Only non US-English resources can be saved as package installed resources")]
            [UIHint("Boolean"), SuppressIf("ForceCustom", true)]
            public bool Custom { get; set; }

            [Caption("Custom"), Description("Defines whether localization strings are saved as customizations (in ./LocalizationCustom) or as package installed resources (./Localization) - Only non US-English resources can be saved as package installed resources")]
            [UIHint("Boolean"), SuppressIf("ForceCustom", false), ReadOnly]
            public bool CustomRO { get; set; }

            public int ClassCount { get { return Classes.Count; } }
            public int EnumCount { get { return Enums.Count; } }
            public int StringCount { get { return Strings.Count; } }

            [UIHint("Hidden")]
            public string PackageName { get; set; } = null!;
            [UIHint("Hidden")]
            public string TypeName { get; set; } = null!;

            [UIHint("Hidden")]
            public bool ForceCustom { get; set; }
            [UIHint("Hidden")]
            public string HiddenCurrentLanguage { get; set; } = null!;

            public LocalizationData GetData(LocalizationData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(LocalizationData data) {
                ObjectSupport.CopyData(data, this);
            }
            public EditModel() {
                Classes = new SerializableList<LocalizationData.ClassData>();
                Enums = new SerializableList<LocalizationData.EnumData>();
                Strings = new SerializableList<LocalizationData.StringData>();
            }
        }

        [AllowGet]
        public ActionResult LocalizeEditFile(string packageName, string typeName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            bool custom = true;
            bool forceCustom = false;
            LocalizationData? data = Localization.Load(package, typeName, Localization.Location.CustomResources);
            if (data == null) {
                data = Localization.Load(package, typeName, Localization.Location.InstalledResources);
                custom = false;
            }
            if (data == null) {
                data = Localization.Load(package, typeName, Localization.Location.DefaultResources);
                custom = false;
            }
            if (data == null)
                throw new InternalError($"No localization data available for package {package.Name}");

            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage)
                custom = true;
#if !DEBUG
            forceCustom = true;
#endif
            EditModel model = new EditModel {
                PackageName = packageName,
                TypeName = typeName,
                Custom = forceCustom || custom,
                CustomRO = forceCustom || custom,
                CurrentLanguage = MultiString.ActiveLanguage,
                HiddenCurrentLanguage = MultiString.ActiveLanguage,
                ForceCustom = forceCustom || MultiString.ActiveLanguage == MultiString.DefaultLanguage,
            };
            model.SetData(data);
            Module.Title = this.__ResStr("modTitle", "Localization Resource \"{0}\"", typeName);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult LocalizeEditFile_Partial(EditModel model, bool RestoreDefaults = false) {
            Package package = Package.GetPackageFromPackageName(model.PackageName);
            LocalizationData? data;
            if (RestoreDefaults) {
                data = Localization.Load(package, model.TypeName, Localization.Location.DefaultResources) ?? throw new InternalError($"Localization for {model.TypeName} not found");

                Localization.SaveAsync(package, model.TypeName, Localization.Location.InstalledResources, null);// delete it
                Localization.SaveAsync(package, model.TypeName, Localization.Location.CustomResources, null);// delete it

                ModelState.Clear();
                EditModel newModel = new EditModel {
                    PackageName = model.PackageName,
                    TypeName = model.TypeName,
                    Custom = model.ForceCustom || model.Custom,
                    CustomRO = model.ForceCustom || model.Custom,
                    CurrentLanguage = MultiString.ActiveLanguage,
                    HiddenCurrentLanguage = MultiString.ActiveLanguage,
                    ForceCustom = model.ForceCustom,
                };
                newModel.SetData(data); // and all the data back into model for final display
                return FormProcessed(newModel, this.__ResStr("okReset", "Localization resource default restored - The localization file has been removed - If you click Save or Apply, a new custom/installed addon file will be created. To keep the defaults only, simply close this form"), OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
            } else {
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = Localization.Load(package, model.TypeName, Localization.Location.CustomResources);
                if (data == null)
                    data = Localization.Load(package, model.TypeName, Localization.Location.InstalledResources);
                if (data == null)
                    data = Localization.Load(package, model.TypeName, Localization.Location.DefaultResources);
                if (data == null) 
                    throw new InternalError($"Localization for {model.TypeName} not found");
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                model.Custom = model.ForceCustom || model.Custom;
                model.CustomRO = model.ForceCustom || model.Custom;
                model.CurrentLanguage = model.HiddenCurrentLanguage;

                if (model.Custom)
                    Localization.SaveAsync(package, model.TypeName, Localization.Location.CustomResources, data);
                else
                    Localization.SaveAsync(package, model.TypeName, Localization.Location.InstalledResources, data);

                return FormProcessed(model, this.__ResStr("okSaved", "Localization resource saved"), OnPopupClose: OnPopupCloseEnum.ReloadNothing);
            }
        }
    }
}