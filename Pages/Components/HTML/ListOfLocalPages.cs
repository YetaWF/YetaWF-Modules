/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Modules.Pages.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Components {

    public abstract class ListOfLocalPagesComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfLocalPagesComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ListOfLocalPages";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Pages package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class ListOfLocalPagesDisplayComponent : ListOfLocalPagesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {

            [Caption("Page"), Description("Shows all page Urls that are part of the Unified Page Set")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; }

            public Entry(string url) {
                Url = url;
            }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 20,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ListOfLocalPagesController), nameof(ListOfLocalPagesController.ListOfLocalPagesDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<string> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                model = model ?? new List<string>();
                List<Entry> list = (from u in model select new Entry(u)).ToList();
                return Task.FromResult(new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                });
            };

            hb.Append($@"
<div class='yt_yetawf_pages_listoflocalpages t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.Pages package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class ListOfLocalPagesEditComponent : ListOfLocalPagesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ListOfLocalPagesSetup {
            public string GridId { get; set; } = null!;
            public string AddUrl { get; set; } = null!;
            public string GridAllId { get; set; } = null!;
        }
        public class NewModel {
            [Caption("Page"), Description("Please select a page and click Add to add it to the list of pages")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlTypeEnum.Local), Trim]
            public string? NewValue { get; set; }
        }

        public class Entry {

            [Caption("Delete"), Description("Click to remove this page from the list")]
            [UIHint("GridDeleteEntry"), ReadOnly]
            public int Delete { get; set; }

            [Caption("Page"), Description("Shows all pages")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; } = null!;

            [UIHint("GridValue"), ReadOnly]
            public string Value { get { return Url; } }

            public Entry(string url) {
                Url = url;
            }
            public Entry() { }
        }

        public class AllEntry {

            public AllEntry() { }

            [Caption("Page"), Description("Defines the page name")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; } = null!;

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; } = null!;

            public AllEntry(PageDefinition page) {
                ObjectSupport.CopyData(page, this);
            }
        }

        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                Reorderable = true,
                RecordType = typeof(Entry),
                InitialPageSize = 0,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ListOfLocalPagesController), nameof(ListOfLocalPagesController.ListOfLocalPagesEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DeletedMessage = __ResStr("removeMsg", "Page {0} has been removed"),
                DeleteConfirmationMessage = __ResStr("confimMsg", "Are you sure you want to remove page {0}?"),
                DeletedColumnDisplay = nameof(Entry.Url),
            };
        }
        internal static GridDefinition GetGridAllUsersModel() {
            return new GridDefinition() {
                RecordType = typeof(AllEntry),
                InitialPageSize = 10,
                AjaxUrl = Utility.UrlFor(typeof(ListOfLocalPagesController), nameof(ListOfLocalPagesController.ListOfLocalPagesBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (PageDefinitionDataProvider pagesDP = new PageDefinitionDataProvider()) {
                        DataProviderGetRecords<PageDefinition> browseItems = await pagesDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new AllEntry(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public async Task<string> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                model = model ?? new List<string>();
                List<Entry> list = (from u in model select new Entry(u)).ToList();
                return Task.FromResult(new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                });
            };

            hb.Append($@"
<div class='yt_yetawf_pages_listoflocalpages t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}");

            using (Manager.StartNestedComponent(FieldName)) {

                NewModel newModel = new NewModel();
                hb.Append($@"
    <div class='t_newvalue'>
        {await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue))}
        {await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue))}
        <input name='btnAdd' type='button' class='y_button' value='Add' disabled='disabled' />
    </div>");

            }

            GridModel gridAll = new GridModel() {
                GridDef = GetGridAllUsersModel()
            };
            ListOfLocalPagesSetup setup = new ListOfLocalPagesSetup {
                AddUrl = GetSiblingProperty<string>($"{PropertyName}_AjaxUrl"),
                GridId = grid.GridDef.Id,
                GridAllId = gridAll.GridDef.Id
            };

            hb.Append($@"
    <div id='{DivId}_coll'>
        {await ModuleActionHelper.BuiltIn_ExpandAction(__ResStr("lblFindPages", "Find Pages"), __ResStr("ttFindPages", "Expand to find pages available for this site")).RenderAsNormalLinkAsync() }
    </div>
    <div id='{DivId}_exp' style='display:none'>
        {await ModuleActionHelper.BuiltIn_CollapseAction(__ResStr("lblAllPages", "All Pages"), __ResStr("ttAllPages", "Shows all designed pages available in the site - Select a page to update the dropdown list above, so the page can be added to the list of pages")).RenderAsNormalLinkAsync() }
        {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, gridAll, nameof(gridAll.GridDef), gridAll.GridDef, "Grid")}
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"
$YetaWF.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');
new YetaWF_Pages.ListOfLocalPagesEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
        public static Task<GridRecordData> GridRecordAsync(string fieldPrefix, object model) {
            GridRecordData record = new GridRecordData() {
                GridDef = GetGridModel(false),
                Data = model,
                FieldPrefix = fieldPrefix,
            };
            return Task.FromResult(record);
        }
    }
}
