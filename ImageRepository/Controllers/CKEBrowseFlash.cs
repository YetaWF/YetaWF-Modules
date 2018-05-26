/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

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

    public class CKEBrowseFlashModuleController : ControllerImpl<YetaWF.Modules.ImageRepository.Modules.CKEBrowseFlashModule> {

        public CKEBrowseFlashModuleController() { }

        [Trim]
        public class Model {

            [Caption("Flash Image Selection"), Description("Select a Flash image or upload a new Flash image")]
            [UIHint("YetaWF_ImageRepository_FlashSelection"), Required, Trim]
            public string FlashImageName { get; set; }

            public FlashSelectionInfo FlashImageName_Info { get; set; }

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

            public async Task UpdateAsync(YetaWFManager manager, ModuleDefinition module) {
                FlashImageName_Info = new FlashSelectionInfo(module, FolderGuid, SubFolder) {
                    AllowUpload = true,
                };
                await FlashImageName_Info.InitAsync();
            }
        }

        [AllowGet]
        public async Task<ActionResult> CKEBrowseFlash(Guid __FolderGuid, Guid __SubFolder, string CKEditor, int CKEditorFuncNum, string langCode) {
            Model model = new Model {
                FolderGuid = __FolderGuid,
                SubFolder = __SubFolder == Guid.Empty ? null : __SubFolder.ToString(),
                CKEditor = CKEditor,
                CKEditorFuncNum = CKEditorFuncNum,
                LangCode = langCode,
            };
            await model.UpdateAsync(Manager, Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> CKEBrowseFlash_Partial(Model model) {
            await model.UpdateAsync(Manager, Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // build javascript to return selected Flash image to CKEditor
            string imageUrl = model.FlashImageName_Info.MakeFlashUrl(model.FlashImageName);
            string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, YetaWFManager.JserEncode(imageUrl));

            return FormProcessed(model, /*this.__ResStr("okSaved", "Flash image saved"),*/ OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, ExtraJavaScript: js);
        }
    }
}