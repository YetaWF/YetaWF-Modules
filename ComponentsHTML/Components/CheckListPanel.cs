/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Endpoints;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the CheckListPanel component implementation.
    /// </summary>
    public abstract class CheckListPanelComponentBase : YetaWFComponent {

        internal const string TemplateName = "CheckListPanel";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Allows selection of multiple checkboxes from a list using a grid.
    /// </summary>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    public class CheckListPanelDisplayComponent : CheckListPanelComponentBase, IYetaWFComponent<List<SelectionCheckListEntry>> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class Entry {

            [Caption("Selected")]
            [UIHint("Boolean"), ReadOnly]
            public bool Value { get; set; }

            [Caption("Name")]
            [UIHint("String"), ReadOnly]
            public string Text { get; set; } = null!;

            [Caption("Description")]
            [UIHint("String"), ReadOnly]
            public string? Tooltip { get; set; }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(CheckListPanelEndpoints), GridSupport.DisplaySortFilter),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<SelectionCheckListEntry> model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<SelectionCheckListDetail>? details;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out details))
                details = new List<SelectionCheckListDetail>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                List<Entry> list = new List<Entry>();
                foreach (SelectionCheckListEntry selEntry in model) {
                    SelectionCheckListDetail? detail = (from l in details where l.Key == selEntry.Key select l).FirstOrDefault();
                    if (detail == null)
                        throw new InternalError($"Missing detail entry in {FieldName} for key {selEntry.Key}");
                    list.Add(new Entry {
                        Text = detail.Text.ToString(),
                        Tooltip = detail.Description?.ToString(),
                        Value = selEntry.Value
                    });
                }
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_checklistpanel t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName!, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }

    /// <summary>
    /// Allows selection of multiple checkboxes from a list using a grid.
    /// </summary>
    [UsesAdditional("Header", "bool", "true", "Defines whether the grid header is shown.")]
    public class CheckListPanelEditComponent : CheckListPanelComponentBase, IYetaWFComponent<List<SelectionCheckListEntry>> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class Entry {

            [Caption("Selected")]
            [UIHint("Boolean")]
            public bool Value { get; set; }

            [Caption("Name")]
            [UIHint("String"), ReadOnly]
            public string Text { get; set; } = null!;

            [Caption("Description")]
            [UIHint("String"), ReadOnly]
            public string? Tooltip { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public string Key { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(CheckListPanelEndpoints), GridSupport.EditSortFilter),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(List<SelectionCheckListEntry> model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<SelectionCheckListDetail>? details;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out details))
                details = new List<SelectionCheckListDetail>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                List<Entry> list = new List<Entry>();
                foreach (SelectionCheckListEntry selEntry in model) {
                    SelectionCheckListDetail? detail = (from l in details where l.Key == selEntry.Key select l).FirstOrDefault();
                    if (detail == null)
                        throw new InternalError($"Missing detail entry in {FieldName} for key {selEntry.Key}");
                    list.Add(new Entry {
                        Text = detail.Text.ToString(),
                        Tooltip = detail.Description?.ToString(),
                        //Enabled = detail.Enabled,
                        Key = selEntry.Key,
                        Value = selEntry.Value
                    });
                }
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return Task.FromResult(data);
            };

            hb.Append($@"
<div class='yt_checklistpanel t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName!, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
}
