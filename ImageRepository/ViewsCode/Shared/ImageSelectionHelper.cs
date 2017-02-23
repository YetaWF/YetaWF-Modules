/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.ImageRepository.Views.Shared {

    public class ImageSelectionHelper<TModel> : RazorTemplate<TModel> { }

    public static class ImageSelectionHelper {
#if MVC6
        public static MvcHtmlString RenderUploadControl(this IHtmlHelper<string> htmlHelper, string name, ImageSelectionInfo info)
#else
        public static MvcHtmlString RenderUploadControl(this HtmlHelper<object> htmlHelper, string name, ImageSelectionInfo info)
#endif
        {
            // the upload control
            FileUpload1 upload = new FileUpload1() {
                SaveURL = YetaWFManager.UrlFor(typeof(YetaWF.Modules.ImageRepository.Controllers.Shared.ImageSelectionHelperController), "SaveImage",
                    new { FolderGuid = info.FolderGuid, SubFolder = info.SubFolder, FileType = info.FileType }),
            };
#if MVC6
            return MvcHtmlString.Create(htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1")));
#else
            return htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1"));
#endif
        }

    }
}