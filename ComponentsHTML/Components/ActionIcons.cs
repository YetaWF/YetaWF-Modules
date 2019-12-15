/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ActionIcons component implementation.
    /// </summary>
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
            await KendoUICore.AddFileAsync("kendo.menu.min.js");
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

                case Grid.GridActionsEnum.DropdownMenu: {
                        model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;
                        string buttonId = ControlId + "_btn";
                        ActionIconsSetup setup = new ActionIconsSetup {
                            MenuId = ControlId + "_menu",
                        };
                        string menuHTML = await CoreRendering.RenderMenuAsync(model, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true);

                        if (!string.IsNullOrWhiteSpace(menuHTML)) {
                            hb.Append($@"
<button id='{buttonId}' type='button' class='yt_actionicons'>
    {HE(__ResStr("dropdownText", "Manage"))}<span class='k-icon k-i-arrow-60-down'></span>
    {menuHTML}
</button>");
                            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ActionIconsComponent('{buttonId}', {Utility.JsonSerialize(setup)});");
                        }
                        break;
                    }
                case Grid.GridActionsEnum.Mini: {
                        model.RenderMode = ModuleAction.RenderModeEnum.NormalMenu;
                        string buttonId = ControlId + "_btn";
                        ActionIconsSetup setup = new ActionIconsSetup {
                            MenuId = ControlId + "_menu",
                        };
                        string menuHTML = await CoreRendering.RenderMenuAsync(model, setup.MenuId, Globals.CssGridActionMenu, HtmlHelper: HtmlHelper, Hidden: true);

                        if (!string.IsNullOrWhiteSpace(menuHTML)) {
                            hb.Append($@"
<button id='{buttonId}' href='#' class='yt_actionicons t_mini'><span class='k-icon k-i-arrow-60-down'></span>
    {menuHTML}
</button>");
                            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ActionIconsComponent('{buttonId}', {Utility.JsonSerialize(setup)});");
                        }
                        break;
                    }
            }
            return hb.ToString();
        }
    }
}
