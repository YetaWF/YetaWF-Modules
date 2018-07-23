/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class Password20Component : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "Password20";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlAttributes.Add("type", "password");
            HtmlAttributes.Add("autocomplete", "off");
            return await TextEditComponent.RenderTextAsync(this, model, "yt_password20");
        }
    }
}
