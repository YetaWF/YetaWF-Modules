/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeEditFileModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeEditFileModule> {

        public LocalizeEditFileModuleController() { }

        /*DO NOT [Trim]*/
        public class EditModel {

            [Caption("Current Language"), Description("Displays the current language used - Use User Settings to change the current language")]
            [UIHint("String"), ReadOnly]
            public string CurrentLanguage { get; set; }

            [Caption("Comment"), Description("An optional comment")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), AllowHtml, StringLength(LocalizationData.MaxComment), Trim]
            public string Comment { get; set; }

            [Caption("Classes"), Description("Shows all the localizable string fragments found in class properties and the class's Header, Footer and Legend attributes")]
            [UIHint("YetaWF_Languages_LocalizeClasses"), SuppressIfEqual("ClassCount", 0)]
            public SerializableList<LocalizationData.ClassData> Classes { get; set; }
            [Caption("Enums"), Description("Shows all the localizable string fragments found in EnumDescription attributes")]
            [UIHint("YetaWF_Languages_LocalizeEnums"), SuppressIfEqual("EnumCount", 0)]
            public SerializableList<LocalizationData.EnumData> Enums { get; set; }
            [Caption("Strings"), Description("Shows all the localizable string fragments found in __ResStr() calls")]
            [UIHint("YetaWF_Languages_LocalizeStrings"), SuppressIfEqual("StringCount", 0)]
            public SerializableList<LocalizationData.StringData> Strings { get; set; }

            [Caption("Custom"), Description("Defines whether localization strings are saved as customizations (in ./AddonsCustom) or as package installed resources (./Addons) - Only non US-English resources can be saved as package installed resources")]
            [UIHint("Boolean"), SuppressIfEqual("ForceCustom", true)]
            public bool Custom { get; set; }

            [Caption("Custom"), Description("Defines whether localization strings are saved as customizations (in ./AddonsCustom) or as package installed resources (./Addons) - Only non US-English resources can be saved as package installed resources")]
            [UIHint("Boolean"), SuppressIfEqual("ForceCustom", false), ReadOnly]
            public bool CustomRO { get; set; }

            public int ClassCount { get { return Classes.Count; } }
            public int EnumCount { get { return Enums.Count; } }
            public int StringCount { get { return Strings.Count; } }

            [UIHint("Hidden")]
            public string PackageName { get; set; }
            [UIHint("Hidden")]
            public string TypeName { get; set; }

            [UIHint("Hidden")]
            public bool ForceCustom { get; set; }
            [UIHint("Hidden")]
            public string HiddenCurrentLanguage { get; set; }

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

        [HttpGet]
        public ActionResult LocalizeEditFile(string packageName, string typeName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            bool custom = true;
            bool forceCustom = false;
            LocalizationData data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.CustomResources);
            if (data == null) {
                data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.InstalledResources);
                custom = false;
            }
            if (data == null) {
                data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.DefaultResources);
                custom = false;
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult LocalizeEditFile_Partial(EditModel model, bool RestoreDefaults = false) {
            Package package = Package.GetPackageFromPackageName(model.PackageName);
            LocalizationData data = null;
            if (RestoreDefaults) {
                ModelState.Clear();
                model = new EditModel {
                    PackageName = model.PackageName,
                    TypeName = model.TypeName,
                    Custom = model.ForceCustom || model.Custom,
                    CustomRO = model.ForceCustom || model.Custom,
                    CurrentLanguage = model.HiddenCurrentLanguage,
                };
                data = LocalizationSupport.Load(package, model.TypeName, LocalizationSupport.Location.DefaultResources);
                model.SetData(data); // and all the data back into model for final display
                LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.InstalledResources, null);// delete it
                LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.CustomResources, null);// delete it
                return FormProcessed(model, this.__ResStr("okReset", "Localization resource default restored - The localization file has been removed - If you click Save or Apply, a new custom/installed addon file will be created. To keep the defaults only, simply close this form"), OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
            } else {
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = LocalizationSupport.Load(package, model.TypeName, LocalizationSupport.Location.CustomResources);
                if (data == null)
                    data = LocalizationSupport.Load(package, model.TypeName, LocalizationSupport.Location.InstalledResources);
                if (data == null)
                    data = LocalizationSupport.Load(package, model.TypeName, LocalizationSupport.Location.DefaultResources);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                model.Custom = model.ForceCustom || model.Custom;
                model.CustomRO = model.ForceCustom || model.Custom;
                model.CurrentLanguage = model.HiddenCurrentLanguage;

                if (model.Custom)
                    LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.CustomResources, data);
                else
                    LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.InstalledResources, data);

                return FormProcessed(model, this.__ResStr("okSaved", "Localization resource saved"), OnPopupClose: OnPopupCloseEnum.ReloadNothing);
            }
        }
    }
}