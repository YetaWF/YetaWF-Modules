/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays a menu consisting of ModuleActions (YetaWF.Core.Menus.MenuList). A menu consists of zero, one or more ModuleActions. May be null, in which case nothing is rendered.
    /// ModuleActions are used throughout YetaWF and define a specific action a user can take, typically a link or button which directs to a URL.
    /// </summary>
    /// <example>
    /// [Caption("Actions"), Description("All available actions")]
    /// [UIHint("ActionIcons"), ReadOnly]
    /// public MenuList Commands {
    ///     get {
    ///         MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
    ///         actions.New(Module.GetAction_DownloadLink(FileName), ModuleAction.ActionLocationEnum.GridLinks);
    ///         actions.New(Module.GetAction_RemoveLink(FileName), ModuleAction.ActionLocationEnum.GridLinks);
    ///         return actions;
    ///     }
    /// }
    /// </example>
    [UsesAdditional("GridActionsEnum", "Grid.GridActionsEnum", "GridActionsEnum.Icons", "Defines the appearance of the module actions.")]
    public class ActionIconsComponent : YetaWFComponent, IYetaWFComponent<MenuList> {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ActionIconsComponent), name, defaultValue, parms); }

        internal const string TemplateName = "ActionIcons";

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }
        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(DropDownButtonComponent.TemplateName, YetaWFComponentBase.ComponentType.Display);
            await base.IncludeAsync();
        }

        internal class ActionIconsSetup {
            public string MenuId { get; set; }
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(MenuList model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.Count == 0)
                return null;

            Grid.GridActionsEnum actionStyle = PropData.GetAdditionalAttributeValue<Grid.GridActionsEnum>("GridActionsEnum", UserSettings.GetProperty<Grid.GridActionsEnum>("GridActions"));
            if (model.Count == 1) {
                actionStyle = Grid.GridActionsEnum.Icons;
                model.RenderMode = ModuleAction.RenderModeEnum.IconsOnly;
            }

            switch (actionStyle) {

                default:
                case Grid.GridActionsEnum.Icons:
                    hb.Append($@"
{await MenuDisplayComponent.RenderMenuAsync(model, null, Addons.Templates.ActionIcons.CssActionIcons, HtmlHelper: HtmlHelper)}");
                    break;

                case Grid.GridActionsEnum.DropdownMenu:
                case Grid.GridActionsEnum.Mini: {

                        model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;
                        string buttonId = ControlId + "_btn";
                        ActionIconsSetup setup = new ActionIconsSetup {
                            MenuId = ControlId + "_menu",
                        };
                        string menuHTML = await MenuDisplayComponent.RenderMenuAsync(model, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true);

                        if (!string.IsNullOrWhiteSpace(menuHTML)) {

                            DropDownButtonComponent.Model ddModel = new DropDownButtonComponent.Model {
                                Text = __ResStr("dropdownText", "Manage"),
                                Tooltip = null,
                                ButtonId = buttonId,
                                MenuHTML = menuHTML,
                                Mini = actionStyle == Grid.GridActionsEnum.Mini,
                            };

                            hb.Append($@"
<div class='yt_actionicons{(actionStyle == Grid.GridActionsEnum.Mini ? " t_mini" : "")}' id='{ControlId}'>
    {await HtmlHelper.ForDisplayContainerAsync(ddModel, DropDownButtonComponent.TemplateName)}
</div>");

                            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ActionIconsComponent('{ControlId}', {Utility.JsonSerialize(setup)});");
                        }
                    }
                    break;
            }
            return hb.ToString();
        }
    }
}
