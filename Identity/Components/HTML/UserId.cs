/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
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

    public class UserIdDisplayComponent : UserIdComponentBase, IYetaWFComponent<int?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<string> RenderAsync(int? model) {
            return RenderAsync(model??0);
        }
        public async Task<string> RenderAsync(int model) {

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("span");
            tag.AddCssClass("yt_yetawf_identity_userid");
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

            hb.Append(tag.ToString(YTagRenderMode.Normal));
            if (actionDisplay != null)
                hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
            if (actionLoginAs != null)
                hb.Append(await actionLoginAs.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));

            return hb.ToString();
        }
    }
    public class UserIdEditComponent : UserIdComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class UserIdSetup {
            public string GridAllId { get; internal set; }
            public string HiddenId { get; internal set; }
            public string NameId { get; internal set; }
            public string NoUser { get; internal set; }
        }

        public class UserIdUI {
            [UIHint("Hidden"), ReadOnly]
            public int UserId { get; set; }

            [UIHint("Text40"), ReadOnly, StringLength(Globals.MaxUser), AdditionalMetadata("Copy", true), AdditionalMetadata("ReadOnly", true)]
            public string UserName { get; set; }

            [Caption("All Users"), Description("Shows all users - Select a user to update the user shown above")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition AllUsers { get; set; }
        }

        public class AllEntry {

            [Caption("Name"), Description("Displays the user's name")]
            [UIHint("String"), ReadOnly]
            public string UserName { get; set; }

            [Caption("Email Address"), Description("Displays the user's email address")]
            [UIHint("YetaWF_Identity_Email"), ReadOnly]
            public string Email { get; set; }

            [Caption("Status"), Description("The user's current account status")]
            [UIHint("Enum"), ReadOnly]
            public UserStatusEnum UserStatus { get; set; }

            [Caption("Used Id"), Description("Displays the user's internal Id")]
            [UIHint("IntValue"), ReadOnly]
            public int UserIdDisplay { get; set; }

            [UIHint("Hidden")]
            public int UserId { get; set; }

            public AllEntry(UserDefinition user) {
                ObjectSupport.CopyData(user, this);
            }
        }
        internal static GridDefinition GetGridAllUsersModel(bool header) {
            return new GridDefinition() {
                RecordType = typeof(AllEntry),
                InitialPageSize = 10,
                ShowHeader = header,
                AjaxUrl = Utility.UrlFor(typeof(UserIdController), nameof(UserIdController.UsersBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                        DataProviderGetRecords<UserDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sorts, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new AllEntry(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }
        public async Task<string> RenderAsync(int model) {

            HtmlBuilder hb = new HtmlBuilder();

            string type = PropData.GetAdditionalAttributeValue<string>("Force", null);

            if (type == "Grid" || (await IsLargeUserBaseAsync() && type != "DropDown")) {
                string hiddenId = UniqueId();
                string allId = UniqueId();
                string nameId = UniqueId();
                string noUser = __ResStr("noUser", "(none)");

                bool header = PropData.GetAdditionalAttributeValue("Header", true);

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

                ui.AllUsers = GetGridAllUsersModel(header);
                ui.AllUsers.Id = allId;

                hb.Append($@"
<div class='yt_yetawf_identity_userid t_large t_edit' id='{DivId}'>
    {await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "Hidden", HtmlAttributes: new { id = hiddenId, __NoTemplate = true })}");

                using (Manager.StartNestedComponent(FieldName)) {

                    hb.Append($@"
    <div class='t_name'>
        {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.UserName), HtmlAttributes: new { id = nameId })}
        {ImageHTML.BuildKnownIcon("#RemoveLight", title: __ResStr("ttClear", "Clear the current selection"), cssClass: "t_clear")}
    </div>
    {await HtmlHelper.ForLabelAsync(ui, nameof(ui.AllUsers))}
    {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.AllUsers))}");

                }
                hb.Append($@"
</div>");

                UserIdSetup setup = new UserIdSetup {
                    GridAllId = ui.AllUsers.Id,
                    HiddenId = hiddenId,
                    NameId = nameId,
                    NoUser = noUser,
                };

                Manager.ScriptManager.AddLast($@"new YetaWF_Identity.UserIdEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

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
            return hb.ToString();
        }
    }
}
