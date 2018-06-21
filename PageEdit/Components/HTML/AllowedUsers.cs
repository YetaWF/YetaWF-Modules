/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

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
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.PageEdit.Controllers;

namespace YetaWF.Modules.PageEdit.Components {

    public abstract class AllowedUsersComponentBase : YetaWFComponent {

        public const string TemplateName = "AllowedUsers";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class AllowedUsersEditComponent : AllowedUsersComponentBase, IYetaWFComponent<SerializableList<PageDefinition.AllowedUser>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class GridAllowedUser {

            protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

            [Caption("Delete"), Description("Click to delete a user")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

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

            [UIHint("RawInt"), ReadOnly]
            public int UserId { get; set; }
            [UIHint("Raw"), ReadOnly]
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
        }
        public class NewModel {
            [Caption("New User"), Description("Enter a new user name and click Add")]
            [UIHint("Text40"), StringLength(Globals.MaxUser), Trim]
            public string NewValue { get; set; }
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
        public async Task<YHtmlString> RenderAsync(SerializableList<PageDefinition.AllowedUser> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_yetawf_pageedit_allowedusers t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{this.__ResStr("dupmsg", "User {0} has already been added")}'
        data-addedmsg='{this.__ResStr("addedmsg", "User {0} has been added")}'
        data-remmsg='{this.__ResStr("remmsg", "User {0} has been removed")}'>");

            List<GridAllowedUser> list;
            if (model != null) {
                list = new List<GridAllowedUser>();
                foreach (PageDefinition.AllowedUser allowedUser in model) {
                    string userName = await Resource.ResourceAccess.GetUserNameAsync(allowedUser.UserId);
                    list.Add(new GridAllowedUser(allowedUser, userName));
                }
            } else
                list = new List<GridAllowedUser>();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
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

                string ajaxUrl = YetaWFManager.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AddUserToPage));

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
        {await ModuleActionHelper.BuiltIn_ExpandAction(this.__ResStr("lblFindUsers", "Find Users"), this.__ResStr("ttFindUsers", "Expand to find user names available on this site")).RenderAsNormalLinkAsync() }
    </div>
    <div id='{DivId}_exp' style='display:none'>
        {await ModuleActionHelper.BuiltIn_CollapseAction(this.__ResStr("lblAllUserNames", "All User Names"), this.__ResStr("ttAllUserNames", "Shows all user names available on this site - Select a user name to update the text box above, so the user name can be added to the list of user names - Click to close")).RenderAsNormalLinkAsync() }");

            grid = new GridModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AllowedUsersBrowse_GridData)),
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
    YetaWF_Basics.ExpandCollapse('{DivId}', '{DivId}_coll', '{DivId}_exp');");

            using (DocumentReady(hb, DivId)) {
        hb.Append($@"
    $('#{tmpltId}_list').jqGrid('sortableRows', {{ connectWith: '#{tmpltId}_list' }});
    YetaWF_PageEdit_AllowedUsers.init($('#{tmpltId}'), $('#{tmpltId}_list'), $('#{DivId}_listall'), $('#{tmpltId} .t_edit[name$=\'.NewValue\']'));
");
            }

            hb.Append($@"
</script>");
            return hb.ToYHtmlString();
        }
    }
}
