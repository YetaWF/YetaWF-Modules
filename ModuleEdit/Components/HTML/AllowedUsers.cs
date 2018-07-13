/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
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

        public const string TemplateName = "AllowedUsers";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class AllowedUsersEditComponent : AllowedUsersComponentBase, IYetaWFComponent<SerializableList<ModuleDefinition.AllowedUser>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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
        public async Task<YHtmlString> RenderAsync(SerializableList<ModuleDefinition.AllowedUser> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_yetawf_moduleedit_allowedusers t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{this.__ResStr("dupmsg", "User {0} has already been added")}'
        data-addedmsg='{this.__ResStr("addedmsg", "User {0} has been added")}'
        data-remmsg='{this.__ResStr("remmsg", "User {0} has been removed")}'>");

            Type gridEntryType = PropData.GetAdditionalAttributeValue<Type>("GridEntry");
            if (gridEntryType == null)
                gridEntryType = typeof(ModuleDefinition.GridAllowedUser);

            List<ModuleDefinition.GridAllowedUser> list = new List<ModuleDefinition.GridAllowedUser>();
            if (model != null) {
                // we have to create a more derived type here to get all the "extra" fields
                foreach (ModuleDefinition.AllowedUser u in model) {
                    ModuleDefinition.GridAllowedUser user = (ModuleDefinition.GridAllowedUser)Activator.CreateInstance(gridEntryType);
                    ObjectSupport.CopyData(u, user);
                    await user.SetUserAsync(u.UserId);
                    list.Add(user);
                }
            }

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = gridEntryType,
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "UserId",
                    DisplayProperty = "DisplayUserName",
                    ResourceRedirect = Manager.CurrentModuleEdited,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            using (Manager.StartNestedComponent(FieldName)) {

                string ajaxUrl = YetaWFManager.UrlFor(typeof(AllowedUsersController), nameof(AllowedUsersController.AddUserToModule));

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
    YetaWF_Basics.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');");

            using (DocumentReady(hb, DivId)) {
        hb.Append($@"
    $('#{tmpltId}_list').jqGrid('sortableRows', {{ connectWith: '#{tmpltId}_list' }});
    YetaWF_ModuleEdit_AllowedUsers.init($('#{tmpltId}'), $('#{tmpltId}_list'), $('#{DivId}_listall'), $('#{tmpltId} .t_edit[name$=\'.NewValue\']'));
");
            }

            hb.Append($@"
</script>");
            return hb.ToYHtmlString();
        }
    }
}
