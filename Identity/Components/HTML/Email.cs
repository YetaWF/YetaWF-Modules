/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Components {

    public abstract class EmailComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EmailComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Email";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays the model as an email address. For authorized users, icons are rendered to login as the user shown and to display user information.
    /// </summary>
    /// <example>
    /// [Caption("Email Address"), Description("The user's email address")]
    /// [UIHint("YetaWF_Identity_Email"), ReadOnly]
    /// public string Email { get; set; }
    /// </example>
    public class EmailDisplayComponent : EmailComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            using (UserDefinitionDataProvider userDefDP = new UserDefinitionDataProvider()) {
                ModuleAction actionDisplay = null;
                ModuleAction actionLoginAs = null;
                string userName;
                UserDefinition user = null;
                if (!string.IsNullOrWhiteSpace(model)) {
                    user = await userDefDP.GetItemByEmailAsync(model);
                    if (user == null) {
                        userName = model;
                    } else {
                        userName = user.Email;
                        UsersDisplayModule modDisp = new UsersDisplayModule();
                        actionDisplay = modDisp.GetAction_Display(null, user.UserName);
                        LoginModule modLogin = (LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
                        actionLoginAs = await modLogin.GetAction_LoginAsAsync(user.UserId, user.UserName);
                    }
                } else
                    userName = __ResStr("noEmail", "(not specified)");

                hb.Append($@"
<div{FieldSetup(FieldType.Anonymous)} class='yt_yetawf_identity_email t_display{GetClasses()}{HtmlBuilder.GetClassAttribute(HtmlAttributes)}'>
    {HE(userName)}
    {(actionDisplay != null ? await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly) : null)}
    {(actionLoginAs != null ? await actionLoginAs.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly) : null)}
</div>");
            }
            return hb.ToString();
        }
    }
}
