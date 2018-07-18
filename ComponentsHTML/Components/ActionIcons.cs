/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ActionIconsComponent : YetaWFComponent, IYetaWFComponent<MenuList> {

        public const string TemplateName = "ActionIcons";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public override async Task IncludeAsync() {
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.button.min.js");
            await KendoUICore.AddFileAsync("kendo.menu.min.js");
            await base.IncludeAsync();
        }

        public async Task<YHtmlString> RenderAsync(MenuList model) {

            HtmlBuilder hb = new HtmlBuilder();

            Grid.GridActionsEnum actionStyle = Grid.GridActionsEnum.Icons;
            if (model.Count > 1) {
                Grid.GridActionsEnum gridActionStyle;
                if (!PropData.TryGetAdditionalAttributeValue<Grid.GridActionsEnum>("GridActionsEnum", out gridActionStyle))
                    gridActionStyle = UserSettings.GetProperty<Grid.GridActionsEnum>("GridActions");
                actionStyle = gridActionStyle;
            }
            switch (actionStyle) {
                default:
                case Grid.GridActionsEnum.Icons:
                    return await CoreRendering.RenderMenuAsync(model, null, Addons.Templates.ActionIcons.CssActionIcons, HtmlHelper: HtmlHelper);
                case Grid.GridActionsEnum.DropdownMenu: {
                    model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;

                    string id = Manager.UniqueId();
                    string idButton = id + "_btn";
                    string idMenu = id + "_menu";
                    hb.Append("<button id=\"{0}\" type=\"button\" class=\"yt_actionicons\">{1}<span class=\"k-icon k-i-arrow-60-down\"></span></button>",
                        idButton, this.__ResStr("dropdownText", "Manage"));
                    hb.Append(await CoreRendering.RenderMenuAsync(model, idMenu, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper));

                    ScriptBuilder sb = new ScriptBuilder();
                    sb.Append($"YetaWF_TemplateActionIcons.initMenu('{id}', $('#{idButton}'), $('#{idMenu}'));");

                    hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());
                    return hb.ToYHtmlString();
                }
            }
        }
    }
}
