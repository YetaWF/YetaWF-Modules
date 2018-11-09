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

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ActionIconsComponent), name, defaultValue, parms); }

        public const string TemplateName = "ActionIcons";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public class ActionIconsSetup {
            public string MenuId { get; set; }
        }

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
                    hb.Append($@"
{await CoreRendering.RenderMenuAsync(model, null, Addons.Templates.ActionIcons.CssActionIcons, HtmlHelper: HtmlHelper)}");
                    break;
                case Grid.GridActionsEnum.DropdownMenu: {
                    model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;
                    string buttonId = ControlId + "_btn";
                    ActionIconsSetup setup = new ActionIconsSetup {
                        MenuId = ControlId + "_menu",
                    };
                    hb.Append($@"
<button id='{buttonId}' type='button' class='yt_actionicons'>
    {HE(__ResStr("dropdownText", "Manage"))}<span class='k-icon k-i-arrow-60-down'></span>
    {await CoreRendering.RenderMenuAsync(model, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true)}
</button>
<script>
    new YetaWF_ComponentsHTML.ActionIconsComponent('{buttonId}', {YetaWFManager.JsonSerialize(setup)});
</script>");
                    break;
                }
            }
            return hb.ToYHtmlString();
        }
    }
}
