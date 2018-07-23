/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif


namespace YetaWF.Modules.Identity.Components {

    public abstract class UserIdComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(UserIdComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "UserId";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public const int MAXUSERS = 50; // maximum # of users for a dropdown

        public static async Task<bool> IsLargeUserBaseAsync() {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, 1, null, null);
                return recs.Total > MAXUSERS;
            }
        }
    }

    public class UserIdDisplayComponent : UserIdComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(int model) {

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("span");
            tag.AddCssClass("yt_yetawf_identity_useridt_display");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);

            ModuleAction actionDisplay = null;
            ModuleAction actionLoginAs = null;
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await dataProvider.GetItemByUserIdAsync(model);
                string userName = "";
                if (user == null) {
                    if (model != 0)
                        userName = string.Format("({0})", model);
                } else {
                    userName = user.UserName;
                    Modules.UsersDisplayModule modDisp = new Modules.UsersDisplayModule();
                    actionDisplay = modDisp.GetAction_Display(null, userName);
                    Modules.LoginModule modLogin = (Modules.LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(Modules.LoginModule));
                    actionLoginAs = await modLogin.GetAction_LoginAsAsync(model, userName);
                }
                tag.SetInnerText(userName);
            }

            hb.Append(tag.ToYHtmlString(YTagRenderMode.Normal));
            if (actionDisplay != null)
                hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
            if (actionLoginAs != null)
                hb.Append(await actionLoginAs.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));

            return hb.ToYHtmlString();
        }
    }
    public class UserIdEditComponent : UserIdComponentBase, IYetaWFComponent<int> {

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
                    UserName = __ResStr("unknownUser", "({0})", userId);
                } else {
                    UserName = user.UserName;
                }
                __Value = userId.ToString();
            }
        }
        public class UserIdUI {
            [UIHint("Hidden")]
            public int UserId { get; set; }

            [UIHint("Text40"), StringLength(Globals.MaxUser), AdditionalMetadata("Copy", true), AdditionalMetadata("ReadOnly", true)]
            public string UserName { get; set; }

            [Caption("All Users"), Description("Shows all users - Select a user to update the user shown above")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition AllUsers { get; set; }
        }
        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("Name"), Description("Displays the user's name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Displays the user's email address")]
            [UIHint("YetaWF_Identity_Email"), ReadOnly]
            public string Email { get; set; }

            [Caption("Used Id"), Description("Displays the user's internal Id")]
            [UIHint("IntValue"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Status"), Description("The user's current account status")]
            [UIHint("Enum"), ReadOnly]
            public UserStatusEnum UserStatus { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUserId { get { return UserId.ToString(); } }
            [UIHint("Raw"), ReadOnly]
            public string RawUserName { get { return UserName; } }

            public GridAllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        public async Task<YHtmlString> RenderAsync(int model) {

            HtmlBuilder hb = new HtmlBuilder();

            string type = PropData.GetAdditionalAttributeValue<string>("Force", null);

            UserIdUI ui = new UserIdUI {
                UserId = model,
            };

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(model);
                if (user == null)
                    ui.UserName = __ResStr("noUser", "(none)");
                else
                    ui.UserName = user.UserName;
            }

            if (type == "Grid" || (await IsLargeUserBaseAsync() && type != "DropDown")) {
                string hiddenId = UniqueId();
                string allId = UniqueId();
                string nameId = UniqueId();
                string noUser = __ResStr("noUser", "(none)");

                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("#RemoveLight", Package, "UserId");
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, title: __ResStr("ttClear", "Clear the current selection"), alt: __ResStr("altClear", "Clear the current selection"));
                tagImg.AddCssClass("t_clear");

                bool header = PropData.GetAdditionalAttributeValue("Header", true);
                ui.AllUsers = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(UserIdController), nameof(UserIdController.UsersBrowse_GridData)),
                    Id = allId,
                    RecordType = typeof(GridAllEntry),
                    ShowHeader = header
                };


                hb.Append($@"
<div class='yt_yetawf_identity_userid t_large t_edit' id='{DivId}'>
    {await HtmlHelper.ForEditAsAsync(Container, PropertyName, FieldName, ui, nameof(ui.UserId), model, "Hidden", HtmlAttributes: new { id = hiddenId })}");

                using (Manager.StartNestedComponent(FieldName)) {

                    hb.Append($@"
    <div class='t_name'>
        {await HtmlHelper.ForEditAsync(ui, nameof(ui.UserName),HtmlAttributes: new { id = nameId, data_nouser = noUser })}
        {tagImg.ToString(YTagRenderMode.StartTag)}
    </div>
    {await HtmlHelper.ForLabelAsync(ui, nameof(ui.AllUsers))}
    {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.AllUsers))}");

                }
                hb.Append($@"
</div>
<script>");
                using (DocumentReady(hb, DivId)) {
                    hb.Append($@"
    YetaWF_Identity_UserId.init($('#{DivId}'), $('#{hiddenId}'), $('#{nameId}'), $('#{allId}'));");
                }
                hb.Append($@"
</script>");
            } else {

                hb.Append($@"
<div class='yt_yetawf_identity_userid t_small t_edit'>");

                using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                    List<DataProviderSortInfo> sorts = null;
                    DataProviderSortInfo.Join(sorts, new DataProviderSortInfo { Field = "UserName", Order = DataProviderSortInfo.SortDirection.Ascending });
                    DataProviderGetRecords<UserDefinition> recs = await userDP.GetItemsAsync(0, MAXUSERS, sorts, null);
                    List<SelectionItem<int>> list = (from u in recs.Data select new SelectionItem<int> {
                        Text = u.UserName,
                        Value = u.UserId,
                    }).ToList();
                    list.Insert(0, new SelectionItem<int> {
                        Text = __ResStr("select", "(select)"),
                        Value = 0,
                    });
                    hb.Append(await DropDownListIntComponent.RenderDropDownListAsync(this, model, list, null));
                }

                hb.Append($@"
</div>");
            }
            return hb.ToYHtmlString();
        }
    }
}
