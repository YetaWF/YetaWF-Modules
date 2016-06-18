/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTestUrlModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTestUrlModule> {

        public TemplateTestUrlModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string Url { get; set; }

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string LocalUrl { get; set; }

            [Caption("Remote Url"), Description("Remote")]
            [UIHint("Url"), UrlValidation]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string RemoteUrl { get; set; }

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), ReadOnly]
            public string ROUrl { get; set; }

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), ReadOnly]
            public string ROLocalUrl { get; set; }

            [Caption("Remote Url"), Description("Remote")]
            [UIHint("Url"), ReadOnly]
            public string RORemoteUrl { get; set; }

            [Caption("New Url"), Description("New Url")]
            [UIHint("Text80"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlHelperEx.UrlTypeEnum.New), Required, Trim]
            public string NewUrl { get; set; }

            public EditModel() {
                ROUrl = "/Tests/Text";
                ROLocalUrl = "/Tests/Text";
                RORemoteUrl = "http://www.softelvdm.com";
            }
        }

        [HttpGet]
        public ActionResult TemplateTestUrl() {
            EditModel model = new EditModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemplateTestUrl_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}