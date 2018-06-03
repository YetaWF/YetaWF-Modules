using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using System.Collections.Generic;
using System;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Components {

    public abstract class MenuComponentBase : YetaWFComponent {

        public const string TemplateName = "Menu";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public enum MenuStyleEnum {
            Automatic = 0,
            Bootstrap = 1,
            Kendo = 2,
        }

        public enum DirectionEnum {
            [EnumDescription("Opens towards the bottom")]
            Bottom = 0,
            [EnumDescription("Opens towards the top")]
            Top = 1,
            [EnumDescription("Opens towards the left")]
            Left = 2,
            [EnumDescription("Opens towards the right")]
            Right = 3,
        }

        public enum OrientationEnum {
            [EnumDescription("Horizontal menu")]
            Horizontal = 0,
            [EnumDescription("Vertical menu")]
            Vertical = 1,
        }

        public enum AnimationEnum {
            [EnumDescription("Slides up")]
            SlideUp = 0,
            [EnumDescription("Slides down")]
            SlideDown = 1,
            [EnumDescription("Fades in")]
            FadeIn = 2,
            [EnumDescription("Expands up")]
            ExpandsUp = 3,
            [EnumDescription("Expands down")]
            ExpandsDown = 4,
        }

        public class MenuData {
            public MenuList MenuList { get; set; }
            public DirectionEnum Direction { get; set; }
            public OrientationEnum Orientation { get; set; }
            public string CssClass { get; set; }
            public int HoverDelay { get; set; }
            public bool ShowPath { get; set; }

            public string GetDirection() {
                switch (Direction) {
                    default: return "default";
                    case DirectionEnum.Top: return "top";
                    case DirectionEnum.Bottom: return "bottom";
                    case DirectionEnum.Left: return "left";
                    case DirectionEnum.Right: return "right";
                }
            }
            public string GetOrientation() {
                switch (Orientation) {
                    default:
                    case OrientationEnum.Horizontal: return "horizontal";
                    case OrientationEnum.Vertical: return "vertical";
                }
            }
            //public string GetOpenEffects() {
            //    return GetEffects(OpenAnimation);
            //}
            //public string GetCloseEffects() {
            //    return GetEffects(CloseAnimation);
            //}
            //public string GetEffects(AnimationEnum anim) {
            //    switch (anim) {
            //        default:
            //        case AnimationEnum.SlideUp: return "slideIn:up";
            //        case AnimationEnum.SlideDown: return "slideIn:down";
            //        case AnimationEnum.FadeIn: return "fadeIn";
            //        case AnimationEnum.ExpandsDown: return "expand:down";
            //        case AnimationEnum.ExpandsUp: return "expand:up";
            //    }
            //}
        }
#if MVC6
        internal static async Task<YHtmlString> RenderMenuAsync(IHtmlHelper htmlHelper, MenuList model, string id = null, string cssClass = null, ModuleAction.RenderEngineEnum RenderEngine = ModuleAction.RenderEngineEnum.JqueryMenu) {
#else
        internal static async Task<YHtmlString> RenderMenuAsync(HtmlHelper htmlHelper, MenuList model, string id = null, string cssClass = null, ModuleAction.RenderEngineEnum RenderEngine = ModuleAction.RenderEngineEnum.JqueryMenu) {
#endif

            HtmlBuilder hb = new HtmlBuilder();
            int level = 0;

            if (model.Count == 0)
                return new YHtmlString("");
            string menuContents = await RenderLIAsync(htmlHelper, model, null, model.RenderMode, RenderEngine, model.LICssClass, level);
            if (string.IsNullOrWhiteSpace(menuContents))
                return new YHtmlString("");

            // <ul class= style= >
            YTagBuilder ulTag = new YTagBuilder("ul");
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
        private static async Task<YHtmlString> RenderMenuAsync(IHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {
#else
        private static async Task<YHtmlString> RenderMenuAsync(HtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, string cssClass, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, int level) {
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
        private static async Task<string> RenderLIAsync(IHtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {
#else
        private static async Task<string> RenderLIAsync(HtmlHelper htmlHelper, List<ModuleAction> subMenu, Guid? subGuid, ModuleAction.RenderModeEnum renderMode, ModuleAction.RenderEngineEnum renderEngine, string liCss, int level) {
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

                                YHtmlString menuContents = new YHtmlString((await menuEntry.RenderAsync(renderMode, RenderEngine: renderEngine, HasSubmenu: true, BootstrapSmartMenuLevel: level)).ToString()); //$$eliminate YHtmlString
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

                            YHtmlString menuContents = new YHtmlString((await menuEntry.RenderAsync(renderMode, RenderEngine: renderEngine, BootstrapSmartMenuLevel: level)).ToString());
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

    public class MenuDisplayComponent : MenuComponentBase, IYetaWFComponent<MenuComponentBase.MenuData> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(MenuComponentBase.MenuData model) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuStyleEnum style = PropData.GetAdditionalAttributeValue("Style", MenuStyleEnum.Automatic);
            if (style == MenuStyleEnum.Automatic)
                style = Manager.SkinInfo.UsingBootstrap ? MenuStyleEnum.Bootstrap : MenuStyleEnum.Kendo;

            if (style == MenuStyleEnum.Bootstrap) {
                string menu = (await RenderMenuAsync(HtmlHelper, model.MenuList, DivId, null, YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.BootstrapSmartMenu)).ToString();
                if (!string.IsNullOrWhiteSpace(menu)) {
                    Manager.AddOnManager.AddAddOnGlobalAsync("github.com.vadikom", "smartmenus").Wait(); // multilevel navbar
                    hb.Append(menu);
                }
            } else {
                string menu = "";//$$$
                //$$$ HtmlString menu = Html.RenderAsync(Model.MenuList, DivId, Model.CssClass, YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.JqueryMenu).Result;
                if (!string.IsNullOrWhiteSpace(menu)) {
                    //Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.popup.min.js").Wait(); // is now a prereq of kendo.window (2017.2.621)
                    Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.menu.min.js").Wait();
                    hb.Append($@"
<div class='yt_yetawf_menus_menu t_display' role='navigation'>
    {menu}
</div>
<script>
    $('#{DivId}).kendoMenu({{
        direction: '{model.GetDirection()}',
        orientation: '{model.GetOrientation()}',
        popupCollision: 'fit flip',");

                    //if (Module.UseAnimation) {
                    //    @:  animation: {
                    //    @:      open: { effects: '@Module.GetOpenEffects()', duration: @Module.OpenDuration },
                    //    @:      close: { effects: '@Module.GetCloseEffects()', duration: @Module.CloseDuration }
                    //    @:  },
                    //},

                    hb.Append($@"
        hoverDelay: {model.HoverDelay}
    }});");

                    if (model.ShowPath) {
                        using (DocumentReady(hb, DivId)) {
                            hb.Append($@"
            YetaWF_Menu.init('@DivId');
        }}");
                        }
                        hb.Append($@"
    }}
</script>");
                    }
                }
            }

            return hb.ToYHtmlString();
        }
    }
}
