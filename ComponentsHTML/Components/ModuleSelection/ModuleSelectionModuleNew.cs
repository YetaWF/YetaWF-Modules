/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Internal component used by the ModuleSelection component. Not intended for application use.
    /// </summary>
    public class ModuleSelectionModuleNewEditComponent : YetaWFComponent, IYetaWFComponent<Guid?> {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleSelectionModuleNewEditComponent), name, defaultValue, parms); }

        internal const string TemplateName = "ModuleSelectionModuleNew";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {

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
            return await DropDownListComponent.RenderDropDownListAsync(this, model.ToString(), list, null);
        }

        internal static string RenderReplacementPackageModulesNew(string areaName) {
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
