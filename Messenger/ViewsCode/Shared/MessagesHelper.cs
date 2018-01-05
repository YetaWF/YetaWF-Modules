/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Collections.Generic;
using YetaWF.Core.Pages;
using YetaWF.Modules.Messenger.DataProvider;
#if MVC6
#else
#endif

namespace YetaWF.Modules.Messenger.Views.Shared {

    public class MessagesHelper<TModel> : RazorTemplate<TModel> { }

    public class MessageData {
        public List<Message> Messages { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public int TotalMessages { get; set; }

        public MessageData() {
            Messages = new List<Message>();
        }
    }

}
