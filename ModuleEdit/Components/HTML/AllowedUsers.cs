/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
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
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.ModuleEdit.Controllers;

namespace YetaWF.Modules.ModuleEdit.Components {

    public abstract class AllowedUsersComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(AllowedUsersComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "AllowedUsers";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.ModuleEdit package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class AllowedUsersEditComponent : AllowedUsersComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.AllowedUser>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class AllowedUsersSetup {
            public string GridId { get; set; } = null!;
            public string AddUrl { get; set; } = null!;
            public string GridAllId { get; set; } = null!;
        }

        public class NewModel {
            [Caption("New User"), Description("Enter a new user name and click Add")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), Trim]
            public string? NewValue { get; set; }
        }
        public class ExtraData {
            public Guid EditGuid { get; set; }
        }

        public class AllEntry {

            [Caption("User"), Description("User Name")]
            [UIHint("String"), ReadOnly]
            public string? UserName { get; set; }

            public AllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        internal static GridDefinition GetGridModel(bool header) {
            return new GridDefinition() {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ResourceRedirect = Manager.CurrentModuleEdited,
                RecordType = typeof(ModuleDefinition.GridAllowedUser),
                PageSizes = new List<int>() { 5, 10, 20 },
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AllowedUsersEdit_SortFilter)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<ModuleDefinition.GridAllowedUser> recs = DataProviderImpl<ModuleDefinition.GridAllowedUser>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DeletedMessage = __ResStr("removeMsg", "User {0} has been removed"),
                DeleteConfirmationMessage = __ResStr("confimMsg", "Are you sure you want to remove user {0}?"),
                DeletedColumnDisplay = nameof(ModuleDefinition.GridAllowedUser.DisplayUserName),
            };
        }
        internal static GridDefinition GetGridAllUsersModel() {
            return new GridDefinition() {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                RecordType = typeof(AllEntry),
                InitialPageSize = 10,
                AjaxUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AllowedUsersBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
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

        public async Task<string> RenderAsync(SerializableList<ModuleDefinition.AllowedUser> model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            GridModel grid = new GridModel() {
                GridDef = GetGridModel(header)
            };
            grid.GridDef.ExtraData = new ExtraData { EditGuid = Manager.CurrentModuleEdited!.ModuleGuid };
            grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                List<ModuleDefinition.GridAllowedUser> list = new List<ModuleDefinition.GridAllowedUser>();
                if (model != null) {
                    foreach (ModuleDefinition.AllowedUser u in model) {
                        ModuleDefinition.GridAllowedUser user = new ModuleDefinition.GridAllowedUser();
                        ObjectSupport.CopyData(u, user);
                        await user.SetUserAsync(u.UserId);
                        list.Add(user);
                    }
                }
                DataSourceResult data = new DataSourceResult {
                    Data = list.ToList<object>(),
                    Total = list.Count,
                };
                return data;
            };

            hb.Append($@"
<div class='yt_yetawf_moduleedit_allowedusers t_edit' id='{DivId}'>
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
            AllowedUsersSetup setup = new AllowedUsersSetup {
                AddUrl = Utility.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AddUserToModule)),
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
new YetaWF_ModuleEdit.AllowedUsersEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
        public static async Task<GridRecordData> GridRecordAsync(string fieldPrefix, object model, Guid editGuid) {
            GridRecordData record = new GridRecordData() {
                GridDef = GetGridModel(false),
                Data = model,
                FieldPrefix = fieldPrefix,
            };
            // module being edited
            ModuleDefinition? module = await ModuleDefinition.LoadAsync(editGuid);
            record.GridDef.ResourceRedirect = module;

            return record;
        }
    }
}
