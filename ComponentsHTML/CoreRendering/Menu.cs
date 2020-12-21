/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        /// <summary>
        /// Renders a complete menu.
        /// </summary>
        /// <param name="menu">The menu to render.</param>
        /// <param name="id">The menu ID to generate.</param>
        /// <param name="cssClass">The optional CSS classes to use for the menu.</param>
        /// <param name="HtmlHelper">The HtmlHelper instance.</param>
        /// <returns>Returns the complete menu as HTML.</returns>
        public Task<string> RenderMenuListAsync(MenuList menu, string id = null, string cssClass = null, YHtmlHelper HtmlHelper = null) {
            return RenderMenuAsync(menu, id, cssClass, RenderEngine: ModuleAction.RenderEngineEnum.BootstrapSmartMenu, HtmlHelper: HtmlHelper);
        }

        internal static async Task<string> RenderMenuAsync(MenuList menu, string id = null, string cssClass = null, ModuleAction.RenderEngineEnum RenderEngine = ModuleAction.RenderEngineEnum.KendoMenu, bool Hidden = false, YHtmlHelper HtmlHelper = null) {
            int level = 0;

            if (menu.Count == 0)
                return string.Empty;
            string menuContents = await RenderLIAsync(HtmlHelper, menu, null, menu.RenderMode, RenderEngine, menu.LICssClass, level);
            if (string.IsNullOrWhiteSpace(menuContents))
                return string.Empty;

            string style = string.Empty;

            if (Hidden)
                style = " style=display:none;";

            string css = $"t_lvl{level}";
            css = CssManager.CombineCss(css, Manager.AddOnManager.CheckInvokedCssModule(cssClass));
            if (RenderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                css = CssManager.CombineCss(css, "nav");
                css = CssManager.CombineCss(css, "navbar-nav");
            }

            if (!string.IsNullOrWhiteSpace(id))
                id = $" id='{id}'";

            return $@"<ul{id} class='{css}'{style}>{menuContents}</ul>";
        }
        internal static async Task<string> RenderMenuAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {

            string menuContents = await RenderLIAsync(htmlHelper, subMenu, subGuid, renderMode, renderEngine, null, level);
            if (string.IsNullOrWhiteSpace(menuContents)) return string.Empty;

            string css = $"t_lvl{level}";
            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                css = CssManager.CombineCss(css, "dropdown-menu");
            if (subGuid != null) {
                css = CssManager.CombineCss(css, "t_megamenu_content");
                css = CssManager.CombineCss(css, "mega-menu"); // used by smartmenus
            }
            css = CssManager.CombineCss(css, cssClass);

            return $@"<ul class='{css}'>{menuContents}</ul>";
        }

        internal static async Task<string> RenderLIAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {

            HtmlBuilder hb = new HtmlBuilder();

            ++level;

            if (subGuid != null) {
                // megamenu content
                string css = string.Empty;
                css = CssManager.CombineCss(css, liCss);
                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                    css = CssManager.CombineCss(css, "nav-item");

                string contents = string.Empty;
                ModuleDefinition subMod = await ModuleDefinition.LoadAsync((Guid)subGuid, AllowNone: true);
                if (subMod != null) {
                    subMod.ShowTitle = false; // don't show the module title in a submenu (temp. override)
                    if (htmlHelper == null)
                        throw new InternalError("HtmlHelper required for module rendering");
                    contents = await subMod.RenderModuleAsync(htmlHelper);
                }
                hb.Append($"<li class='t_megamenu_content{css}'>{contents}</li>\n");

            } else {
                foreach (var menuEntry in subMenu) {

                    if (menuEntry.Enabled && await menuEntry.RendersSomethingAsync()) {

                        bool rendered = false;

                        Guid? subModGuid = null;
                        if (!Manager.EditMode) {
                            // don't show submodule in edit mode
                            if ((menuEntry.SubModule != null && menuEntry.SubModule != Guid.Empty))
                                subModGuid = menuEntry.SubModule;
                        }

                        if (subModGuid != null || (menuEntry.SubMenu != null && menuEntry.SubMenu.Count > 0)) {

                            string subMenuContents = await RenderMenuAsync(htmlHelper, menuEntry.SubMenu, subModGuid, menuEntry.CssClass, renderMode, renderEngine, level);
                            if (!string.IsNullOrWhiteSpace(subMenuContents)) {

                                string css = string.Empty;
                                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                                    css = CssManager.CombineCss(css, "dropdown");
                                    css = CssManager.CombineCss(css, "nav-item");
                                }
                                if (subModGuid != null)
                                    css = CssManager.CombineCss(css, "t_megamenu_hassub");
                                css = CssManager.CombineCss(css, liCss);

                                string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, HasSubmenu: true, BootstrapSmartMenuLevel: level);

                                css = string.IsNullOrWhiteSpace(css) ? string.Empty : $" class='{css}'";
                                hb.Append($"<li{css}>\n{menuContents}\n{subMenuContents}</li>\n");
                                rendered = true;
                            }
                        }

                        if (!rendered) {
                            string css = string.Empty;
                            css = CssManager.CombineCss(css, liCss);

                            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                                css = CssManager.CombineCss(css, "nav-item");

                            css = string.IsNullOrWhiteSpace(css) ? string.Empty : $" class='{css}'";
                            string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, BootstrapSmartMenuLevel: level);

                            hb.Append($"<li{css}>{menuContents}</li>\n");
                        }
                    }
                }
            }

            --level;

            return hb.ToString();
        }
    }
}
