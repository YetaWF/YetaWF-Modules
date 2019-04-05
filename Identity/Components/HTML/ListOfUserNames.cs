/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Components {

    public abstract class ListOfUserNamesComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfUserNamesComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ListOfUserNames";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfUserNamesDisplayComponent : ListOfUserNamesComponentBase, IYetaWFComponent<SerializableList<User>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Entry {

            [Caption("User Names"), Description("Shows all defined user names")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            public Entry(string userName) {
                UserName = userName;
            }
        }
        internal static GridDefinition GetGridModel(bool header, bool pager, bool useSkin) {
            return new GridDefinition() {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                HighlightOnClick = false,
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                ShowPager = pager,
                UseSkinFormatting = useSkin,
                AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesController), nameof(ListOfUserNamesController.ListOfUserNamesDisplay_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
            };
        }

        public async Task<string> RenderAsync(SerializableList<User> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);
            bool pager = PropData.GetAdditionalAttributeValue("Pager", true);
            bool useSkin = PropData.GetAdditionalAttributeValue("UseSkinFormatting", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header, pager, useSkin)
            };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<Entry> list = new List<Entry>();
                using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                    if (model != null) {
                        foreach (User user in model) {
                            UserDefinition userDef = await userDP.GetItemByUserIdAsync(user.UserId);
                            string userName;
                            if (userDef == null)
                                userName = __ResStr("noUser", "({0})", user.UserId);
                            else
                                userName = userDef.UserName;
                            list.Add(new Entry(userName));
                        }
                    }
                }
                return new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                };
            };

            hb.Append($@"
<div class='yt_listofusernames t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

            return hb.ToString();
        }
    }

    public class ListOfUserNamesEditComponent : ListOfUserNamesComponentBase, IYetaWFComponent<SerializableList<User>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ListOfUserNamesSetup {
            public string GridId { get; set; }
            public string AddUrl { get; set; }
            public string GridAllId { get; internal set; }
        }
        public class NewModel {
            [Caption("User Name"), Description("Please enter a new user name and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }

        public class Entry {

            [Caption("Delete"), Description("Click to remove this user name from the list")]
            [UIHint("GridDeleteEntry"), ReadOnly]
            public int Delete { get; set; }

            [Caption("User Name"), Description("Shows all defined user names")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public int UserId { get; set; }

            public Entry(int userId, string userName) {
                UserId = userId;
                UserName = userName;
            }
            public Entry() { }
        }

        public class AllEntry {

            public AllEntry() { }

            [Caption("User Name"), Description("Defines the user name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            public AllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }

        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                RecordType = typeof(Entry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesController), nameof(ListOfUserNamesController.ListOfUserNamesEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DeletedMessage = __ResStr("removeMsg", "User name {0} has been removed"),
                DeleteConfirmationMessage = __ResStr("confimMsg", "Are you sure you want to remove user name {0}?"),
                DeletedColumnDisplay = nameof(Entry.UserName),
            };
        }
        internal static GridDefinition GetGridAllUsersModel() {
            return new GridDefinition() {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                RecordType = typeof(AllEntry),
                InitialPageSize = 10,
                AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesController), nameof(ListOfUserNamesController.ListOfUserNamesBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                        DataProviderGetRecords<UserDefinition> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new ListOfUserNamesEditComponent.AllEntry(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public async Task<string> RenderAsync(SerializableList<User> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<Entry> list = new List<Entry>();
                using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                    if (model != null) {
                        foreach (User user in model) {
                            UserDefinition userDef = await userDP.GetItemByUserIdAsync(user.UserId);
                            string userName;
                            if (userDef == null)
                                userName = __ResStr("noUser", "({0})", user.UserId);
                            else
                                userName = userDef.UserName;
                            list.Add(new Entry(user.UserId, userName));
                        }
                    }
                }
                return new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count
                };
            };

            hb.Append($@"
<div class='yt_listofusernames t_edit' id='{DivId}'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}");

            using (Manager.StartNestedComponent(FieldName)) {

                NewModel newModel = new NewModel();
                hb.Append($@"
    <div class='t_newvalue'>
        {await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue))}
        {await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue), Validation: false)}
        <input name='btnAdd' type='button' value='Add' disabled='disabled' />
    </div>");

            }

            GridModel gridAll = new GridModel() {
                GridDef = GetGridAllUsersModel()
            };
            ListOfUserNamesSetup setup = new ListOfUserNamesSetup {
                AddUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesController), nameof(ListOfUserNamesController.AddUserName)),
                GridId = grid.GridDef.Id,
                GridAllId = gridAll.GridDef.Id
            };

            hb.Append($@"
    <div id='{DivId}_coll'>
        {await ModuleActionHelper.BuiltIn_ExpandAction(__ResStr("lblFindUsers", "Find Users"), __ResStr("ttFindUsers", "Expand to find user names available on this site")).RenderAsNormalLinkAsync() }
    </div>
    <div id='{DivId}_exp' style='display:none'>
        {await ModuleActionHelper.BuiltIn_CollapseAction(__ResStr("lblAllUserNames", "All User Names"), __ResStr("ttAllUserNames", "Shows all user names available on this site - Select a user name to update the text box above, so the user name can be added to the list of user names - Click to close")).RenderAsNormalLinkAsync() }
        {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, gridAll, nameof(gridAll.GridDef), gridAll.GridDef, "Grid")}
    </div>
</div>");

            Manager.ScriptManager.AddLast($@"
$YetaWF.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');
new YetaWF_Identity.ListOfUserNamesEditComponent('{DivId}', {YetaWFManager.JsonSerialize(setup)});");

            return hb.ToString();
        }
        public static async Task<GridRecordData> GridRecordAsync(string fieldPrefix, object model) {
            // handle async properties
            await YetaWFController.HandlePropertiesAsync(model);
            GridRecordData record = new GridRecordData() {
                GridDef = GetGridModel(false),
                Data = model,
                FieldPrefix = fieldPrefix,
            };
            return record;
        }
    }
}
