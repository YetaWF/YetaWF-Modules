/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays a menu.
    /// </summary>
    public partial class MenuDisplayComponent {

        /// <summary>
        /// Defines the overall menu appearance and behavior.
        /// </summary>
        public class MenuSetup {
            /// <summary>
            /// Defines the orientation of the menu.
            /// </summary>
            public OrientationEnum Orientation { get; set; }
            /// <summary>
            /// Defines the width of a vertical menu. Ignored for horizontal or mobile menus.
            /// </summary>
            public int VerticalWidth { get; set; }
            /// <summary>
            /// Defines the maximum width of the screen where a mobile menu is displayed. 
            /// Once the screen is above the defined width, a regular horizontal or vertical menu is displayed.
            /// Set to 0 to never display a mobile menu.
            /// </summary>
            public int SmallMenuMaxWidth { get; set; }
            /// <summary>
            /// Defines whether the dropdown is automatically opened when hovering over a horizontal menu.
            /// </summary>
            public bool OpenOnHover { get; set; }
            /// <summary>
            /// Defines the number of milliseconds the cursor can move outside a horizontal menu before all submenus are closed. Ignored for vertical and mobile menus.
            /// </summary>
            public int HoverDelay { get; set; }
            /// <summary>
            /// Defines the alignment of dropdown menus.
            /// </summary>
            public HorizontalAlignEnum HorizontalAlign { get; set; }
        }   
        internal static async Task<string> RenderMenuAsync(MenuList? menu, string? id = null, string? cssClass = null, bool Hidden = false, YHtmlHelper? HtmlHelper = null, bool WantArrows = false, int? width = null) {

            if (menu == null || menu.Count == 0)
                return string.Empty;
            string menuContents = await RenderLIAsync(HtmlHelper, menu, null, menu.RenderMode, menu.LICssClass, 0, WantArrows);
            if (string.IsNullOrWhiteSpace(menuContents))
                return string.Empty;

            string style = string.Empty;
            string styleBits = string.Empty;
            if (Hidden)
                styleBits += "display:none;";
            if (width != null)
                styleBits += $"width:{(int)width}px;";
            if (!string.IsNullOrEmpty(styleBits))
                style = $" style='{styleBits}'";

            string css = "t_lvl0";
            css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(cssClass));

            if (id == null)
                id = Manager.UniqueId();
        
            return $@"<ul{(id != null ? $" id='{id}'" : "")} class='t_menu {css}'{style}>{menuContents}</ul>";
        }

        private static async Task<string> RenderMenuAsync(YHtmlHelper? htmlHelper, List<ModuleAction>? subMenu, Guid? subGuid, string? cssClass, ModuleAction.RenderModeEnum renderMode, int level, bool wantArrows) {

            string menuContents = await RenderLIAsync(htmlHelper, subMenu, subGuid, renderMode, null, level, wantArrows);
            if (string.IsNullOrWhiteSpace(menuContents)) return string.Empty;

            string css = $"t_lvl{level}";
            if (subGuid != null)
                css = CssManager.CombineCss(css, "t_megamenu_content");
            css = CssManager.CombineCss(css, cssClass);

            return $@"<ul class='t_menu {css}' style='display:none'>{menuContents}</ul>";
        }

        /// <summary>
        /// Renders  module actions for use in a menu.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="subMenu"></param>
        /// <param name="subGuid"></param>
        /// <param name="renderMode"></param>
        /// <param name="liCss"></param>
        /// <param name="level"></param>
        /// <param name="wantArrows"></param>
        /// <returns></returns>
        public static async Task<string> RenderLIAsync(YHtmlHelper? htmlHelper, List<ModuleAction>? subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, string? liCss, int level, bool wantArrows) {

            HtmlBuilder hb = new HtmlBuilder();

            if (subGuid != null) {
                // megamenu content
                string css = string.Empty;
                css = CssManager.CombineCss(css, liCss);

                string contents = string.Empty;
                ModuleDefinition? subMod = await ModuleDefinition.LoadAsync((Guid)subGuid, AllowNone: true);
                if (subMod != null) {
                    subMod.ShowTitle = false; // don't show the module title in a submenu (temp. override)
                    if (htmlHelper == null)
                        throw new InternalError("HtmlHelper required for module rendering");
                    contents = await subMod.RenderModuleWithContainerAsync(htmlHelper);
                }
                hb.Append($"<li class='t_menu t_megamenu_content {css} t_lvl{level}'>{contents}</li>\n");

            } else if (subMenu != null) {

                int totalEntries = 0, separatorEntries = 0;
                foreach (var menuEntry in subMenu) {

                    if (menuEntry.EntryType == ModuleAction.MenuEntryType.Separator || (menuEntry.Enabled && await menuEntry.RendersSomethingAsync())) {
                        ++totalEntries;
                        if (menuEntry.EntryType == ModuleAction.MenuEntryType.Separator)
                            ++separatorEntries;

                        bool rendered = false;

                        Guid? subModGuid = null;
                        if (!Manager.EditMode) {
                            // don't show submodule in edit mode
                            if (menuEntry.SubModule != null && menuEntry.SubModule != Guid.Empty)
                                subModGuid = menuEntry.SubModule;
                        }

                        if (subModGuid != null || (menuEntry.SubMenu != null && menuEntry.SubMenu.Count > 0)) {

                            string subMenuContents = await RenderMenuAsync(htmlHelper, menuEntry.SubMenu, subModGuid, menuEntry.CssClass, renderMode, level + 1, wantArrows);
                            if (!string.IsNullOrWhiteSpace(subMenuContents)) {

                                string css = string.Empty;
                                if (subModGuid != null)
                                    css = CssManager.CombineCss(css, "t_megamenu_hassub");
                                css = CssManager.CombineCss(css, liCss);
                                css = CssManager.CombineCss(css, "t_hassub");
                                string? arrow = null;
                                if (wantArrows)
                                    arrow = SkinSVGs.GetCaret(AreaRegistration.CurrentPackage, "down");
                                string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, null, EndIcon: arrow);

                                hb.Append($"<li class='t_menu t_lvl{level} {css}'>\n{menuContents}\n{subMenuContents}</li>\n");
                                rendered = true;
                            }
                        }

                        if (!rendered) {
                            string css = string.Empty;
                            css = CssManager.CombineCss(css, liCss);

                            if (menuEntry.EntryType != ModuleAction.MenuEntryType.Parent) {
                                string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, null);
                                hb.Append($"<li class='t_menu t_lvl{level} {css}'>{menuContents}</li>\n");
                            }
                        }
                    }
                }
                if (totalEntries == separatorEntries) // we only generated separators
                    return string.Empty;
            }
            return hb.ToString();
        }
    }
}
