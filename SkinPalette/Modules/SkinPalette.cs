/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SkinPalette#License */

using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SkinPalette.Modules;

public class SkinPaletteModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinPaletteModule>, IInstallableModel { }

[ModuleGuid("{915a366d-facb-4d02-b8f8-bb1acef73c4c}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
//[ModuleCategory("...")]
public class SkinPaletteModule : ModuleDefinition {

    public SkinPaletteModule() {
        Title = this.__ResStr("modTitle", "Skin Palette");
        Name = this.__ResStr("modName", "Skin Palette (Skin)");
        Description = this.__ResStr("modSummary", "Edits the current skin");
        UsePartialFormCss = false;
        ShowTitleActions = false;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;
        Invokable = true;
        CssClass = "yCondense";
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SkinPaletteModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        public const int MaxColor = 200;
        public const int MaxFont = 200;
        public const int MaxBorder = 200;
        public const int MaxRadius = 200;
        public const int MaxShadow = 200;
        public const int MaxLine = 200;
        public const int MaxFontSize = 200;
        public const int MaxPadding = 200;
        public const int MaxWidth = 200;
        public const int MaxHeight = 200;

        public enum BasicThemeEnum {
            [EnumDescription("Light", "A \"light\" theme, which has a light background and a darker text color")]
            Light = 0,
            [EnumDescription("Dark", "A \"dark\" theme, which has a dark background and a lighter text color")]
            Dark = 1,
        }


        [Category("Page"), CaptionStatic("--body-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BodyBg { get; set; } = null!;

        [Category("Page"), CaptionStatic("--body-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BodyClr { get; set; } = null!;

        [Category("Page"), CaptionStatic("--body-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string BodyFont { get; set; } = null!;

        [Category("Page"), CaptionStatic("--body-dis-opacity"), Description("Opacity")]
        [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
        public decimal BodyDisabledOpacity { get; set; }



        [Category("Overlay"), CaptionStatic("--overlay-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OverlayBg { get; set; } = null!;

        [Category("Overlay"), CaptionStatic("--overlay-opacity"), Description("Opacity")]
        [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
        public decimal OverlayOpacity { get; set; }



        [Category("modStandard"), CaptionStatic("--mstd-title-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string ModStandardTitleFont { get; set; } = null!;



        [Category("modPanel"), CaptionStatic("--mpnl-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ModPanelBg { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ModPanelClr { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string ModPanelPadding { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string ModPanelBorder { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string ModPanelBorderRadius { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string ModPanelShadow { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-title-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string ModPanelTitleFont { get; set; } = null!;

        [Category("modPanel"), CaptionStatic("--mpnl-link-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string ModPanelLinkFont { get; set; } = null!;



        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0Bg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0Clr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuHorz0Font { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuHorz0Border { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuHorz0BorderRadius { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz0Padding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0ABg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0AClr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0ABgHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0AClrHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0ABgPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz0AClrPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz0APadding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuHorz0DDWidth { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-0-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuHorz0DDHeight { get; set; } = null!;



        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1Bg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1Clr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuHorz1Font { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuHorz1Border { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuHorz1BorderRadius { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz1Padding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1ABg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1AClr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1ABgHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1AClrHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1ABgPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1AClrPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz1APadding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuHorz1DDWidth { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuHorz1DDHeight { get; set; } = null!;


        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-mm-width"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuHorz1MMWidth { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-mm-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1MMBg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-mm-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz1MMClr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-1-mm-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuHorz1MMFont { get; set; } = null!;


        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2Bg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2Clr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuHorz2Font { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuHorz2Border { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuHorz2BorderRadius { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz2Padding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2ABg { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2AClr { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2ABgHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2AClrHover { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2ABgPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuHorz2AClrPath { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuHorz2APadding { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuHorz2DDWidth { get; set; } = null!;

        [Category("Main Menu (horz.)"), CaptionStatic("--mm-2-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuHorz2DDHeight { get; set; } = null!;



        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0Bg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0Clr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuVert0Font { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuVert0Border { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuVert0BorderRadius { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert0Padding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0ABg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0AClr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0ABgHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0AClrHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0ABgPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert0AClrPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert0APadding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuVert0DDWidth { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-0-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuVert0DDHeight { get; set; } = null!;



        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1Bg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1Clr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuVert1Font { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuVert1Border { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuVert1BorderRadius { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert1Padding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1ABg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1AClr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1ABgHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1AClrHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1ABgPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1AClrPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert1APadding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuVert1DDWidth { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuVert1DDHeight { get; set; } = null!;


        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-mm-width"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuVert1MMWidth { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-mm-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1MMBg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-mm-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert1MMClr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-1-mm-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuVert1MMFont { get; set; } = null!;


        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2Bg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2Clr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuVert2Font { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuVert2Border { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuVert2BorderRadius { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert2Padding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2ABg { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2AClr { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2ABgHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2AClrHover { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2ABgPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuVert2AClrPath { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuVert2APadding { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuVert2DDWidth { get; set; } = null!;

        [Category("Main Menu (vert.)"), CaptionStatic("--mmv-2-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuVert2DDHeight { get; set; } = null!;



        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0Bg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0Clr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuSm0Font { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuSm0Border { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuSm0BorderRadius { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm0Padding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0ABg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0AClr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0ABgHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0AClrHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0ABgPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm0AClrPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm0APadding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuSm0DDWidth { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-0-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuSm0DDHeight { get; set; } = null!;



        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1Bg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1Clr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuSm1Font { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuSm1Border { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuSm1BorderRadius { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm1Padding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1ABg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1AClr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1ABgHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1AClrHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1ABgPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1AClrPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm1APadding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuSm1DDWidth { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuSm1DDHeight { get; set; } = null!;


        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-mm-width"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuSm1MMWidth { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-mm-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1MMBg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-mm-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm1MMClr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-1-mm-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuSm1MMFont { get; set; } = null!;


        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2Bg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2Clr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string MainMenuSm2Font { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MainMenuSm2Border { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MainMenuSm2BorderRadius { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm2Padding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2ABg { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2AClr { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2ABgHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2AClrHover { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2ABgPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MainMenuSm2AClrPath { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string MainMenuSm2APadding { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string MainMenuSm2DDWidth { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mmsm-2-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string MainMenuSm2DDHeight { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mm-svg-small-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string MenuSVGSmallBorder { get; set; } = null!;

        [Category("Main Menu (small)"), CaptionStatic("--mm-svg-small-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string MenuSVGSmallBorderRadius { get; set; } = null!;



        [Category("Mini Scroll"), CaptionStatic("--ms-1-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string MiniScrollClr { get; set; } = null!;



        [Category("Popup Menu"), CaptionStatic("--pm-1-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1Bg { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1Clr { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string PopupMenu1Font { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PopupMenu1Border { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PopupMenu1BorderRadius { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string PopupMenu1Padding { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1ABg { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1AClr { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1ABgHover { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1AClrHover { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1ABgPath { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu1AClrPath { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string PopupMenu1APadding { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string PopupMenu1DDWidth { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-1-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string PopupMenu1DDHeight { get; set; } = null!;


        [Category("Popup Menu"), CaptionStatic("--pm-2-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2Bg { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2Clr { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string PopupMenu2Font { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PopupMenu2Border { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PopupMenu2BorderRadius { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string PopupMenu2Padding { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2ABg { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2AClr { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-bg-hover"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2ABgHover { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2AClrHover { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-bg-path"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2ABgPath { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-clr-path"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PopupMenu2AClrPath { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-a-padding"), Description("Padding")]
        [UIHint("Text20"), StringLength(MaxPadding), Required]
        public string PopupMenu2APadding { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-ddwidth"), Description("Width")]
        [UIHint("Text20"), StringLength(MaxWidth), Required]
        public string PopupMenu2DDWidth { get; set; } = null!;

        [Category("Popup Menu"), CaptionStatic("--pm-2-ddheight"), Description("Height")]
        [UIHint("Text20"), StringLength(MaxHeight), Required]
        public string PopupMenu2DDHeight { get; set; } = null!;



        [Category("Tooltip"), CaptionStatic("--tt-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TTBg { get; set; } = null!;

        [Category("Tooltip"), CaptionStatic("--tt-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TTClr { get; set; } = null!;

        [Category("Tooltip"), CaptionStatic("--tt-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string TTFont { get; set; } = null!;

        [Category("Tooltip"), CaptionStatic("--tt-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TTBorder { get; set; } = null!;

        [Category("Tooltip"), CaptionStatic("--tt-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string TTBorderRadius { get; set; } = null!;

        [Category("Tooltip"), CaptionStatic("--tt-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string TTShadow { get; set; } = null!;



        [Category("Dialog"), CaptionStatic("--dialog-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DialogBg { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DialogClr { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-title-bg"), Description("Background")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DialogTitleBg { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-title-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DialogTitleClr { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-title-border"), Description("Title Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DialogTitleBorder { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-title-border-radius"), Description("Title Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string DialogTitleBorderRadius { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-title-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string DialogTitleFont { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-line"), Description("Line")]
        [UIHint("Text40"), StringLength(MaxLine), Required]
        public string DialogLine { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-line-padding"), Description("Line Padding")]
        [UIHint("Text40"), StringLength(MaxPadding), Required]
        public string DialogLinePadding { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DialogBorder { get; set; } = null!;

        [Category("Dialog"), CaptionStatic("--dialog-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string DialogBorderRadius { get; set; } = null!;



        [Category("Toast"), CaptionStatic("--tst-title-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string TstTitleFont { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-msg-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string TstMsgFont { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TstBorder { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string TstBorderRadius { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-info-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstInfoBg { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-info-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstInfoClr { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-info-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string TstInfoShadow { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-info-line"), Description("Line")]
        [UIHint("Text40"), StringLength(MaxLine), Required]
        public string TstInfoLine { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-warn-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstWarnBg { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-warn-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstWarnClr { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-warn-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string TstWarnShadow { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-warn-line"), Description("Line")]
        [UIHint("Text40"), StringLength(MaxLine), Required]
        public string TstWarnLine { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-error-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstErrorBg { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-error-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TstErrorClr { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-error-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string TstErrorShadow { get; set; } = null!;

        [Category("Toast"), CaptionStatic("--tst-error-line"), Description("Line")]
        [UIHint("Text40"), StringLength(MaxLine), Required]
        public string TstErrorLine { get; set; } = null!;



        [Category("Sidebar"), CaptionStatic("--bar-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarBg { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarClr { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-bg-hover"), Description("Color, Hover")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarBgHover { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-clr-hover"), Description("Color, Hover")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarClrHover { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-bg-active"), Description("Color, Hover")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarBgActive { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-clr-active"), Description("Color, Hover")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string BarClrActive { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string BarBorder { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string BarBorderRadius { get; set; } = null!;

        [Category("Sidebar"), CaptionStatic("--bar-h1-font-size"), Description("Font Size")]
        [UIHint("Text20"), StringLength(MaxFontSize), Required]
        public string BarH1FontSize { get; set; } = null!;



        [Category("PropertyList"), CaptionStatic("--prop-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PropBorder { get; set; } = null!;

        [Category("PropertyList"), CaptionStatic("--prop-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PropBorderRadius { get; set; } = null!;

        [Category("PropertyList"), CaptionStatic("--prop-cat-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PropCatBg { get; set; } = null!;

        [Category("PropertyList"), CaptionStatic("--prop-cat-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PropCatClr { get; set; } = null!;

        [Category("PropertyList"), CaptionStatic("--prop-cat-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PropCatBorder { get; set; } = null!;

        [Category("PropertyList"), CaptionStatic("--prop-cat-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PropCatBorderRadius { get; set; } = null!;



        [Category("Input"), CaptionStatic("--inp-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string InputBg { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string InputClr { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string InputBorder { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-border-hover"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string InputBorderHover { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-border-focus"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string InputBorderFocus { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string InputBorderRadius { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-error-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxRadius), Required]
        public string InputError { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-ph-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string InputPlaceholderClr { get; set; } = null!;

        [Category("Input"), CaptionStatic("--inp-ph-opacity"), Description("Opacity")]
        [UIHint("Decimal"), AdditionalMetadata("Step", 0.1), Range(0.0, 1.0), Required]
        public decimal InputPlaceholderOpacity { get; set; }


        [Category("Dropdown"), CaptionStatic("--dd-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDBg { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDClr { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDBgHover { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDClrHover { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-bg-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDBgActive { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-clr-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDClrActive { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDBgFocus { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDClrFocus { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DDBorder { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-border-hover"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DDBorderHover { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-border-focus"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DDBorderFocus { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string DDBorderRadius { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListBg { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListClr { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListBgHover { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListClrHover { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListBgFocus { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string DDListClrFocus { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string DDListBorder { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string DDListBorderRadius { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string DDListShadow { get; set; } = null!;

        [Category("Dropdown"), CaptionStatic("--dd-p-shadow-focus"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string DDListShadowFocus { get; set; } = null!;



        [Category("Anchor"), CaptionStatic("--a-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string AnchorClr { get; set; } = null!;

        [Category("Anchor"), CaptionStatic("--a-dec"), Description("Decoration")]
        [UIHint("Text20"), StringLength(MaxFont), Required]
        public string AnchorDec { get; set; } = null!;

        [Category("Anchor"), CaptionStatic("--a-clr-hover"), Description("Color, Hover")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string AnchorClrHover { get; set; } = null!;

        [Category("Anchor"), CaptionStatic("--a-dec-hover"), Description("Decoration, Hover")]
        [UIHint("Text20"), StringLength(MaxFont), Required]
        public string AnchorDecHover { get; set; } = null!;

        [Category("Anchor"), CaptionStatic("--a-clr-focus"), Description("Color, Focus")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string AnchorClrFocus { get; set; } = null!;

        [Category("Anchor"), CaptionStatic("--a-dec-focus"), Description("Decoration, Focus")]
        [UIHint("Text20"), StringLength(MaxFont), Required]
        public string AnchorDecFocus { get; set; } = null!;



        [Category("Button"), CaptionStatic("--button-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonBg { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonClr { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonBgHover { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonClrHover { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonBgFocus { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonClrFocus { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string ButtonBorder { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string ButtonBorderRadius { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string ButtonShadow { get; set; } = null!;

        [Category("Button"), CaptionStatic("--button-focus-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string ButtonShadowFocus { get; set; } = null!;



        [Category("ButtonLite"), CaptionStatic("--buttonlite-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteBg { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteClr { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteBgHover { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteClrHover { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteBgFocus { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ButtonLiteClrFocus { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string ButtonLiteBorder { get; set; } = null!;

        [Category("ButtonLite"), CaptionStatic("--buttonlite-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string ButtonLiteBorderRadius { get; set; } = null!;



        [Category("Progressbar"), CaptionStatic("--pbar-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PbarBg { get; set; } = null!;

        [Category("Progressbar"), CaptionStatic("--pbar-value-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PbarValueBg { get; set; } = null!;

        [Category("Progressbar"), CaptionStatic("--pbar-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PbarBorder { get; set; } = null!;

        [Category("Progressbar"), CaptionStatic("--pbar-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PbarBorderRadius { get; set; } = null!;


        [Category("Table"), CaptionStatic("--tbl-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBg { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClr { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBgHover { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClrHover { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-bg-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBgActive { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClrActive { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBgFocus { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClrFocus { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-bg-highlight"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBgHighlight { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr-highlight"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClrHighlight { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-bg-lowlight"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableBgLowlight { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-clr-lowlight"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableClrLowlight { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string TableFont { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TableBorder { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-border-lite"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TableBorderLite { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string TableBorderRadius { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string TableShadow { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderBg { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderClr { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderBgHover { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderClrHover { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-bg-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderBgActive { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-clr-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TableHeaderClrActive { get; set; } = null!;

        [Category("Table"), CaptionStatic("--tbl-header-font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string TableHeaderFont { get; set; } = null!;


        [Category("Tabs"), CaptionStatic("--tabs-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsBg { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsClr { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-strip-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsStripBg { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-strip-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TabsStripBorder { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-strip-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string TabsStripBorderRadius { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabBg { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabClr { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-bg-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabBgHover { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-clr-hover"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabClrHover { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-bg-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabBgActive { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-clr-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabClrActive { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-bg-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabBgFocus { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-clr-focus"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string TabsTabClrFocus { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string TabsTabBorder { get; set; } = null!;

        [Category("Tabs"), CaptionStatic("--tabs-tab-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string TabsTabBorderRadius { get; set; } = null!;



        [Category("Step"), CaptionStatic("--step-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string StepBg { get; set; } = null!;

        [Category("Step"), CaptionStatic("--step-clr"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string StepClr { get; set; } = null!;

        [Category("Step"), CaptionStatic("--step-bg-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string StepBgActive { get; set; } = null!;

        [Category("Step"), CaptionStatic("--step-clr-active"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string StepClrActive { get; set; } = null!;



        [Category("Panel"), CaptionStatic("--panel-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string PanelBg { get; set; } = null!;

        //[Category("Panel"), CaptionStatic("--panel-clr"), Description("Color")]
        //[UIHint("Color"), StringLength(MaxColor), Required]
        //public string PanelClr { get; set; } = null!;

        [Category("Panel"), CaptionStatic("--panel-border"), Description("Border")]
        [UIHint("Text40"), StringLength(MaxBorder), Required]
        public string PanelBorder { get; set; } = null!;

        [Category("Panel"), CaptionStatic("--panel-border-radius"), Description("Border Radius")]
        [UIHint("Text40"), StringLength(MaxRadius), Required]
        public string PanelBorderRadius { get; set; } = null!;

        [Category("Panel"), CaptionStatic("--panel-shadow"), Description("Shadow")]
        [UIHint("Text40"), StringLength(MaxShadow), Required]
        public string PanelShadow { get; set; } = null!;


        [Category("Switch"), CaptionStatic("--switch-bg-on"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string SwitchBgOn { get; set; } = null!;

        [Category("Switch"), CaptionStatic("--switch-clr-on"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string SwitchClrOn { get; set; } = null!;

        [Category("Switch"), CaptionStatic("--switch-bg-off"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string SwitchBgOff { get; set; } = null!;

        [Category("Switch"), CaptionStatic("--switch-clr-off"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string SwitchClrOff { get; set; } = null!;

        [Category("Switch"), CaptionStatic("--switch-bg-switch"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string SwitchBgSwitch { get; set; } = null!;


        [Category("Indicators"), CaptionStatic("--mod-current-bg"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string ModCurrentBg { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-page-noUserAnon"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnPageNoUserAnon { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-page-noAnon"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnPageNoAnon { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-page-noUser"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnPageNoUser { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-mod-noUserAnon"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnModNoUserAnon { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-mod-noAnon"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnModNoAnon { get; set; } = null!;

        [Category("Indicators"), CaptionStatic("--own-mod-noUser"), Description("Color")]
        [UIHint("Color"), StringLength(MaxColor), Required]
        public string OwnModNoUser { get; set; } = null!;



        [Category("Config"), CaptionStatic("CSS Variables"), Description("Paste your settings to edit further or copy to skin SCSS file to use in a skin")]
        [UIHint("TextAreaSourceOnly"), AdditionalMetadata("Spellcheck", false), AdditionalMetadata("EmHeight", 30), AdditionalMetadata("Copy", true), StringLength(0)]
        public string CSSVariables { get; set; } = null!;

        [Category("Config"), CaptionStatic("Theme"), Description("The name of the theme - Initially displays the current theme name, but can be changed beore saving the CSS variables to create a new theme")]
        [UIHint("Text40"), StringLength(SiteDefinition.MaxTheme), Required]//TODO: validation
        public string Theme { get; set; } = null!;

        [Category("Config"), CaptionStatic(""), Description("")]
        [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
        public ModuleAction Load { get; set; }




        [Category("Auto Gen"), CaptionStatic("Theme Style"), Description("")]
        [UIHint("Enum")]
        public BasicThemeEnum GenTheme { get; set; }

        [Category("Auto Gen"), CaptionStatic("Background Color"), Description("Background")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenBg { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Foreground Color"), Description("Color")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenClr { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Background Active Color"), Description("Background")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenBgActive { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Foreground Active Color"), Description("Color")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenClrActive { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Background Shaded Color"), Description("Background")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenBgShaded { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Foreground Shaded Color"), Description("Color")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenClrShaded { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Background Shaded Active Color"), Description("Background")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenBgShadedActive { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Foreground Shaded Active Color"), Description("Color")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenClrShadedActive { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Font"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string GenFont { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("SmallFont"), Description("Font")]
        [UIHint("Text40"), StringLength(MaxFont), Required]
        public string GenSmallFont { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Border Width"), Description("Border Width")]
        [UIHint("IntValue2"), Range(0, 20), Required]
        public int GenBorderWidth { get; set; }

        [Category("Auto Gen"), CaptionStatic("Border Color"), Description("Border")]
        [UIHint("Color"), AdditionalMetadata("ColorOnly", true), StringLength(MaxColor), Required]
        public string GenBorderClr { get; set; } = null!;

        [Category("Auto Gen"), CaptionStatic("Radius Pixels"), Description("Border Radius")]
        [UIHint("IntValue2"), Range(0, 20), Required]
        public int GenBorderRadius { get; set; }


        [Category("Auto Gen"), CaptionStatic(""), Description("")]
        [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.ButtonOnly), ReadOnly]
        public ModuleAction AutoGenerate { get; set; }


        public Model() {
            Theme = UserSettings.GetProperty<string?>("Theme") ?? Manager.CurrentSite.Theme ?? SiteDefinition.DefaultTheme;
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

    public async Task<ActionInfo> RenderModuleAsync() {
        if (YetaWFManager.Deployed) return ActionInfo.Empty;
        if (CssLegacy.IsLegacyBrowser()) return ActionInfo.Empty;
        if (Manager.IsInPopup) return ActionInfo.Empty;

        Model model = new Model { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (string.Compare(model.Theme, "Default", true) == 0)
            throw new Error(this.__ResStr("notDefault", "The Default theme cannot be changed - It has to be saved using a different theme name"));

        string file = Path.Combine(Manager.SkinInfo.Folder, "Themes", $"{model.Theme}.css");
        bool newFile = !await FileSystem.FileSystemProvider.FileExistsAsync(file);
        await FileSystem.FileSystemProvider.WriteAllTextAsync(file, model.CSSVariables);

        if (newFile) {
            // when a new theme is saved, activate it for the current user
            await UserSettings.SetPropertyAsync("Theme", model.Theme);
            await Manager.CurrentSite.SaveAsync();
            return await FormProcessedAsync(model, this.__ResStr("okUpdated", "Theme {0} successfully saved - Your user settings have been updated to use the new theme", model.Theme), ForceReload: true);
        } else
            return await FormProcessedAsync(model, this.__ResStr("ok", "Theme {0} successfully saved", model.Theme));
    }
}
