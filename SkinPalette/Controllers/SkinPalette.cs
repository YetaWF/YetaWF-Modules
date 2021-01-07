/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.SkinPalette.Controllers {

    public class SkinPaletteModuleController : ControllerImpl<YetaWF.Modules.SkinPalette.Modules.SkinPaletteModule> {

        public SkinPaletteModuleController() { }

        [Trim]
        public class Model {

            public const int MaxColor = 80;
            public const int MaxFont = 200;
            public const int MaxBorder = 80;
            public const int MaxRadius = 80;
            public const int MaxShadow = 80;
            public const int MaxLine = 200;
            public const int MaxFontSize = 80;
            public const int MaxPadding = 80;
            

            [Category("Page"), Caption("--body-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BodyBg { get; set; } = null!;

            [Category("Page"), Caption("--body-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string BodyClr { get; set; } = null!;

            [Category("Page"), Caption("--body-font"), Description("Font")]
            [UIHint("Text40"), StringLength(MaxFont), Required]
            public string BodyFont { get; set; } = null!;



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



            [Category("Overlay"), Caption("--overlay-bg"), Description("Background")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string OverlayBg { get; set; } = null!;

            [Category("Overlay"), Caption("--overlay-opacity"), Description("Opacity")]
            [UIHint("Decimal"), Range(0.0, 1.0), Required]
            public decimal OverlayOpacity { get; set; }



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
            public string DialogFont { get; set; } = null!;

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
            public string SidebarBg { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr"), Description("Color")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string SidebarClr { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-bg-hover"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string SidebarBgHover { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr-hover"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string SidebarClrHover { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-bg-active"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string SidebarBgActive { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-clr-active"), Description("Color, Hover")]
            [UIHint("Color"), StringLength(MaxColor), Required]
            public string SidebarClrActive { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-border"), Description("Border")]
            [UIHint("Text40"), StringLength(MaxBorder), Required]
            public string BarBorder { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-border-radius"), Description("Border Radius")]
            [UIHint("Text40"), StringLength(MaxRadius), Required]
            public string BarBorderRadius { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-h1-font-size"), Description("Font Size")]
            [UIHint("Text20"), StringLength(MaxFontSize), Required]
            public string SidebarH1FontSize { get; set; } = null!;

            [Category("Sidebar"), Caption("--bar-h1-padding-bottom"), Description("Padding")]
            [UIHint("Text20"), StringLength(MaxPadding), Required]
            public string SidebarH1PaddingBottom { get; set; } = null!;



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
            public string ButtonFocusShadow { get; set; } = null!;



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
            public string? CSSVariables { get; set; }

            [Category("Config"), Caption(""), Description("")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
            public ModuleAction Load { get; set; }

            public Model() {
                Load = new ModuleAction {
                    LinkText = this.__ResStr("apply", "Apply"),
                    Mode = ModuleAction.ActionModeEnum.Any,
                    Name = "Apply",
                    Style = ModuleAction.ActionStyleEnum.Nothing,
                    Tooltip = this.__ResStr("applyTT", "Apply the CSS Variables to the current skin"),
                };
            }
        }

        [AllowGet]
        public ActionResult SkinPalette() {

            if (CssLegacy.IsLegacyBrowser()) return new EmptyResult();
            if (Manager.IsInPopup) return new EmptyResult();

            Model model = new Model {};
            return View(model);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult SkinPalette_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model);
        }
    }
}
