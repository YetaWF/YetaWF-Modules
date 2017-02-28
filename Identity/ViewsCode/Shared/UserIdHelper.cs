/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class UserId<TModel> : RazorTemplate<TModel> { }

    public static class UserIdHelper {
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

        // RESEARCH: Expand this to support huge number of user accounts (using a grid)
#if MVC6
        public static HtmlString RenderUserId<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#else
        public static HtmlString RenderUserId<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null)
#endif
        {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                int total;
                List<SelectionItem<int>> list = (from u in userDP.GetItems(0, 0, null, null, out total) select new SelectionItem<int> {
                    Text = u.UserName,
                    Value = u.UserId,
                }).ToList();
                return htmlHelper.RenderDropDownSelectionList<int>(name, model, list, HtmlAttributes: HtmlAttributes);
            }
        }
    }
}