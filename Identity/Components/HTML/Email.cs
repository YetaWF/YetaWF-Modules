﻿using System.Threading.Tasks;
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

        public const string TemplateName = "Email";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class EmailDisplayComponent : EmailComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("span");
            FieldSetup(tag, FieldType.Anonymous);

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
                        UsersDisplayModule modDisp = new UsersDisplayModule();
                        actionDisplay = modDisp.GetAction_Display(null, userName);
                        LoginModule modLogin = (LoginModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(LoginModule));
                        actionLoginAs = await modLogin.GetAction_LoginAsAsync(user.UserId, userName);
                    }
                } else
                    userName = this.__ResStr("noEmail", "(not specified)");
                tag.SetInnerText(userName);
            }
            hb.Append(tag.ToString(YTagRenderMode.Normal));
            if (actionDisplay != null)
                hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
            if (actionLoginAs != null)
                hb.Append(await actionLoginAs.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
            return hb.ToYHtmlString();
        }
    }
}