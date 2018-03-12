/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Views;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using YetaWF.Core.Support;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Views.Shared {

    public class Email<TModel> : RazorTemplate<TModel> { }

    public static class EmailHelper {
        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EmailHelper), name, defaultValue, parms); }
#if MVC6
        public static async Task<HtmlString> RenderEmailDisplayAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#else
        public static async Task<HtmlString> RenderEmailDisplayAsync<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#endif
        {
            TagBuilder tag = new TagBuilder("span");
            htmlHelper.FieldSetup(tag, name, HtmlAttributes: HtmlAttributes, Validation: false, Anonymous: true);

            ModuleAction actionDisplay = null;
            ModuleAction actionLoginAs = null;
            using (UserDefinitionDataProvider userDefDP = new UserDefinitionDataProvider()) {
                UserDefinition user = null;
                string userName = "";
                if (!string.IsNullOrWhiteSpace(model)) {
                    user = await userDefDP.GetItemByEmailAsync(model);
                    if (user == null) {
                        userName = model;
                    } else {
                        userName = user.UserName;
                        Modules.UsersDisplayModule modDisp = new Modules.UsersDisplayModule();
                        actionDisplay = modDisp.GetAction_Display(null, userName);
                        Modules.LoginModule modLogin = (Modules.LoginModule) await ModuleDefinition.CreateUniqueModuleAsync(typeof(Modules.LoginModule));
                        actionLoginAs = await modLogin.GetAction_LoginAsAsync(user.UserId, userName);
                    }
                } else
                    userName = __ResStr("noEmail", "(not specified)");
                tag.SetInnerText(userName);
            }
            string html = tag.ToString(TagRenderMode.Normal);
            if (actionDisplay != null)
                html += await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly);
            if (actionLoginAs != null)
                html += await actionLoginAs.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly);
            return new HtmlString(html);
        }
    }
}