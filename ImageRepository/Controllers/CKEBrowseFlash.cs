/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;

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

            public void Update(YetaWFManager manager, ModuleDefinition module) {
                FlashImageName_Info = new FlashSelectionInfo(manager, module, FolderGuid, SubFolder) {
                    AllowUpload = true,
                };
            }
        }

        [HttpGet]
        public ActionResult CKEBrowseFlash(Guid __FolderGuid, Guid __SubFolder, string CKEditor, int CKEditorFuncNum, string langCode) {
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

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult CKEBrowseFlash_Partial(Model model) {
            model.Update(Manager, Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // build javascript to return selected Flash image to CKEditor
            string imageUrl = model.FlashImageName_Info.MakeFlashUrl(model.FlashImageName);
            string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, YetaWFManager.JserEncode(Manager.GetCDNUrl(imageUrl)));

            return FormProcessed(model, /*this.__ResStr("okSaved", "Flash image saved"),*/ OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, ExtraJavaScript: js);
        }
    }
}