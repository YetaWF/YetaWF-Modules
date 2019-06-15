/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using System.Threading.Tasks;
using YetaWF.Modules.ImageRepository.Components;
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

            public async Task UpdateAsync(ModuleDefinition module) {
                ImageName_Info = new ImageSelectionInfo(module, FolderGuid, SubFolder) {
                    AllowUpload = true,
                };
                await ImageName_Info.InitAsync();
            }
        }

        [AllowGet]
        public async Task<ActionResult> CKEBrowseImages(Guid __FolderGuid, Guid __SubFolder, string CKEditor, int CKEditorFuncNum, string langCode) {
            Model model = new Model {
                FolderGuid = __FolderGuid,
                SubFolder = __SubFolder == Guid.Empty ? null : __SubFolder.ToString(),
                CKEditor = CKEditor,
                CKEditorFuncNum = CKEditorFuncNum,
                LangCode = langCode,
            };
            await model.UpdateAsync(Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CKEBrowseImages_Partial(Model model) {
            await model.UpdateAsync(Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // build javascript to return selected image to CKEditor
            string imageUrl = model.ImageName_Info.MakeImageUrl(model.ImageName);
            string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, Utility.JserEncode(imageUrl));

            return FormProcessed(model, /*this.__ResStr("okSaved", "Image saved"), */ OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, ExtraJavaScript: js);
        }
    }
}