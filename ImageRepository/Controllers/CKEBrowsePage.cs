/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ImageRepository.Controllers {

    public class CKEBrowsePageModuleController : ControllerImpl<YetaWF.Modules.ImageRepository.Modules.CKEBrowsePageModule> {

        public CKEBrowsePageModuleController() { }

        [Trim]
        public class Model {

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string PageUrl { get; set; }

            [UIHint("Hidden")]
            public string CKEditor { get; set; }
            [UIHint("Hidden")]
            public int CKEditorFuncNum { get; set; }
            [UIHint("Hidden")]
            public string LangCode { get; set; }

            public Model() { }

            public void Update(ModuleDefinition module) { }
        }

        [HttpGet]
        public ActionResult CKEBrowsePage(string CKEditor, int CKEditorFuncNum, string langCode) {
            Model model = new Model {
                CKEditor = CKEditor,
                CKEditorFuncNum = CKEditorFuncNum,
                LangCode = langCode,
            };
            model.Update(Module);
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult CKEBrowsePage_Partial(Model model) {
            model.Update(Module);
            if (!ModelState.IsValid)
                return PartialView(model);

            // build javascript to return selected Page image to CKEditor
            string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, YetaWFManager.JserEncode(model.PageUrl));
            return FormProcessed(model, OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, ExtraJavaScript: js);
        }
    }
}