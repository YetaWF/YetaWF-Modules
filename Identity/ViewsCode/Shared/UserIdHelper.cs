/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.Controllers.Shared;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class UserId<TModel> : RazorTemplate<TModel> { }

    public static class UserIdHelper {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(UserIdHelper), name, defaultValue, parms); }

        public const int MAXUSERS = 50; // maximum # of users for a dropdown

#if MVC6
        public static HtmlString RenderUserIdDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderUserIdDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {
            TagBuilder tag = new TagBuilder("span");
            htmlHelper.FieldSetup(tag, name, HtmlAttributes: HtmlAttributes, Validation: false, Anonymous: true);

            ModuleAction actionDisplay = null;
            ModuleAction actionLoginAs = null;
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = dataProvider.GetItemByUserId(model);
                string userName = "";
                if (user == null) {
                    if (model != 0)
                        userName = string.Format("({0})", model);
                } else {
                    userName = user.UserName;
                    Modules.UsersDisplayModule modDisp = new Modules.UsersDisplayModule();
                    actionDisplay = modDisp.GetAction_Display(null, userName);
                    Modules.LoginModule modLogin = (Modules.LoginModule)ModuleDefinition.CreateUniqueModule(typeof(Modules.LoginModule));
                    actionLoginAs = modLogin.GetAction_LoginAs(model, userName);
                }
                tag.SetInnerText(userName);
            }
            string html = tag.ToString(TagRenderMode.Normal);
            if (actionDisplay != null)
                html += actionDisplay.Render(ModuleAction.RenderModeEnum.IconsOnly);
            if (actionLoginAs != null)
                html += actionLoginAs.Render(ModuleAction.RenderModeEnum.IconsOnly);
            return new HtmlString(html);
        }

#if MVC6
        public static bool IsLargeUserBase()
#else
        public static bool IsLargeUserBase()
#endif
        {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                int total;
                userDP.GetItems(0, 1, null, null, out total);
                return total > MAXUSERS;
            }
        }

#if MVC6
        public static HtmlString RenderUserId<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderUserId<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                List<DataProviderSortInfo> sorts = null;
                DataProviderSortInfo.Join(sorts, new DataProviderSortInfo { Field = "UserName", Order = DataProviderSortInfo.SortDirection.Ascending });
                int total;
                List<SelectionItem<int>> list = (from u in userDP.GetItems(0, MAXUSERS, sorts, null, out total) select new SelectionItem<int> {
                    Text = u.UserName,
                    Value = u.UserId,
                }).ToList();
                list.Insert(0, new SelectionItem<int> {
                    Text = __ResStr("select", "(select)"),
                    Value = 0,
                });
                return htmlHelper.RenderDropDownSelectionList<int>(name, model, list, HtmlAttributes: HtmlAttributes);
            }
        }
#if MVC6
        public static HtmlString RenderUserIdName<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, int model, string id, string noUser)
#else
        public static HtmlString RenderUserIdName<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, string id, string noUser)
#endif
        {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                string s;
                UserDefinition user = userDP.GetItemByUserId(model);
                if (user == null)
                    s = noUser;
                else
                    s = user.UserName;
                return htmlHelper.RenderTextBoxDisplay(name, s, HtmlAttributes: new { @class="yt_text40 t_display", id = id, data_nouser = noUser });
            }
        }
#if MVC6
        public static HtmlString RenderClear<TModel>(this IHtmlHelper<TModel> htmlHelper)
#else
        public static HtmlString RenderClear<TModel>(this HtmlHelper<TModel> htmlHelper)
#endif
        {
            SkinImages skinImages = new SkinImages();
            string imageUrl = skinImages.FindIcon_Template("#RemoveLight", YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage, "UserId");
            TagBuilder tagImg = ImageHelper.BuildKnownImageTag(imageUrl, title: __ResStr("ttClear", "Clear the current selection"), alt: __ResStr("altClear", "Clear the current selection"));
            tagImg.AddCssClass("t_clear");
            return tagImg.ToHtmlString(TagRenderMode.StartTag);
        }

        public class GridAllModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
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
#if MVC6
        public static HtmlString RenderAllUsers<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string id)
#else
        public static HtmlString RenderAllUsers<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string id)
#endif
        {
            bool header;
            if (!htmlHelper.TryGetControlInfo<bool>("", "Header", out header))
                header = true;
            GridAllModel model = new GridAllModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(UserIdHelperController), nameof(UserIdHelperController.UsersBrowse_GridData)),
                    Id = id,
                    RecordType = typeof(GridAllEntry),
                    ShowHeader = header
                }
            };
#if MVC6
            return new HtmlString(htmlHelper.DisplayFor(m => model.GridDef).AsString());
#else
            return htmlHelper.DisplayFor(m => model.GridDef);
#endif
        }
    }
}