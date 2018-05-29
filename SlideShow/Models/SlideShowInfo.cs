/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SlideShow#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.SlideShow.Support;
using YetaWF.Modules.SlideShow.Views.Shared;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SlideShow.Models {

    [TemplateAction(TemplateName)]
    public class SlideShowInfo : ITemplateAction {

        public const int MaxTransition = 500;

        public const string TemplateName = "YetaWF_SlideShow_SlideShowInfo";

        public enum SlideShowAction {
            MoveLeft = 1,
            MoveRight = 2,
            Add = 3,
            Insert = 4,
            Remove = 5,
        }
        public enum SlideShowType {
            [EnumDescription("Thumbnails", "Images with thumbnails")]
            Thumbnails = 0,
            [EnumDescription("Bullets", "Images with bullets")]
            Bullets = 1,
        }
        public enum FillModeEnum {
            [EnumDescription("Stretch", "Stretch the image to fill available space")]
            Stretch = 0,
            [EnumDescription("Contain", "Keep aspect ratio and display inside available space")]
            Contain = 1,
            [EnumDescription("Cover", "Keep aspect ratio and cover entire available space")]
            Cover = 2,
            [EnumDescription("Actual Size", "Images are displayed actual size")]
            ActualSize = 4,
            [EnumDescription("Mix", "Contain large images and display small images actual size")]
            Mix = 5,
        }
        public enum HoverEnum {
            [EnumDescription("No Pause", "No Pause")]
            NoPause = 0,
            [EnumDescription("Pause (Desktop)", "Pause for Desktop")]
            PauseDesktop = 1,
            [EnumDescription("Pause (Touch Device)", "Pause for touch device")]
            PauseTouch = 2,
            [EnumDescription("Pause", "Pause")]
            Pause = 3,
        }
        public enum OrientationEnum {
            [EnumDescription("Horizontal")]
            Horizontal = 1,
            [EnumDescription("Vertical")]
            Vertical = 2,
        }
        public enum LocationEnum {
            [EnumDescription("None", "Position is unchanged")]
            None = 0,
            [EnumDescription("Horizontal", "Horizontally centered")]
            Horizontal = 1,
            [EnumDescription("Vertical", "Vertically centered")]
            Vertical = 2,
            [EnumDescription("Both", "Vertically and horizontally centered")]
            Both = 3,
        }

        public class SlideInfo {

            public const int MaxCaption = 100;

            [Caption("Image"), Description("The image for this slideshow entry")]
            [UIHint("Image"), AdditionalMetadata("ImageType", ImageSupport.ImageType), AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
            [AdditionalMetadata("LinkToImage", true), AdditionalMetadata("File", true)]
            public string Image { get; set; }
            [UIHint("Hidden")]
            public Guid Image_Guid { get; set; }

            [Caption("Thumbnail Image"), Description("The optional thumbnail image for this slideshow entry")]
            [UIHint("Image"), AdditionalMetadata("ImageType", ImageSupport.ImageType), AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
            [AdditionalMetadata("LinkToImage", true), AdditionalMetadata("File", true)]
            public string ThumbnailImage { get; set; }
            [UIHint("Hidden")]
            public Guid ThumbnailImage_Guid { get; set; }

            [Caption("Image Transition"), Description("The transition used to display this image - Use the help link to preview all available transitions")]
            [UIHint("YetaWF_SlideShow_Transition"), StringLength(SlideShowInfo.MaxTransition), Required]
            [HelpLink("http://www.jssor.com/development/tool-slideshow-transition-viewer.html")]
            public string ImageTransition { get; set; }

            [Caption("Url"), Description("The optional Url visited when the image is clicked")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string Url { get; set; }

            [Caption("Caption"), Description("The optional caption for this image")]
            [StringLength(MaxCaption)]
            [UIHint("Text40"), Trim]
            public string Caption { get; set; }

            [Caption("Caption Transition (In)"), Description("The transition used to display the caption - Use the help link to preview all available caption transitions")]
            [UIHint("YetaWF_SlideShow_CaptionTransition"), StringLength(SlideShowInfo.MaxTransition), Required]
            [HelpLink("http://www.jssor.com/development/tool-caption-transition-viewer.html")]
            public string CaptionTransitionIn { get; set; }

            [Caption("Caption Transition (Out)"), Description("The transition used to remove the caption - Use the help link to preview all available caption transitions")]
            [UIHint("YetaWF_SlideShow_CaptionTransition"), StringLength(SlideShowInfo.MaxTransition), Required]
            [HelpLink("http://www.jssor.com/development/tool-caption-transition-viewer.html")]
            public string CaptionTransitionOut { get; set; }

            [Caption("Left"), Description("The left offset of the caption (in pixels)")]
            [UIHint("IntValue4"), Range(0, 9999)]
            public int CaptionLeft { get; set; }
            [Caption("Top"), Description("The top offset of the caption (in pixels)")]
            [UIHint("IntValue4"), Range(0, 9999)]
            public int CaptionTop { get; set; }
            [Caption("Width"), Description("The width of the caption (in pixels)")]
            [UIHint("IntValue4"), Range(0, 9999)]
            public int CaptionWidth { get; set; }
            [Caption("Height"), Description("The height of the caption (in pixels)")]
            [UIHint("IntValue4"), Range(0, 9999)]
            public int CaptionHeight { get; set; }

            public SlideInfo() {
                ImageTransition = TransitionHelper.Transition.DEFAULT;
                CaptionTransitionIn = CaptionTransitionHelper.CaptionTransition.DEFAULT;
                CaptionTransitionOut = CaptionTransitionHelper.CaptionTransition.DEFAULT;
                CaptionLeft = 0; CaptionTop = 0; CaptionWidth = 100; CaptionHeight = 100;
            }
        }

        public SlideShowInfo() {
            Style = SlideShowType.Thumbnails;
            ShowArrows = true;
            FillMode = FillModeEnum.Stretch;
            Width = 600;
            Height = 400;
            SlideShow = true;
            DefaultTransition = TransitionHelper.Transition.RANDOM;
            Loop = true;
            AutoPlayInterval = 3000;
            Hover = HoverEnum.Pause;
            Orientation = OrientationEnum.Horizontal;

            BulletStyle = "b01";
            BulletsAlwaysShown = true;
            BulletLocation = LocationEnum.None;
            BulletSpacingX = BulletSpacingY = 0;

            ArrowStyle = "a01";
            ArrowsAlwaysShown = true;
            ArrowLocation = LocationEnum.Vertical;

            ThumbnailStyle = "t01";
            ThumbsAlwaysShown = true;
            ThumbnailCount = 5;
            ThumbnailLocation = LocationEnum.Horizontal;
            ThumbnailOrientation = OrientationEnum.Horizontal;
            ThumbnailLoop = true;
            ThumbnailSpacingX = 0;
            ThumbnailSpacingY = 0;

            Captions = true;
            DefaultCaptionTransitionIn = "{$Duration:900,x:0.6,$Easing:{$Left:$JssorEasing$.$EaseInOutSine},$Opacity:2}";
            DefaultCaptionTransitionOut = "";

            Slides = new SerializableList<SlideInfo> {
                new SlideInfo {
                        Image = "/No Image Available",
                        ThumbnailImage = "/No Image Available",
                        Caption = "Caption",
                        CaptionLeft = 10, CaptionTop = 20, CaptionWidth = 100, CaptionHeight = 40,
                },
                new SlideInfo {
                        Image = "/No Image Available",
                        ThumbnailImage = "/No Image Available",
                        Caption = "Caption",
                        CaptionLeft = 10, CaptionTop = 20, CaptionWidth = 100, CaptionHeight = 40,
                },
                new SlideInfo {
                        Image = "/No Image Available",
                        ThumbnailImage = "/No Image Available",
                        Caption = "Caption",
                        CaptionLeft = 10, CaptionTop = 20, CaptionWidth = 100, CaptionHeight = 40,
                },
            };
        }
        public async Task SavingAsync(string propertyName, Guid moduleGuid) {
            int index = 0;
            foreach (var s in Slides) {
                await DataProviderImpl.SaveImagesAsync(moduleGuid, s);
                s.Image = string.Format("{0},{1},{2}", moduleGuid, "Image", s.Image_Guid);
                s.ThumbnailImage = string.Format("{0},{1},{2}", moduleGuid, "ThumbnailImage", s.ThumbnailImage_Guid);
                ++index;
            }
        }

        [Category("Slide Show"), Caption("Style"), Description("Defines the visual style of the slide show")]
        [UIHint("Enum")]
        public SlideShowType Style { get; set; }

        [Category("Slide Show"), Caption("Show Arrows"), Description("Defines whether navigation arrows are available")]
        [UIHint("Boolean")]
        public bool ShowArrows { get; set; }

        [Category("Slide Show"), Caption("Fill Mode"), Description("Defines how the image fills the available space")]
        [UIHint("Enum")]
        public FillModeEnum FillMode { get; set; }

        [Category("Slide Show"), Caption("Slider Width"), Description("The width of the entire slider (in pixels)")]
        [UIHint("IntValue4"), Range(10, 9999)]
        public int Width { get; set; }
        [Category("Slide Show"), Caption("Slider Height"), Description("The height of the entire slider (in pixels)")]
        [UIHint("IntValue4"), Range(10, 9999)]
        public int Height { get; set; }

        [Data_Binary] // TODO: Expand SQLDerived to support subtables so this doesn't need to be binary
        public SerializableList<SlideInfo> Slides { get; set; }

        [Category("Slide Show"), Caption("Slide Show"), Description("Defines whether the images are automatically changing")]
        [UIHint("Boolean")]
        public bool SlideShow { get; set; }

        [Category("Slide Show"), Caption("Image Transition"), Description("The transition used between images - individual images can override the transition")]
        [UIHint("YetaWF_SlideShow_Transition"), AdditionalMetadata("NoDefault", true), AdditionalMetadata("Random", true), StringLength(MaxTransition), Required]
        [HelpLink("http://www.jssor.com/development/tool-slideshow-transition-viewer.html")]
        public string DefaultTransition { get; set; }

        [Category("Slide Show"), Caption("Loop"), Description("Defines whether the images loop (wrap around when the last image is displayed)")]
        [UIHint("Boolean")]
        public bool Loop { get; set; }

        [Category("Slide Show"), Caption("Interval"), Description("Interval before the next slide is displayed when images are automatically changing")]
        [UIHint("IntValue4"), Range(500, 20000)]
        public int AutoPlayInterval { get; set; }

        [Category("Slide Show"), Caption("Mouse Hover"), Description("Defines whether the slideshow pauses when the mouse hovers over the slider")]
        [UIHint("Enum")]
        public HoverEnum Hover { get; set; }

        [Category("Slide Show"), Caption("Orientation"), Description("Defines the orientation to play the slideshow")]
        [UIHint("Enum")]
        public OrientationEnum Orientation { get; set; }

        [Category("Bullets"), Caption("Bullet Style"), Description("Bullet style - use the help link to preview all available skins")]
        [UIHint("YetaWF_SlideShow_BulletStyle"), StringLength(10), Required]
        [HelpLink("http://www.jssor.com/skins/")]
        public string BulletStyle { get; set; }

        [Category("Bullets"), Caption("Always Shown"), Description("Defines whether the bullets are always shown or only when the mouse is over the slider")]
        [UIHint("Boolean")]
        public bool BulletsAlwaysShown { get; set; }

        [Category("Bullets"), Caption("Location"), Description("Defines the position of the bullets")]
        [UIHint("Enum")]
        public LocationEnum BulletLocation { get; set; }

        [Category("Bullets"), Caption("X Spacing"), Description("The number of pixels between bullets horizontally")]
        [UIHint("IntValue2"), Range(0, 99)]
        public int BulletSpacingX { get; set; }
        [Category("Bullets"), Caption("Y Spacing"), Description("The number of pixels between bullets vertically")]
        [UIHint("IntValue2"), Range(0, 99)]
        public int BulletSpacingY { get; set; }

        [Category("Thumbnails"), Caption("Thumbnail Style"), Description("Thumbnail style - use the help link to preview all available skins")]
        [UIHint("YetaWF_SlideShow_ThumbnailStyle"), StringLength(10), Required]
        [HelpLink("http://www.jssor.com/skins/")]
        public string ThumbnailStyle { get; set; }

        [Category("Thumbnails"), Caption("Always Shown"), Description("Defines whether the thumbnails are always shown or only when the mouse is over the slider")]
        [UIHint("Boolean")]
        public bool ThumbsAlwaysShown { get; set; }

        [Category("Thumbnails"), Caption("Count"), Description("The number of thumbnails shown")]
        [UIHint("IntValue2"), Range(1, 99)]
        public int ThumbnailCount { get; set; }

        [Category("Thumbnails"), Caption("Location"), Description("Defines the position of the thumbnails")]
        [UIHint("Enum")]
        public LocationEnum ThumbnailLocation { get; set; }

        [Category("Thumbnails"), Caption("Orientation"), Description("Defines the orientation of the thumbnails")]
        [UIHint("Enum")]
        public OrientationEnum ThumbnailOrientation { get; set; }

        [Category("Thumbnails"), Caption("Loop"), Description("Defines whether the thumbnail images loop (wrap around when the last image is displayed)")]
        [UIHint("Boolean")]
        public bool ThumbnailLoop { get; set; }

        [Category("Thumbnails"), Caption("X Spacing"), Description("The number of pixels between thumbnails horizontally")]
        [UIHint("IntValue2"), Range(0, 99)]
        public int ThumbnailSpacingX { get; set; }
        [Category("Thumbnails"), Caption("Y Spacing"), Description("The number of pixels between thumbnails vertically")]
        [UIHint("IntValue2"), Range(0, 99)]
        public int ThumbnailSpacingY { get; set; }

        [Category("Arrows"), Caption("Arrow Style"), Description("Arrow style - use the help link to preview all available skins")]
        [UIHint("YetaWF_SlideShow_ArrowStyle"), StringLength(10), Required]
        [HelpLink("http://www.jssor.com/skins/")]
        public string ArrowStyle { get; set; }

        [Category("Arrows"), Caption("Always Shown"), Description("Defines whether the arrows are always shown or only when the mouse is over the slider")]
        [UIHint("Boolean")]
        public bool ArrowsAlwaysShown { get; set; }

        [Category("Arrows"), Caption("Location"), Description("Defines the position of the arrows")]
        [UIHint("Enum")]
        public LocationEnum ArrowLocation { get; set; }

        [Category("Captions"), Caption("Captions"), Description("Defines whether captions are shown")]
        [UIHint("Boolean")]
        public bool Captions { get; set; }

        [Category("Captions"), Caption("Caption Transition (In)"), Description("The transition used to display the caption (play in) - Use the help link to preview all available caption transitions")]
        [UIHint("YetaWF_SlideShow_CaptionTransition"), AdditionalMetadata("None", true), AdditionalMetadata("NoDefault", true), AdditionalMetadata("Random", true), StringLength(SlideShowInfo.MaxTransition), Required]
        [HelpLink("http://www.jssor.com/development/tool-caption-transition-viewer.html")]
        public string DefaultCaptionTransitionIn { get; set; }

        [Category("Captions"), Caption("Caption Transition (Out)"), Description("The transition used to remove the caption (play out) - Use the help link to preview all available caption transitions")]
        [UIHint("YetaWF_SlideShow_CaptionTransition"), AdditionalMetadata("None", true), AdditionalMetadata("NoDefault", true), AdditionalMetadata("Random", true), StringLength(SlideShowInfo.MaxTransition), Required]
        [HelpLink("http://www.jssor.com/development/tool-caption-transition-viewer.html")]
        public string DefaultCaptionTransitionOut { get; set; }

        public bool ExecuteAction(int action, bool modelIsValid, object extraData) {
            bool processed = false;
            SlideShowAction slideAction = (SlideShowAction)action;
            int slideIndex = Convert.ToInt32((string) extraData);
            switch (slideAction) {
                case SlideShowAction.Insert:
                    if (modelIsValid) {
                        InsertSlide(slideIndex);
                        processed = true;
                    }
                    break;
                case SlideShowAction.Add:
                    if (modelIsValid) {
                        AddSlide(slideIndex);
                        processed = true;
                    }
                    break;
                case SlideShowAction.Remove:
                    RemoveSlide(slideIndex);
                    processed = true;
                    break;
                case SlideShowAction.MoveLeft:
                    if (modelIsValid) {
                        MoveSlideLeft(slideIndex);
                        processed = true;
                    }
                    break;
                case SlideShowAction.MoveRight:
                    if (modelIsValid) {
                        MoveSlideRight(slideIndex);
                        processed = true;
                    }
                    break;
                default:
                    throw new InternalError("Invalid action {0}", slideAction);
            }
            return processed;
        }
        private void InsertSlide(int slideIndex) {
            Slides.Insert(slideIndex, new SlideInfo());
        }
        private void AddSlide(int slideIndex) {
            Slides.Insert(slideIndex+1, new SlideInfo());
        }
        private void RemoveSlide(int slideIndex) {
            Slides.RemoveAt(slideIndex);
        }
        private void MoveSlideLeft(int slideIndex) {
            if (slideIndex <= 0) throw new InternalError("Invalid slide index");
            SlideInfo slide = Slides[slideIndex];
            Slides.RemoveAt(slideIndex);
            Slides.Insert(slideIndex - 1, slide);
        }
        private void MoveSlideRight(int slideIndex) {
            if (slideIndex >= Slides.Count-1) throw new InternalError("Invalid slide index");
            SlideInfo slide = Slides[slideIndex];
            Slides.RemoveAt(slideIndex);
            Slides.Insert(slideIndex + 1, slide);
        }
    }
}
