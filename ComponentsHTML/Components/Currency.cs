/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class CurrencyComponentBase : YetaWFComponent {

        public const string TemplateName = "Currency";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class CurrencyDisplayComponent : CurrencyComponentBase, IYetaWFComponent<decimal?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(decimal model) {
            return await RenderAsync((decimal?)model);
        }
        public Task<YHtmlString> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(HE(Formatting.FormatAmount((decimal)model)));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class CurrencyEditComponent : CurrencyComponentBase, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.userevents.min.js");
            await KendoUICore.AddFileAsync("kendo.numerictextbox.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?) model);
        }
        public Task<YHtmlString> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_currency");
            tag.AddCssClass("t_edit");

            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);

            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                tag.MergeAttribute("data-min", ((double)rangeAttr.Minimum).ToString("F3"));
                tag.MergeAttribute("data-max", ((double)rangeAttr.Maximum).ToString("F3"));
            }
            if (model != null)
                tag.MergeAttribute("value", Formatting.FormatAmount((decimal)model));

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
}
