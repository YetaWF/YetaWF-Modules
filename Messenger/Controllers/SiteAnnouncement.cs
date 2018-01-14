/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Messenger.DataProvider;
using Microsoft.AspNet.SignalR;
using System;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {
    public class SiteAnnouncementModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SiteAnnouncementModule> {

        public SiteAnnouncementModuleController() { }

        [Trim]
        public class AddModel {

            [Caption("Title"), Description("Defines the title of the message to be sent")]
            [UIHint("Text80"), StringLength(SiteAccouncement.MaxTitle), Trim, Required]
            public string Title { get; set; }

            [TextAbove("Please enter the message to be sent to all users that are currently using the site (anonymous and logged on users).")]
            [Caption("Message"), Description("Defines the message to be sent")]
            [UIHint("TextArea"), AdditionalMetadata("PageBrowse", true), AdditionalMetadata("Encode", false), StringLength(SiteAccouncement.MaxMessage), Trim, Required]
            public string Message { get; set; }

            [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
            [UIHint("Boolean"), SuppressIfEqual("IsDemoMode", false), ReadOnly]
            public bool TestModeDemo { get { return TestMode; } set { TestMode = value; } }

            [Caption("Test Mode"), Description("Select to test sending a message to yourself (no other users will receive this message)")]
            [UIHint("Boolean"), SuppressIfEqual("IsDemoMode", true)]
            public bool TestModeProd { get { return TestMode; } set { TestMode = value; } }

            public bool TestMode { get; set; }

            [Caption(" "), Description(" ")]
            [UIHint("String"), SuppressIfEqual("IsDemoMode", false), ReadOnly]
            public string Description { get; set; }

            public bool IsDemoMode { get { return Manager.IsDemo; } }

            public AddModel() {
                TestMode = IsDemoMode;
                Description = this.__ResStr("demo", "In Demo mode, the message is sent to the current user only - Other users do not receive the message.");
            }

            public SiteAccouncement GetData() {
                SiteAccouncement data = new SiteAccouncement();
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
            ObjectSupport.CopyData(new SiteAccouncement(), model);
            model.Title = this.__ResStr("anncTitle", "Site Announcement");
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult SiteAnnouncement_Partial(AddModel model) {
            model.UpdateData();
            if (!ModelState.IsValid)
                return PartialView(model);

            if (model.TestMode) {
                return FormProcessed(model, model.Message, model.Title, OnClose: OnCloseEnum.UpdateInPlace, OnPopupClose: OnPopupCloseEnum.UpdateInPlace, PopupOptions: "{encoded:true}");
            } else {
                using (SiteAccouncementDataProvider dataProvider = new SiteAccouncementDataProvider()) {
                    if (!dataProvider.AddItem(model.GetData())) {
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "New site announcement couldn't be added"));
                        return PartialView(model);
                    }

                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<YetaWF_Messenger_SiteAnnouncement>();
                    Dispatch(context.Clients.All, "message", model.Message, model.Title);

                }
                return FormProcessed(model);
            }
        }

        private void Dispatch(dynamic targets, string message, params object[] parms) {
            targets.Invoke(message, parms);
        }

        public class YetaWF_Messenger_SiteAnnouncement : Hub { }
    }
}
