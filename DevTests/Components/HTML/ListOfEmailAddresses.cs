/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.DevTests.Controllers;

namespace YetaWF.Modules.DevTests.Components {

    public abstract class ListOfEmailAddressesComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfEmailAddressesComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ListOfEmailAddresses";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// A sample implementation of a component that displays a list of email addresses in a grid. The model contains the list of email addresses.
    /// </summary>
    /// <remarks>
    /// The test sample page is available at Tests > Components > ListOfEmailAddresses (standard YetaWF site).
    /// </remarks>
    /// <example>
    /// [Caption("Email Addresses"), Description("List of email addresses")]
    /// [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ReadOnly]
    /// public List&lt;string&gt; EmailAddresses { get; set; }
    /// </example>
    public class ListOfEmailAddressesDisplayComponent : ListOfEmailAddressesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {
            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("String"), ReadOnly]
            public string EmailAddress { get; set; }

            public Entry(string text) {
                EmailAddress = text;
            }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 5,
                PageSizes = new List<int> { 5, 10, 20 },
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ListOfEmailAddressesController), nameof(ListOfEmailAddressesController.ListOfEmailAddressesDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
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
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<Entry> list = new List<Entry>();
                if (model != null)
                    list = (from u in model select new Entry(u)).ToList();
                return Task.FromResult(new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                });
            };

            hb.Append($@"
<div class='yt_yetawf_devtests_listofemailaddresses t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }
    /// <summary>
    /// A sample implementation of a component that allows selecting/entering a list of email addresses in a grid. The model contains the list of email addresses.
    /// </summary>
    /// <remarks>
    /// The test sample page is available at Tests > Components > ListOfEmailAddresses (standard YetaWF site).
    /// </remarks>
    /// <example>
    /// [Caption("Email Addresses"), Description("List of email addresses")]
    /// [UIHint("YetaWF_DevTests_ListOfEmailAddresses")]
    /// public List&lt;string&gt; EmailAddresses { get; set; }
    /// </example>
    public class ListOfEmailAddressesEditComponent : ListOfEmailAddressesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ListOfEmailAddressesSetup {
            public string GridId { get; set; }
            public string AddUrl { get; set; }
        }

        public class NewModel {
            [Caption("Email Address"), Description("Please enter a new email address and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }

        public class Entry {

            [Caption("Delete"), Description("Click to remove this email address from the list")]
            [UIHint("GridDeleteEntry"), ReadOnly]
            public int Delete { get; set; }

            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("String"), ReadOnly]
            public string EmailAddress { get; set; }

            [UIHint("GridValue"), ReadOnly]
            public string Value { get { return EmailAddress; } }

            public Entry(string text) {
                EmailAddress = text;
            }
            public Entry() { }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(Entry),
                InitialPageSize = 5,
                PageSizes = new List<int> { 5, 10, 20 },
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(ListOfEmailAddressesController), nameof(ListOfEmailAddressesController.ListOfEmailAddressesEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DeletedMessage = __ResStr("removeMsg", "Email address {0} has been removed"),
                DeleteConfirmationMessage = __ResStr("confimMsg", "Are you sure you want to remove email address {0}?"),
                DeletedColumnDisplay = nameof(Entry.EmailAddress),
            };
        }

        public async Task<string> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<Entry> list = new List<Entry>();
                if (model != null)
                    list = (from u in model select new Entry(u)).ToList();
                return Task.FromResult(new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                });
            };

            hb.Append($@"
<div class='yt_yetawf_devtests_listofemailaddresses t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}");

            using (Manager.StartNestedComponent(FieldName)) {

                NewModel newModel = new NewModel();
                hb.Append($@"
    <div class='t_newvalue'>
        {await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue))}
        {await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue))}
        <input name='btnAdd' type='button' value='Add' disabled='disabled' />
    </div>");

            }

            ListOfEmailAddressesSetup setup = new ListOfEmailAddressesSetup {
                AddUrl = GetSiblingProperty<string>($"{PropertyName}_AjaxUrl"),
                GridId = grid.GridDef.Id,
            };

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_DevTests.ListOfEmailAddressesEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

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
