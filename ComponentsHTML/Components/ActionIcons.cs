using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons.Templates;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class ActionIconsComponent : YetaWFComponent, IYetaWFComponent<MenuList> {

        public const string TemplateName = "ActionIcons";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(MenuList model) {

            HtmlBuilder hb = new HtmlBuilder();

            GridHelper.GridActionsEnum actionStyle = GridHelper.GridActionsEnum.Icons;
            if (model.Count > 1) {
                GridHelper.GridActionsEnum gridActionStyle;
                if (!PropData.TryGetAdditionalAttributeValue<GridHelper.GridActionsEnum>("GridActionsEnum", out gridActionStyle))
                    gridActionStyle = UserSettings.GetProperty<GridHelper.GridActionsEnum>("GridActions");
                actionStyle = gridActionStyle;
            }
            switch (actionStyle) {
                default:
                case GridHelper.GridActionsEnum.Icons:
                    return new YHtmlString((await model.RenderAsync(HtmlHelper, null, ActionIcons.CssActionIcons)).ToString());
                case GridHelper.GridActionsEnum.DropdownMenu: {
                        MenuList menuActions = model;
                        menuActions.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;

                        string id = Manager.UniqueId();
                        string idButton = id + "_btn";
                        string idMenu = id + "_menu";
                        hb.Append("<button id=\"{0}\" type=\"button\" class=\"yt_actionicons\">{1}<span class=\"k-icon k-i-arrow-60-down\"></span></button>", 
                            idButton, this.__ResStr("dropdownText", "Manage"));
                        hb.Append(await menuActions.RenderAsync(HtmlHelper, idMenu, Globals.CssGridActionMenu));

                        ScriptBuilder sb = new ScriptBuilder();
                        sb.Append($"YetaWF_TemplateActionIcons.initMenu('{id}', $('#{idButton}'), $('#{idMenu}'));");

                        hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());
                        return hb.ToYHtmlString();
                    }
            }
        }
    }
}
