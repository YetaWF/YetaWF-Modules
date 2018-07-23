/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TimeSpanComponent : YetaWFComponent {

        public const string TemplateName = "TimeSpan";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class TimeSpanDisplayComponent : TimeSpanComponent, IYetaWFComponent<TimeSpan?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(TimeSpan model) {
            return await RenderAsync((TimeSpan?)model);
        }
        public Task<YHtmlString> RenderAsync(TimeSpan? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_timespan");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                tag.SetInnerText(Formatting.FormatTimeSpan(model));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class TimeSpanEditComponent : TimeSpanComponent, IYetaWFComponent<TimeSpan>, IYetaWFComponent<TimeSpan?> {

        public class TimeSpanUI {
            public TimeSpanUI() { }
            public TimeSpanUI(TimeSpan ts) { Span = ts; }

            private TimeSpan Span = new TimeSpan();

            [Caption("Days"), Description("Number of days")]
            [UIHint("IntValue4"), Required, Range(0, 999999)]
            public int Days { get { return Span.Days; } set { } }
            [Caption("Hours"), Description("Number of hours")]
            [UIHint("IntValue2"), Required, Range(0, 23)]
            public int Hours { get { return Span.Hours; } set { } }
            [Caption("Minutes"), Description("Number of minutes")]
            [UIHint("IntValue2"), Required, Range(0, 59)]
            public int Minutes { get { return Span.Minutes; } set { } }
            [Caption("Seconds"), Description("Number of seconds")]
            [UIHint("IntValue2"), Required, Range(0, 59)]
            public int Seconds { get { return Span.Seconds; } set { } }
        }

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(TimeSpan model) {
            return await RenderAsync((TimeSpan?) model);
        }
        public async Task<YHtmlString> RenderAsync(TimeSpan? model) {
            HtmlBuilder hb = new HtmlBuilder();

            TimeSpanUI ts = new TimeSpanUI(model??new TimeSpan());

            hb.Append($"<div id='{ControlId}' class='yt_timespan t_edit'>");

            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model.ToString(), "Hidden", HtmlAttributes: HtmlAttributes, Validation: Validation));

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div class='t_days'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Days))}
    {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Days))}{ValidationMessage(nameof(TimeSpanUI.Days))}
</div>
<div class='t_hours'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Hours))}
    {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Hours))}{ValidationMessage(nameof(TimeSpanUI.Hours))}
</div>
<div class='t_minutes'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Minutes))}
    {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Minutes))}{ValidationMessage(nameof(TimeSpanUI.Minutes))}
</div>
<div class='t_seconds'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Seconds))}
    {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Seconds))}{ValidationMessage(nameof(TimeSpanUI.Seconds))}
</div>");

            }
            hb.Append($"</div>");

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"YetaWF_TimeSpan.init('{ControlId}');");

            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return hb.ToYHtmlString();
        }
    }
}
