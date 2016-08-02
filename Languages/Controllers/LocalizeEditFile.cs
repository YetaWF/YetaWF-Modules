/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

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

            [UIHint("Hidden")]
            public string PackageName { get; set; }
            [UIHint("Hidden")]
            public string TypeName { get; set; }

            [Caption("Classes"), Description("Shows all the localizable string fragments found in class properties and the class's Header, Footer and Legend attributes")]
            [UIHint("YetaWF_Languages_LocalizeClasses"), SuppressIfEqual("ClassCount", 0)]
            public SerializableList<LocalizationData.ClassData> Classes { get; set; }
            [Caption("Enums"), Description("Shows all the localizable string fragments found in EnumDescription attributes")]
            [UIHint("YetaWF_Languages_LocalizeEnums"), SuppressIfEqual("EnumCount", 0)]
            public SerializableList<LocalizationData.EnumData> Enums { get; set; }
            [Caption("Strings"), Description("Shows all the localizable string fragments found in __ResStr() calls")]
            [UIHint("YetaWF_Languages_LocalizeStrings"), SuppressIfEqual("StringCount", 0)]
            public SerializableList<LocalizationData.StringData> Strings { get; set; }

            public int ClassCount { get { return Classes.Count; } }
            public int EnumCount { get { return Enums.Count; } }
            public int StringCount { get { return Strings.Count; } }

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
            LocalizationData data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.CustomResources);
            if (data == null)
                data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.InstalledResources);
            if (data == null)
                data = LocalizationSupport.Load(package, typeName, LocalizationSupport.Location.DefaultResources);
            EditModel model = new EditModel { PackageName = packageName, TypeName = typeName };
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
                model = new EditModel { PackageName = model.PackageName, TypeName = model.TypeName };
                data = LocalizationSupport.Load(package, model.TypeName, LocalizationSupport.Location.DefaultResources);
                model.SetData(data); // and all the data back into model for final display
                LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.CustomResources, null);// delete it
                return FormProcessed(model, this.__ResStr("okReset", "Localization resource default restored - The custom addon file has been removed. If you click Save or Apply, a new custom addon file will be created. To keep the defaults only, simply close this form."), OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace);
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
                LocalizationSupport.Save(package, model.TypeName, LocalizationSupport.Location.CustomResources, data);
                return FormProcessed(model, this.__ResStr("okSaved", "Localization resource saved"), OnPopupClose: OnPopupCloseEnum.ReloadNothing);
            }
        }
    }
}