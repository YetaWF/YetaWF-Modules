using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Content.Controllers {

    public class ContentModuleController : ControllerImpl<YetaWF.Modules.Content.Modules.ContentModule> {

        public class ContentModel {
            [UIHint("Content"), StringLength(0), AdditionalMetadata("EmHeight", 25)]
            //$$$ [AdditionalMetadata("TextAreaSave", true), AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), AdditionalMetadata("PageBrowse", true)]
            public string Contents { get; set; }
            [UIHint("Hidden")]
            public string Url { get; set; }
        }
        public class ContentModelDisplay {
            [UIHint("Content"), ReadOnly]
            public string Contents { get; set; }
        }

        [AllowGet]
        public ActionResult Content() {
            //$$$ if (Module.Feed) {
            //    string rssUrl = string.IsNullOrWhiteSpace(Module.FeedMainUrl) ? Manager.CurrentSite.HomePageUrl : Module.FeedMainUrl;
            //    Manager.LinkAltManager.AddLinkAltTag(AreaRegistration.CurrentPackage.AreaName, "application/rss+xml", Module.FeedTitle, rssUrl);
            //}

            if (Manager.EditMode /*$$$$$ && Module.EditOnPage*/ && Module.IsAuthorized(ModuleDefinition.RoleDefinition.Edit)) {
                ContentModel model = new ContentModel {
                    Contents = Module.Contents,
                    Url = Manager.CurrentRequestUrl,
                };
                return View("Content", model);
            } else {
                ContentModelDisplay model = new ContentModelDisplay { Contents = Module.Contents };
                return View("ContentDisplay", model);
            }
        }

        [AllowPost]
        [Permission("Edit")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Content_Partial(ContentModel model) {
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
