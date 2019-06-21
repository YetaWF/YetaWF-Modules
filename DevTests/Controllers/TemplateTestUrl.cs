/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTestUrlModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTestUrlModule> {

        public TemplateTestUrlModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string Url { get; set; }

            [Caption("Url"), Description("Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string Url2 { get; set; }

            [Caption("Local Url"), Description("Local Url")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
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
            [UIHint("Text80"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlTypeEnum.New), Required, Trim]
            public string NewUrl { get; set; }

            public EditModel() {
                ROUrl = "/Tests/Text";
                ROLocalUrl = "/Tests/Text";
                RORemoteUrl = "https://softelvdm.com";
            }
        }

        [AllowGet]
        public ActionResult TemplateTestUrl() {
            EditModel model = new EditModel {
                Url = "/Tests/Text",
                Url2 = "https://softelvdm.com",
                LocalUrl = "/Tests/Text",
                RemoteUrl = "https://softelvdm.com",
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTestUrl_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
