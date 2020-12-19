/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Messenger.Modules;

namespace YetaWF.Modules.Messenger.Views {

    public class SiteAnnouncementsUnavailableView : YetaWFView, IYetaWFView<BrowseSiteAnnouncementModule, object> {

        public const string ViewName = "SiteAnnouncementsUnavailable";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(BrowseSiteAnnouncementModule module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='{Globals.CssDivWarning}'>
    {this.__ResStr("unavailable", "Site announcements log is not available - See <a href='https://yetawf.com/Documentation/YetaWF/Messenger' target='_blank' rel='noopener noreferrer'>https://yetawf.com/Documentation/YetaWF/Messenger</a> for additional information.")}
</div>");

            return Task.FromResult(hb.ToString());
        }
    }
}
