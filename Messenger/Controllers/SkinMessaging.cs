/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using Microsoft.AspNet.SignalR;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Core.Identity;
using System;
using YetaWF.Core.Localize;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class SkinMessagingModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.SkinMessagingModule> {

        public SkinMessagingModuleController() { }

        public class MessageModel { }

        [AllowGet]
        public ActionResult SkinMessaging() {
            Signalr.Use();
            return new EmptyResult();
        }
    }

    public class YetaWF_Messenger_Messaging : Hub {

        public void Send(string toUser, string message) {

            YetaWFManager manager = Signalr.SetupEnvironment();
            using (MessagingDataProvider msgDP = new MessagingDataProvider()) {
                if (manager.UserId == 0) throw new InternalError("No current user");

                try {

                    int toUserId = Resource.ResourceAccess.GetUserId(toUser);
                    if (toUserId == 0) throw new Error(this.__ResStr("noUser", "User {0} doesn't exist", toUser));

                    Message msg = new Message {
                        FromUser = manager.UserId,
                        ToUser = toUserId,
                        Seen = false,
                        MessageText = message,
                    };
                    if (!msgDP.AddItem(msg)) throw new InternalError("Message not delivered - Message could not be saved");

                    Clients.User(toUser).message(manager.UserName, message, Formatting.FormatDateTime(msg.Sent));
                    Clients.User(manager.UserName).messageSent(toUser, message, Formatting.FormatDateTime(msg.Sent));

                } catch (Exception exc) {
                    string messageText = exc.Message;

                    Clients.Caller.notifyException(messageText);
                    Message msg = new Message {
                        FromUser = manager.UserId,
                        ToUser = 0,
                        Seen = false,
                        MessageText = messageText,
                    };
                    msgDP.AddItem(msg);
                }
            }
        }
    }
}
