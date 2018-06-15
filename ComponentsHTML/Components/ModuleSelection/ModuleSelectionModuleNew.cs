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

    public class ModuleSelectionModuleNewEditComponent : YetaWFComponent, IYetaWFComponent<Guid?> {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleSelectionModuleNewEditComponent), name, defaultValue, parms); }

        public const string TemplateName = "ModuleSelectionModuleNew";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(Guid? model) {

            string areaName = await GetAreaNameFromGuidAsync(true, model);
            model = model ?? Guid.Empty;
            List<SelectionItem<string>> list = new List<SelectionItem<string>>();
            if (!string.IsNullOrWhiteSpace(areaName)) {
                list = (
                    from module in InstalledModules.Modules
                    where module.Value.Package.AreaName == areaName
                    orderby module.Value.DisplayName.ToString() select
                        new SelectionItem<string> {
                            Text = module.Value.DisplayName.ToString(),
                            Value = module.Key.ToString(),
                            Tooltip = module.Value.Summary,
                        }).ToList<SelectionItem<string>>();
            }
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("none", "(none)"), Value = null });
            return await DropDownListComponent.RenderDropDownListAsync(this, model.ToString(), list, "yt_moduleselectionmodulenew");
        }

        internal static YHtmlString RenderReplacementPackageModulesNew(string areaName) {
            List<SelectionItem<string>> list = new List<SelectionItem<string>>();
            if (!string.IsNullOrWhiteSpace(areaName)) {
                list = (
                    from module in InstalledModules.Modules
                    where module.Value.Package.AreaName == areaName
                    orderby module.Value.DisplayName.ToString() select
                        new SelectionItem<string> {
                            Text = module.Value.DisplayName.ToString(),
                            Value = module.Key.ToString(),
                            Tooltip = module.Value.Summary,
                        }).ToList<SelectionItem<string>>();
            }
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("none", "(none)"), Value = null });
            return DropDownListEditComponentBase<string>.RenderDataSource(list, areaName);
        }

        internal static async Task<string> GetAreaNameFromGuidAsync(bool newMods, Guid? moduleGuid) {
            if (moduleGuid != null && moduleGuid != Guid.Empty) {
                if (newMods) {
                    InstalledModules.ModuleTypeEntry modEntry = InstalledModules.TryFindModuleEntry((Guid)moduleGuid);
                    if (modEntry != null)
                        return modEntry.Package.AreaName;
                    else
                        moduleGuid = null;
                } else {
                    return (from m in await DesignedModules.LoadDesignedModulesAsync() where m.ModuleGuid == (Guid)moduleGuid select m.AreaName).FirstOrDefault();
                }
            }
            return null;
        }
    }
}
