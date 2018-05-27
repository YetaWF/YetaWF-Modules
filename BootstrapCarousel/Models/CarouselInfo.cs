/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.BootstrapCarousel.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.BootstrapCarousel.Models {

    [TemplateAction(TemplateName)]
    public class CarouselInfo : ITemplateAction {

        public const string TemplateName = "YetaWF_BootstrapCarousel_SlideShow";

        public enum CarouselAction {
            Apply = 0,
            MoveLeft = 1,
            MoveRight = 2,
            Add = 3,
            Insert = 4,
            Remove = 5,
        }

        public class CarouselItem {

            public const int MaxCaption = 1000;

            [Caption("Image"), Description("The image for this carousel entry - All images should be the same size as the carousel will resize to display each image")]
            [UIHint("Image"), AdditionalMetadata("ImageType", ImageSupport.ImageType), AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
            [AdditionalMetadata("File", true)]
            public string Image { get; set; }
            [UIHint("Hidden")]
            public Guid Image_Guid { get; set; }

            [AdditionalMetadata("ImageType", ImageSupport.ImageType), AdditionalMetadata("Width", 0), AdditionalMetadata("Height", 0)]
            [AdditionalMetadata("File", true)]
            [UIHint("Image")]
            [DontSave] // Image with metadata for display rendering only
            public string ImageDisplay { get { return Image; } }

            [Caption("Url"), Description("The optional Url visited when the image is clicked")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string Url { get; set; }

            [StringLength(MaxCaption)]
            public MultiString CompleteCaption { get; set; }

            [Caption("Caption"), Description("The optional caption for this image")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 4), Trim]
            [DontSave]
            public string Caption {
                get {
                    return CompleteCaption[MultiString.ActiveLanguage];
                }
                set {
                    CompleteCaption[MultiString.ActiveLanguage] = value;
                }
            }

            public CarouselItem() {
                CompleteCaption = new MultiString();
            }
        }

        public CarouselInfo() {
            Slides = new SerializableList<CarouselItem> {
                new CarouselItem { Image = "(None)", },
                new CarouselItem { Image = "(None)", },
                new CarouselItem { Image = "(None)", },
            };
            Interval = 5000;
            Pause = true;
            Wrap = true;
            Keyboard = true;
        }
        public async Task SavingAsync(string propertyName, Guid moduleGuid) {
            int index = 0;
            foreach (CarouselItem s in Slides) {
                await DataProviderImpl.SaveImagesAsync(moduleGuid, s);
                s.Image = string.Format("{0},{1},{2}", moduleGuid, "Image", s.Image_Guid);
                ++index;
            }
        }

        [Data_Binary]
        public SerializableList<CarouselItem> Slides { get; set; }

        [Category("Slide Show"), Caption("Interval"), Description("Interval (in milliseconds) before the next slide is displayed when images are automatically changing - 0 to turn off")]
        [UIHint("IntValue6"), Range(0, 20000)]
        public int Interval { get; set; }

        [Category("Slide Show"), Caption("Loop"), Description("Defines whether the images loop (wrap around when the last image is displayed)")]
        [UIHint("Boolean")]
        public bool Wrap { get; set; }

        [Category("Slide Show"), Caption("PauseOnHover"), Description("Defines whether the carousel pauses cycling when the mouse cursor enters the carousel and resumes cycling when the mouse cursor leaves the carousel")]
        [UIHint("Boolean")]
        public bool Pause { get; set; }

        [Category("Slide Show"), Caption("Keyboard"), Description("Defines whether the carousel should react to keyboard events")]
        [UIHint("Boolean")]
        public bool Keyboard { get; set; }

        [DontSave]
        public int _ActiveTab { get; set; }

        public bool ExecuteAction(int action, bool modelIsValid, object extraData) {
            bool processed = false;
            CarouselAction slideAction = (CarouselAction)action;
            int slideIndex = Convert.ToInt32((string) extraData);
            switch (slideAction) {
                case CarouselAction.Apply:
                    if (modelIsValid)
                        processed = true;
                    break;
                case CarouselAction.Insert:
                    if (modelIsValid) {
                        InsertSlide(slideIndex);
                        processed = true;
                    }
                    break;
                case CarouselAction.Add:
                    if (modelIsValid) {
                        AddSlide(slideIndex);
                        processed = true;
                    }
                    break;
                case CarouselAction.Remove:
                    RemoveSlide(slideIndex);
                    processed = true;
                    break;
                case CarouselAction.MoveLeft:
                    if (modelIsValid) {
                        MoveSlideLeft(slideIndex);
                        processed = true;
                    }
                    break;
                case CarouselAction.MoveRight:
                    if (modelIsValid) {
                        MoveSlideRight(slideIndex);
                        processed = true;
                    }
                    break;
                default:
                    throw new InternalError("Invalid action {0}", slideAction);
            }
            _ActiveTab = Math.Min(Slides.Count - 1, Math.Max(0, _ActiveTab));
            return processed;
        }
        private void InsertSlide(int slideIndex) {
            Slides.Insert(slideIndex, new CarouselItem());
        }
        private void AddSlide(int slideIndex) {
            Slides.Insert(slideIndex+1, new CarouselItem());
            _ActiveTab = slideIndex + 1;
        }
        private void RemoveSlide(int slideIndex) {
            Slides.RemoveAt(slideIndex);
            _ActiveTab = slideIndex;
        }
        private void MoveSlideLeft(int slideIndex) {
            if (slideIndex <= 0) throw new InternalError("Invalid slide index");
            CarouselItem slide = Slides[slideIndex];
            Slides.RemoveAt(slideIndex);
            Slides.Insert(slideIndex - 1, slide);
            _ActiveTab = slideIndex - 1;
        }
        private void MoveSlideRight(int slideIndex) {
            if (slideIndex >= Slides.Count-1) throw new InternalError("Invalid slide index");
            CarouselItem slide = Slides[slideIndex];
            Slides.RemoveAt(slideIndex);
            Slides.Insert(slideIndex + 1, slide);
            _ActiveTab = slideIndex + 1;
        }
    }
}
