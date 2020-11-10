/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Menu component implementation.
    /// </summary>
    public abstract class MenuComponentBase : YetaWFComponent {

        internal const string TemplateName = "Menu";

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

        /// <summary>
        /// Defines the menu rendering engine.
        /// </summary>
        public enum MenuStyleEnum {
            /// <summary>
            /// The menu used with the page skin is used.
            /// </summary>
            Automatic = 0,
            /// <summary>
            /// A bootstrap menu is used.
            /// </summary>
            Bootstrap = 1,
            /// <summary>
            /// A Kendo UI menu is used.
            /// </summary>
            Kendo = 2,
        }

        /// <summary>
        /// Defines the direction in which the menu opens.
        /// </summary>
        public enum DirectionEnum {
            /// <summary>
            /// Opens towards the bottom.
            /// </summary>
            [EnumDescription("Opens towards the bottom")]
            Bottom = 0,
            /// <summary>
            /// Opens towards the top.
            /// </summary>
            [EnumDescription("Opens towards the top")]
            Top = 1,
            /// <summary>
            /// Opens towards the left.
            /// </summary>
            [EnumDescription("Opens towards the left")]
            Left = 2,
            /// <summary>
            /// Opens towards the right.
            /// </summary>
            [EnumDescription("Opens towards the right")]
            Right = 3,
        }

        /// <summary>
        /// Defines the menu orientation.
        /// </summary>
        public enum OrientationEnum {
            /// <summary>
            /// Horizontal menu.
            /// </summary>
            [EnumDescription("Horizontal menu")]
            Horizontal = 0,
            /// <summary>
            /// Vertical menu.
            /// </summary>
            [EnumDescription("Vertical menu")]
            Vertical = 1,
        }

        /// <summary>
        /// An instance of this class is used as data model for the Menu component.
        /// </summary>
        public class MenuData {
            /// <summary>
            /// Defines the menu entries.
            /// </summary>
            public MenuList MenuList { get; set; }
            /// <summary>
            /// The direction in which the menu opens.
            /// </summary>
            public DirectionEnum Direction { get; set; }
            /// <summary>
            /// The orientation of the menu.
            /// </summary>
            public OrientationEnum Orientation { get; set; }
            /// <summary>
            /// The CSS class added to the top-level HTML tag representing the menu. May be null.
            /// </summary>
            public string CssClass { get; set; }
            /// <summary>
            /// Defines the delay (in milliseconds) before the menu is opened/closed, which is used to avoid accidental closure on leaving the menu.
            /// This is ignored for Bootstrap menus.
            /// </summary>
            public int HoverDelay { get; set; }
            /// <summary>
            /// Defines whether the menu entries representing the active page are highlighted.
            /// This is not currently implemented.
            /// </summary>
            public bool ShowPath { get; set; }
        }
    }

    /// <summary>
    /// Displays a menu.
    /// </summary>
    /// <remarks>
    /// This is primarily used by the Menu Module and Main Menu Module (YetaWF.Menus package) and is not directly used by applications.
    /// </remarks>
    [UsesAdditional("Style", "MenuComponentBase.MenuStyleEnum", "Automatic", "Defines the menu rendering engine.")]
    public class MenuDisplayComponent : MenuComponentBase, IYetaWFComponent<MenuComponentBase.MenuData> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(MenuComponentBase.MenuData model) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuStyleEnum style = PropData.GetAdditionalAttributeValue("Style", MenuStyleEnum.Automatic);
            if (style == MenuStyleEnum.Automatic)
                style = Manager.SkinInfo.UsingBootstrap ? MenuStyleEnum.Bootstrap : MenuStyleEnum.Kendo;

            if (style == MenuStyleEnum.Bootstrap) {

                string menu = (await CoreRendering.RenderMenuAsync(model.MenuList, DivId, null, RenderEngine: YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.BootstrapSmartMenu, HtmlHelper: HtmlHelper));
                if (!string.IsNullOrWhiteSpace(menu)) {
                    await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "github.com.vadikom.smartmenus"); // multilevel navbar
                    hb.Append(menu);
                }

            } else {

                string menu = (await CoreRendering.RenderMenuAsync(model.MenuList, DivId, model.CssClass, RenderEngine: YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.KendoMenu, HtmlHelper: HtmlHelper));
                if (!string.IsNullOrWhiteSpace(menu)) {

                    //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                    await KendoUICore.AddFileAsync("kendo.menu.min.js");

                    hb.Append($@"
<div id='{ControlId}' class='yt_kendomenu t_display' role='navigation'>
    {menu}
</div>");
                    Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MenuComponent('{ControlId}', {{ MenuId: '{DivId}', Style: {(int)style}, Direction: {(int)model.Direction}, Orientation: {(int)model.Orientation}, PopupCollision: 'fit flip', HoverDelay: {model.HoverDelay} }});");
                }
            }

            return hb.ToString();
        }
    }
}
