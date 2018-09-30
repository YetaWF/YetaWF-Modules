/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ReferencedModulesComponentBase : YetaWFComponent {

        public const string TemplateName = "ReferencedModules";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ReferencedModulesDisplayComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {

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
        internal static Grid2Definition GetGridModel(bool header) {
            return new Grid2Definition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = YetaWFManager.UrlFor(typeof(ReferencedModulesController), nameof(ReferencedModulesController.ReferencedModulesDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = new SerializableList<ModuleDefinition.ReferencedModule>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            Grid2Model grid = new Grid2Model() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<AddOnManager.Module> allMods = Manager.AddOnManager.GetUniqueInvokedCssModules();

                List<Entry> mods = new List<Entry>();
                foreach (AddOnManager.Module allMod in allMods) {
                    if ((from m in model where m.ModuleGuid == allMod.ModuleGuid select m).FirstOrDefault() != null) {
                        ModuleDefinition modDef = await ModuleDefinition.CreateUniqueModuleAsync(allMod.ModuleType);
                        if (modDef != null) {
                            mods.Add(new Entry {
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
            };

            hb.Append($@"
<div class='yt_invokedmodules t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Softelvdm_Grid_Grid2", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToYHtmlString();
        }
    }
    public class ReferencedModulesEditComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class Entry {

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

            [UIHint("Hidden"), ReadOnly]
            public Guid ModuleGuid { get; set; }
        }
        internal static Grid2Definition GetGridModel(bool header) {
            return new Grid2Definition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = YetaWFManager.UrlFor(typeof(ReferencedModulesController), nameof(ReferencedModulesController.ReferencedModulesEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = new SerializableList<ModuleDefinition.ReferencedModule>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            Grid2Model grid = new Grid2Model() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<AddOnManager.Module> allMods = Manager.AddOnManager.GetUniqueInvokedCssModules();

                List<Entry> mods = new List<Entry>();
                foreach (AddOnManager.Module allMod in allMods) {
                    ModuleDefinition modDef = await ModuleDefinition.CreateUniqueModuleAsync(allMod.ModuleType);
                    if (modDef != null) {
                        mods.Add(new Entry {
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
            };

            hb.Append($@"
<div class='yt_invokedmodules t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Softelvdm_Grid_Grid2", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToYHtmlString();
        }
    }
}
