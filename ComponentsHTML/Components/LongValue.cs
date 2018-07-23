/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class LongValueComponent : YetaWFComponent {

        public const string TemplateName = "LongValue";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class LongValueDisplayComponent : LongValueComponent, IYetaWFComponent<long>, IYetaWFComponent<long?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(long model) {
            return await RenderAsync((long?)model);
        }
        public Task<YHtmlString> RenderAsync(long? model) {
            HtmlBuilder hb = new HtmlBuilder();
            model = model ?? 0;
            hb.Append($@"<div class='yt_longvalue t_display'>{HE(model.ToString())}</div>");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class LongValueEditComponent : LongValueComponent, IYetaWFComponent<long>, IYetaWFComponent<long?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(long model) {
            return await RenderAsync((long?) model);
        }
        public async Task<YHtmlString> RenderAsync(long? model) {
            model = model ?? 0;
            HtmlAttributes.Add("class", "yt_text20");
            HtmlAttributes.Add("maxlength", "30");
            return await TextEditComponent.RenderTextAsync(this, model.ToString(), "yt_longvalue");
        }
    }
}
