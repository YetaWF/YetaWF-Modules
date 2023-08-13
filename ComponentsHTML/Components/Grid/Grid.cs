/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Endpoints;
using YetaWF.Modules.ComponentsHTML.Views;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Grid component implementation.
    /// </summary>
    public abstract class GridComponentBase : YetaWFComponent {

        internal const int MIN_COL_WIDTHPIX = 12;
        internal const int MIN_COL_WIDTHCH = 2;

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridComponentBase), name, defaultValue, parms); }

        /// <summary>
        /// Defines the component's name.
        /// </summary>
        public const string TemplateName = "Grid";

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

    internal class GridSetup {
        public bool CanSort { get; internal set; }
        public bool CanFilter { get; internal set; }
        public bool CanReorder { get; internal set; }
        public bool ShowPager { get; internal set; }
        public string FieldName { get; set; } = null!;
        public string AjaxUrl { get; set; } = null!;
        [JsonConverter(typeof(GridDisplayComponent.StaticDataConverter))]
        public List<object> StaticData { get; internal set; } = null!;
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Records { get; set; }
        public int Pages { get; set; }
        public GridDefinition.SizeStyleEnum SizeStyle { get; internal set; }
        public List<GridColumnDefinition> Columns { get; set; }
        public int MinColumnWidth { get; set; }
        public string? SaveSettingsColumnWidthsUrl { get; set; }
        public string? SaveSettingsColumnSelectionUrl { get; set; }
        public string? SaveExpandCollapseUrl { get; set; }
        public object? ExtraData { get; set; }
        public string HighlightCss { get; set; } = null!;
        public string DisabledCss { get; set; } = null!;
        public string RowHighlightCss { get; set; } = null!;
        public string RowDragDropHighlightCss { get; set; } = null!;
        public string SortActiveCss { get; set; } = null!;
        public Guid? SettingsModuleGuid { get; set; }
        public bool HighlightOnClick { get; set; }

        public List<string>? PanelHeaderSearchColumns { get; set; }

        public string? DeletedMessage { get; set; }
        public string? DeleteConfirmationMessage { get; set; }
        public string? DeletedColumnDisplay { get; set; }

        public bool NoSubmitContents { get; set; }

        [JsonIgnore]
        public string HeaderHTML { get; set; } = null!;

        public GridSetup() {
            Columns = new List<GridColumnDefinition>();
            PanelHeaderSearchColumns = new List<string>();
        }
    }

    internal class GridColumnDefinition {
        public string Name { get; set; } = null!;
        public bool Sortable { get; set; }
        public GridDefinition.SortBy Sort { get; set; }
        public bool Locked { get; set; }
        public bool OnlySubmitWhenChecked { get; set; }
        public GridColumnInfo.FilterOptionEnum FilterOp { get; set; }
        public string? FilterType { get; set; }
        public string? FilterId { get; set; }
        public string? MenuId { get; set; }
        public bool Visible { get; set; }
    }

    internal class SearchUI {
        [Caption(""), Description("")]
        [UIHint("Search"), StringLength(80)]
        public string __Search { get; set; } = null!;
    }
    internal class ColumnSelectionUI {
        [Caption(""), Description("")]
        [UIHint("CheckListMenu")]
        public List<SelectionCheckListEntry> __ColumnSelection { get; set; } = null!;
        public List<SelectionCheckListDetail> __ColumnSelection_List { get; set; } = null!;
        public string __ColumnSelection_DDSVG { get { return "fas-columns"; } }
    }

    /// <summary>
    /// Displays a grid. If the model is null, nothing is rendered. The model defines various attributes of the grid.
    /// </summary>
    /// <example>
    /// [Caption("All Users"), Description("Shows all users")]
    /// [UIHint("Grid"), ReadOnly]
    /// public GridDefinition AllUsers { get; set; }
    /// </example>
    public partial class GridDisplayComponent : GridComponentBase, IYetaWFComponent<GridDefinition> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            // Add required menu support
            await Manager.AddOnManager.AddAddOnNamedAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, "github.com.grsmto.simplebar");
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "MenuUL", ComponentType.Display);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(GridDefinition model) {

            if (model == null) return string.Empty;

            HtmlBuilder hb = new HtmlBuilder();

            if (model.ShowFilter == null)
                model.ShowFilter = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowGridSearchToolbar");
            if (model.DropdownActionWidth == null)
                model.DropdownActionWidth = GetDropdownActionWidthInChars();

            GridDictionaryInfo.ReadGridDictionaryInfo dictInfo = await GridDictionaryInfo.LoadGridColumnDefinitionsAsync(model);

            GridLoadSave.GridSavedSettings gridSavedSettings;
            int pageSize = model.InitialPageSize;
            int initialPage = 0;
            int page = 0;
            if (GridLoadSave.UseGridSettings(model.SettingsModuleGuid)) {
                gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)model.SettingsModuleGuid!, initialPage + 1, pageSize);
                pageSize = gridSavedSettings.PageSize;
                initialPage = gridSavedSettings.CurrentPage - 1;
                page = gridSavedSettings.CurrentPage - 1;
            } else {
                gridSavedSettings = new GridLoadSave.GridSavedSettings {
                    CurrentPage = 1,
                    PageSize = pageSize,
                };
                if (model.InitialFilters != null)
                    gridSavedSettings.Columns = model.InitialFilters;
            }

            GridSetup setup = new GridSetup() {
                FieldName = FieldName,
                AjaxUrl = model.AjaxUrl,
                ShowPager = model.ShowPager,
                Page = page,
                PageSize = pageSize,
                ExtraData = model.ExtraData,
                SizeStyle = model.SizeStyle,
                HighlightCss = "tg_highlight",
                DisabledCss = "tg_disabled",
                RowHighlightCss = "tg_selecthighlight",
                RowDragDropHighlightCss = "tg_dragdrophighlight",
                SortActiveCss = "tg_active",
                SettingsModuleGuid = model.SettingsModuleGuid,
                SaveSettingsColumnWidthsUrl = dictInfo.SaveColumnWidths != false ? Utility.UrlFor(typeof(GridSaveSettingsEndpoints), GridSaveSettingsEndpoints.GridSaveColumnWidths) : null,
                SaveSettingsColumnSelectionUrl = model.PanelHeaderColumnSelection ? Utility.UrlFor(typeof(GridSaveSettingsEndpoints), GridSaveSettingsEndpoints.GridSaveHiddenColumns) : null,
                SaveExpandCollapseUrl = model.PanelHeader && model.PanelCanMinimize ? Utility.UrlFor(typeof(GridPanelSaveSettingsEndpoints), GridPanelSaveSettingsEndpoints.SaveExpandCollapse) : null,
                DeletedMessage = model.DeletedMessage,
                DeleteConfirmationMessage = model.DeleteConfirmationMessage != null && UserSettings.GetProperty<bool>("ConfirmDelete") ? model.DeleteConfirmationMessage : null,
                DeletedColumnDisplay = model.DeletedColumnDisplay,
                CanReorder = model.Reorderable,
                HighlightOnClick = model.HighlightOnClick,
                PanelHeaderSearchColumns = model.PanelHeaderSearchColumns,
            };

            // Data
            if (model.DirectDataAsync == null)
                throw new InternalError($"{nameof(model.DirectDataAsync)} not set in {nameof(GridDefinition)} model");

            List<DataProviderSortInfo>? sorts = gridSavedSettings.GetSortInfo();
            List<DataProviderFilterInfo>? filters = gridSavedSettings.GetFilterInfo();
            // add default sort if no sort provided
            if (sorts == null && !string.IsNullOrWhiteSpace(dictInfo.SortColumn)) {
                // update sort column in saved settings.
                GridDefinition.ColumnInfo sortCol;
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
                    DataSourceResult dsPart = model.SortFilterStaticData.Invoke(setup.StaticData, 0, int.MaxValue, sorts, filters);
                    data = dsPart;
                    data.Total = ds.Total;
                } else {
                    data = ds;
                }
            } else {
                data = await model.DirectDataAsync(page * pageSize, pageSize, sorts, filters);
            }
            // handle async properties
            await GridPartialDataView.HandlePropertiesAsync(data.Data);
            setup.Records = data.Total;

            if (pageSize > 0)
                setup.Pages = data.Total / pageSize + (data.Total % pageSize != 0 ? 1 : 0);
            else
                setup.Pages = 0;

            string tableHTML = await RenderTableHTML(HtmlHelper, model, data, setup.StaticData, dictInfo, gridSavedSettings.Columns, FieldName, true, page * pageSize, pageSize);

            string noSubmitClass = "";
            if (model.IsStatic) {
                setup.NoSubmitContents = (from col in setup.Columns where col.OnlySubmitWhenChecked select col).FirstOrDefault() != null;
                if (setup.NoSubmitContents)
                    noSubmitClass = $" {Forms.CssFormNoSubmitContents}";
            }

            string cssTableStyle = "";
            switch (model.SizeStyle) {
                case GridDefinition.SizeStyleEnum.SizeGiven:
                    cssTableStyle = $" style='width:auto'";// updated client-side
                    break;
                case GridDefinition.SizeStyleEnum.SizeToFit:
                    cssTableStyle = $" style='width:100%'";
                    break;
                case GridDefinition.SizeStyleEnum.SizeAuto:
                    break;
            }

            hb.Append($@"
<div id='{model.Id}' name='{FieldName}' class='yt_grid t_display{noSubmitClass} {(model.UseSkinFormatting ? "tg_skin" : "tg_noskin")} {(gridSavedSettings.Collapsed ? "t_collapsed" : "t_expanded")}'>");

            if (model.PanelHeader) {
                string actions = await HtmlHelper.ForDisplayAsync(model, nameof(model.PanelHeaderActions), UIHint: "ModuleActions");

                hb.Append($@"
    <div class='yGridPanelTitle'>
         <h1>{Utility.HE(model.PanelHeaderTitle)}</h1>
         <div class='{Globals.CssModuleLinksContainer}'>
            {actions}
         </div>
         <div class='yPanelActionsContainer'>");

                if (model.PanelHeaderSearch) {

                    SearchUI searchUI = new SearchUI();

                    hb.Append($@"
            <div class='tg_searchinput' {Basics.CssTooltip}='{HAE(model.PanelHeaderSearchTT)}'>
                {await HtmlHelper.ForEditAsync(searchUI, nameof(SearchUI.__Search), HtmlAttributes: new { AutoDelay = model.PanelHeaderAutoSearch})}
            </div>");
                }

                string reloadHTML = "";
                if (!model.IsStatic && model.SupportReload)
                    reloadHTML = $@"<button class='tg_reload y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnTop", "Reload the current page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sync-alt")}</button>";

                if (!string.IsNullOrEmpty(reloadHTML)) {
                    hb.Append($@"
            <div class='tg_pgbtns'>
                {reloadHTML}
            </div>");
                }

                if (setup.PageSize != 0) {

                    PagerUI pagerUI = GetPagerUI(model, setup);

                    hb.Append($@"
            <div class='tg_pgsel'>
                {await HtmlHelper.ForEditAsync(pagerUI, nameof(PagerUI.__PageSelection))}
            </div>");
                }

                if (model.PanelHeaderColumnSelection) {

                    List<SelectionCheckListDetail> list = GetColumnSelection(model, dictInfo, gridSavedSettings, out List<SelectionCheckListEntry> checkList);
                    ColumnSelectionUI colSelUI = new ColumnSelectionUI() {
                        __ColumnSelection = checkList,
                        __ColumnSelection_List = list,
                    };
                    hb.Append($@"
            <div class='tg_pgcolsel'>
                {await HtmlHelper.ForEditAsync(colSelUI, nameof(ColumnSelectionUI.__ColumnSelection))}
            </div>");
                }

                if (model.PanelCanMinimize) {

                    hb.Append($@"
            <div class='yGridPanelExpColl'>
                <button class='y_buttonlite t_exp' {Basics.CssTooltip}='{Utility.HAE(__ResStr("exp", "Click to expand this panel"))}'>
                    {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-window-maximize")}
                </button>
                <button class='y_buttonlite t_coll' {Basics.CssTooltip}='{Utility.HAE(__ResStr("coll", "Click to collapse this panel"))}'>
                    {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-window-minimize")}
                </button>
            </div>");
                }

                hb.Append($@"
        </div>
    </div>");
            }

            hb.Append($@"
    <div class='tg_table' data-simplebar data-simplebar-auto-hide='true'>
        <table role='presentation'{cssTableStyle}>
            {setup.HeaderHTML}
            <tbody>
{tableHTML}
            </tbody>
        </table>
    </div>");

            if (model.ShowPager) {

                using (Manager.StartNestedComponent(FieldName)) {

                    hb.Append($@"
    <div id='{model.Id}_Pager' class='tg_pager {Forms.CssFormNoSubmitContents}'>
        {await RenderPagerAsync(model, data, gridSavedSettings, dictInfo, setup)}
    </div>");
                }
            }

            // loading
            if (!model.IsStatic) {
                hb.Append($@"
    <div id='{model.Id}_Loading' style='display:none' class='tg_loading'>
        <div class='t_text'>{__ResStr("loading", "Loading ...")}</div>
    </div>");
            }

            hb.Append($@"
</div>");


            Manager.ScriptManager.AddLast($@"
new YetaWF_ComponentsHTML.Grid('{model.Id}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        private enum FilterBoolEnum {
            [EnumDescription("All", "Select all")]
            All = 0,
            [EnumDescription("Yes", "Select selected/enabled entries")]
            Yes = 1,
            [EnumDescription("No", "Select deselected/disabled entries")]
            No = 2,
        }
        private class FilterComplexUI {
            [UIHint("Hidden"), ReadOnly]
            public string Value { get; set; } = null!;
            [UIHint("Hidden"), ReadOnly]
            public string Url { get; set; } = null!;
            [UIHint("Hidden"), ReadOnly]
            public string UIHint { get; set; } = null!;
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
            [UIHint("DropDownListIntNull"), AdditionalMetadata("AdjustWidth", false)]
            public int? Value { get; set; }
            public List<SelectionItem<int?>> Value_List { get; set; } = null!;
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
            public string Value { get; set; } = null!;
        }
        private class FilterStringUI {
            [UIHint("Text"), StringLength(100)]
            public string Value { get; set; } = null!;
        }

        private async Task GetHeadersAsync(GridDefinition gridDef, GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, GridSetup setup, GridLoadSave.GridSavedSettings gridSavedSettings) {

            HtmlBuilder hb = new HtmlBuilder();
            HtmlBuilder filterhb = new HtmlBuilder();

            string cssHead = "";
            if (!gridDef.ShowHeader)
                cssHead = " style='visibility:collapse'";

            hb.Append($@"
<thead{cssHead}>
    <tr class='tg_header'>");

            int colIndex = 0;
            foreach (var d in dictInfo.ColumnInfo) {

                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;

                if (gridCol.Hidden)
                    continue;

                PropertyData prop = ObjectSupport.GetPropertyData(gridDef.RecordType, propName);
                if (prop.UIHint == null)
                    continue;

                gridSavedSettings.Columns.TryGetValue(prop.Name, out GridDefinition.ColumnInfo? columnInfo);

                // Visible
                bool colVisible = columnInfo != null ? columnInfo.Visible : dictInfo.GetColumnStatus(propName) != ColumnVisibilityStatus.NotShown;

                // Caption
                string? caption = prop.GetCaption(gridDef.ResourceRedirect);
                if (!gridCol.Hidden && gridDef.ResourceRedirect != null && string.IsNullOrWhiteSpace(caption))
                    continue;// we need a caption if we're using resource redirects

                // Description
                string? description = prop.GetDescription(gridDef.ResourceRedirect);

                // Width
                int widthPix = 0, widthCh = 0;
                if (gridCol.Icons != 0) {
                    gridCol.Sort = false;

                    Grid.GridActionsEnum actionStyle = UserSettings.GetProperty<Grid.GridActionsEnum>("GridActions");

                    gridCol.ChWidth = gridCol.PixWidth = 0;
                    if (actionStyle == Grid.GridActionsEnum.DropdownMenu) {
                        if (gridCol.Icons <= 1)
                            actionStyle = Grid.GridActionsEnum.Icons;
                        else
                            widthCh = gridDef.DropdownActionWidth ?? 12;
                    }
                    if (actionStyle == Grid.GridActionsEnum.ButtonBar) {
                        widthCh = Math.Abs(gridCol.Icons) * 5;
                    }
                    if (actionStyle == Grid.GridActionsEnum.Icons) {
                        widthPix = 10 + (Math.Abs(gridCol.Icons) * (16 + 4) + 10);
                    }
                }
                if (gridCol.PixWidth != 0)
                    widthPix = gridCol.PixWidth;
                else if (gridCol.ChWidth != 0)
                    widthCh = gridCol.ChWidth;

                GridDefinition.SortBy sort = GridDefinition.SortBy.NotSpecified;
                if (columnInfo != null) {
                    if (columnInfo.Width >= 0)
                        widthPix = columnInfo.Width; // override calculated width

                    if (gridCol.Sort) {
                        if (columnInfo.Sort == GridDefinition.SortBy.Ascending || columnInfo.Sort == GridDefinition.SortBy.Descending)
                            sort = columnInfo.Sort;
                    }
                }

                // Alignment
                string? alignCss = null;
                switch (gridCol.Align) {
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
                if (gridCol.Sort) {
                    setup.CanSort = true;
                    switch (sort) {
                        case GridDefinition.SortBy.NotSpecified:
                            sortHtml = $"<span class='tg_sorticon'><span class='tg_sortboth tg_active'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort")}</span><span class='tg_sortasc'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-up")}</span><span class='tg_sortdesc'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-down")}</span></span>";
                            break;
                        case GridDefinition.SortBy.Ascending:
                            sortHtml = $"<span class='tg_sorticon'><span class='tg_sortboth'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort")}</span><span class='tg_sortasc tg_active'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-up")}</span><span class='tg_sortdesc'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-down")}</span></span>";
                            break;
                        case GridDefinition.SortBy.Descending:
                            sortHtml = $"<span class='tg_sorticon'><span class='tg_sortboth'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort")}</span><span class='tg_sortasc'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-up")}</span><span class='tg_sortdesc tg_active'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sort-down")}</span></span>";
                            break;
                    }
                }

                string resizeHTML = string.Empty;
                if (!gridCol.Locked)
                    resizeHTML = "<span class='tg_resize'>&nbsp;</span>";

                string cssStyle = string.Empty;
                if (gridDef.SizeStyle == GridDefinition.SizeStyleEnum.SizeGiven || gridDef.SizeStyle == GridDefinition.SizeStyleEnum.SizeToFit) {
                    if (widthPix > 0) {
                        widthPix = (widthPix < MIN_COL_WIDTHPIX) ? MIN_COL_WIDTHPIX : widthPix;
                        cssStyle = $"width:{widthPix}px;";
                    } else {
                        widthCh = (widthCh < MIN_COL_WIDTHCH) ? MIN_COL_WIDTHCH : widthCh;
                        cssStyle = $"width:{widthCh}ch;";
                    }
                }
                if (!colVisible)
                    cssStyle += "display:none;";
                if (!string.IsNullOrWhiteSpace(cssStyle))
                    cssStyle = $" style='{cssStyle}'";

                // Render column header
                hb.Append($@"
        <th class='{alignCss} tg_c_{propName.ToLower()}'{cssStyle}>
            <span {Basics.CssTooltipSpan}='{HAE(description ?? "")}'>{HE(caption)}</span>{sortHtml}{resizeHTML}
        </th>");

                List<GridColumnInfo.FilterOptionEnum> filterOpts = new List<GridColumnInfo.FilterOptionEnum>();
                GridColumnInfo.FilterOptionEnum filterOp = GridColumnInfo.FilterOptionEnum.None;
                string filterValue = string.Empty;
                string? filterType = null;
                string? idMenu = null;
                string? idFilter = null;

                if (!gridDef.IsStatic) {

                    filterhb.Append($@"
        <th class='tg_f_{propName.ToLower()}'{(colVisible ? string.Empty : " style='display:none;'")}>");

                    if (gridCol.FilterOptions.Count > 0) {

                        if (columnInfo != null) {
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

                        await YetaWFCoreRendering.Render.AddPopupsAddOnsAsync();// using popups
                        ComplexFilter? complexFilter =  await YetaWFComponentExtender.GetComplexFilterFromUIHintAsync(prop.UIHint);
                        if (complexFilter != null) {

                            filterType = "complex";
                            FilterComplexUI filterUI = new FilterComplexUI {
                                Value = filterValue,
                                Url = complexFilter.Url,
                                UIHint = complexFilter.UIHint,
                            };

                            filterhb.Append($@"
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Url))}
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.UIHint))}
                    <button class='tg_ffilter y_buttonlite_text' tabindex='0'>{HE(__ResStr("filter", "Filter"))}</button>
                    <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>
                </ div>");

                        } else if (prop.PropInfo.PropertyType == typeof(bool) || prop.PropInfo.PropertyType == typeof(bool?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum>();// none
                            filterType = "bool";
                            FilterBoolUI filterUI = new FilterBoolUI {
                                Value = (filterOp != GridColumnInfo.FilterOptionEnum.None) ? (Convert.ToBoolean(filterValue) ? FilterBoolEnum.Yes : FilterBoolEnum.No) : FilterBoolEnum.All,
                            };
                            filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.Equal;

                            filterhb.Append($@"
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>");

                        } else if (prop.PropInfo.PropertyType == typeof(int) || prop.PropInfo.PropertyType == typeof(int?) || prop.PropInfo.PropertyType == typeof(long) || prop.PropInfo.PropertyType == typeof(long?)) {

                            List<SelectionItem<int?>>? entries = await YetaWFComponentExtender.GetSelectionListIntFromUIHintAsync(prop.UIHint);
                            if (entries == null) {
                                // regular int/long
                                filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                    GridColumnInfo.FilterOptionEnum.GreaterEqual, GridColumnInfo.FilterOptionEnum.GreaterThan, GridColumnInfo.FilterOptionEnum.LessEqual, GridColumnInfo.FilterOptionEnum.LessThan,
                                    GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                                };
                                filterType = "long";
                                FilterLongUI filterUI = new FilterLongUI();
                                if (filterOp != GridColumnInfo.FilterOptionEnum.None) {
                                    try {
                                        filterUI.Value = Convert.ToInt64(filterValue);
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");

                            } else {
                                // this is a dynamic enumerated value
                                filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                    GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                                };
                                filterType = "dynenum";
                                entries.Insert(0, new SelectionItem<int?> {
                                    Value = -1,
                                    Text = __ResStr("noSel", "(no selection)")
                                });
                                FilterDynEnumUI filterUI = new FilterDynEnumUI {
                                    Value = 0,
                                    Value_List = entries,
                                };
                                if (filterOp != GridColumnInfo.FilterOptionEnum.None) {
                                    try {
                                        filterUI.Value = Convert.ToInt32(filterValue);
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.Equal;

                                filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
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
                            if (filterOp != GridColumnInfo.FilterOptionEnum.None) {
                                try {
                                    filterUI.Value = Convert.ToDecimal(filterValue);
                                } catch (Exception) { }
                            }
                            filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.GreaterEqual;

                            filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");

                        } else if (prop.PropInfo.PropertyType == typeof(DateTime) || prop.PropInfo.PropertyType == typeof(DateTime?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.GreaterEqual, GridColumnInfo.FilterOptionEnum.LessEqual,
                            };

                            if (prop.UIHint == "DateTime" || prop.UIHint.EndsWith("_DateTime")) {
                                filterType = "datetime";
                                FilterDateTimeUI filterUI = new FilterDateTimeUI();
                                if (filterOp != GridColumnInfo.FilterOptionEnum.None) {
                                    try {
                                        DateTime dt = Convert.ToDateTime(filterValue);
                                        filterUI.Value = YetaWF.Core.Localize.Formatting.GetUtcDateTime(dt);
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");

                            } else if (prop.UIHint == "Date" || prop.UIHint.EndsWith("_Date")) {
                                filterType = "date";
                                FilterDateUI filterUI = new FilterDateUI();
                                if (filterOp != GridColumnInfo.FilterOptionEnum.None) {
                                    try {
                                        DateTime dt = Convert.ToDateTime(filterValue);
                                        filterUI.Value = YetaWF.Core.Localize.Formatting.GetUtcDateTime(dt).Date;
                                    } catch (Exception) { }
                                }
                                filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.GreaterEqual;

                                filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");

                            } else {
                                throw new InternalError("Need DateTime or Date UIHint for DateTime data");
                            }

                        } else if (prop.PropInfo.PropertyType == typeof(Guid) || prop.PropInfo.PropertyType == typeof(Guid?)) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.Contains, GridColumnInfo.FilterOptionEnum.StartsWith, GridColumnInfo.FilterOptionEnum.Endswith
                            };
                            filterType = "guid";
                            FilterGuidUI filterUI = new FilterGuidUI {
                                Value = (filterOp != GridColumnInfo.FilterOptionEnum.None) ? filterValue : string.Empty,
                            };
                            filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.Contains;

                            filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");

                        } else if (prop.PropInfo.PropertyType.IsEnum) {

                            filterOpts = new List<GridColumnInfo.FilterOptionEnum> {
                                GridColumnInfo.FilterOptionEnum.Equal, GridColumnInfo.FilterOptionEnum.NotEqual
                            };
                            filterType = "enum";
                            EnumData enumData = ObjectSupport.GetEnumData(prop.PropInfo.PropertyType);

                            List<SelectionItem<int?>> entries = new List<SelectionItem<int?>>();
                            foreach (EnumDataEntry entry in enumData.Entries) {
                                entries.Add(new SelectionItem<int?> {
                                    Text = entry.Caption,
                                    Tooltip = entry.Description,
                                    Value = (int)entry.Value,
                                });
                            }
                            if ((from e in entries where e.Value == -1 select e).FirstOrDefault() == null) {
                                entries.Insert(0, new SelectionItem<int?> {
                                    Value = -1,
                                    Text = __ResStr("noSel", "(no selection)")
                                });
                            }
                            FilterDynEnumUI filterUI = new FilterDynEnumUI {
                                Value = (filterOp != GridColumnInfo.FilterOptionEnum.None) ? Convert.ToInt32(filterValue) : -1,
                                Value_List = entries,
                            };
                            filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : GridColumnInfo.FilterOptionEnum.Equal;

                            filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>");

                        } else {
                            filterOpts = GetFilterOptions(gridCol);
                            filterType = "text";
                            FilterStringUI filterUI = new FilterStringUI {
                                Value = (filterOp != GridColumnInfo.FilterOptionEnum.None) ? filterValue : string.Empty,
                            };
                            filterOp = filterOp != GridColumnInfo.FilterOptionEnum.None ? filterOp : filterOpts.First();

                            filterhb.Append($@"
                <button class='tg_fmenu y_buttonlite_text' {Basics.CssTooltip}='{HAE(searchToolTip)}'><span>{HE(GetFilterIcon(filterOp))}</span></button>
                <div class='tg_fctrls'>
                    {await HtmlHelper.ForEditAsync(filterUI, nameof(filterUI.Value), HtmlAttributes: new { id = idFilter })}
                </div>
                <button class='tg_fclear y_buttonlite' tabindex='0'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-times")}</button>");
                        }

                    } else {
                        filterhb.Append($@"
            &nbsp;");
                    }
                    filterhb.Append($@"
            {(filterOp != GridColumnInfo.FilterOptionEnum.None && idMenu != null ? GetFilterMenu(gridDef, filterOpts, filterOp, idMenu, colIndex) : null)}
        </th>");

                }

                // Build column definition
                setup.Columns.Add(new GridColumnDefinition {
                    Name = prop.Name,
                    Sortable = gridCol.Sort,
                    Sort = sort,
                    OnlySubmitWhenChecked = gridCol.OnlySubmitWhenChecked,
                    Locked = gridCol.Locked,
                    FilterOp = filterOp,
                    FilterType = filterType,
                    FilterId = idFilter,
                    MenuId = idMenu,
                    Visible = colVisible,
                });

                ++colIndex;
            }

            if (filterhb.Length > 0) {

                setup.CanFilter = true;

                string filterStyle = "";
                if (gridDef.ShowFilter != true)
                    filterStyle = " style='display:none'";

                hb.Append($@"
    </tr>
    <tr class='tg_filter'{filterStyle}>
        {filterhb.ToString()}
    </tr>
</thead>");
            } else {
                hb.Append($@"
    </tr>
</thead>");
            }

            setup.HeaderHTML = hb.ToString();
        }

        private string GetFilterMenu(GridDefinition gridModel, List<GridColumnInfo.FilterOptionEnum> filterOpts, GridColumnInfo.FilterOptionEnum? filterOp, string idMenu, int colIndex) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($"<ul id='{idMenu}' style='display:none'>");
            foreach (GridColumnInfo.FilterOptionEnum option in filterOpts) {
                string icon = GetFilterIcon(option);
                string text = GetFilterText(option);
                hb.Append($"<li data-sel='{(int)option}'><a href='#'><span class='t_fmenuicon'>{HE(icon)}</span><span class='t_fmenutext'>{HE(text)}</span></a></li>");
            }
            hb.Append("</ul>");
            return hb.ToString();
        }

        private GridColumnInfo.FilterOptionEnum GetFilterOptionEnum(string filterOp) {
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
                case "Complex": return GridColumnInfo.FilterOptionEnum.None;
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

        private List<SelectionCheckListDetail> GetColumnSelection(GridDefinition gridDef, GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, GridLoadSave.GridSavedSettings gridSavedSettings, out List<SelectionCheckListEntry> checkList) {

            List<SelectionCheckListDetail> list = new List<SelectionCheckListDetail>();
            checkList = new List<SelectionCheckListEntry>();

            foreach (KeyValuePair<string, GridColumnInfo> d in dictInfo.ColumnInfo) {

                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;

                if (gridCol.Hidden)
                    continue;

                PropertyData prop = ObjectSupport.GetPropertyData(gridDef.RecordType, propName);

                gridSavedSettings.Columns.TryGetValue(propName, out GridDefinition.ColumnInfo? columnInfo);
                ColumnVisibilityStatus colStatus = dictInfo.GetColumnStatus(propName);

                // Caption
                string? caption = prop.GetCaption(gridDef.ResourceRedirect);
                if (!gridCol.Hidden && gridDef.ResourceRedirect != null && string.IsNullOrWhiteSpace(caption))
                    continue;// we need a caption if we're using resource redirects

                // Description
                string? description = prop.GetDescription(gridDef.ResourceRedirect);

                list.Add(new SelectionCheckListDetail { Key = propName, Text = caption, Description = description, Enabled = colStatus != ColumnVisibilityStatus.AlwaysShown });

                // Visible
                bool colVisible = columnInfo != null ? columnInfo.Visible : colStatus != ColumnVisibilityStatus.NotShown;
                checkList.Add(new SelectionCheckListEntry { Key = propName, Value = colVisible });
            }
            return list;
        }

        internal static async Task<string> RenderTableHTML(YHtmlHelper htmlHelper,
                GridDefinition model, DataSourceResult data, List<object>? staticData, GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, GridDefinition.ColumnDictionary colDict, string fieldPrefix, bool readOnly, int skip, int take) {

            HtmlBuilder hb = new HtmlBuilder();

            if (data.Total == 0 || model.IsStatic) {

                string styleCss = "";
                if (data.Total > 0)
                    styleCss = " style='display:none'";

                hb.Append($@"
<tr role='row'{styleCss} class='tg_emptytr'>
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

                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, model, dictInfo, colDict, fieldPrefix, record, recordCount, origin, hide));
                    ++recordCount;
                }
            } else {
                if (!Manager.IsPostRequest) {
                    // when initially rendering a grid with 0 records, we have to prepare for all templates
                    await YetaWFComponentExtender.AddComponentSupportForType(model.RecordType);
                }
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordHTMLAsync(YHtmlHelper htmlHelper,
                GridDefinition gridModel, GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, GridDefinition.ColumnDictionary colDict, string fieldPrefix, object record, int recordCount, int origin, bool hide) {

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
                            output = await htmlHelper.ForEditComponentAsync(record, colName, value, prop.UIHint);
                            output += htmlHelper.ValidationMessage(Manager.NestedComponentPrefix, colName);
                        } else {
                            output = await htmlHelper.ForDisplayComponentAsync(record, colName, value, prop.UIHint);
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
<tr role='row'{originData} class='{lightCss}'{trStyle} tabindex='0'>");

                foreach (string colName in dictInfo.ColumnInfo.Keys) {

                    GridColumnInfo gridCol = dictInfo.ColumnInfo[colName];
                    if (!gridCol.Hidden) {

                        colDict.TryGetValue(colName, out GridDefinition.ColumnInfo? columnInfo);
                        bool colVisible = columnInfo != null ? columnInfo.Visible : dictInfo.GetColumnStatus(colName) != ColumnVisibilityStatus.NotShown;

                        if (colVisible || gridModel.IsStatic) {
                            PropertyData prop = ObjectSupport.GetPropertyData(recordType, colName);
                            object value = prop.GetPropertyValue<object>(record);

                            if (prop.UIHint == null)
                                continue;
                            if (gridModel.ResourceRedirect != null && string.IsNullOrWhiteSpace(prop.GetCaption(gridModel.ResourceRedirect)))
                                continue;// we need a caption if we're using resource redirects

                            // Alignment
                            string? tdCss = null;
                            switch (gridCol.Align) {
                                case GridHAlignmentEnum.Unspecified:
                                case GridHAlignmentEnum.Left:
                                    tdCss = "tg_left";
                                    break;
                                case GridHAlignmentEnum.Center:
                                    tdCss = "tg_center";
                                    break;
                                case GridHAlignmentEnum.Right:
                                    tdCss = "tg_right";
                                    break;
                            }

                            // Truncate
                            if (gridCol.Truncate && gridCol.Icons == 0) // the icon column cannot be truncated
                                tdCss = CssManager.CombineCss(tdCss, "tg_truncate");

                            hb.Append($@"
    <td role='gridcell' class='{tdCss} tg_c_{colName.ToLower()}'{(!colVisible ? " style='display:none;'" : null)}>");

                            if (hbHidden.Length > 0) { // add all hidden fields to first cell
                                hb.Append(hbHidden.ToString());
                                hbHidden = new HtmlBuilder();
                            }

                            string output;
                            if ((YetaWFManager.IsDemo || Manager.IsDemoUser) && prop.HasAttribute(nameof(ExcludeDemoModeAttribute))) {
                                output = __ResStr("demo", "(Demo - N/A)");
                            } else if (recordEnabled && !prop.ReadOnly) {
                                output = await htmlHelper.ForEditComponentAsync(record, colName, value, prop.UIHint);
                                output += htmlHelper.ValidationMessage(Manager.NestedComponentPrefix, colName);
                            } else {
                                output = await htmlHelper.ForDisplayComponentAsync(record, colName, value, prop.UIHint);
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
            public List<SelectionItem<int>> __PageSelection_List { get; set; } = null!;
        }

        private async Task<string> RenderPagerAsync(GridDefinition gridModel, DataSourceResult data, GridLoadSave.GridSavedSettings gridSavedSettings, GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, GridSetup setup) {

            HtmlBuilder hb = new HtmlBuilder();

            PagerUI pagerUI = GetPagerUI(gridModel, setup);

            string reloadHTML = "";
            string searchHTML = "";
            if (!gridModel.PanelHeader && !gridModel.IsStatic && gridModel.SupportReload)
                reloadHTML = $@"<button class='tg_reload y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnTop", "Reload the current page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-sync-alt")}</button>";
            if (setup.CanFilter)
                searchHTML = $@"<button class='tg_search y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnFilter", "Turn the filter bar on or off"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-search")}</button>";

            if (!string.IsNullOrEmpty(reloadHTML) || !string.IsNullOrEmpty(searchHTML)) {
                hb.Append($@"
    <div class='tg_pgbtns'>
        {reloadHTML}{searchHTML}
    </div>");
            }

            if (setup.PageSize != 0) {

                string topHTML = $@"<button class='tg_pgtop y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnFirst", "Go to the first page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-fast-backward")}</button>";
                string prevHTML = $@"<button class='tg_pgprev y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnPrev", "Go to the previous page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-backward")}</span></button>";
                string nextHTML = $@"<button class='tg_pgnext y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnNext", "Go to the next page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-forward")}</button>";
                string bottomHTML = $@"<button class='tg_pgbottom y_buttonlite' {Basics.CssTooltip}='{HAE(__ResStr("btnLast", "Go to the last page"))}'>{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-fast-forward")}</button>";

                hb.Append($@"
    <div class='tg_pgctl'>
        {topHTML}{prevHTML}
        <div class='tg_pgnum'>
            {await HtmlHelper.ForLabelAsync(pagerUI, nameof(PagerUI.__Page))}
            {await HtmlHelper.ForEditAsync(pagerUI, nameof(PagerUI.__Page))}
        </div>
        {nextHTML}{bottomHTML}
    </div>");

                if (!gridModel.PanelHeader) {
                    hb.Append($@"
    <div class='tg_pgsel'>
        {await HtmlHelper.ForLabelAsync(pagerUI, nameof(PagerUI.__PageSelection))}
        {await HtmlHelper.ForEditAsync(pagerUI, nameof(PagerUI.__PageSelection))}
    </div>");
                }
            }

            hb.Append($@"
    <div class='tg_totals'>
    </div>");

            return hb.ToString();
        }

        private static PagerUI GetPagerUI(GridDefinition gridModel, GridSetup setup) {
            List<SelectionItem<int>> selList = new List<SelectionItem<int>>();
            foreach (int size in gridModel.PageSizes) {
                if (size == GridDefinition.AllPages) {
                    selList.Add(new SelectionItem<int> {
                        Text = __ResStr("allPages", "All"),
                        Tooltip = __ResStr("allPagesTT", "Show all entries"),
                        Value = size,
                    });
                } else {
                    selList.Add(new SelectionItem<int> {
                        Text = size.ToString(),
                        Tooltip = __ResStr("pagesTT", "Show {0} entries", size),
                        Value = size,
                    });
                }
            }

            PagerUI pagerUI = new PagerUI {
                __Page = setup.Page + 1,
                __PageSelection = setup.PageSize,
                __PageSelection_List = selList,
            };
            return pagerUI;
        }

        internal int GetDropdownActionWidthInChars() {
            string s = __ResStr("dropdownWidth", "12");
            return Convert.ToInt32(s);
        }

        // Custom serializer to minimize static data being transferred

        internal class StaticDataConverter : JsonConverter<List<object>> {

            public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, List<object> value, JsonSerializerOptions options) {
                string array = Utility.JsonSerialize(value, Utility._JsonSettingsGetSetUIHint);
                writer.WriteRawValue(array);
            }
        }
    }
}

