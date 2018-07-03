/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

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
    }

    public class MenuDisplayComponent : MenuComponentBase, IYetaWFComponent<MenuComponentBase.MenuData> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(MenuComponentBase.MenuData model) {

            HtmlBuilder hb = new HtmlBuilder();

            MenuStyleEnum style = PropData.GetAdditionalAttributeValue("Style", MenuStyleEnum.Automatic);
            if (style == MenuStyleEnum.Automatic)
                style = Manager.SkinInfo.UsingBootstrap ? MenuStyleEnum.Bootstrap : MenuStyleEnum.Kendo;

            if (style == MenuStyleEnum.Bootstrap) {
                string menu = (await CoreRendering.RenderMenuAsync(model.MenuList, DivId, null, RenderEngine: YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.BootstrapSmartMenu, HtmlHelper: HtmlHelper)).ToString();
                if (!string.IsNullOrWhiteSpace(menu)) {
                    await Manager.AddOnManager.AddAddOnNamedAsync(Package.Domain, Package.Product, "github.com.vadikom.smartmenus"); // multilevel navbar
                    hb.Append(menu);
                }
            } else {
                string menu = (await CoreRendering.RenderMenuAsync(model.MenuList, DivId, model.CssClass, RenderEngine: YetaWF.Core.Modules.ModuleAction.RenderEngineEnum.JqueryMenu, HtmlHelper: HtmlHelper)).ToString();
                if (!string.IsNullOrWhiteSpace(menu)) {
                    //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
                    await KendoUICore.AddFileAsync("kendo.menu.min.js");
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
