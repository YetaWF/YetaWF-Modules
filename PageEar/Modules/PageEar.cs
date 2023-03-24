/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Image;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.PageEar.Modules;

public class PageEarModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageEarModule>, IInstallableModel { }

[ModuleGuid("{b140e078-a3a8-4be7-8383-fd901e882b35}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class PageEarModule : ModuleDefinition {

    public PageEarModule() {
        Name = this.__ResStr("modName", "Page Ear");
        Description = this.__ResStr("modSummary", "Adds a configurable page ear to each page where this module is located. The module can be added anywhere on the page (in a pane) and is only visible in Site Edit Mode. The page ear itself (in the upper right corner) is only shown in Site View Mode once all required module settings have been defined (like the images and URL).");
        WantFocus = WantSearch = false;
        ShowTitle = false;
        AdImage_Data = Array.Empty<byte>();
        CoverImage_Data = Array.Empty<byte>();
        ClickUrl = Manager.CurrentSite.HomePageUrl;
        SmallSize = 80;
        Animate = true;
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PageEarModuleDataProvider(); }
    public override async Task ModuleSavingAsync() {
        (int width, int height) = await ImageSupport.GetImageSizeAsync(AdImage_Data);
        LargeSize = height;
    }

    [Category("General"), Caption("Ad Image"), Description("The image used for the page ear (the advertisement) - this should be the same size as the CoverImage image")]
    [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
    [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
    [DontSave]
    public string? AdImage {
        get {
            if (_adImage == null) {
                if (AdImage_Data.Length > 0)
                    _adImage = ModuleGuid.ToString() + ",AdImage_Data";
            }
            return _adImage;
        }
        set {
            _adImage = value;
        }
    }
    private string? _adImage = null;

    [Data_Binary, CopyAttribute]
    public byte[] AdImage_Data { get; set; }

    [Category("General"), Caption("Cover Image"), Description("The image used to cover the lower left of the ad image (when the page year is expanded) - this should be the same size as the AdImage image")]
    [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType), StringLength(Globals.MaxFileName)]
    [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
    [DontSave]
    public string? CoverImage {
        get {
            if (_coverImage == null) {
                if (CoverImage_Data.Length > 0)
                    _coverImage = ModuleGuid.ToString() + ",CoverImage_Data";
            }
            return _coverImage;
        }
        set {
            _coverImage = value;
        }
    }
    private string? _coverImage = null;

    [Data_Binary, CopyAttribute]
    public byte[] CoverImage_Data { get; set; }

    [Category("General"), Caption("Small Size"), Description("The width and height (square) of the image when the page ear is collapsed")]
    [UIHint("IntValue4"), Range(10, 200)]
    public int SmallSize { get; set; }

    public int LargeSize { get; set; }

    [Category("General"), Caption("Url"), Description("The URL where the user is directed when clicking on the page ear")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
    [StringLength(Globals.MaxUrl), Required, Trim]
    public string ClickUrl { get; set; }

    [Category("General"), Caption("Animate"), Description("Defines whether page ear opening/closing is animated")]
    [UIHint("Boolean")]
    public bool Animate { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class Model { }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!Manager.EditMode && (AdImage_Data.Length == 0 || CoverImage_Data.Length == 0 || string.IsNullOrWhiteSpace(ClickUrl)))
            return ActionInfo.Empty;
        return await RenderAsync(new Model());
    }
}