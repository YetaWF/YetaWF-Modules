/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System.Web.Mvc;
using System.Web.Mvc.Html;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Views.Shared {

    public class FlashSelectionHelper<TModel> : RazorTemplate<TModel> { }

    public static class FlashSelectionHelper {

        public static MvcHtmlString RenderUploadControl(this HtmlHelper<object> htmlHelper, string name, FlashSelectionInfo info) {
            // the upload control
            FileUpload1 upload = new FileUpload1() {
                SaveURL = YetaWFManager.UrlFor(typeof(YetaWF.Modules.ImageRepository.Controllers.Shared.FlashSelectionHelperController), "SaveFlashImage",
                    new { FolderGuid = info.FolderGuid, SubFolder = info.SubFolder, FileType = info.FileType }),
            };
            return htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1"));
        }

    }
}