/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Views.Shared {

    public class UserId<TModel> : RazorTemplate<TModel> { }

    public static class UserIdHelper {

        public static MvcHtmlString RenderUserIdDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null) {
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
            return MvcHtmlString.Create(html);
        }

        // RESEARCH: Expand this to support huge number of user accounts (using a grid)
        public static MvcHtmlString RenderUserId<TModel>(this HtmlHelper<TModel> htmlHelper, string name, int model, object HtmlAttributes = null) {
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