/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

        internal class MenuSetup {
            public int SmallMenuMaxWidth { get; set; }
            public int HoverDelay { get; set; }
        }   
        internal static async Task<string> RenderMenuAsync(MenuList menu, string id = null, string cssClass = null, bool Hidden = false, YHtmlHelper HtmlHelper = null, bool WantArrows = false) {

            if (menu.Count == 0)
                return string.Empty;
            string menuContents = await RenderLIAsync(HtmlHelper, menu, null, menu.RenderMode, menu.LICssClass, 0, WantArrows);
            if (string.IsNullOrWhiteSpace(menuContents))
                return string.Empty;

            string style = string.Empty;
            if (Hidden)
                style = " style='display:none;'";

            string css = "t_lvl0";
            css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(cssClass));

            if (id == null)
                id = Manager.UniqueId();
        
            return $@"<ul{(id != null ? $" id='{id}'" : "")} class='{css}'{style}>{menuContents}</ul>";
        }

        private static async Task<string> RenderMenuAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, int level, bool wantArrows) {

            string menuContents = await RenderLIAsync(htmlHelper, subMenu, subGuid, renderMode, null, level, wantArrows);
            if (string.IsNullOrWhiteSpace(menuContents)) return string.Empty;

            string css = $"t_lvl{level}";
            if (subGuid != null)
                css = CssManager.CombineCss(css, "t_megamenu_content");
            css = CssManager.CombineCss(css, cssClass);

            return $@"<ul class='{css}' style='display:none'>{menuContents}</ul>";
        }

        private static async Task<string> RenderLIAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, string liCss, int level, bool wantArrows) {

            HtmlBuilder hb = new HtmlBuilder();

            if (subGuid != null) {
                // megamenu content
                string css = string.Empty;
                css = CssManager.CombineCss(css, liCss);

                string contents = string.Empty;
                ModuleDefinition subMod = await ModuleDefinition.LoadAsync((Guid)subGuid, AllowNone: true);
                if (subMod != null) {
                    subMod.ShowTitle = false; // don't show the module title in a submenu (temp. override)
                    if (htmlHelper == null)
                        throw new InternalError("HtmlHelper required for module rendering");
                    contents = await subMod.RenderModuleAsync(htmlHelper);
                }
                hb.Append($"<li class='t_megamenu_content {css} t_lvl{level}'>{contents}</li>\n");

            } else {
                foreach (var menuEntry in subMenu) {

                    if (menuEntry.Enabled && await menuEntry.RendersSomethingAsync()) {

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
                                string arrow = null;
                                if (wantArrows)
                                    arrow = SkinSVGs.Get(AreaRegistration.CurrentPackage, level == 0 ? "fas-caret-down" : "fas-caret-right");
                                string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, EndIcon: arrow);

                                hb.Append($"<li class='t_lvl{level} {css}'>\n{menuContents}\n{subMenuContents}</li>\n");
                                rendered = true;
                            }
                        }

                        if (!rendered) {
                            string css = string.Empty;
                            css = CssManager.CombineCss(css, liCss);

                            string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null);

                            hb.Append($"<li class='t_lvl{level} {css}'>{menuContents}</li>\n");
                        }
                    }
                }
            }
            return hb.ToString();
        }
    }
}
