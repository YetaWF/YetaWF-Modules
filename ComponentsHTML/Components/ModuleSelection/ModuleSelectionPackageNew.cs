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

    public class ModuleSelectionPackageNewEditComponent : YetaWFComponent, IYetaWFComponent<Guid> {

        public const string TemplateName = "ModuleSelectionPackageNew";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(Guid model) {

            string areaName = await ModuleSelectionModuleNewEditComponent.GetAreaNameFromGuidAsync(false, model);
            List<SelectionItem<string>> list = (
                from p in InstalledModules.Packages orderby p.Name select
                    new SelectionItem<string> {
                        Text = this.__ResStr("package", "{0}", p.Name),
                        Value = p.AreaName,
                        Tooltip = this.__ResStr("packageTT", "{0} - {1}", p.Description.ToString(), p.CompanyDisplayName),
                    }).ToList<SelectionItem<string>>();
            list = (from l in list orderby l.Text select l).ToList();
            list.Insert(0, new SelectionItem<string> { Text = this.__ResStr("selectPackage", "(select)"), Value = null });

            return await DropDownListComponent.RenderDropDownListAsync(this, areaName, list, "yt_moduleselectionpackagenew");
        }
    }
}
