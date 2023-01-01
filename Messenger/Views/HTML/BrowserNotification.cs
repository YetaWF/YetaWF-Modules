/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

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

    public class BrowserNotificationsView : YetaWFView, IYetaWFView2<BrowserNotificationsModule, BrowserNotificationsModuleController.Model> {

        public const string ViewName = "BrowserNotifications";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(BrowserNotificationsModule module, BrowserNotificationsModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            string idForm = UniqueId();
            string idSend = UniqueId();
            string idCancel = UniqueId();

            if (!Manager.AddOnManager.HasModuleReference(ModuleDefinition.GetPermanentGuid(typeof(SkinBrowserNotificationsModule)))) {

                hb.Append($@"
<div class='yDivAlert'>
    {HE(this.__ResStr("noMsg", "Messaging services are not available (add a reference to the \"Web Browser Notifications (Skin)\" module in Site Settings)."))}
</div>");

            } else {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { Text=this.__ResStr("btnSend", "Send"), ButtonType= ButtonTypeEnum.Apply, },
        new FormButton() { Text=this.__ResStr("btnSendClose", "Send & Close"), ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            }
            return hb.ToString();
        }

        public async Task<string> RenderPartialViewAsync(BrowserNotificationsModule module, BrowserNotificationsModuleController.Model model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();
        }
    }
}
