/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ModuleSelectionPackageExistingEditComponent : YetaWFComponent, IYetaWFComponent<Guid?> {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleSelectionPackageExistingEditComponent), name, defaultValue, parms); }

        public const string TemplateName = "ModuleSelectionPackageExisting";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(Guid? model) {

            model = model ?? Guid.Empty;
            string areaName = await ModuleSelectionModuleNewEditComponent.GetAreaNameFromGuidAsync(false, model);
            List<SelectionItem<string>> list = (
                from p in InstalledModules.Packages orderby p.Name select
                    new SelectionItem<string> {
                        Text = __ResStr("package", "{0}", p.Name),
                        Value = p.AreaName,
                        Tooltip = __ResStr("packageTT", "{0} - {1}", p.Description.ToString(), p.CompanyDisplayName),
                    }).ToList<SelectionItem<string>>();
            list = (from l in list orderby l.Text select l).ToList();
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("selectPackage", "(select)"), Value = null });

            return await DropDownListComponent.RenderDropDownListAsync(this, areaName, list, "yt_moduleselectionpackageexisting");
        }
        internal static async Task<YHtmlString> RenderReplacementPackageModulesDesignedAsync(string areaName) {
            List<SelectionItem<string>> list = (
                from module in await DesignedModules.LoadDesignedModulesAsync()
                where module.AreaName == areaName
                orderby module.Name select
                    new SelectionItem<string> {
                        Text = module.Name,
                        Value = module.ModuleGuid.ToString(),
                        Tooltip = module.Description,
                    }).ToList<SelectionItem<string>>();
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("none", "(none)"), Value = null });
            return DropDownListEditComponentBase<string>.RenderDataSource(list, areaName);
        }
        internal static async Task<YHtmlString> RenderReplacementPackageModulesDesignedAsync(Guid modGuid) {
            string areaName = await ModuleSelectionModuleNewEditComponent.GetAreaNameFromGuidAsync(false, modGuid);
            List<SelectionItem<string>> list = (
                from module in await DesignedModules.LoadDesignedModulesAsync()
                where module.AreaName == areaName
                orderby module.Name select
                    new SelectionItem<string> {
                        Text = module.Name,
                        Value = module.ModuleGuid.ToString(),
                        Tooltip = module.Description,
                    }).ToList<SelectionItem<string>>();
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("none", "(none)"), Value = null });
            return DropDownListEditComponentBase<string>.RenderDataSource(list, areaName);
        }
    }
}
