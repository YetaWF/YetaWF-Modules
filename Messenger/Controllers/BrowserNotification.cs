/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class BrowserNotificationsModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.BrowserNotificationsModule> {

        public BrowserNotificationsModuleController() { }

        [Trim]
        [Header("The module Web Browser Notifications (Skin) must be defined as a site-wide reference in order for notifications to be sent to the desktop.")]
        public class Model {

            [Caption("Title"), Description("Defines the title of the message to be sent")]
            [UIHint("Text80"), StringLength(80), Trim, Required]
            public string Title { get; set; }

            [TextAbove("Please enter the message to be sent as a desktop notification.")]
            [Caption("Message"), Description("Defines the message to be sent")]
            [UIHint("TextAreaSourceOnly"), StringLength(120), Trim, Required]
            public string Message { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult BrowserNotifications() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> BrowserNotifications_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            // Send the message. This can be customized with icons, url and timeout value.
            await YetaWF_Messenger_BrowserNotificationsHub.SendMessageAsync(model.Title, model.Message, "", 5000, "https://YetaWF.com/");

            return FormProcessed(model);
        }
    }
}
