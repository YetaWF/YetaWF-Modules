/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Web.Mvc;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Views.Shared {

    public class Email<TModel> : RazorTemplate<TModel> { }

    public static class EmailHelper {
        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EmailHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderEmailDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null) {
            TagBuilder tag = new TagBuilder("span");
            htmlHelper.FieldSetup(tag, name, HtmlAttributes: HtmlAttributes, Validation: false, Anonymous: true);

            ModuleAction actionDisplay = null;
            ModuleAction actionLoginAs = null;
            using (UserDefinitionDataProvider userDefDP = new UserDefinitionDataProvider()) {
                UserDefinition user = null;
                string userName = "";
                if (!string.IsNullOrWhiteSpace(model)) {
                    user = userDefDP.GetItemByEmail(model);
                    if (user == null) {
                        userName = model;
                    } else {
                        userName = user.UserName;
                        Modules.UsersDisplayModule modDisp = new Modules.UsersDisplayModule();
                        actionDisplay = modDisp.GetAction_Display(null, userName);
                        Modules.LoginModule modLogin = (Modules.LoginModule)ModuleDefinition.CreateUniqueModule(typeof(Modules.LoginModule));
                        actionLoginAs = modLogin.GetAction_LoginAs(user.UserId, userName);
                    }
                } else
                    userName = __ResStr("noEmail", "(not specified)");
                tag.SetInnerText(userName);
            }
            string html = tag.ToString(TagRenderMode.Normal);
            if (actionDisplay != null)
                html += actionDisplay.Render(ModuleAction.RenderModeEnum.IconsOnly);
            if (actionLoginAs != null)
                html += actionLoginAs.Render(ModuleAction.RenderModeEnum.IconsOnly);
            return MvcHtmlString.Create(html);
        }
    }
}