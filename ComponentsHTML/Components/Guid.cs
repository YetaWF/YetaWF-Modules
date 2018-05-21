using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class GuidComponent : YetaWFComponent {

        public const string TemplateName = "Guid";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class GuidDisplayComponent : GuidComponent, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        public Task<YHtmlString> RenderAsync(Guid? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && model != Guid.Empty) {
                hb.Append(((Guid)model).ToString());
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class GuidEditComponent : GuidComponent, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(Guid model) {
            return await RenderAsync((Guid?) model);
        }
        public async Task<YHtmlString> RenderAsync(Guid? model) {
            HtmlAttributes.Add("class", "yt_text40");
            HtmlAttributes.Add("maxlength", "40");
            return await TextEditComponent.RenderTextAsync(model != null ? model.ToString() : "", this, "yt_guid");
        }
    }
}
