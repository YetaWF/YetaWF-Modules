/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Components {

    public abstract class ResourceUsersComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ResourceUsersComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ResourceUsers";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ResourceUsersDisplayComponent : ResourceUsersComponentBase, IYetaWFComponent<List<User>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridAllowedUserDisplay {

            [Caption("User"), Description("User Name")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            public GridAllowedUserDisplay(int userId) {
                UserId = userId;
            }
        }
        public async Task<YHtmlString> RenderAsync(List<User> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_yetawf_identity_resourceusers t_display'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            List<GridAllowedUserDisplay> users;
            if (model == null)
                users = new List<GridAllowedUserDisplay>();
            else
                users = (from u in model select new GridAllowedUserDisplay(u.UserId)).ToList();

            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedUserDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
    public class ResourceUsersEditComponent : ResourceUsersComponentBase, IYetaWFComponent<List<User>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class NewModel {
            [Caption("New User"), Description("Enter a new user name and click Add")]
            [UIHint("Text80"), StringLength(Globals.MaxUser), Trim]
            public string NewValue { get; set; }
        }
        [Trim]
        public class GridAllowedUser {

            [Caption("Delete"), Description("Click to delete a user")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("User"), Description("User Name")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserNameFromId { get; set; }

            [UIHint("RawInt"), ReadOnly]
            public int UserId { get; set; }
            [UIHint("Raw"), ReadOnly]
            public string UserName { get; set; }

            public GridAllowedUser(int userId, string userName) {
                UserId = UserNameFromId = userId;
                UserName = userName;
            }
        }
        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("User"), Description("User Name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUserName { get { return UserName; } }

            public GridAllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        public async Task<YHtmlString> RenderAsync(List<User> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_yetawf_identity_resourceusers t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{__ResStr("dupmsg", "User {0} has already been added")}'
        data-addedmsg='{__ResStr("addedmsg", "User {0} has been added")}'
        data-remmsg='{__ResStr("remmsg", "User {0} has been removed")}'>");

            List<GridAllowedUser> users = new List<GridAllowedUser>();
            if (model != null) {
                foreach (User u in model) {
                    string userName = await Resource.ResourceAccess.GetUserNameAsync(u.UserId);
                    users.Add(new GridAllowedUser(u.UserId, userName));
                }
            }

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = users.ToList<object>(),
                Total = users.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridAllowedUser),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "UserId",
                    DisplayProperty = "UserName"
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            using (Manager.StartNestedComponent(FieldName)) {

                string ajaxUrl = YetaWFManager.UrlFor(typeof(ResourceUsersController), nameof(ResourceUsersController.AddUserToResource));

                NewModel newModel = new NewModel();
                hb.Append("<div class='t_newvalue'>");
                hb.Append(await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue)));
                hb.Append(await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue)));
                hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.HtmlAttributeEncode(ajaxUrl));
                hb.Append("</div>");
            }

            hb.Append(@"
    </div>");

            hb.Append($@"
    <div id='{DivId}_coll'>
        {await ModuleActionHelper.BuiltIn_ExpandAction(__ResStr("lblFindUsers", "Find Users"), __ResStr("ttFindUsers", "Expand to find user names available on this site")).RenderAsNormalLinkAsync() }
    </div>
    <div id='{DivId}_exp' style='display:none'>
        {await ModuleActionHelper.BuiltIn_CollapseAction(__ResStr("lblAllUserNames", "All User Names"), __ResStr("ttAllUserNames", "Shows all user names available on this site - Select a user name to update the text box above, so the user name can be added to the list of user names - Click to close")).RenderAsNormalLinkAsync() }");

            grid = new GridModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ResourceUsersController), nameof(ResourceUsersController.ResourceUsersBrowse_GridData)),
                    Id = DivId + "_listall",
                    RecordType = typeof(GridAllEntry),
                    ShowHeader = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid"));

            hb.Append($@"
    </div>
</div>
<script>
    $YetaWF.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');
    $('#{tmpltId}_list').jqGrid('sortableRows', {{ connectWith: '#{tmpltId}_list' }});
    {BeginDocumentReady(DivId)}
        YetaWF_Identity_ResourceUsers.init($('#{DivId}_listall'), $('#{tmpltId} .t_edit[name$=\'.NewValue\']'));
    {EndDocumentReady()}
</script>");

            return hb.ToYHtmlString();
        }
    }
}
