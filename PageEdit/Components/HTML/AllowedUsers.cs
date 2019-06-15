/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.PageEdit.Controllers;

namespace YetaWF.Modules.PageEdit.Components {

    public abstract class AllowedUsersComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(AllowedUsersComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "AllowedUsers";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class AllowedUsersEditComponent : AllowedUsersComponentBase, IYetaWFComponent<SerializableList<PageDefinition.AllowedUser>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class AllowedUsersSetup {
            public string GridId { get; set; }
            public string AddUrl { get; set; }
            public string GridAllId { get; internal set; }
        }
        public class NewModel {
            [Caption("New User"), Description("Enter a new user name and click Add")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), Trim]
            public string NewValue { get; set; }
        }

        public class GridAllowedUser {

            [Caption("Delete"), Description("Click to delete a user")]
            [UIHint("GridDeleteEntry"), ReadOnly]
            public int Delete { get; set; }

            [Caption("User"), Description("User Description")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int DisplayUserId { get; set; }

            [Caption("View"), Description("The user has permission to view the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum View { get; set; }

            [Caption("Edit"), Description("The user has permission to edit the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Edit { get; set; }

            [Caption("Remove"), Description("The user has permission to remove the page")]
            [UIHint("Enum")]
            public PageDefinition.AllowedEnum Remove { get; set; }

            [UIHint("Hidden"), ReadOnly]
            public int UserId { get; set; }
            [UIHint("Hidden"), ReadOnly]
            public string UserName { get; set; }

            public GridAllowedUser(PageDefinition.AllowedUser allowedUser, string userName) {
                ObjectSupport.CopyData(allowedUser, this);
                UserId = DisplayUserId = allowedUser.UserId;
                UserName = userName;
            }
            public GridAllowedUser(int userId, string userName) {
                UserId = DisplayUserId = userId;
                UserName = userName;
                View = PageDefinition.AllowedEnum.Yes;
            }
            public GridAllowedUser() { }
        }

            public class AllEntry {

            [Caption("User"), Description("User Name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            public AllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(GridAllowedUser),
                PageSizes = new List<int>() { 5, 10, 20 },
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AllowedUsersEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<GridAllowedUser> recs = DataProviderImpl<GridAllowedUser>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DeletedMessage = __ResStr("removeMsg", "User {0} has been removed"),
                DeleteConfirmationMessage = __ResStr("confimMsg", "Are you sure you want to remove user {0}?"),
                DeletedColumnDisplay = nameof(GridAllowedUser.UserName),
            };
        }
        internal static GridDefinition GetGridAllUsersModel() {
            return new GridDefinition() {
                RecordType = typeof(AllEntry),
                InitialPageSize = 10,
                AjaxUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AllowedUsersBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                        DataProviderGetRecords<UserDefinition> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new AllowedUsersEditComponent.AllEntry(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        public async Task<string> RenderAsync(SerializableList<PageDefinition.AllowedUser> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                List<GridAllowedUser> list;
                if (model != null) {
                    list = new List<GridAllowedUser>();
                    foreach (PageDefinition.AllowedUser allowedUser in model) {
                        string userName = await Resource.ResourceAccess.GetUserNameAsync(allowedUser.UserId);
                        list.Add(new GridAllowedUser(allowedUser, userName));
                    }
                } else
                    list = new List<GridAllowedUser>();
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return data;
            };

            hb.Append($@"
<div class='yt_yetawf_pageedit_allowedusers t_edit' id='{DivId}'>
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

            GridModel gridAll = new GridModel() {
                GridDef = GetGridAllUsersModel()
            };
            AllowedUsersSetup setup = new AllowedUsersSetup {
                AddUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AddUserToPage)),
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
new YetaWF_PageEdit.AllowedUsersEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

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
