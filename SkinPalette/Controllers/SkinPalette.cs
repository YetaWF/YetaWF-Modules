/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Site;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.SkinPalette.Controllers {

    public class SkinPaletteModuleController : ControllerImpl<YetaWF.Modules.SkinPalette.Modules.SkinPaletteModule> {

        public SkinPaletteModuleController() { }

        [Trim]
        public class Model {

            public const int MaxColor = 200;
            public const int MaxFont = 200;
            public const int MaxBorder = 200;
            public const int MaxRadius = 200;
            public const int MaxShadow = 200;
            public const int MaxLine = 200;
            public const int MaxFontSize = 200;
            public const int MaxMargin = 200;
            public const int MaxPadding = 200;
            public const int MaxWidth = 200;
            public const int MaxHeight = 200;

            public enum BasicThemeEnum {
                [EnumDescription("Light", "A \"light\" theme, which has a light background and a darker text color")]
                Light = 0,
                [EnumDescription("Dark", "A \"dark\" theme, which has a dark background and a lighter text color")]
                Dark = 1,
            }


            [Category("Page"), Caption("--body-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BodyBg { get; set; } = null!;

            [Category("Page"), Caption("--body-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BodyClr { get; set; } = null!;
           
            [Category("Page"), Caption("--body-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string BodyFont { get; set; } = null!;

            [Category("Page"), Caption("--body-dis-opacity"), Description("Opacity")]
            [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
            public decimal BodyDisabledOpacity { get; set; }



            [Category("Overlay"), Caption("--overlay-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OverlayBg { get; set; } = null!;

            [Category("Overlay"), Caption("--overlay-opacity"), Description("Opacity")]
            [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
            public decimal OverlayOpacity { get; set; }



            [Category("modStandard"), Caption("--mstd-title-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string ModStandardTitleFont { get; set; } = null!;



            [Category("modPanel"), Caption("--mpnl-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ModPanelBg { get; set; } = null!;

            [Category("modPanel"), Caption("--mpnl-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ModPanelClr { get; set; } = null!;

            [Category("modPanel"), Caption("--mpnl-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string ModPanelBorder { get; set; } = null!;

            [Category("modPanel"), Caption("--mpnl-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string ModPanelBorderRadius { get; set; } = null!;

            [Category("modPanel"), Caption("--mpnl-title-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string ModPanelTitleFont { get; set; } = null!;

            [Category("modPanel"), Caption("--mpnl-link-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string ModPanelLinkFont { get; set; } = null!;
                        


            [Category("Main Menu"), Caption("--mm-0-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0Bg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0Clr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string MainMenu0Font { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string MainMenu0Border { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string MainMenu0BorderRadius { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu0Padding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0ABg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0AClr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-bg-hover"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0ABgHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0AClrHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-bg-path"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0ABgPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-clr-path"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu0AClrPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu0APadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-a-small-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu0ASmallPadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-ddwidth"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string MainMenu0DDWidth { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-0-ddheight"), Description("Height")]
            [UIHint("Text20"), StringLength(MaxHeight), Required]
            public string MainMenu0DDHeight { get; set; } = null!;



            [Category("Main Menu"), Caption("--mm-1-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1Bg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1Clr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string MainMenu1Font { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string MainMenu1Border { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string MainMenu1BorderRadius { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-small-margin"), Description("Margin")]
            [UIHint("Text20"), StringLength(MaxMargin), Required]
            public string MainMenu1SmallMargin { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu1Padding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-small-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu1SmallPadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1ABg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1AClr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-bg-hover"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1ABgHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1AClrHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-bg-path"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1ABgPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-clr-path"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1AClrPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu1APadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-a-small-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu1ASmallPadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-ddwidth"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string MainMenu1DDWidth { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-ddheight"), Description("Height")]
            [UIHint("Text20"), StringLength(MaxHeight), Required]
            public string MainMenu1DDHeight { get; set; } = null!;


            [Category("Main Menu"), Caption("--mm-1-mm-width"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string MainMenu1MMWidth { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-mm-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1MMBg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-mm-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu1MMClr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-1-mm-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string MainMenu1MMFont { get; set; } = null!;


            [Category("Main Menu"), Caption("--mm-2-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2Bg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2Clr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string MainMenu2Font { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string MainMenu2Border { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string MainMenu2BorderRadius { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-small-margin"), Description("Margin")]
            [UIHint("Text20"), StringLength(MaxMargin), Required]
            public string MainMenu2SmallMargin { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu2Padding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-small-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu2SmallPadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2ABg { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2AClr { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-bg-hover"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2ABgHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2AClrHover { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-bg-path"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2ABgPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-clr-path"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string MainMenu2AClrPath { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu2APadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-a-small-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string MainMenu2ASmallPadding { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-ddwidth"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string MainMenu2DDWidth { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-2-ddheight"), Description("Height")]
            [UIHint("Text20"), StringLength(MaxHeight), Required]
            public string MainMenu2DDHeight { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-svg-small-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string MainMenuSVGSmallBorder { get; set; } = null!;

            [Category("Main Menu"), Caption("--mm-svg-small-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string MainMenuSVGSmallBorderRadius { get; set; } = null!;



            [Category("Popup Menu"), Caption("--pm-1-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1Bg { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1Clr { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string PopupMenu1Font { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PopupMenu1Border { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PopupMenu1BorderRadius { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string PopupMenu1Padding { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1ABg { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1AClr { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-bg-hover"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1ABgHover { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1AClrHover { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-bg-path"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1ABgPath { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-clr-path"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu1AClrPath { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-a-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string PopupMenu1APadding { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-ddwidth"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string PopupMenu1DDWidth { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-1-ddheight"), Description("Height")]
            [UIHint("Text20"), StringLength(MaxHeight), Required]
            public string PopupMenu1DDHeight { get; set; } = null!;


            [Category("Popup Menu"), Caption("--pm-2-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2Bg { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2Clr { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string PopupMenu2Font { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PopupMenu2Border { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PopupMenu2BorderRadius { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string PopupMenu2Padding { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2ABg { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2AClr { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-bg-hover"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2ABgHover { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2AClrHover { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-bg-path"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2ABgPath { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-clr-path"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PopupMenu2AClrPath { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-a-padding"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string PopupMenu2APadding { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-ddwidth"), Description("Width")]
            [UIHint("Text20"), StringLength(MaxWidth), Required]
            public string PopupMenu2DDWidth { get; set; } = null!;

            [Category("Popup Menu"), Caption("--pm-2-ddheight"), Description("Height")]
            [UIHint("Text20"), StringLength(MaxHeight), Required]
            public string PopupMenu2DDHeight { get; set; } = null!;



            [Category("Tooltip"), Caption("--tt-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TTBg { get; set; } = null!;

            [Category("Tooltip"), Caption("--tt-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TTClr { get; set; } = null!;

            [Category("Tooltip"), Caption("--tt-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string TTFont { get; set; } = null!;

            [Category("Tooltip"), Caption("--tt-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TTBorder { get; set; } = null!;

            [Category("Tooltip"), Caption("--tt-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string TTBorderRadius { get; set; } = null!;

            [Category("Tooltip"), Caption("--tt-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string TTShadow { get; set; } = null!;



            [Category("Dialog"), Caption("--dialog-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DialogBg { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DialogClr { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-title-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DialogTitleBg { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-title-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DialogTitleClr { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-title-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string DialogTitleFont { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-line"), Description("Line")]
            [UIHint("Text40"), StringLength(MaxLine), Required]
            public string DialogLine { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string DialogBorder { get; set; } = null!;

            [Category("Dialog"), Caption("--dialog-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string DialogBorderRadius { get; set; } = null!;



            [Category("Toast"), Caption("--tst-title-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string TstTitleFont { get; set; } = null!;

            [Category("Toast"), Caption("--tst-msg-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string TstMsgFont { get; set; } = null!;

            [Category("Toast"), Caption("--tst-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TstBorder { get; set; } = null!;

            [Category("Toast"), Caption("--tst-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string TstBorderRadius { get; set; } = null!;

            [Category("Toast"), Caption("--tst-info-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstInfoBg { get; set; } = null!;

            [Category("Toast"), Caption("--tst-info-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstInfoClr { get; set; } = null!;

            [Category("Toast"), Caption("--tst-info-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string TstInfoShadow { get; set; } = null!;

            [Category("Toast"), Caption("--tst-info-line"), Description("Line")]
            [UIHint("Text40"), StringLength(MaxLine), Required]
            public string TstInfoLine { get; set; } = null!;

            [Category("Toast"), Caption("--tst-warn-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstWarnBg { get; set; } = null!;

            [Category("Toast"), Caption("--tst-warn-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstWarnClr { get; set; } = null!;

            [Category("Toast"), Caption("--tst-warn-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string TstWarnShadow { get; set; } = null!;

            [Category("Toast"), Caption("--tst-warn-line"), Description("Line")]
            [UIHint("Text40"), StringLength(MaxLine), Required]
            public string TstWarnLine { get; set; } = null!;

            [Category("Toast"), Caption("--tst-error-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstErrorBg { get; set; } = null!;

            [Category("Toast"), Caption("--tst-error-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TstErrorClr { get; set; } = null!;

            [Category("Toast"), Caption("--tst-error-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string TstErrorShadow { get; set; } = null!;

            [Category("Toast"), Caption("--tst-error-line"), Description("Line")]
            [UIHint("Text40"), StringLength(MaxLine), Required]
            public string TstErrorLine { get; set; } = null!;



            [Category("Sidebar"), Caption("--bar-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarBg { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarClr { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-bg-hover"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarBgHover { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr-hover"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarClrHover { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-bg-active"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarBgActive { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr-active"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BarClrActive { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string BarBorder { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string BarBorderRadius { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-h1-font-size"), Description("Font Size")]
            [UIHint("Text20"), StringLength(MaxFontSize), Required]
            public string BarH1FontSize { get; set; } = null!;



            [Category("PropertyList"), Caption("--prop-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PropBorder { get; set; } = null!;

            [Category("PropertyList"), Caption("--prop-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PropBorderRadius { get; set; } = null!;

            [Category("PropertyList"), Caption("--prop-cat-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PropCatBg { get; set; } = null!;

            [Category("PropertyList"), Caption("--prop-cat-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PropCatClr { get; set; } = null!;

            [Category("PropertyList"), Caption("--prop-cat-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PropCatBorder { get; set; } = null!;

            [Category("PropertyList"), Caption("--prop-cat-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PropCatBorderRadius { get; set; } = null!;



            [Category("Input"), Caption("--inp-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string InputBg { get; set; } = null!;

            [Category("Input"), Caption("--inp-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string InputClr { get; set; } = null!;

            [Category("Input"), Caption("--inp-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string InputBorder { get; set; } = null!;

            [Category("Input"), Caption("--inp-border-hover"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string InputBorderHover { get; set; } = null!;

            [Category("Input"), Caption("--inp-border-focus"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string InputBorderFocus { get; set; } = null!;

            [Category("Input"), Caption("--inp-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string InputBorderRadius { get; set; } = null!;

            [Category("Input"), Caption("--inp-error-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxRadius), Required]
            public string InputError { get; set; } = null!;

            [Category("Input"), Caption("--inp-ph-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string InputPlaceholderClr { get; set; } = null!;

            [Category("Input"), Caption("--inp-ph-opacity"), Description("Opacity")]
            [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
            public decimal InputPlaceholderOpacity { get; set; }


            [Category("Dropdownlist"), Caption("--dd-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDBg { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDClr { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDBgHover { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDClrHover { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-bg-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDBgActive { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-clr-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDClrActive { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDBgFocus { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDClrFocus { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string DDBorder { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-border-hover"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string DDBorderHover { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-border-focus"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string DDBorderFocus { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string DDBorderRadius { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListBg { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListClr { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListBgHover { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListClrHover { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListBgFocus { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string DDListClrFocus { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string DDListBorder { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string DDListBorderRadius { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string DDListShadow { get; set; } = null!;

            [Category("Dropdownlist"), Caption("--dd-p-shadow-focus"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string DDListShadowFocus { get; set; } = null!;



            [Category("Anchor"), Caption("--a-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string AnchorClr { get; set; } = null!;

            [Category("Anchor"), Caption("--a-dec"), Description("Decoration")]
            [UIHint("Text20"), StringLength(MaxFont), Required]
            public string AnchorDec { get; set; } = null!;

            [Category("Anchor"), Caption("--a-clr-hover"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string AnchorClrHover { get; set; } = null!;

            [Category("Anchor"), Caption("--a-dec-hover"), Description("Decoration, Hover")]
            [UIHint("Text20"), StringLength(MaxFont), Required]
            public string AnchorDecHover { get; set; } = null!;

            [Category("Anchor"), Caption("--a-clr-focus"), Description("Color, Focus")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string AnchorClrFocus { get; set; } = null!;

            [Category("Anchor"), Caption("--a-dec-focus"), Description("Decoration, Focus")]
            [UIHint("Text20"), StringLength(MaxFont), Required]
            public string AnchorDecFocus { get; set; } = null!;



            [Category("Button"), Caption("--button-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonBg { get; set; } = null!;

            [Category("Button"), Caption("--button-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonClr { get; set; } = null!;

            [Category("Button"), Caption("--button-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonBgHover { get; set; } = null!;

            [Category("Button"), Caption("--button-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonClrHover { get; set; } = null!;

            [Category("Button"), Caption("--button-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonBgFocus { get; set; } = null!;

            [Category("Button"), Caption("--button-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonClrFocus { get; set; } = null!;

            [Category("Button"), Caption("--button-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string ButtonBorder { get; set; } = null!;

            [Category("Button"), Caption("--button-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string ButtonBorderRadius { get; set; } = null!;

            [Category("Button"), Caption("--button-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string ButtonShadow { get; set; } = null!;

            [Category("Button"), Caption("--button-focus-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string ButtonShadowFocus { get; set; } = null!;



            [Category("ButtonLite"), Caption("--buttonlite-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteBg { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteClr { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteBgHover { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteClrHover { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteBgFocus { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ButtonLiteClrFocus { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string ButtonLiteBorder { get; set; } = null!;

            [Category("ButtonLite"), Caption("--buttonlite-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string ButtonLiteBorderRadius { get; set; } = null!;



            [Category("Progressbar"), Caption("--pbar-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PbarBg { get; set; } = null!;

            [Category("Progressbar"), Caption("--pbar-value-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PbarValueBg { get; set; } = null!;

            [Category("Progressbar"), Caption("--pbar-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PbarBorder { get; set; } = null!;

            [Category("Progressbar"), Caption("--pbar-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PbarBorderRadius { get; set; } = null!;


            [Category("Table"), Caption("--tbl-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBg { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClr { get; set; } = null!;

            [Category("Table"), Caption("--tbl-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBgHover { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClrHover { get; set; } = null!;

            [Category("Table"), Caption("--tbl-bg-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBgActive { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClrActive { get; set; } = null!;

            [Category("Table"), Caption("--tbl-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBgFocus { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClrFocus { get; set; } = null!;

            [Category("Table"), Caption("--tbl-bg-highlight"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBgHighlight { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr-highlight"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClrHighlight { get; set; } = null!;

            [Category("Table"), Caption("--tbl-bg-lowlight"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableBgLowlight { get; set; } = null!;

            [Category("Table"), Caption("--tbl-clr-lowlight"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableClrLowlight { get; set; } = null!;

            [Category("Table"), Caption("--tbl-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string TableFont { get; set; } = null!;

            [Category("Table"), Caption("--tbl-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TableBorder { get; set; } = null!;

            [Category("Table"), Caption("--tbl-border-lite"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TableBorderLite { get; set; } = null!;

            [Category("Table"), Caption("--tbl-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string TableBorderRadius { get; set; } = null!;

            [Category("Table"), Caption("--tbl-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string TableShadow { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderBg { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderClr { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderBgHover { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderClrHover { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-bg-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderBgActive { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-clr-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TableHeaderClrActive { get; set; } = null!;

            [Category("Table"), Caption("--tbl-header-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string TableHeaderFont { get; set; } = null!;


            [Category("Tabs"), Caption("--tabs-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsBg { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsClr { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-strip-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsStripBg { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-strip-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TabsStripBorder { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-strip-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string TabsStripBorderRadius { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabBg { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabClr { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-bg-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabBgHover { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-clr-hover"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabClrHover { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-bg-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabBgActive { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-clr-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabClrActive { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-bg-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabBgFocus { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-clr-focus"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string TabsTabClrFocus { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string TabsTabBorder { get; set; } = null!;

            [Category("Tabs"), Caption("--tabs-tab-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string TabsTabBorderRadius { get; set; } = null!;



            [Category("Step"), Caption("--step-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string StepBg { get; set; } = null!;

            [Category("Step"), Caption("--step-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string StepClr { get; set; } = null!;

            [Category("Step"), Caption("--step-bg-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string StepBgActive { get; set; } = null!;

            [Category("Step"), Caption("--step-clr-active"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string StepClrActive { get; set; } = null!;



            [Category("Panel"), Caption("--panel-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string PanelBg { get; set; } = null!;

            //[Category("Panel"), Caption("--panel-clr"), Description("Color")]
            //[UIHint("Color"), StringLength(MaxColor), Required]
            //public string PanelClr { get; set; } = null!;

            [Category("Panel"), Caption("--panel-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string PanelBorder { get; set; } = null!;

            [Category("Panel"), Caption("--panel-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string PanelBorderRadius { get; set; } = null!;

            [Category("Panel"), Caption("--panel-shadow"), Description("Shadow")]
            [UIHint("Text40"), StringLength(MaxShadow), Required]
            public string PanelShadow { get; set; } = null!;



            [Category("Indicators"), Caption("--mod-current-bg"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string ModCurrentBg { get; set; } = null!;

            [Category("Indicators"), Caption("--own-page-noUserAnon"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnPageNoUserAnon { get; set; } = null!;

            [Category("Indicators"), Caption("--own-page-noAnon"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnPageNoAnon { get; set; } = null!;

            [Category("Indicators"), Caption("--own-page-noUser"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnPageNoUser { get; set; } = null!;

            [Category("Indicators"), Caption("--own-mod-noUserAnon"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnModNoUserAnon { get; set; } = null!;

            [Category("Indicators"), Caption("--own-mod-noAnon"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnModNoAnon { get; set; } = null!;

            [Category("Indicators"), Caption("--own-mod-noUser"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OwnModNoUser { get; set; } = null!;



            [Category("Config"), Caption("CSS Variables"), Description("Paste your settings to edit further or copy to skin SCSS file to use in a skin")]
            [UIHint("TextAreaSourceOnly"), AdditionalMetadata("Spellcheck", false), AdditionalMetadata("EmHeight", 30), AdditionalMetadata("Copy", true), StringLength(0)]
            public string CSSVariables { get; set; } = null!;

            [Category("Config"), Caption("Theme"), Description("The name of the theme - Initially displays the current theme name, but can be changed beore saving the CSS variables to create a new theme")]
            [UIHint("Text40"), StringLength(SiteDefinition.MaxTheme), Required]//TODO: validation
            public string Theme { get; set; } = null!;

            [Category("Config"), Caption(""), Description("")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
            public ModuleAction Load { get; set; }




            [Category("Auto Gen"), Caption("Theme Style"), Description("")]
            [UIHint("Enum")]
            public BasicThemeEnum GenTheme { get; set; }

            [Category("Auto Gen"), Caption("Background Color"), Description("Background")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenBg { get; set; } = null!;

            [Category("Auto Gen"), Caption("Foreground Color"), Description("Color")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenClr { get; set; } = null!;

            [Category("Auto Gen"), Caption("Background Active Color"), Description("Background")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenBgActive { get; set; } = null!;

            [Category("Auto Gen"), Caption("Foreground Active Color"), Description("Color")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenClrActive { get; set; } = null!;

            [Category("Auto Gen"), Caption("Background Shaded Color"), Description("Background")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenBgShaded { get; set; } = null!;

            [Category("Auto Gen"), Caption("Foreground Shaded Color"), Description("Color")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenClrShaded { get; set; } = null!;

            [Category("Auto Gen"), Caption("Background Shaded Active Color"), Description("Background")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenBgShadedActive { get; set; } = null!;

            [Category("Auto Gen"), Caption("Foreground Shaded Active Color"), Description("Color")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenClrShadedActive { get; set; } = null!;

            [Category("Auto Gen"), Caption("Font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string GenFont { get; set; } = null!;

            [Category("Auto Gen"), Caption("SmallFont"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string GenSmallFont { get; set; } = null!;

            [Category("Auto Gen"), Caption("Border Width"), Description("Border Width")]
            [UIHint("IntValue2"), Range(0, 20), Required]
            public int GenBorderWidth { get; set; }

            [Category("Auto Gen"), Caption("Border Color"), Description("Border")]
            [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
            public string GenBorderClr { get; set; } = null!;

            [Category("Auto Gen"), Caption("Radius Pixels"), Description("Border Radius")]
            [UIHint("IntValue2"), Range(0, 20), Required]
            public int GenBorderRadius { get; set; }




            [Category("Auto Gen"), Caption(""), Description("")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
            public ModuleAction AutoGenerate { get; set; }


            public Model() {
                Theme = Manager.CurrentSite.Theme;
                Load = new ModuleAction {
                    LinkText = this.__ResStr("save", "Save"),
                    Mode = ModuleAction.ActionModeEnum.Any,
                    Name = "Save",
                    Style = ModuleAction.ActionStyleEnum.Nothing,
                    Tooltip = this.__ResStr("saveTT", "Save the CSS Variables for the current theme"),
                };
                AutoGenerate = new ModuleAction {
                    LinkText = this.__ResStr("generate", "Generate"),
                    Mode = ModuleAction.ActionModeEnum.Any,
                    Name = "AutoGen",
                    Style = ModuleAction.ActionStyleEnum.Nothing,
                    Tooltip = this.__ResStr("generateTT", "Auto-popuplate most CSS variables using basic settings"),
                };
            }
        }

        [AllowGet]
        public ActionResult SkinPalette() {

            if (YetaWFManager.Deployed) return new EmptyResult();
            if (CssLegacy.IsLegacyBrowser()) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            Model model = new Model {};
            return View(model);
        }

        [AllowPost]
        [ExcludeDemoMode]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SkinPalette_Partial(Model model) {

            if (!ModelState.IsValid)
                return PartialView(model);

            if (string.Compare(model.Theme, "Default", true) == 0)
                throw new Error(this.__ResStr("notDefault", "The Default theme cannot be changed - It has to be saved using a different theme name"));

            string file = Path.Combine(Manager.SkinInfo.Folder, "Themes", $"{model.Theme}.css");
            bool newFile = !await FileSystem.FileSystemProvider.FileExistsAsync(file);
            await FileSystem.FileSystemProvider.WriteAllTextAsync(file, model.CSSVariables);

            if (newFile) {
                // when a new theme is saved, activate it for the current site
                Manager.CurrentSite.Theme = model.Theme;
                await Manager.CurrentSite.SaveAsync();
                return FormProcessed(model, this.__ResStr("okUpdated", "Theme {0} successfully saved - The current site has been updated to use the new theme", model.Theme), ForceRedirect: true);
            } else
                return FormProcessed(model, this.__ResStr("ok", "Theme {0} successfully saved", model.Theme));
        }
    }
}
