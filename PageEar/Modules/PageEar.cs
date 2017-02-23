/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEar#License */

using System;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Image;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEar.Modules {

    public class PageEarModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageEarModule>, IInstallableModel { }

    [ModuleGuid("{b140e078-a3a8-4be7-8383-fd901e882b35}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PageEarModule : ModuleDefinition {

        public PageEarModule() {
            Name = this.__ResStr("modName", "Page Ear");
            Description = this.__ResStr("modSummary", "Page ear support (page peel, page corner) for site advertisement");
            WantFocus = WantSearch = false;
            ShowTitle = false;
            AdImage_Data = new byte[] { };
            CoverImage_Data = new byte[] { };
            ClickUrl = Manager.CurrentSite.HomePageUrl;
            SmallSize = 80;
            Animate = true;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PageEarModuleDataProvider(); }
        public override void ModuleSaving() {
            System.Drawing.Size size = ImageSupport.GetImageSize(AdImage_Data);
            LargeSize = size.Height;
        }

        [Category("General"), Caption("Ad Image"), Description("The image used for the page ear (the advertisement) - this should be the same size as the CoverImage image")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
        [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
        [DontSave]
        public string AdImage {
            get {
                if (_adImage == null) {
                    if (AdImage_Data != null && AdImage_Data.Length > 0)
                        _adImage = ModuleGuid.ToString() + ",AdImage_Data";
                }
                return _adImage;
            }
            set {
                _adImage = value;
            }
        }
        private string _adImage = null;

        [Data_Binary, CopyAttribute]
        public byte[] AdImage_Data { get; set; }

        [Category("General"), Caption("Cover Image"), Description("The image used to cover the lower left of the ad image (when the page year is expanded) - this should be the same size as the AdImage image")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType), StringLength(Globals.MaxFileName)]
        [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
        [DontSave]
        public string CoverImage {
            get {
                if (_coverImage == null) {
                    if (CoverImage_Data != null && CoverImage_Data.Length > 0)
                        _coverImage = ModuleGuid.ToString() + ",CoverImage_Data";
                }
                return _coverImage;
            }
            set {
                _coverImage = value;
            }
        }
        private string _coverImage = null;

        [Data_Binary, CopyAttribute]
        public byte[] CoverImage_Data { get; set; }

        [Category("General"), Caption("Small Size"), Description("The width and height (square) of the image when the page ear is collapsed")]
        [UIHint("IntValue4"), Range(10, 200)]
        public int SmallSize { get; set; }

        public int LargeSize { get; set; }

        [Category("General"), Caption("Url"), Description("The url where the user is directed when clicking on the page ear")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Required, Trim]
        public string ClickUrl { get; set; }

        [Category("General"), Caption("Animate"), Description("Defines whether page ear opening/closing is animated")]
        [UIHint("Boolean")]
        public bool Animate { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}