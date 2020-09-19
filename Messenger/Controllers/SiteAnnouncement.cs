/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Messenger.DataProvider;
#if MVC6
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
#else
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {
    public class SiteAnnouncementModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SiteAnnouncementModule> {

        public SiteAnnouncementModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Title"), Description("Defines the title of the message to be sent")]
            [UIHint("Text80"), StringLength(DataProvider.SiteAnnouncement.MaxTitle), Trim, Required]
            public string Title { get; set; }

            [TextAbove("Please enter the message to be sent to all users that are currently using the site (anonymous and logged on users).")]
            [Caption("Message"), Description("Defines the message to be sent")]
            [UIHint("TextArea"), AdditionalMetadata("PageBrowse", true), AdditionalMetadata("Encode", false), StringLength(DataProvider.SiteAnnouncement.MaxMessage), Trim, Required]
            public string Message { get; set; }

            [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
            [UIHint("Boolean"), SuppressIf(nameof(IsDemoMode), false), ReadOnly]
            public bool TestModeDemo { get { return TestMode; } set { TestMode = value; } }

            [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
            [UIHint("Boolean"), SuppressIf(nameof(IsDemoMode), true)]
            public bool TestModeProd { get { return TestMode; } set { TestMode = value; } }

            public bool TestMode { get; set; }

            [Caption(" "), Description(" ")]
            [UIHint("String"), SuppressIf(nameof(IsDemoMode), false), ReadOnly]
            public string Description { get; set; }

            public bool IsDemoMode { get { return YetaWFManager.IsDemo; } }

            public AddModel() {
                TestMode = IsDemoMode;
                Description = this.__ResStr("demo", "In Demo mode, the message is sent to the current user only - Other users do not receive the message.");
            }

            public SiteAnnouncement GetData() {
                SiteAnnouncement data = new SiteAnnouncement();
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void UpdateData() {
                if (IsDemoMode)
                    TestMode = IsDemoMode;
            }
        }

        [AllowGet]
        public ActionResult SiteAnnouncement() {
            AddModel model = new AddModel {};
            ObjectSupport.CopyData(new SiteAnnouncement(), model);
            model.Title = this.__ResStr("anncTitle", "Site Announcement");
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SiteAnnouncement_Partial(AddModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            if (model.TestMode) {
                return FormProcessed(model, model.Message, model.Title, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace, PopupOptions: "{encoded:true, canClose: true, autoClose: 0}");
            } else {
                using (SiteAnnouncementDataProvider siteAnnounceDP = new SiteAnnouncementDataProvider()) {

                    await YetaWF_Messenger_SiteAnnouncementsHub.SendMessageAsync(model.Message, model.Title);

                    if (await siteAnnounceDP.IsInstalledAsync()) {
                        if (!await siteAnnounceDP.AddItemAsync(model.GetData()))
                            throw new Error(this.__ResStr("noLog", "Message sent. New site announcement log record couldn't be added"));
                    }
                }
                return FormProcessed(model);
            }
        }
    }
}
