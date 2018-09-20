/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Components {

    public abstract class ListOfUserNamesComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfUserNamesComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "ListOfUserNames";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfUserNamesDisplayComponent : ListOfUserNamesComponentBase, IYetaWFComponent<List<int>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridDisplay {
            [Caption("User Names"), Description("Shows all defined user names")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Raw"), ReadOnly]
            public int UserId { get; set; }

            public GridDisplay(UserDefinitionDataProvider userDP, UserDefinition user, int userId) {
                if (user == null) {
                    UserName = __ResStr("noUser", "({0})", userId);
                } else {
                    ObjectSupport.CopyData(user, this);
                }
                UserId = userId;
            }
        }
        public async Task<YHtmlString> RenderAsync(List<int> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_listofusernames t_display'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            List<GridDisplay> list = new List<GridDisplay>();
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                if (model != null) {
                    foreach (int userId in model) {
                        UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                        list.Add(new GridDisplay(userDP, user, userId));
                    }
                }
            }

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
    public class ListOfUserNamesEditComponent : ListOfUserNamesComponentBase, IYetaWFComponent<List<int>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class NewModel {
            [Caption("User Name"), Description("Please enter a new user name and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }
        [Trim]
        public class GridEdit {

            public GridEdit() { }

            [Caption("Delete"), Description("Click to remove this user name from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("User Name"), Description("Shows all defined user names")]
            [UIHint("Text80"), StringLength(Globals.MaxUser), UserNameValidation, ListNoDuplicates, Required, Trim]
            public string UserName { get; set; }

            [UIHint("Hidden"), Required, Trim]
            public string __Value { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return UserName; } }

            public GridEdit(UserDefinitionDataProvider userDP, int userId) {
                UserDefinition user = YetaWFManager.Syncify(() => userDP.GetItemByUserIdAsync(userId));
                if (user == null) {
                    UserName = __ResStr("noUser", "({0})", userId);
                } else {
                    UserName = user.UserName;
                }
                __Value = userId.ToString();
            }
        }
        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("User Name"), Description("Defines the user name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUserName { get { return UserName; } }

            public GridAllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        public async Task<YHtmlString> RenderAsync(List<int> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_listofusernames t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{__ResStr("dupmsg", "User name {0} has already been added")}'
        data-addedmsg='{__ResStr("addedmsg", "User name {0} has been added")}'
        data-remmsg='{__ResStr("remmsg", "User name {0} has been removed")}'>");

            List<GridEdit> list = new List<GridEdit>();
            using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                if (model != null)
                    list = (from u in model select new GridEdit(userDP, u)).ToList();
            }

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 10,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "__TextKey",
                    DisplayProperty = "__TextDisplay"
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            using (Manager.StartNestedComponent(FieldName)) {

                string ajaxUrl = GetSiblingProperty<string>($"{PropertyName}_AjaxUrl");

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
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfUserNamesController), nameof(ListOfUserNamesController.ListOfUserNamesBrowse_GridData)),
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
    {BeginDocumentReady(DivId)}
        YetaWF_Identity_ListOfUserNames.init($('#{DivId}_listall'), $('#{tmpltId} .t_edit[name$=\'.NewValue\']'));
    {EndDocumentReady()}
</script>");
            return hb.ToYHtmlString();
        }
    }
}
