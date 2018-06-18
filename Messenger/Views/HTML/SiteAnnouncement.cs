﻿using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Messenger.Controllers;
using YetaWF.Modules.Messenger.Modules;

namespace YetaWF.Modules.Messenger.Views {

    public class SiteAnnouncementView : YetaWFView, IYetaWFView2<SiteAnnouncementModule, SiteAnnouncementModuleController.AddModel> {

        public const string ViewName = "SiteAnnouncement";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<YHtmlString> RenderViewAsync(SiteAnnouncementModule module, SiteAnnouncementModuleController.AddModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            string idForm = UniqueId();
            string idSend = UniqueId();
            string idCancel = UniqueId();

            if (!Manager.AddOnManager.HasModuleReference(ModuleDefinition.GetPermanentGuid(typeof(SkinSiteAnnouncementsModule)))) {

                hb.Append($@"
<div class='yDivAlert'>
    {HE(this.__ResStr("noMsg", "Messaging services are not available (add a reference to the \"Site Announcements(Skin)\" module in Site Settings)."))}
</div>");

            } else {

                hb.Append($@"
{await RenderBeginFormAsync()}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { Text=this.__ResStr("btnSend", "Send"), ButtonType= ButtonTypeEnum.Submit, },
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, },
    })}
{await RenderEndFormAsync()}");

            }
            return hb.ToYHtmlString();
        }

        public async Task<YHtmlString> RenderPartialViewAsync(SiteAnnouncementModule module, SiteAnnouncementModuleController.AddModel model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToYHtmlString();
        }
    }
}
