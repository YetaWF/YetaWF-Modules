/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

    /// <summary>
    /// Base class for the ReferencedModules component implementation.
    /// </summary>
    public abstract class ReferencedModulesComponentBase : YetaWFComponent {

        internal const string TemplateName = "ReferencedModules";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays all modules contained in the model as a grid listing details about each module.
    /// </summary>
    /// <example>
    /// [Caption("Referenced Modules"), Description("Referenced Modules")]
    /// [UIHint("ReferencedModules"), ReadOnly]
    /// public SerializableList&lt;ModuleDefinition.ReferencedModule&gt; ReferencedModules { get; set; }
    /// </example>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    public class ReferencedModulesDisplayComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class Entry {

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
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ReferencedModulesController), nameof(ReferencedModulesController.ReferencedModulesDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = new SerializableList<ModuleDefinition.ReferencedModule>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
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
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }

    /// <summary>
    /// Displays all unique modules available as a grid listing details about each module, allowing the user to select 0, 1 or multiple modules with the grid using the check box displayed. Modules contained within the model are selected.
    /// </summary>
    /// <example>
    /// [Caption("Referenced Modules"), Description("Referenced Modules")]
    /// [UIHint("ReferencedModules")]
    /// public SerializableList&lt;ModuleDefinition.ReferencedModule&gt; ReferencedModules { get; set; }
    /// </example>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    public class ReferencedModulesEditComponent : ReferencedModulesComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.ReferencedModule>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class Entry {

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
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ReferencedModulesController), nameof(ReferencedModulesController.ReferencedModulesEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(SerializableList<ModuleDefinition.ReferencedModule> model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = new SerializableList<ModuleDefinition.ReferencedModule>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
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
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
}
