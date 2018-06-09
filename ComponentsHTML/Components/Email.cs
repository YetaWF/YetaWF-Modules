using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class EmailComponent : YetaWFComponent {

        public const string TemplateName = "Email";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class EmailDisplayComponent : EmailComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (!string.IsNullOrWhiteSpace(model))
                hb.Append($@"<div class='yt_email t_display'><a href='mailto: {YetaWFManager.HtmlAttributeEncode(model)}'>{YetaWFManager.HtmlEncode(model)}</a></div>");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class EmailEditComponent : EmailComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlAttributes.Add("class", "yt_text40");
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr == null)
                HtmlAttributes.Add("maxlength", "40");
            return await TextEditComponent.RenderTextAsync(this, model != null ? model.ToString() : "", "yt_email");
        }
    }
}
