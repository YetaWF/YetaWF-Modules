/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif

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
        public Task<string> RenderMenuListAsync(MenuList menu, string id = null, string cssClass = null, YHtmlHelper HtmlHelper = null)
        {
            return RenderMenuAsync(menu, id, cssClass, RenderEngine: ModuleAction.RenderEngineEnum.BootstrapSmartMenu, HtmlHelper: HtmlHelper);
        }

        internal static async Task<string> RenderMenuAsync(MenuList menu, string id = null, string cssClass = null,
            ModuleAction.RenderEngineEnum RenderEngine = ModuleAction.RenderEngineEnum.KendoMenu, bool Hidden = false, YHtmlHelper HtmlHelper = null)
        {

            HtmlBuilder hb = new HtmlBuilder();
            int level = 0;

            if (menu.Count == 0)
                return null;
            string menuContents = await RenderLIAsync(HtmlHelper, menu, null, menu.RenderMode, RenderEngine, menu.LICssClass, level);
            if (string.IsNullOrWhiteSpace(menuContents))
                return null;

            // <ul class= style= >
            YTagBuilder ulTag = new YTagBuilder("ul");
            if (Hidden)
                ulTag.Attributes.Add("style", "display:none");
            if (!string.IsNullOrWhiteSpace(cssClass))
                ulTag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));
            ulTag.AddCssClass(string.Format("t_lvl{0}", level));
            if (RenderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                ulTag.AddCssClass("nav");
                ulTag.AddCssClass("navbar-nav");
            }
            if (!string.IsNullOrWhiteSpace(id))
                ulTag.Attributes.Add("id", id);
            hb.Append(ulTag.ToString(YTagRenderMode.StartTag));

            // <li>....</li>
            hb.Append(menuContents);

            // </ul>
            hb.Append(ulTag.ToString(YTagRenderMode.EndTag));

            return hb.ToString();
        }
        internal static async Task<string> RenderMenuAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {

            HtmlBuilder hb = new HtmlBuilder();

            string menuContents = await RenderLIAsync(htmlHelper, subMenu, subGuid, renderMode, renderEngine, null, level);
            if (string.IsNullOrWhiteSpace(menuContents)) return null;

            // <ul>
            YTagBuilder ulTag = new YTagBuilder("ul");
            ulTag.AddCssClass(string.Format("t_lvl{0}", level));
            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                ulTag.AddCssClass("dropdown-menu");
            if (subGuid != null) {
                ulTag.AddCssClass("t_megamenu_content");
                ulTag.AddCssClass("mega-menu"); // used by smartmenus
            }
            if (!string.IsNullOrWhiteSpace(cssClass))
                ulTag.AddCssClass(cssClass);
            hb.Append(ulTag.ToString(YTagRenderMode.StartTag));

            // <li>....</li>
            hb.Append(menuContents);

            // </ul>
            hb.Append(ulTag.ToString(YTagRenderMode.EndTag));

            return hb.ToString();
        }

        internal static async Task<string> RenderLIAsync(YHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {
            HtmlBuilder hb = new HtmlBuilder();

            ++level;

            if (subGuid != null) {
                // megamenu content
                // <li>
                YTagBuilder tag = new YTagBuilder("li");
                tag.AddCssClass("t_megamenu_content");
                if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                    tag.AddCssClass("nav-item");
                hb.Append(tag.ToString(YTagRenderMode.StartTag));

                ModuleDefinition subMod = await ModuleDefinition.LoadAsync((Guid)subGuid, AllowNone: true);
                if (subMod != null) {
                    subMod.ShowTitle = false; // don't show the module title in a submenu (temp. override)
                    if (htmlHelper == null)
                        throw new InternalError("HtmlHelper required for module rendering");
                    hb.Append(await subMod.RenderModuleAsync(htmlHelper));
                }

                hb.Append("</li>\n");
            } else {
                foreach (var menuEntry in subMenu) {

                    if (menuEntry.Enabled && await menuEntry.RendersSomethingAsync()) {

                        bool rendered = false;
                        string subMenuContents = null;

                        Guid? subModGuid = null;
                        if (!Manager.EditMode) {
                            // don't show submodule in edit mode
                            if ((menuEntry.SubModule != null && menuEntry.SubModule != Guid.Empty))
                                subModGuid = menuEntry.SubModule;
                        }

                        if (subModGuid != null || (menuEntry.SubMenu != null && menuEntry.SubMenu.Count > 0)) {

                            subMenuContents = await RenderMenuAsync(htmlHelper, menuEntry.SubMenu, subModGuid, menuEntry.CssClass, renderMode, renderEngine, level);
                            if (!string.IsNullOrWhiteSpace(subMenuContents)) {
                                // <li>
                                YTagBuilder tag = new YTagBuilder("li");
                                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                                    tag.AddCssClass("dropdown");
                                    tag.AddCssClass("nav-item");
                                }
                                if (subModGuid != null)
                                    tag.AddCssClass("t_megamenu_hassub");
                                if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                                hb.Append(tag.ToString(YTagRenderMode.StartTag));

                                string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, HasSubmenu: true, BootstrapSmartMenuLevel: level);
                                hb.Append(menuContents);

                                hb.Append("\n");
                                hb.Append(subMenuContents);

                                hb.Append("</li>\n");
                                rendered = true;
                            }
                        }

                        if (!rendered) {
                            // <li>
                            YTagBuilder tag = new YTagBuilder("li");
                            //if (!menuEntry.Enabled)
                            //    tag.MergeAttribute("disabled", "disabled");
                            if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                                tag.AddCssClass("nav-item");
                            }
                            hb.Append(tag.ToString(YTagRenderMode.StartTag));

                            string menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, BootstrapSmartMenuLevel: level);
                            hb.Append(menuContents);

                            hb.Append("</li>\n");
                        }
                    }
                }
            }

            --level;

            return hb.ToString();
        }
    }
}
