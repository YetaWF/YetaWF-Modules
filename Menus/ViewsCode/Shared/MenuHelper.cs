/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;

namespace YetaWF.Modules.Menus.Views.Shared {

    public class Menu<TModel> : RazorTemplate<TModel> { }

    public static class MenuHelper {

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
                    case MenuHelper.DirectionEnum.Top: return "top";
                    case MenuHelper.DirectionEnum.Bottom: return "bottom";
                    case MenuHelper.DirectionEnum.Left: return "left";
                    case MenuHelper.DirectionEnum.Right: return "right";
                }
            }
            public string GetOrientation() {
                switch (Orientation) {
                    default:
                    case MenuHelper.OrientationEnum.Horizontal: return "horizontal";
                    case MenuHelper.OrientationEnum.Vertical: return "vertical";
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
}