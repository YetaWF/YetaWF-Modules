/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using Newtonsoft.Json;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using Newtonsoft.Json.Serialization;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Grid component implementation.
    /// </summary>
    public abstract class GridComponentBase : YetaWFComponent {

        internal const int MIN_COL_WIDTH = 12;

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Grid";

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
    }

    internal class GridSetup {
        public bool CanSort { get; internal set; }
        public bool CanFilter { get; internal set; }
        public bool CanReorder { get; internal set; }
        public bool ShowPager { get; internal set; }
        public string FieldName { get; set; }
        public string AjaxUrl { get; set; }
        [JsonConverter(typeof(GridDisplayComponent.StaticDataConverter))]
        public List<object> StaticData { get; internal set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Records { get; set; }
        public int Pages { get; set; }
        public List<GridColumnDefinition> Columns { get; set; }
        public string FilterMenusHTML { get; set; }
        public int MinColumnWidth { get; set; }
        public string SaveSettingsColumnWidthsUrl { get; set; }
        public object ExtraData { get; set; }
        public string HoverCss { get; set; }
        public string HighlightCss { get; set; }
        public string DisabledCss { get; set; }
        public string RowHighlightCss { get; set; }
        public string RowDragDropHighlightCss { get; set; }
        public string SortActiveCss { get; set; }
        public Guid? SettingsModuleGuid { get; set; }
        public bool HighlightOnClick { get; set; }

        public string DeletedMessage { get; set; }
        public string DeleteConfirmationMessage { get; set; }
        public string DeletedColumnDisplay { get; set; }

        public bool NoSubmitContents { get; set; }

        [JsonIgnore]
        public int TotalWidth { get; set; }
        [JsonIgnore]
        public string HeaderHTML { get; set; }

        public GridSetup() {
            Columns = new List<GridColumnDefinition>();
        }
    }

    internal class GridColumnDefinition {
        public string Name { get; set; }
        public bool Sortable { get; set; }
        public GridDefinition.SortBy Sort { get; set; }
        public bool Locked { get; set; }
        public bool OnlySubmitWhenChecked { get; set; }
        public GridColumnInfo.FilterOptionEnum? FilterOp { get; set; }
        public string FilterType { get; set; }
        public string FilterId { get; set; }
        public string MenuId { get; set; }
    }

    /// <summary>
    /// Implementation of the Grid display component.
    /// </summary>
    public partial class GridDisplayComponent : GridComponentBase, IYetaWFComponent<GridDefinition> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(YetaWF.Core.Controllers.AreaRegistration.CurrentPackage.AreaName, "fontawesome.com.fontawesome");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.menu.min.js");
            await JqueryUICore.UseAsync();
            await base.IncludeAsync();
        }

        private string buttonCss;

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(GridDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.ShowFilter == null)
                model.ShowFilter = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowGridSearchToolbar");
            if (model.DropdownActionWidth == null)
                model.DropdownActionWidth = GetDropdownActionWidthInChars();
            buttonCss = model.UseSkinFormatting ? " ui-corner-all ui-pg-button ui-state-default tg_button" : " tg_button";

            string idEmpty = UniqueId();

            ObjectSupport.ReadGridDictionaryInfo dictInfo = await YetaWF.Core.Components.Grid.LoadGridColumnDefinitionsAsync(model);

            if (model.Reorderable) {
                if (model.InitialPageSize != 0 || !model.IsStatic)
                    throw new InternalError("Unsupported options used for reorderable grid");
            }

            YetaWF.Core.Components.Grid.GridSavedSettings gridSavedSettings;
            int pageSize = model.InitialPageSize;
            int initialPage = 0;
            int page = 0;
            if (model.SettingsModuleGuid != null && model.SettingsModuleGuid != Guid.Empty) {
                gridSavedSettings = YetaWF.Core.Components.Grid.LoadModuleSettings((Guid)model.SettingsModuleGuid, initialPage + 1, pageSize);
                pageSize = gridSavedSettings.PageSize;
                initialPage = gridSavedSettings.CurrentPage - 1;
                page = gridSavedSettings.CurrentPage - 1;
            } else {
                gridSavedSettings = new YetaWF.Core.Components.Grid.GridSavedSettings {
                    CurrentPage = 1,
                    PageSize = pageSize,
                };
            }

            GridSetup setup = new GridSetup() {
                FieldName = FieldName,
                AjaxUrl = model.AjaxUrl,
                ShowPager = model.ShowPager,
                Page = page,
                PageSize = pageSize,
                MinColumnWidth = MIN_COL_WIDTH,
                ExtraData = model.ExtraData,
                HoverCss = model.UseSkinFormatting ? "ui-state-hover" : "tg_hover",
                HighlightCss = model.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                DisabledCss = model.UseSkinFormatting ? "ui-state-disabled" : "tg_disabled",
                RowHighlightCss = model.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                RowDragDropHighlightCss = model.UseSkinFormatting ? "ui-state-active" : "tg_dragdrophighlight",
                SortActiveCss = "tg_active",
                SettingsModuleGuid = model.SettingsModuleGuid,
                SaveSettingsColumnWidthsUrl = YetaWFManager.UrlFor(typeof(YetaWF.Core.Controllers.GridSaveSettingsController), nameof(YetaWF.Core.Controllers.GridSaveSettingsController.GridSaveColumnWidths)),
                DeletedMessage = model.DeletedMessage,
                DeleteConfirmationMessage = model.DeleteConfirmationMessage != null && UserSettings.GetProperty<bool>("ConfirmDelete") ? model.DeleteConfirmationMessage : null,
                DeletedColumnDisplay = model.DeletedColumnDisplay,
                CanReorder = model.Reorderable,
                HighlightOnClick = model.HighlightOnClick,
            };

            // Data
            if (model.DirectDataAsync == null)
                throw new InternalError($"{nameof(model.DirectDataAsync)} not set in {nameof(GridDefinition)} model");

            List<DataProviderSortInfo> sorts = gridSavedSettings.GetSortInfo();
            List<DataProviderFilterInfo> filters = gridSavedSettings.GetFilterInfo();
            // add default sort if no sort provided
            if (sorts == null && !string.IsNullOrWhiteSpace(dictInfo.SortColumn)) {
                // update sort column in saved settings.
                GridDefinition.ColumnInfo sortCol = null;
                if (gridSavedSettings.Columns.ContainsKey(dictInfo.SortColumn)) {
                    sortCol = gridSavedSettings.Columns[dictInfo.SortColumn];
                } else {
                    sortCol = new GridDefinition.ColumnInfo();
                    gridSavedSettings.Columns.Add(dictInfo.SortColumn, sortCol);
                }
                sortCol.Sort = dictInfo.SortBy;
                sorts = gridSavedSettings.GetSortInfo();
            }

            // Headers
            await GetHeadersAsync(model, dictInfo, setup, gridSavedSettings);

            DataSourceResult data;
            if (model.IsStatic) {
                DataSourceResult ds = await model.DirectDataAsync(0, int.MaxValue, null, null);
                setup.StaticData = ds.Data;
                if (model.SortFilterStaticData != null && model.SortFilterStaticData != GridDefinition.DontSortFilter) {
                    DataSourceResult dsPart = model.SortFilterStaticData?.Invoke(setup.StaticData, 0, int.MaxValue, sorts, filters);
                    data = dsPart;
                    data.Total = ds.Total;
                } else {
                    data = ds;
                }
            } else {
                data = await model.DirectDataAsync(page * pageSize, pageSize, sorts, filters);
            }
            // handle async properties
            await YetaWFController.HandlePropertiesAsync(data.Data);
            setup.Records = data.Total;

            if (pageSize > 0)
                setup.Pages = data.Total / pageSize + (data.Total % pageSize != 0 ? 1 : 0);
            else
                setup.Pages = 0;

            string tableHTML = await RenderTableHTML(HtmlHelper, model, data, setup.StaticData, dictInfo, FieldName, true, page * pageSize, pageSize);

            string noSubmitClass = "";
            if (model.IsStatic) {
                setup.NoSubmitContents = (from col in setup.Columns where col.OnlySubmitWhenChecked select col).FirstOrDefault() != null;
                if (setup.NoSubmitContents)
                    noSubmitClass = $" {Forms.CssFormNoSubmitContents}";
            }

            string cssTableStyle = "";
            switch (model.SizeStyle) {
                case GridDefinition.SizeStyleEnum.SizeGiven:
                    cssTableStyle = $" style='width:{setup.TotalWidth}px'";
                    break;
                case GridDefinition.SizeStyleEnum.SizeToFit:
                    cssTableStyle = $" style='width:100%'";
                    break;
                case GridDefinition.SizeStyleEnum.SizeAuto:
                    break;
            }

            // add ui-corner-top for rounded edges
            hb.Append($@"
<div id='{model.Id}' class='yt_grid t_display{noSubmitClass} {(model.UseSkinFormatting ? "tg_skin" : "tg_noskin")}'>
    <div class='tg_table{(model.UseSkinFormatting ? " ui-widget ui-widget-content" : "")}'>
        <table role='presentation'{cssTableStyle}>
            {setup.HeaderHTML}
            <tbody>
{tableHTML}
            </tbody>
        </table>
    </div>");

            if (model.ShowPager) {

                using (Manager.StartNestedComponent(FieldName)) {

                    // add ui-corner-bottom for rounded bottom edge
                    hb.Append($@"
    <div id='{model.Id}_Pager' class='tg_pager{(model.UseSkinFormatting ? " ui-state-default" : "")}'>
        {await RenderPagerAsync(model, data, gridSavedSettings, dictInfo, setup)}
    </div>");
                }
            }

            // loading
            if (!model.IsStatic) {
                hb.Append($@"
    <div id='{model.Id}_Loading' style='display:none' class='tg_loading{(model.UseSkinFormatting ? " ui-state-default" : "")}'>
        <div class='t_text'>{__ResStr("loading", "Loading ...")}</div>
    </div>");
            }

            hb.Append($@"
</div>");


            hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.Grid('{model.Id}', {JsonConvert.SerializeObject(setup, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml })});
</script>");

            return hb.ToYHtmlString();
        }

        private enum FilterBoolEnum {
            [EnumDescription("All", "Select all")]
            All = 0,
            [EnumDescription("Yes", "Select selected/enabled entries")]
            Yes = 1,
            [EnumDescription("No", "Select deselected/disabled entries")]
            No = 2,
        }
        private class FilterBoolUI {
            [UIHint("Enum"), AdditionalMetadata("AdjustWidth", false)]
            public FilterBoolEnum Value { get; set; }
        }
        private class FilterLongUI {
            [UIHint("LongValue")]
            public long? Value { get; set; }
        }
        private class FilterDynEnumUI {
            [UIHint("DropDownListInt"), AdditionalMetadata("AdjustWidth", false)]
            public int Value { get; set; }
            public List<SelectionItem<int>> Value_List { get; set; }
        }
        private class FilterDecimalUI {
            [UIHint("Decimal")]
            public decimal? Value { get; set; }
        }
        private class FilterDateTimeUI {
            [UIHint("DateTime")]
            public DateTime? Value { get; set; }
        }
        private class FilterDateUI {
            [UIHint("Date")]
            public DateTime? Value { get; set; }
        }
        private class FilterGuidUI {
            [UIHint("Text"), StringLength(100)]
            public string Value { get; set; }
        }
        private class FilterStringUI {
            [UIHint("Text"), StringLength(100)]
            public string Value { get; set; }
        }

        private async Task GetHeadersAsync(GridDefinition gridDef, ObjectSupport.ReadGridDictionaryInfo dictInfo, GridSetup setup, YetaWF.Core.Components.Grid.GridSavedSettings gridSavedSettings) {

            HtmlBuilder hb = new HtmlBuilder();
            HtmlBuilder filterhb = new HtmlBuilder();
            HtmlBuilder hbFilterMenus = new HtmlBuilder();

            string cssHead = "";
            if (!gridDef.ShowHeader)
                cssHead = " style='visibility:collapse'";

            hb.Append($@"
<thead{cssHead}>
    <tr class='tg_header{(gridDef.UseSkinFormatting ? " ui-state-default" : "")}'>");

            int colIndex = 0;
            foreach (var d in dictInfo.ColumnInfo) {

                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;

                if (gridCol.Hidden)
                    continue;

                PropertyData prop = ObjectSupport.GetPropertyData(gridDef.RecordType, propName);

                // Caption
                string caption = prop.GetCaption(gridDef.ResourceRedirect);
                if (!gridCol.Hidden && gridDef.ResourceRedirect != null && string.IsNullOrWhiteSpace(caption))
                    continue;// we need a caption if we're using resource redirects

                // Description
                string description = prop.GetDescription(gridDef.ResourceRedirect);

                // Locked
                bool locked = gridCol.Locked;

                // Width
                int width = 0;
                if (gridCol.Icons != 0) {
                    gridCol.Sortable = false;
                    YetaWF.Core.Components.Grid.GridActionsEnum actionStyle = YetaWF.Core.Components.Grid.GridActionsEnum.Icons;
                    if (gridCol.Icons > 1)
                        actionStyle = UserSettings.GetProperty<YetaWF.Core.Components.Grid.GridActionsEnum>("GridActions");
                    gridCol.ChWidth = gridCol.PixWidth = 0;
                    gridCol.Alignment = GridHAlignmentEnum.Center;
                    if (actionStyle == YetaWF.Core.Components.Grid.GridActionsEnum.DropdownMenu) {
                        width = (gridDef.DropdownActionWidth ?? 12) * Manager.CharWidthAvg;
                    } else {
                        width = 10 + (Math.Abs(gridCol.Icons) * (16 + 4) + 10);
                    }
                }
                if (gridCol.ChWidth != 0) {
                    width = gridCol.ChWidth * Manager.CharWidthAvg;
                } else if (gridCol.PixWidth != 0)
                    width = gridCol.PixWidth;

                GridDefinition.SortBy sort = GridDefinition.SortBy.NotSpecified;
                if (gridSavedSettings.Columns.ContainsKey(prop.Name)) {
                    GridDefinition.ColumnInfo columnInfo = gridSavedSettings.Columns[prop.Name];
                    if (columnInfo.Width >= 0)
                        width = columnInfo.Width; // override calculated width

                    if (gridCol.Sortable) {
                        if (columnInfo.Sort == GridDefinition.SortBy.Ascending || columnInfo.Sort == GridDefinition.SortBy.Descending)
                            sort = columnInfo.Sort;
                    }
                }
                width = (width < MIN_COL_WIDTH) ? MIN_COL_WIDTH : width;

                // Alignment
                string alignCss = null;
                switch (gridCol.Alignment) {
                    case GridHAlignmentEnum.Unspecified:
                    case GridHAlignmentEnum.Left:
                        alignCss = "tg_left";
                        break;
                    case GridHAlignmentEnum.Center:
                        alignCss = "tg_center";
                        break;
                    case GridHAlignmentEnum.Right:
                        alignCss = "tg_right";
                        break;
                }
                string sortHtml = "";
                if (gridCol.Sortable) {
                    setup.CanSort = true;
                    switch (sort) {
                        case GridDefinition.SortBy.NotSpecified:
                            sortHtml = "<span class='tg_sorticon'><span class='tg_sortasc fas fa-sort-up'></span><span class='tg_sortdesc fas fa-sort-down'></span></span>";
                            break;
                        case GridDefinition.SortBy.Ascending:
                            sortHtml = "<span class='tg_sorticon'><span class='tg_sortasc tg_active fas fa-sort-up'></span><span class='tg_sortdesc fas fa-sort-down'></span></span>";
                            break;
                        case GridDefinition.SortBy.Descending:
                            sortHtml = "<span class='tg_sorticon'><span class='tg_sortasc fas fa-sort-up'></span><span class='tg_sortdesc tg_active fas fa-sort-down'></span></span>";
                            break;
                    }
                }

                string resizeHTML = "";
                if (!gridCol.Locked) {
                    resizeHTML = "<span class='tg_resize'>&nbsp;</span>";
                }

                string cssWidth = "";
                if (gridDef.SizeStyle == GridDefinition.SizeStyleEnum.SizeGiven || gridDef.SizeStyle == GridDefinition.SizeStyleEnum.SizeToFit)
                    cssWidth = $" style='width:{width}px'";

                // Render column header
                hb.Append($@"
        <th class='{alignCss} tg_c_{propName.ToLower()}{(gridDef.UseSkinFormatting ? " ui-state-default" : "")}'{cssWidth}>
            {resizeHTML}<span {Basics.CssTooltipSpan}='{HAE(description ?? "")}'>{HE(caption)}</span>{sortHtml}
        </th>");

                List<GridColumnInfo.FilterOptionEnum> filterOpts = new List<GridColumnInfo.FilterOptionEnum>();
                GridColumnInfo.FilterOptionEnum? filterOp = null;
                string filterValue = null;
                string filterType = null;
                string idMenu = null;
                string idFilter = null;

                if (!gridDef.IsStatic) {

                    filterhb.Append($@"
        <th class='tg_f_{propName.ToLower()}'>");

                    if (gridCol.FilterOptions.Count > 0) {

                        if (gridSavedSettings.Columns.ContainsKey(prop.Name)) {
                            GridDefinition.ColumnInfo columnInfo = gridSavedSettings.Columns[prop.Name];
                            if (!string.IsNullOrWhiteSpace(columnInfo.FilterOperator)) {
                                filterOp = GetFilterOptionEnum(columnInfo.FilterOperator);
                                filterValue = columnInfo.FilterValue;
                            }
                        }

                        idFilter = UniqueId();
                        idMenu = UniqueId();
                        string searchToolTip = __ResStr("searchTT", "Select search method");

                        filterhb.Append($@"
            <div class='tg_fentry'>");

                        if (prop.PropInfo.PropertyType == typeof(bool) || prop.PropInfo.PropertyType == typeof(bool?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum>();// none
                            filterType = "bool";
                            FilterBoolUI filterUI = new FilterBoolUI {
                                Value = (filterOp != null) ? (Convert.ToBoolean(filterValue) ? FilterBoolEnum.Yes : FilterBoolEnum.No) : FilterBoolEnum.All,
                            };
                            filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.Equal;

                            filterhb.Append($@"
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>");

                        } else if (prop.PropInfo.PropertyType == typeof(int) || prop.PropInfo.PropertyType == typeof(int?) || prop.PropInfo.PropertyType == typeof(long) || prop.PropInfo.PropertyType == typeof(long?)) {

                            List<SelectionItem<int>> entries = await YetaWFComponentExtender.GetSelectionListIntFromUIHintAsync(prop.UIHint);
                            if (entries == null) {
                                // regular int/long
                                filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                    GridColumnInfo.FilterOptionEnum.GreaterEqual, GridColumnInfo.FilterOptionEnum.GreaterThan, GridColumnInfo.FilterOptionEnum.LessEqual, GridColumnInfo.FilterOptionEnum.LessThan,
                                    GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                                };
                                filterType = "long";
                                FilterLongUI filterUI = new FilterLongUI();
                                if (filterOp != null) {
                                    try {
                                        filterUI.Value = Convert.ToInt64(filterValue);
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");

                            } else {
                                // this is a dynamic enumerated value
                                filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                    GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                                };
                                filterType = "dynenum";
                                entries.Insert(0, new SelectionItem<int> {
                                    Value = -1,
                                    Text = __ResStr("noSel", "(no selection)")
                                });
                                FilterDynEnumUI filterUI = new FilterDynEnumUI {
                                    Value = 0,
                                    Value_List = entries,
                                };
                                filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.Equal;

                                filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>");

                            }
                        } else if (prop.PropInfo.PropertyType == typeof(decimal) || prop.PropInfo.PropertyType == typeof(decimal?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.GreaterEqual, GridColumnInfo.FilterOptionEnum.GreaterThan, GridColumnInfo.FilterOptionEnum.LessEqual, GridColumnInfo.FilterOptionEnum.LessThan,
                                GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                            };
                            filterType = "decimal";
                            FilterDecimalUI filterUI = new FilterDecimalUI();
                            if (filterOp != null) {
                                try {
                                    filterUI.Value = Convert.ToDecimal(filterValue);
                                } catch (Exception) { }
                            }
                            filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.GreaterEqual;

                            filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");

                        } else if (prop.PropInfo.PropertyType == typeof(DateTime) || prop.PropInfo.PropertyType == typeof(DateTime?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.GreaterEqual, GridColumnInfo.FilterOptionEnum.LessEqual,
                            };

                            if (prop.UIHint == "DateTime") {
                                filterType = "datetime";
                                FilterDateTimeUI filterUI = new FilterDateTimeUI();
                                if (filterOp != null) {
                                    try {
                                        DateTime dt = Convert.ToDateTime(filterValue);
                                        filterUI.Value = YetaWF.Core.Localize.Formatting.GetUtcDateTime(dt);
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");

                            } else if (prop.UIHint == "Date") {
                                filterType = "date";
                                FilterDateUI filterUI = new FilterDateUI();
                                if (filterOp != null) {
                                    try {
                                        DateTime dt = Convert.ToDateTime(filterValue);
                                        filterUI.Value = YetaWF.Core.Localize.Formatting.GetUtcDateTime(dt).Date;
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");

                            } else {
                                throw new InternalError("Need DateTime or Date UIHint for DateTime data");
                            }

                        } else if (prop.PropInfo.PropertyType == typeof(Guid) || prop.PropInfo.PropertyType == typeof(Guid?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.Contains, GridColumnInfo.FilterOptionEnum.StartsWith, GridColumnInfo.FilterOptionEnum.Endswith
                            };
                            filterType = "guid";
                            FilterGuidUI filterUI = new FilterGuidUI {
                                Value = (filterOp != null) ? filterValue : null,
                            };
                            filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.Contains;

                            filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");

                        } else if (prop.PropInfo.PropertyType.IsEnum) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                            };
                            filterType = "enum";
                            EnumData enumData = ObjectSupport.GetEnumData(prop.PropInfo.PropertyType);

                            List<SelectionItem<int>> entries = new List<SelectionItem<int>>();
                            foreach (EnumDataEntry entry in enumData.Entries) {
                                entries.Add(new SelectionItem<int> {
                                    Text = entry.Caption,
                                    Tooltip = entry.Description,
                                    Value = (int)entry.Value,
                                });
                            }
                            if ((from e in entries where e.Value == -1 select e).FirstOrDefault() == null) {
                                entries.Insert(0, new SelectionItem<int> {
                                    Value = -1,
                                    Text = __ResStr("noSel", "(no selection)")
                                });
                            }
                            FilterDynEnumUI filterUI = new FilterDynEnumUI {
                                Value = (filterOp != null) ? Convert.ToInt32(filterValue) : -1,
                                Value_List = entries,
                            };
                            filterOp = filterOp ?? GridColumnInfo.FilterOptionEnum.Equal;

                            filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>");

                        } else {
                            filterOpts = GetFilterOptions(gridCol);
                            filterType = "text";
                            FilterStringUI filterUI = new FilterStringUI {
                                Value = (filterOp != null) ? filterValue : null,
                            };
                            filterOp = filterOp ?? filterOpts.First();

                            filterhb.Append($@"
                <div class='tg_fmenu{buttonCss}' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></div>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <div class='tg_fclear{buttonCss}'><span class='fas fa-times'></span></div>");
                        }

                        hbFilterMenus.Append(GetFilterMenu(gridDef, filterOpts, filterOp, idMenu, colIndex));

                        filterhb.Append($@"
            </div>");

                    } else {
                        filterhb.Append($@"
            &nbsp;");
                    }
                    filterhb.Append($@"
        </th>");

                }

                // Build column definition
                setup.Columns.Add(new GridColumnDefinition {
                    Name = prop.Name,
                    Sortable = gridCol.Sortable,
                    Sort = sort,
                    OnlySubmitWhenChecked = gridCol.OnlySubmitWhenChecked,
                    Locked = gridCol.Locked,
                    FilterOp = filterOp,
                    FilterType = filterType,
                    FilterId = idFilter,
                    MenuId = idMenu,
                });

                setup.TotalWidth += width;

                ++colIndex;
            }

            if (filterhb.Length > 0) {

                setup.CanFilter = true;

                string filterStyle = "";
                if (gridDef.ShowFilter != true)
                    filterStyle = " style='display:none'";

                hb.Append($@"
    </tr>
    <tr class='tg_filter{(gridDef.UseSkinFormatting ? " ui-state-default" : "")}'{filterStyle}>
        {filterhb.ToString()}
    </tr>
</thead>");
            } else {
                hb.Append($@"
    </tr>
</thead>");
            }

            setup.HeaderHTML = hb.ToString();
            setup.FilterMenusHTML += hbFilterMenus.ToString();
        }

        private string GetFilterMenu(GridDefinition gridModel, List<GridColumnInfo.FilterOptionEnum> filterOpts, GridColumnInfo.FilterOptionEnum? filterOp, string idMenu, int colIndex) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($"<ul id='{idMenu}' style='display:none'>");
            foreach (GridColumnInfo.FilterOptionEnum option in filterOpts) {
                string icon = GetFilterIcon(option);
                string text = GetFilterText(option);
                string liCss = option == filterOp ? $" class='{(gridModel.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight")}'" : "";
                hb.Append($"<li data-sel='{(int)option}'{liCss}><span class='t_fmenuicon'>{HE(icon)}</span><span class='t_fmenutext'>{HE(text)}</span></li>");
            }
            hb.Append("</ul>");
            hb.Append($@"
<script>
    $('#{idMenu}').kendoMenu({{
        orientation: 'vertical',
        select: function(ev) {{ YetaWF_ComponentsHTML.Grid.menuSelected(ev.item, {colIndex}); }}
    }});
</script>");// JQuery/Kendo UI Use
            return hb.ToString();
        }

        private GridColumnInfo.FilterOptionEnum? GetFilterOptionEnum(string filterOp) {
            switch (filterOp) {
                case "==": return GridColumnInfo.FilterOptionEnum.Equal;
                case "!=": return GridColumnInfo.FilterOptionEnum.NotEqual;
                case "<": return GridColumnInfo.FilterOptionEnum.LessThan;
                case "<=": return GridColumnInfo.FilterOptionEnum.LessEqual;
                case ">": return GridColumnInfo.FilterOptionEnum.GreaterThan;
                case ">=": return GridColumnInfo.FilterOptionEnum.GreaterEqual;
                case "StartsWith": return GridColumnInfo.FilterOptionEnum.StartsWith;
                case "NotStartsWith": return GridColumnInfo.FilterOptionEnum.NotStartsWith;
                case "EndsWith": return GridColumnInfo.FilterOptionEnum.Endswith;
                case "NotEndsWith": return GridColumnInfo.FilterOptionEnum.NotEndswith;
                case "Contains": return GridColumnInfo.FilterOptionEnum.Contains;
                case "NotContains": return GridColumnInfo.FilterOptionEnum.NotContains;
            }
            throw new InternalError($"Unexpected filter operator {filterOp}");
        }

        private string GetFilterText(GridColumnInfo.FilterOptionEnum filterOp) {
            switch (filterOp) {
                case GridColumnInfo.FilterOptionEnum.Equal: return __ResStr("f_equals", "Equal");
                case GridColumnInfo.FilterOptionEnum.GreaterEqual: return __ResStr("f_ge", "Greater or equal");
                case GridColumnInfo.FilterOptionEnum.GreaterThan: return __ResStr("f_g", "Greater");
                case GridColumnInfo.FilterOptionEnum.LessEqual: return __ResStr("f_le", "Less or equal");
                case GridColumnInfo.FilterOptionEnum.LessThan: return __ResStr("f_l", "Less");
                case GridColumnInfo.FilterOptionEnum.NotEqual: return __ResStr("f_ne", "Not equal");
                case GridColumnInfo.FilterOptionEnum.StartsWith: return __ResStr("f_bw", "Begins with");
                case GridColumnInfo.FilterOptionEnum.NotStartsWith: return __ResStr("f_nbw", "Does not begin with");
                case GridColumnInfo.FilterOptionEnum.Contains: return __ResStr("f_contains", "Contains");
                case GridColumnInfo.FilterOptionEnum.NotContains: return __ResStr("f_ncontains", "Does not contain");
                case GridColumnInfo.FilterOptionEnum.Endswith: return __ResStr("f_endswith", "Ends with");
                case GridColumnInfo.FilterOptionEnum.NotEndswith: return __ResStr("f_nendswith", "Does not end with");
            }
            throw new InternalError($"Unexpected filter op {filterOp}");
        }

        private string GetFilterIcon(GridColumnInfo.FilterOptionEnum? filterOp) {
            if (filterOp == null)
                throw new InternalError("No filter op available");
            switch (filterOp) {
                case GridColumnInfo.FilterOptionEnum.Equal: return "==";
                case GridColumnInfo.FilterOptionEnum.GreaterEqual: return ">=";
                case GridColumnInfo.FilterOptionEnum.GreaterThan: return ">";
                case GridColumnInfo.FilterOptionEnum.LessEqual: return "<=";
                case GridColumnInfo.FilterOptionEnum.LessThan: return "<";
                case GridColumnInfo.FilterOptionEnum.NotEqual: return "!=";
                case GridColumnInfo.FilterOptionEnum.StartsWith: return "^";
                case GridColumnInfo.FilterOptionEnum.NotStartsWith: return "!^";
                case GridColumnInfo.FilterOptionEnum.Contains: return "~";
                case GridColumnInfo.FilterOptionEnum.NotContains: return "!~";
                case GridColumnInfo.FilterOptionEnum.Endswith: return "$";
                case GridColumnInfo.FilterOptionEnum.NotEndswith: return "!$";
            }
            throw new InternalError($"Unexpected filter option {filterOp}");
        }

        private List<GridColumnInfo.FilterOptionEnum> GetFilterOptions(GridColumnInfo gridCol) {
            return gridCol.FilterOptions.ToList();
        }

        internal static async Task<string> RenderTableHTML(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                GridDefinition model, DataSourceResult data, List<object> staticData, ObjectSupport.ReadGridDictionaryInfo dictInfo, string fieldPrefix, bool readOnly, int skip, int take) {

            HtmlBuilder hb = new HtmlBuilder();

            if (data.Total == 0 || model.IsStatic) {

                string styleCss = "";
                if (data.Total > 0)
                    styleCss = " style='display:none'";

                hb.Append($@"
<tr role='row'{styleCss} class='tg_emptytr{(model.UseSkinFormatting ? " ui-widget-content" : "")}'>
    <td role='gridcell' colspan='{dictInfo.VisibleColumns}'>
        <div class='tg_emptydiv'>
            {HE(model.NoRecordsText)}
        </div>
    </td>
</tr>");

            }

            if (data.Total > 0) {

                int recordCount = 0;
                int first = skip;
                int last = skip + (take == 0 ? int.MaxValue : take);

                foreach (object record in data.Data) {

                    int origin = -1;
                    if (staticData != null) {
                        origin = staticData.IndexOf(record);
                        if (origin < 0)
                            throw new InternalError($"Data record being rendered not found in static data");
                    }
                    bool hide = model.IsStatic && !(recordCount >= first && recordCount < last);

                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, model, dictInfo, fieldPrefix, record, recordCount, origin, hide));
                    ++recordCount;
                }
            } else {
                if (!Manager.IsPostRequest) {
                    // when initially rendering a grid with 0 records, we have to prepare for all templates
                    await YetaWFComponentExtender.AddComponentForType(model.RecordType);
                }
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordHTMLAsync(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                GridDefinition gridModel, ObjectSupport.ReadGridDictionaryInfo dictInfo, string fieldPrefix, object record, int recordCount, int origin, bool hide) {

            HtmlBuilder hbHidden = new HtmlBuilder();

            using (Manager.StartNestedComponent($"{fieldPrefix}[{(origin >= 0 ? origin : recordCount)}]")) {

                Type recordType = record.GetType();

                bool highlight;
                ObjectSupport.TryGetPropertyValue<bool>(record, "__highlight", out highlight, false);
                bool lowlight;
                ObjectSupport.TryGetPropertyValue<bool>(record, "__lowlight", out lowlight, false);
                // check if the grid is readonly or the record supports an "__editable" grid entry property
                bool recordEnabled = gridModel.IsStatic;
                if (recordEnabled)
                    ObjectSupport.TryGetPropertyValue<bool>(record, "__editable", out recordEnabled, true);

                string lightCss = "";
                if (highlight)
                    lightCss = "tg_highlight";
                else if (lowlight)
                    lightCss = "tg_lowlight";

                // collect hidden fields
                foreach (string colName in dictInfo.ColumnInfo.Keys) {

                    GridColumnInfo gridCol = dictInfo.ColumnInfo[colName];

                    if (gridCol.Hidden) {
                        PropertyData prop = ObjectSupport.GetPropertyData(recordType, colName);
                        object value = prop.GetPropertyValue<object>(record);
                        string output;

                        if (recordEnabled && !prop.ReadOnly) {
                            output = (await htmlHelper.ForEditComponentAsync(record, colName, value, prop.UIHint)).ToString();
                            output += YetaWFComponent.ValidationMessage(htmlHelper, Manager.NestedComponentPrefix, colName).ToString();
                        } else {
                            output = (await htmlHelper.ForDisplayComponentAsync(record, colName, value, prop.UIHint)).ToString();
                        }
                        if (!string.IsNullOrWhiteSpace(output))
                            output = output.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                        if (string.IsNullOrWhiteSpace(output))
                            output = "&nbsp;";
                        hbHidden.Append(output);
                    }
                }

                string trStyle = hide ? " style='display:none'" : "";
                string originData = (origin >= 0) ? $" data-origin='{origin}'" : "";

                HtmlBuilder hb = new HtmlBuilder();
                hb.Append($@"
<tr role='row'{originData} class='{lightCss}{(gridModel.UseSkinFormatting ? " ui-widget-content" : "")}'{trStyle}>");

                foreach (string colName in dictInfo.ColumnInfo.Keys) {

                    GridColumnInfo gridCol = dictInfo.ColumnInfo[colName];
                    if (!gridCol.Hidden) {

                        PropertyData prop = ObjectSupport.GetPropertyData(recordType, colName);
                        object value = prop.GetPropertyValue<object>(record);

                        if (gridModel.ResourceRedirect != null && string.IsNullOrWhiteSpace(prop.GetCaption(gridModel.ResourceRedirect)))
                            continue;// we need a caption if we're using resource redirects

                        // Alignment
                        if (gridCol.Icons != 0)
                            gridCol.Alignment = GridHAlignmentEnum.Center;

                        string alignCss = null;
                        switch (gridCol.Alignment) {
                            case GridHAlignmentEnum.Unspecified:
                            case GridHAlignmentEnum.Left:
                                alignCss = "tg_left";
                                break;
                            case GridHAlignmentEnum.Center:
                                alignCss = "tg_center";
                                break;
                            case GridHAlignmentEnum.Right:
                                alignCss = "tg_right";
                                break;
                        }

                        hb.Append($@"
    <td role='gridcell' class='{alignCss} tg_c_{colName.ToLower()}'>");

                        if (hbHidden.Length > 0) { // add all hidden fields to first cell
                            hb.Append(hbHidden.ToString());
                            hbHidden = new HtmlBuilder();
                        }

                        string output;
                        if (Manager.IsDemo && prop.HasAttribute(nameof(ExcludeDemoModeAttribute))) {
                            output = __ResStr("demo", "(Demo - N/A)");
                        } else if (recordEnabled && !prop.ReadOnly) {
                            output = (await htmlHelper.ForEditComponentAsync(record, colName, value, prop.UIHint)).ToString();
                            output += YetaWFComponent.ValidationMessage(htmlHelper, Manager.NestedComponentPrefix, colName).ToString();
                        } else {
                            output = (await htmlHelper.ForDisplayComponentAsync(record, colName, value, prop.UIHint)).ToString();
                        }
                        if (!string.IsNullOrWhiteSpace(output))
                            output = output.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                        if (string.IsNullOrWhiteSpace(output))
                            output = "&nbsp;";

                        hb.Append(output);

                        hb.Append($@"
    </td>");
                    }
                }
                hb.Append($@"
</tr>");
                return hb.ToString();
            }
        }

        internal class PagerUI {
            [Caption("Page"), Description("Enter the page to show and hit the Return key to go to the page")]
            [UIHint("IntValue4")]
            public int __Page { get; set; }
            [Caption("Entries"), Description("Select the number of entries per page")]
            [UIHint("DropDownListInt")]
            public int __PageSelection { get; set; }
            public List<SelectionItem<int>> __PageSelection_List { get; set; }
        }

        private async Task<string> RenderPagerAsync(GridDefinition gridModel, DataSourceResult data, YetaWF.Core.Components.Grid.GridSavedSettings gridSavedSettings, ObjectSupport.ReadGridDictionaryInfo dictInfo, GridSetup setup) {

            HtmlBuilder hb = new HtmlBuilder();

            List<SelectionItem<int>> selList = new List<SelectionItem<int>>();
            foreach (int size in gridModel.PageSizes) {
                selList.Add(new SelectionItem<int> {
                    Text = size.ToString(),
                    Tooltip = __ResStr("pages", "Show {0} entries", size),
                    Value = size,
                });
            }

            PagerUI pagerUI = new PagerUI {
                __Page = setup.Page + 1,
                __PageSelection = setup.PageSize,
                __PageSelection_List = selList,
            };

            string reloadHTML = "";
            string searchHTML = "";
            if (!gridModel.IsStatic && gridModel.SupportReload)
                reloadHTML = $@"<div class='tg_reload{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnTop", "Reload the current page"))}'><span class='fas fa-sync-alt'></span></div>";
            if (setup.CanFilter)
                searchHTML = $@"<div class='tg_search{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnFilter", "Turn the search bar on or off"))}'><span class='fas fa-search'></span></div>";

            if (!string.IsNullOrEmpty(reloadHTML) || !string.IsNullOrEmpty(searchHTML)) {
                hb.Append($@"
    <div class='tg_pgbtns'>
        {reloadHTML}{searchHTML}
    </div>");
            }

            if (setup.PageSize != 0) {

                string topHTML = $@"<div class='tg_pgtop{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnFirst", "Go to the first page"))}'><span class='fas fa-fast-backward'></span></div>";
                string prevHTML = $@"<div class='tg_pgprev{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnPrev", "Go to the previous page"))}'><span class='fas fa-backward'></span></div>";
                string nextHTML = $@"<div class='tg_pgnext{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnNext", "Go to the next page"))}'><span class='fas fa-forward'></span></div>";
                string bottomHTML = $@"<div class='tg_pgbottom{buttonCss}' {Basics.CssTooltip}='{HAE(__ResStr("btnLast", "Go to the last page"))}'><span class='fas fa-fast-forward'></span></div>";

                hb.Append($@"
    <div class='tg_pgctl'>
        {topHTML}{prevHTML}
        <div class='tg_pgnum'>
            {await HtmlHelper.ForLabelAsync(pagerUI, nameof(PagerUI.__Page))}
            {await HtmlHelper.ForEditAsync(pagerUI, nameof(PagerUI.__Page))}
        </div>
        {nextHTML}{bottomHTML}
    </div>
    <div class='tg_pgsel'>
        {await HtmlHelper.ForLabelAsync(pagerUI, nameof(PagerUI.__PageSelection))}
        {await HtmlHelper.ForEditAsync(pagerUI, nameof(PagerUI.__PageSelection))}
    </div>");
            }

            hb.Append($@"
    <div class='tg_totals'>
    </div>");

            return hb.ToString();
        }

        internal int GetDropdownActionWidthInChars() {
            string s = __ResStr("dropdownWidth", "11");
            return Convert.ToInt32(s);
        }

        // Custom serializer to minimize static data being transferred

        internal class GridEntryContractResolver : DefaultContractResolver {

            public GridEntryContractResolver() { }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (type != typeof(object)) {
                    List<string> propList = new List<string>();
                    List<PropertyData> props = ObjectSupport.GetPropertyData(type);
                    foreach (PropertyData prop in props) {
                        if (prop.Name.StartsWith("__") || (prop.PropInfo.CanRead && prop.PropInfo.CanWrite && !string.IsNullOrWhiteSpace(prop.UIHint))) {
                            propList.Add(prop.Name);
                        }
                    }
                    properties = (from p in properties where propList.Contains(p.PropertyName) select p).ToList();
                }
                return properties;
            }
        }
        internal class StaticDataConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return true;
            }
            public override bool CanRead {
                get { return false; }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                throw new NotImplementedException();
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                string array = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ContractResolver = new GridEntryContractResolver() });
                writer.WriteRawValue(array);
            }
        }
    }
}

