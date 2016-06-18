/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Web.Mvc;
using System.Web.Mvc.Html;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Views.Shared {

    public class ImageSelectionHelper<TModel> : RazorTemplate<TModel> { }

    public static class ImageSelectionHelper {

        public static MvcHtmlString RenderUploadControl(this HtmlHelper<object> htmlHelper, string name, ImageSelectionInfo info) {
            // the upload control
            FileUpload1 upload = new FileUpload1() {
                SaveURL = YetaWFManager.UrlFor(typeof(YetaWF.Modules.ImageRepository.Controllers.Shared.ImageSelectionHelperController), "SaveImage",
                    new { FolderGuid = info.FolderGuid, SubFolder = info.SubFolder, FileType = info.FileType }),
            };
            return htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1"));
        }

    }
}