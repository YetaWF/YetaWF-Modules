/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Core.Identity;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Modules.Messenger.Views.Shared;
using YetaWF.Core.Packages;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Controllers {

    public class MessagingModuleController : ControllerImpl<YetaWF.Modules.Messenger.Modules.MessagingModule> {

        public MessagingModuleController() { }

        [Trim]
        public class EditModel {

            [Caption("To User"), Description("Defines the current user for whom the history is shown")]
            [UIHint("YetaWF_Identity_UserId"), SelectionRequired, SubmitFormOnChange(SubmitFormOnChangeAttribute.SubmitTypeEnum.Apply)]
            public int ToUserId { get; set; }

            [Caption("Message History"), Description("The history of messages sent to the selected user")]
            [UIHint("YetaWF_Messenger_Messages"), ReadOnly]
            public MessageData MessageData { get; set; }

            [UIHint("Hidden")]
            public int MaxMessages { get; set; }

            [Caption("New Message"), Description("Defines the message to send to the user")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), AdditionalMetadata("EmHeight", 3), StringLength(Message.MaxMessageText)]
            public string MessageText { get; set; }

            public string OnlineImage { get; set; }
            public string OfflineImage { get; set; }

            public EditModel() {
                MessageData = new MessageData();
                MaxMessages = 10;
            }
        }

        [AllowGet]
        public ActionResult Messaging() {
            Signalr.Use();
            Package currentPackage = AreaRegistration.CurrentPackage;
            Manager.AddOnManager.AddAddOnNamed(currentPackage.Domain, currentPackage.Product, "Messaging");
            EditModel model = new EditModel { };
            return View(model);
        }
        private async Task<string> UpdateMessage(EditModel model, int toUserId, bool NeedUser = false) {
            if (!NeedUser && toUserId == 0) return null;
            using (MessagingDataProvider msgDP = new MessagingDataProvider()) {

                string toUser = await Resource.ResourceAccess.GetUserNameAsync(model.ToUserId);
                if (string.IsNullOrWhiteSpace(toUser)) throw new Error(this.__ResStr("noUser", "User id {0} doesn't exist", model.ToUserId));

                List<DataProviderFilterInfo> filters = null;
                List<DataProviderFilterInfo> subFilters = null;
                subFilters = DataProviderFilterInfo.Join(subFilters, new DataProviderFilterInfo { Field = "FromUser", Operator = "==", Value = Manager.UserId });
                subFilters = DataProviderFilterInfo.Join(subFilters, new DataProviderFilterInfo { Field = "ToUser", Operator = "==", Value = toUserId });
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Logic = "||", Filters = subFilters });
                subFilters = null;
                subFilters = DataProviderFilterInfo.Join(subFilters, new DataProviderFilterInfo { Field = "ToUser", Operator = "==", Value = Manager.UserId });
                subFilters = DataProviderFilterInfo.Join(subFilters, new DataProviderFilterInfo { Field = "FromUser", Operator = "==", Value = toUserId });
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Logic = "||", Filters = subFilters });
                List<DataProviderSortInfo> sorts = null;
                sorts = DataProviderSortInfo.Join(sorts, new DataProviderSortInfo { Field = "Sent", Order = DataProviderSortInfo.SortDirection.Descending });
                int total;
                model.MessageData.Messages = msgDP.GetItems(0, model.MaxMessages, sorts, filters, out total);
                model.MessageData.Messages.Reverse();
                model.MessageData.FromUser = Manager.UserName;
                model.MessageData.ToUser = toUser;
                model.MessageData.TotalMessages = total;
                return toUser;
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> Messaging_Partial(EditModel model) {

            Manager.NeedUser();
            string toUser = await UpdateMessage(model, model.ToUserId);

            if (!ModelState.IsValid)
                return PartialView(model);

            //if (Manager.RequestForm[Globals.Link_SubmitIsApply] != null)
            return PartialView(model);

            //DateTime msgTime;
            //using (MessagingDataProvider msgDP = new MessagingDataProvider()) {
            //    Message msg = new Message {
            //        FromUser = Manager.UserId,
            //        ToUser = model.ToUserId,
            //        Seen = false,
            //        MessageText = model.MessageText,
            //    };
            //    if (!msgDP.AddItem(msg)) throw new InternalError("Message not delivered - Message could not be saved");
            //    msgTime = msg.Sent;
            //}

            //IHubContext context = GlobalHost.ConnectionManager.GetHubContext<YetaWF_Messenger_Messaging>();
            //context.Clients.User(toUser).message(Manager.UserName, model.MessageText, Formatting.FormatDateTime(msgTime));

            //model.MessageText = null;

            //return FormProcessed(model, this.__ResStr("okSaved", "Message sent"), OnPopupClose: OnPopupCloseEnum.UpdateInPlace, OnClose: OnCloseEnum.UpdateInPlace);
        }
    }
}
