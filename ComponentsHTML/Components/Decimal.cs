/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class DecimalComponent : YetaWFComponent {

        public const string TemplateName = "Decimal";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class DecimalDisplayComponent : DecimalComponent, IYetaWFComponent<Decimal?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?)model);
        }
        public Task<YHtmlString> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && (Decimal)model > Decimal.MinValue && (Decimal)model < Decimal.MaxValue) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_decimal");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                string format = PropData.GetAdditionalAttributeValue("Format", "0.00");
                if (model != null)
                    tag.SetInnerText(((decimal)model).ToString(format));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }

    public class DecimalEditComponent : DecimalComponent, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

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
            tag.AddCssClass("yt_decimal");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            string id = MakeId(tag);

            // handle min/max
            float min = 0, max = 99999999.99F;
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                min = Convert.ToSingle(rangeAttr.Minimum);
                max = Convert.ToSingle(rangeAttr.Maximum);
            }
            string format = PropData.GetAdditionalAttributeValue("Format", "0.00");
            if (model != null)
                tag.MergeAttribute("value", ((decimal)model).ToString(format));

            hb.Append($@"
{tag.ToString(YTagRenderMode.StartTag)}
<script>
new YetaWF_ComponentsHTML.DecimalEditComponent('{id}', {{ Min: {min}, Max: {max} }});
</script>
");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
