﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Templates;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.ImageRepository.Views.Shared {

    public class FlashSelectionHelper<TModel> : RazorTemplate<TModel> { }

    public static class FlashSelectionHelper {

#if MVC6
        public static HtmlString RenderUploadControl(this IHtmlHelper<string> htmlHelper, string name, FlashSelectionInfo info)
#else
        public static HtmlString RenderUploadControl(this HtmlHelper<object> htmlHelper, string name, FlashSelectionInfo info)
#endif
        {
            // the upload control
            FileUpload1 upload = new FileUpload1() {
                SaveURL = YetaWFManager.UrlFor(typeof(YetaWF.Modules.ImageRepository.Controllers.Shared.FlashSelectionHelperController), "SaveFlashImage",
                    new { FolderGuid = info.FolderGuid, SubFolder = info.SubFolder, FileType = info.FileType }),
            };
#if MVC6
            return new HtmlString(htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1")).AsString());
#else
            return htmlHelper.EditorFor(x => upload, UIHintAttribute.TranslateHint("FileUpload1"));
#endif
        }
    }
}