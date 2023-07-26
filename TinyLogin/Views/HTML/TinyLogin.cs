/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLogin#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TinyLogin.Modules;
using static YetaWF.Core.Components.YetaWFComponentBase;

namespace YetaWF.Modules.TinyLogin.Views;

public class TinyLoginView : YetaWFView, IYetaWFView<TinyLoginModule, TinyLoginModule.TinyLoginModel> {

    public const string ViewName = "TinyLogin";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetViewName() { return ViewName; }

    public async Task<string> RenderViewAsync(TinyLoginModule module, TinyLoginModule.TinyLoginModel model) {

        switch (module.Appearance) {
            default:
            case TinyLoginModule.AppearanceEnum.Classic:
                return await RenderClassicViewAsync(module, model);
            case TinyLoginModule.AppearanceEnum.Modern:
                return await RenderModernViewAsync(module, model);
        }
    }

    private async Task<string> RenderClassicViewAsync(TinyLoginModule module, TinyLoginModule.TinyLoginModel model) {
        HtmlBuilder hb = new HtmlBuilder();
        if (model.LoggedOn) {

            ModuleAction logoffAction = await module.GetAction_LogoffAsync(model.LogoffUrl);
            ModuleAction userNameAction = await module.GetAction_UserNameAsync(model.UserUrl, model.UserName!, model.UserTooltip);

            hb.Append($@"
<div class='t_haveuser t_link'>
    {await logoffAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>
<div class='t_haveuser t_user'>
    {await userNameAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>");
        } else {

            ModuleAction loginAction = await module.GetAction_LoginAsync(model.LogonUrl);
            ModuleAction? registerAction = await module.GetAction_RegisterAsync(model.RegisterUrl);

            hb.Append($@"
<div class='t_nouser t_logon'>
    {await loginAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>");
            if (registerAction != null) {
                hb.Append($@"
<div class='t_nouser t_register'>
    {await registerAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
</div>");
            }
        }
        return hb.ToString();
    }

    private async Task<string> RenderModernViewAsync(TinyLoginModule module, TinyLoginModule.TinyLoginModel model) {
        HtmlBuilder hb = new HtmlBuilder();
        if (model.LoggedOn) {

            ModuleAction logoffAction = await module.GetAction_LogoffAsync(model.LogoffUrl);
            ModuleAction userNameAction = await module.GetAction_UserNameAsync(model.UserUrl, model.UserName!, model.UserTooltip);

            List<ModuleAction> actions = new List<ModuleAction>();
            actions.New(userNameAction);
            actions.New(logoffAction);
            actions.New(new ModuleAction { Separator = true });

            // Use RenderLIAsync to render actions
            hb.Append($@"
<ul id='{DivId}' class='yt_menu t_menu t_display t_lvl0 t_horizontal'>
    <li class='t_menu t_lvl0'>
        <a class=''>
            <span>
                {(Manager.UserEmail?.Substring(0, 1) ?? Manager.UserName?.Substring(0, 1) ?? "?").ToUpper()}
            </span>
        </a>
        <ul class='t_menu t_lvl1' style='display:none'>
            {await MenuDisplayComponent.RenderLIAsync(HtmlHelper, actions, null, ModuleAction.RenderModeEnum.NormalMenu, null, 1, true)}
            <li class='t_menu t_lvl1'>
                <a class='yNoPrint y_act_normmenu yaction-link y_act_text' data-tooltip='Both YetaWF demo sites for Windows and Linux are identical' href='https://demo.YetaWF.com/YetaWF_Identity/LoginDirect/LoginAs?UserId=1' target='_blank' rel='noopener noreferrer'>
                    <span>
                        Windows Hosted
                    </span>
                </a>
            </li>
            <li class='t_menu t_lvl1'>
                <a class='yNoPrint y_act_normmenu yaction-link y_act_text' data-tooltip='Both YetaWF demo sites for Windows and Linux are identical' href='https://demo2.YetaWF.com/YetaWF_Identity/LoginDirect/LoginAs?UserId=1' target='_blank' rel='noopener noreferrer'>
                    <span>
                        Linux Hosted
                    </span>
                </a>
            </li>
        </ul>
    </li>
</ul>");
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "Menu", ComponentType.Display);
            MenuDisplayComponent.MenuSetup setup = new MenuDisplayComponent.MenuSetup {
                Orientation = MenuComponentBase.OrientationEnum.Horizontal,
                VerticalWidth = 0,
                SmallMenuMaxWidth = 0,
                HoverDelay = 300,
                HorizontalAlign = MenuComponentBase.HorizontalAlignEnum.Left,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MenuComponent('{DivId}', {Utility.JsonSerialize(setup)});");

        } else {

//            ModuleAction loginAction = await module.GetAction_LoginAsync(model.LogonUrl);
//            ModuleAction? registerAction = await module.GetAction_RegisterAsync(model.RegisterUrl);

//            hb.Append($@"
//<div class='t_nouser t_logon'>
//    {await loginAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}
//</div>
//<div class='t_nouser t_register'>");

//            if (registerAction != null) {
//                hb.Append($@"
//                    {await registerAction.RenderAsync(ModuleAction.RenderModeEnum.NormalLinks)}");
//            }
//            hb.Append($@"
//</div>");
        }
        return hb.ToString();
    }
}
