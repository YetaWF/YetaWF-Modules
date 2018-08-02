/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Controllers {

    public class TextModuleController : ControllerImpl<YetaWF.Modules.Text.Modules.TextModule> {

        public class TextModel {
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 25)]
            [AdditionalMetadata("TextAreaSave", true), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), AdditionalMetadata("PageBrowse", true)]
            public string Contents { get; set; }
            [UIHint("Hidden")]
            public string Url { get; set; }
        }
        public class TextModelDisplay {
            [UIHint("TextArea"), AdditionalMetadata("Encode", false), ReadOnly]
            public string Contents { get; set; }
        }

        [AllowGet]
        public ActionResult Text() {
            if (Module.Feed) {
                string rssUrl = string.IsNullOrWhiteSpace(Module.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : Module.FeedMainUrl;
                Manager.LinkAltManager.AddLinkAltTag(AreaRegistration.CurrentPackage.AreaName, "application/rss+xml", Module.FeedTitle, rssUrl);
            }

            if (Manager.EditMode && Module.EditOnPage && Module.IsAuthorized(ModuleDefinition.RoleDefinition.Edit)) {
                TextModel model = new TextModel {
                    Contents = Module.Contents,
                    Url = Manager.CurrentRequestUrl,
                };
                return View(model);
            } else {
                TextModelDisplay model = new TextModelDisplay { Contents = Module.Contents };
                return View("TextDisplay", model);
            }
        }

        [AllowPost]
        [Permission("Edit")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Text_Partial(TextModel model) {
            Module.Contents = model.Contents;
            await Module.SaveAsync();
            if (IsApply) {
                return FormProcessed(model, "Contents saved", OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule, OnApply: OnApplyEnum.ReloadModule);
            } else {
                if (Manager.IsInPopup) throw new InternalError("Save & Display not available in a popup window");
                Manager.EditMode = false;
                return Redirect(model.Url, SetCurrentEditMode: true);
            }
        }
    }
}
