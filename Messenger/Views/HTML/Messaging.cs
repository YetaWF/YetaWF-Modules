/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Messenger.Controllers;
using YetaWF.Modules.Messenger.Modules;

namespace YetaWF.Modules.Messenger.Views {

    public class MessagingView : YetaWFView, IYetaWFView2<MessagingModule, MessagingModuleController.EditModel> {

        public const string ViewName = "Messaging";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(MessagingModule module, MessagingModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            string idForm = UniqueId();
            string idSend = UniqueId();
            string idCancel = UniqueId();

            if (!Manager.AddOnManager.HasModuleReference(ModuleDefinition.GetPermanentGuid(typeof(SkinMessagingModule)))) {

                hb.Append($@"
<div class='yDivAlert'>
    {HE(this.__ResStr("noMsg", "Messaging services are not available(add a reference to the \"Messaging (Skin)\" module in Site Settings)."))}
</div>");

            } else {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { Text=this.__ResStr("btnSend", "Send"), Name="Send", ButtonType= ButtonTypeEnum.Button, },
        new FormButton() { Text=this.__ResStr("btnClose", "Close"), Name="Cancel", ButtonType= ButtonTypeEnum.Button, },
    })}
{await RenderEndFormAsync()}
<script>
    var mod = new YetaWF_Messenger.MessagingModule('{idForm}', '{idSend}', '{idCancel}', '{JE(model.OnlineImage)}', '{JE(model.OfflineImage)}');
    YetaWF_Basics.addObjectDataById(YConfigs.Forms.CssFormAjax, '@idForm', mod);
</script>");
            }
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(MessagingModule module, MessagingModuleController.EditModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));

            if (Manager.IsPostRequest) {
                hb.Append($@"
<script>
    var mod = YetaWF_Basics.getObjectDataById($('form')[0].id);
    mod.updateOnlineStatus();
</script>
");
            }

            return hb.ToYHtmlString();
        }
    }
}
