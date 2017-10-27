/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ImageRepository.Controllers {

    public class CKEBrowseImagesModuleController : ControllerImpl<YetaWF.Modules.ImageRepository.Modules.CKEBrowseImagesModule> {

        public CKEBrowseImagesModuleController() { }

        [Trim]
        public class Model {

            [Caption("Image Selection"), Description("Select an image or upload a new image")]
            [UIHint("YetaWF_ImageRepository_ImageSelection"), Required, Trim]
            public string ImageName { get; set; }

            public ImageSelectionInfo ImageName_Info { get; set; }

            [UIHint("Hidden")]
            public Guid FolderGuid { get; set; }
            [UIHint("Hidden")]
            public string SubFolder { get; set; }

            [UIHint("Hidden")]
            public string CKEditor { get; set; }
            [UIHint("Hidden")]
            public int CKEditorFuncNum { get; set; }
            [UIHint("Hidden")]
            public string LangCode { get; set; }

            public Model() { }

            public void Update(YetaWFManager manager, ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(manager, module, FolderGuid, SubFolder) {
                    AllowUpload = true,
                };
            }
        }

        [AllowGet]
        public ActionResult CKEBrowseImages(Guid __FolderGuid, Guid __SubFolder, string CKEditor, int CKEditorFuncNum, string langCode) {
            Model model = new Model {
                FolderGuid = __FolderGuid,
                SubFolder = __SubFolder == Guid.Empty ? null : __SubFolder.ToString(),
                CKEditor = CKEditor,
                CKEditorFuncNum = CKEditorFuncNum,
                LangCode = langCode,
            };
            model.Update(Manager, Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult CKEBrowseImages_Partial(Model model) {
            model.Update(Manager, Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // build javascript to return selected image to CKEditor
            string imageUrl = model.ImageName_Info.MakeImageUrl(model.ImageName, ForceHttpHandler: true);
            string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, YetaWFManager.JserEncode(imageUrl));

            return FormProcessed(model, /*this.__ResStr("okSaved", "Image saved"), */ OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, ExtraJavaScript: js);
        }
    }
}