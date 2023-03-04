/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;

#nullable enable

namespace YetaWF.Modules.Identity.Components;

public abstract class ResourceUsersComponentBase : YetaWFComponent {

    protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ResourceUsersComponentBase), name, defaultValue, parms); }

    public const string TemplateName = "ResourceUsers";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetTemplateName() { return TemplateName; }
}

/// <summary>
/// This component is used by the YetaWF.Identity package and is not intended for use by an application.
/// </summary>
[PrivateComponent]
public class ResourceUsersDisplayComponent : ResourceUsersComponentBase, IYetaWFComponent<List<User>?> {

    public override ComponentType GetComponentType() { return ComponentType.Display; }

    public class Entry {

        [Caption("User"), Description("User Name")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        public Entry(int userId) {
            UserId = userId;
        }
        public Entry() { }
    }
    internal static GridDefinition GetGridModel(bool header) {
        return new GridDefinition() {
            RecordType = typeof(Entry),
            InitialPageSize = 5,
            ShowHeader = header,
            AjaxUrl = Utility.UrlFor(typeof(ResourceUsersEndpoints), GridSupport.DisplaySortFilter),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<Entry> recs = DataProviderImpl<Entry>.GetRecords(data, skip, take, sorts, filters);
                return new DataSourceResult {
                    Data = recs.Data.ToList<object>(),
                    Total = recs.Total,
                };
            },
        };
    }

    public async Task<string> RenderAsync(List<User>? model) {

        HtmlBuilder hb = new HtmlBuilder();

        bool header = PropData.GetAdditionalAttributeValue("Header", true);

        GridModel grid = new GridModel() {
            GridDef = GetGridModel(header)
        };
        grid.GridDef.DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
            List<Entry> users;
            if (model == null)
                users = new List<Entry>();
            else
                users = (from u in model select new Entry(u.UserId)).ToList();

            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            return Task.FromResult(data);
        };

        hb.Append($@"
<div class='yt_yetawf_identity_resourceusers t_display'>
    {await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes)}
</div>");

        return hb.ToString();
    }
}
/// <summary>
/// This component is used by the YetaWF.Identity package and is not intended for use by an application.
/// </summary>
[PrivateComponent]
public class ResourceUsersEditComponent : ResourceUsersComponentBase, IYetaWFComponent<List<User>?> {

    public override ComponentType GetComponentType() { return ComponentType.Edit; }

    public class ResourceUsersSetup {
        public string GridId { get; set; } = null!;
        public string AddUrl { get; set; } = null!;
        public string GridAllId { get; set; } = null!;
    }
    public class NewModel {
        [Caption("New User"), Description("Enter a new user name and click Add")]
        [UIHint("Text80"), StringLength(Globals.MaxUser), Trim]
        public string NewValue { get; set; } = null!;
    }

    public class Entry {

        [Caption("Delete"), Description("Click to delete a user")]
        [UIHint("GridDeleteEntry"), ReadOnly]
        public int Delete { get; set; }

        [Caption("User"), Description("User Name")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserNameFromId { get; set; }

        [UIHint("Hidden"), ReadOnly]
        public int UserId { get; set; }
        [UIHint("Hidden"), ReadOnly]
        public string UserName { get; set; } = null!;

        public Entry(int userId, string userName) {
            UserId = UserNameFromId = userId;
            UserName = userName;
        }
        public Entry() { }
    }
    public class AllEntry {

        [Caption("User"), Description("User Name")]
        [UIHint("String"), ReadOnly]
        public string UserName { get; set; } = null!;

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
            AjaxUrl = Utility.UrlFor(typeof(ResourceUsersEndpoints), GridSupport.EditSortFilter),
            SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
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
            AjaxUrl = Utility.UrlFor(typeof(ResourceUsersEndpoints), GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                    DataProviderGetRecords<UserDefinition> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new ResourceUsersEditComponent.AllEntry(s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }
    public async Task<string> RenderAsync(List<User>? model) {

        HtmlBuilder hb = new HtmlBuilder();

        bool header = PropData.GetAdditionalAttributeValue("Header", true);

        GridModel grid = new GridModel() {
            GridDef = GetGridModel(header)
        };
        grid.GridDef.DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
            List<Entry> users = new List<Entry>();
            if (model != null) {
                foreach (User u in model) {
                    string userName = await Resource.ResourceAccess.GetUserNameAsync(u.UserId);
                    users.Add(new Entry(u.UserId, userName));
                }
            }


            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            return data;
        };

        hb.Append($@"
<div class='yt_yetawf_identity_resourceusers t_edit' id='{DivId}'>
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
        ResourceUsersSetup setup = new ResourceUsersSetup {
            AddUrl = Utility.UrlFor(typeof(ResourceUsersEndpoints), nameof(ResourceUsersEndpoints.AddUserToResource)),
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
new YetaWF_Identity.ResourceUsersEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

        return hb.ToString();
    }
}
