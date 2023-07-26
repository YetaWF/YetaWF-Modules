/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

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
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
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
        /// Defines the alignment of dropdown menus.
        /// </summary>
        public enum HorizontalAlignEnum {
            /// <summary>
            /// Align along right side.
            /// </summary>
            Right = 0,
            /// Align along left side.
            Left = 1,
        }

        /// <summary>
        /// An instance of this class is used as data model for the Menu component.
        /// </summary>
        public class MenuData {
            /// <summary>
            /// Defines the menu entries.
            /// </summary>
            public MenuList MenuList { get; set; } = null!;
            /// <summary>
            /// The CSS class added to the top-level HTML tag representing the menu. May be null.
            /// </summary>
            public string? CssClass { get; set; }
            /// <summary>
            /// Defines the delay (in milliseconds) before the menu is closed, which is used to avoid accidental closure on leaving the menu.
            /// </summary>
            public int HoverDelay { get; set; }
            /// <summary>
            /// Defines the orientation of the menu.
            /// </summary>
            public OrientationEnum Orientation { get; set; }
            /// <summary>
            /// Defines the width of the menu (vertical mode only) in pixels.
            /// </summary>
            public int VerticalWidth { get; set; }
            /// <summary>
            /// Defines the largest screen size for which the small menu is shown. If the screen is wider, the large menu is shown.
            /// </summary>
            public int SmallMenuMaxWidth { get; set; }
        }
    }

    /// <summary>
    /// Displays a menu.
    /// </summary>
    public partial class MenuDisplayComponent : MenuComponentBase, IYetaWFComponent<MenuComponentBase.MenuData> {

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

            string css = "yt_menu t_display t_large"; // start out with a large menu
            css = CssManager.CombineCss(css, model.Orientation == OrientationEnum.Horizontal ? "t_horizontal" : "t_vertical");
            string menuHTML = await RenderMenuAsync(model.MenuList, DivId, css, WantArrows: true, width: model.Orientation == OrientationEnum.Vertical ? model.VerticalWidth : null, HtmlHelper: HtmlHelper);
            if (string.IsNullOrWhiteSpace(menuHTML))
                return string.Empty;

            HtmlBuilder hb = new HtmlBuilder();
            // Help menu out so the FOUC is minimized on small screen
            // we generate a large menu (not knowing the screen size).
            // on small screens the large menu is hidden by javascript, which is a bit too late
            // (explicit selector (specificity) below needed for specificity to hide large menu on small sreen during full page load)
            if (model.SmallMenuMaxWidth > 0) {
                hb.Append($@"
<style>
    @media (max-width: {model.SmallMenuMaxWidth}px) {{
        .yPageMenu ul.yt_menu.t_display.t_large {{
            display: none;
        }}
    }}
</style>");
            }

            hb.Append($@"
{menuHTML}");

            MenuSetup setup = new MenuSetup {
                Orientation = model.Orientation,
                VerticalWidth = model.VerticalWidth,
                SmallMenuMaxWidth = model.SmallMenuMaxWidth,
                HoverDelay = model.HoverDelay,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MenuComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
