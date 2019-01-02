/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class UrlRemotePageComponentBase : YetaWFComponent {

        public const string TemplateName = "UrlRemotePage";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class UrlRemotePageEditComponent : UrlRemotePageComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlAttributes.Add("class", "yt_urlremotepage");
            HtmlAttributes.Add("maxlength", Globals.MaxUrl);
            return await TextEditComponent.RenderTextAsync(this, model != null ? model : "", "yt_urlremotepage");
        }
    }
}
