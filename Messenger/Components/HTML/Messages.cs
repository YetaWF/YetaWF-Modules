using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Messenger.DataProvider;

namespace YetaWF.Modules.Messenger.Components {

    public abstract class MessagesComponentBase : YetaWFComponent {

        public const string TemplateName = "Messages";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class MessagesEditComponent : MessagesComponentBase, IYetaWFComponent<MessagesEditComponent.MessageData> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class MessageData {
            public List<Message> Messages { get; set; }
            public string FromUser { get; set; }
            public string ToUser { get; set; }
            public int TotalMessages { get; set; }

            public MessageData() {
                Messages = new List<Message>();
            }
        }

        public async Task<YHtmlString> RenderAsync(MessagesEditComponent.MessageData model) {
            HtmlBuilder hb = new HtmlBuilder();

            string notSeen = YetaWFManager.HtmlAttributeEncode(this.__ResStr("seen", "This message has not been seen"));
            SkinImages skinImages = new SkinImages();
            string notSeenUrl = YetaWFManager.HtmlAttributeEncode(await skinImages.FindIcon_PackageAsync("NotSeen.png", Package));


            hb.Append($@"
<div class='yt_messenger_messages t_display' id='{DivId}' style='display:none'>
    <div class='t_messagearea'>");

            foreach (Message msg in model.Messages) {

                if (msg.FromUser == Manager.UserId) {

                    hb.Append($@"
        <div class='t_thisuser  t_notseen' data-key='{msg.Key}'>
            <div class='t_sent'>
                <img alt='{notSeen}' title='{notSeen}' src='{notSeenUrl}'>
                {YetaWFManager.HtmlEncode(Formatting.FormatDateTime(msg.Sent))}
            </div>
            <div class='t_text'>{YetaWFManager.HtmlEncode(msg.MessageText)}</div>
        </div>");

                } else {

                    hb.Append($@"
        <div class='t_otheruser {(msg.Seen ? "t_seen" : "t_notseen")}' data-key='{msg.Key}'>
            <div class='t_sent'>{YetaWFManager.HtmlEncode(Formatting.FormatDateTime(msg.Sent))}</div>
            <div class='t_text'>{YetaWFManager.HtmlEncode(msg.MessageText)}</div>
        </div>");
                }
            }
            hb.Append($@"
        <div class='t_last' style='clear:both'></div>
    </div>
</div>
<div class='yt_messenger_messages t_display' id='{DivId}_none' style='display:none'>
    {YetaWFManager.HtmlEncode(this.__ResStr("none", "(None)"))}
</div> 
<script>
    var out = document.getElementById('{DivId}');
    out.style.display = '{(model.Messages.Count == 0 ? "none": "")}';
    out.scrollTop = out.scrollHeight - out.clientHeight;
    out = document.getElementById('{DivId}_none');
    out.style.display = '{(model.Messages.Count == 0 ? "": "none")}';
    new YetaWF_Messenger.MessagesTemplate('{DivId}', '{YetaWFManager.JserEncode(model.FromUser)}', '{YetaWFManager.JserEncode(model.ToUser)}');
</script>
");
            return hb.ToYHtmlString();
        }
    }
}
