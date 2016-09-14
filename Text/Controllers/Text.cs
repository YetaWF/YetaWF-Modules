/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Text#License */

using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;

namespace YetaWF.Modules.Text.Controllers {

    public class TextModuleController : ControllerImpl<YetaWF.Modules.Text.Modules.TextModule> {

        public class TextModel {
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 25)]
            [AdditionalMetadata("TextAreaSave", true), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), AdditionalMetadata("PageBrowse", true)]
            [AllowHtml]
            public string Contents { get; set; }
        }
        public class TextModelDisplay {
            [UIHint("TextArea"), AdditionalMetadata("Encode", false)]
            public string Contents { get; set; }
        }

        [HttpGet]
        public ActionResult Text() {
            if (Module.Feed) {
                string rssUrl = string.IsNullOrWhiteSpace(Module.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : Module.FeedMainUrl;
                Manager.LinkAltManager.AddLinkAltTag(AreaRegistration.CurrentPackage.AreaName, "application/rss+xml", Module.FeedTitle, rssUrl);
            }

            if (Manager.EditMode && Module.EditOnPage && Module.IsAuthorized(ModuleDefinition.RoleDefinition.Edit)) {
                TextModel model = new TextModel { Contents = Module.Contents };
                return View(model);
            } else {
                TextModelDisplay model = new TextModelDisplay { Contents = Module.Contents };
                return View("TextDisplay", model);
            }
        }

        [HttpPost]
        [Permission("Edit")]
        [ExcludeDemoMode]
        public ActionResult Text_Partial(TextModel model) {
            Module.Contents = model.Contents;
            Module.Save();
            if (Manager.RequestForm[Globals.Link_SubmitIsApply] != null) {
                return FormProcessed(model, "Contents saved", OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule, OnApply: OnApplyEnum.ReloadModule);
            } else {
                Manager.EditMode = false;
                return FormProcessed(model, OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
            }
        }
    }
}