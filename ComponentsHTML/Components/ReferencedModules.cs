using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ReferencedModulesComponentBase : YetaWFComponent {

        public const string TemplateName = "ReferencedModules";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ReferencedModulesDisplayComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridDisplay {

            [Caption("Name"), Description("Module Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Module Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Module"), Description("Module")]
            [UIHint("String"), ReadOnly]
            public string PermanentName { get; set; }
        }
        protected async Task<DataSourceResult> GetDataSourceResultAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            List<AddOnManager.Module> allMods = Manager.AddOnManager.GetUniqueInvokedCssModules();

            List<GridDisplay> mods = new List<GridDisplay>();
            foreach (AddOnManager.Module allMod in allMods) {
                if ((from m in model where m.ModuleGuid == allMod.ModuleGuid select m).FirstOrDefault() != null) {
                    ModuleDefinition modDef = await ModuleDefinition.CreateUniqueModuleAsync(allMod.ModuleType);
                    if (modDef != null) {
                        mods.Add(new GridDisplay {
                            Name = modDef.Name,
                            Description = modDef.Description,
                            PermanentName = modDef.PermanentModuleName,
                        });
                    }
                }
            }
            DataSourceResult data = new DataSourceResult {
                Data = mods.ToList<object>(),
                Total = mods.Count,
            };
            return data;
        }
        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_invokedmodules t_display'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridDisplay),
                    Data = await GetDataSourceResultAsync(model),
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
    public class ReferencedModulesEditComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class GridEdit {

            [Caption("Use"), Description("Select to include this module")]
            [UIHint("Boolean")]
            public bool UsesModule { get; set; }

            [Caption("Name"), Description("Module Name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Description"), Description("Module Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Module"), Description("Module")]
            [UIHint("String"), ReadOnly]
            public string PermanentName { get; set; }

            [Caption("Guid"), Description("Module Guid")]
            [UIHint("Guid"), ReadOnly]
            public Guid ModuleGuid { get; set; } // this name must match the name used in the class ReferencedModule
        }
        protected async Task<DataSourceResult> GetDataSourceResultAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            List<AddOnManager.Module> allMods = Manager.AddOnManager.GetUniqueInvokedCssModules();

            List<GridEdit> mods = new List<GridEdit>();
            foreach (AddOnManager.Module allMod in allMods) {
                ModuleDefinition modDef = await ModuleDefinition.CreateUniqueModuleAsync(allMod.ModuleType);
                if (modDef != null) {
                    mods.Add(new GridEdit {
                        Name = modDef.Name,
                        Description = modDef.Description,
                        PermanentName = modDef.PermanentModuleName,
                        ModuleGuid = modDef.ModuleGuid,
                        UsesModule = (from m in model where m.ModuleGuid == allMod.ModuleGuid select m).FirstOrDefault() != null
                    });
                }
            }
            DataSourceResult data = new DataSourceResult {
                Data = mods.ToList<object>(),
                Total = mods.Count,
            };
            return data;
        }
        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_invokedmodules t_edit'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEdit),
                    Data = await GetDataSourceResultAsync(model),
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
}
