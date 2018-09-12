/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using System.Collections.Generic;
using System;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        public Task<YHtmlString> RenderMenuListAsync(MenuList menu, string id = null, string cssClass = null,
#if MVC6
            IHtmlHelper HtmlHelper = null
#else
            HtmlHelper HtmlHelper = null
#endif
            )
        {
            return RenderMenuAsync(menu, id, cssClass, RenderEngine: ModuleAction.RenderEngineEnum.BootstrapSmartMenu, HtmlHelper: HtmlHelper);
        }
        public static async Task<YHtmlString> RenderMenuAsync(MenuList menu, string id = null, string cssClass = null,
            ModuleAction.RenderEngineEnum RenderEngine = ModuleAction.RenderEngineEnum.JqueryMenu,
            bool Hidden = false,
#if MVC6
            IHtmlHelper HtmlHelper = null
#else
            HtmlHelper HtmlHelper = null
#endif
            )
        {

            HtmlBuilder hb = new HtmlBuilder();
            int level = 0;

            if (menu.Count == 0)
                return new YHtmlString("");
            string menuContents = await RenderLIAsync(HtmlHelper, menu, null, menu.RenderMode, RenderEngine, menu.LICssClass, level);
            if (string.IsNullOrWhiteSpace(menuContents))
                return new YHtmlString("");

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

            return hb.ToYHtmlString();
        }
#if MVC6
        internal static async Task<YHtmlString> RenderMenuAsync(IHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {
#else
        internal static async Task<YHtmlString> RenderMenuAsync(HtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {
#endif
            HtmlBuilder hb = new HtmlBuilder();

            string menuContents = await RenderLIAsync(htmlHelper, subMenu, subGuid, renderMode, renderEngine, null, level);
            if (string.IsNullOrWhiteSpace(menuContents)) return new YHtmlString("");

            // <ul>
            TagBuilder ulTag = new TagBuilder("ul");
            ulTag.AddCssClass(string.Format("t_lvl{0}", level));
            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                ulTag.AddCssClass("dropdown-menu");
            if (subGuid != null) {
                ulTag.AddCssClass("t_megamenu_content");
                ulTag.AddCssClass("mega-menu"); // used by smartmenus
            }
            if (!string.IsNullOrWhiteSpace(cssClass))
                ulTag.AddCssClass(cssClass);
            hb.Append(ulTag.ToString(TagRenderMode.StartTag));

            // <li>....</li>
            hb.Append(menuContents);

            // </ul>
            hb.Append(ulTag.ToString(TagRenderMode.EndTag));

            return hb.ToYHtmlString();
        }
#if MVC6
        internal static async Task<string> RenderLIAsync(IHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {
#else
        internal static async Task<string> RenderLIAsync(HtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {
#endif
            HtmlBuilder hb = new HtmlBuilder();

            ++level;

            if (subGuid != null) {
                // megamenu content
                // <li>
                TagBuilder tag = new TagBuilder("li");
                tag.AddCssClass("t_megamenu_content");
                if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu)
                    tag.AddCssClass("nav-item");
                hb.Append(tag.ToString(TagRenderMode.StartTag));

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

                            subMenuContents = (await RenderMenuAsync(htmlHelper, menuEntry.SubMenu, subModGuid, menuEntry.CssClass, renderMode, renderEngine, level)).ToString();
                            if (!string.IsNullOrWhiteSpace(subMenuContents)) {
                                // <li>
                                TagBuilder tag = new TagBuilder("li");
                                if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                                    tag.AddCssClass("dropdown");
                                    tag.AddCssClass("nav-item");
                                }
                                if (subModGuid != null)
                                    tag.AddCssClass("t_megamenu_hassub");
                                if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                                hb.Append(tag.ToString(TagRenderMode.StartTag));

                                YHtmlString menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, HasSubmenu: true, BootstrapSmartMenuLevel: level);
                                hb.Append(menuContents);

                                hb.Append("\n");
                                hb.Append(subMenuContents);

                                hb.Append("</li>\n");
                                rendered = true;
                            }
                        }

                        if (!rendered) {
                            // <li>
                            TagBuilder tag = new TagBuilder("li");
                            //if (!menuEntry.Enabled)
                            //    tag.MergeAttribute("disabled", "disabled");
                            if (!string.IsNullOrWhiteSpace(liCss)) tag.AddCssClass(liCss);
                            if (renderEngine == ModuleAction.RenderEngineEnum.BootstrapSmartMenu) {
                                tag.AddCssClass("nav-item");
                            }
                            hb.Append(tag.ToString(TagRenderMode.StartTag));

                            YHtmlString menuContents = await CoreRendering.RenderActionAsync(menuEntry, renderMode, null, RenderEngine: renderEngine, BootstrapSmartMenuLevel: level);
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
