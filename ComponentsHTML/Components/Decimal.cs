/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

    //public class DecimalDisplayComponent : DecimalComponent, IYetaWFComponent<Decimal?> {

    //    public override ComponentType GetComponentType() { return ComponentType.Display; }

    //    public async Task<YHtmlString> RenderAsync(Decimal model) {
    //        return await RenderAsync((Decimal?)model);
    //    }
    //    public Task<YHtmlString> RenderAsync(Decimal? model) {
    //        HtmlBuilder hb = new HtmlBuilder();
    //        if (model != null && (Decimal)model > Decimal.MinValue && (Decimal)model < Decimal.MaxValue) {
    //            YTagBuilder tag = new YTagBuilder("div");
    //            tag.AddCssClass("yt_decimal");
    //            tag.AddCssClass("t_display");
    //            FieldSetup(tag, FieldType.Anonymous);
    //            tag.SetInnerText(YetaWF.Core.Localize.Formatting.FormatDecimal(model));
    //            hb.Append(tag.ToString(YTagRenderMode.Normal));
    //        }
    //        return Task.FromResult(hb.ToYHtmlString());
    //    }
    //}
    public class DecimalEditComponent : DecimalComponent, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.userevents.min.js");
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.numerictextbox.min.js");
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

            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                tag.MergeAttribute("data-min", ((double)rangeAttr.Minimum).ToString("0.000"));
                tag.MergeAttribute("data-min", ((double)rangeAttr.Maximum).ToString("0.000"));
            }
            if (model != null)
                tag.MergeAttribute("value", ((decimal)model).ToString("0.00"));

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.SelfClosing));
        }
    }
}
