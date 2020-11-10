/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// <remarks>
    /// If a Bootstrap skin is used, the menu is rendered as a Bootstrap navbar.
    /// </remarks>
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

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }
        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        internal class ActionIconsSetup {
            public string MenuId { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.button.min.js");

            // Add required menu support
            await KendoUICore.AddFileAsync("kendo.menu.min.js");
            await Manager.AddOnManager.AddTemplateAsync(YetaWF.Modules.ComponentsHTML.Controllers.AreaRegistration.CurrentPackage.AreaName, "MenuUL", ComponentType.Display);

            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
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
{await CoreRendering.RenderMenuAsync(model, null, Addons.Templates.ActionIcons.CssActionIcons, HtmlHelper: HtmlHelper)}");
                    break;

                case Grid.GridActionsEnum.DropdownMenu:
                case Grid.GridActionsEnum.Mini: {

                        model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;
                        string buttonId = ControlId + "_btn";
                        ActionIconsSetup setup = new ActionIconsSetup {
                            MenuId = ControlId + "_menu",
                        };
                        string menuHTML = await CoreRendering.RenderMenuAsync(model, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true);

                        if (!string.IsNullOrWhiteSpace(menuHTML)) {

                            DropDownButtonComponent.Model ddModel = new DropDownButtonComponent.Model {
                                Text = __ResStr("dropdownText", "Manage"),
                                Tooltip = null,
                                ButtonId = buttonId,
                                MenuHTML = menuHTML,
                                Mini = false,
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
