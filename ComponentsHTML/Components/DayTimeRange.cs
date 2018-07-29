/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class DayTimeRangeComponent : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

        public const string TemplateName = "DayTimeRange";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class DayTimeRangeDisplayComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(DayTimeRange model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_daytimerange");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);

                string s = null;
                if (model.Start != null && model.End != null) {
                    if (model.Start2 != null && model.End2 != null) {
                        s = __ResStr("time2", "{0} - {1}, {2} - {3} ", Formatting.FormatTime(model.Start), Formatting.FormatTime(model.End), Formatting.FormatTime(model.Start2), Formatting.FormatTime(model.End2));
                    } else {
                        s = __ResStr("time1", "{0} - {1}", Formatting.FormatTime(model.Start), Formatting.FormatTime(model.End));
                    }
                } else
                    s = __ResStr("time0", "");
                tag.SetInnerText(s);
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class DayTimeRangeEditComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange> {

        public class DayTimeRangeUI {

            public DayTimeRangeUI(DayTimeRange model) {
                Closed = true;
                if (model.Start != null && model.End != null) {
                    Closed = false;
                    if (model.Start2 != null && model.End2 != null) {
                        Start2 = (DateTime)model.Start2;
                        End2 = (DateTime)model.End2;
                        Additional = true;
                    }
                    Start = (DateTime)model.Start;
                    End = (DateTime)model.End;
                }
            }

            [Caption("From"), Description("Starting time")]
            [UIHint("Time")]
            [Required]
            public DateTime Start { get; set; }
            [Caption("To"), Description("Ending time")]
            [UIHint("Time")]
            [DayTimeRangeToValidation, Required]
            public DateTime End { get; set; }

            [Caption("From"), Description("Number of minutes")]
            [UIHint("Time")]
            [DayTimeRangeFrom2Validation, RequiredIf("Additional", true)]
            public DateTime Start2 { get; set; }
            [Caption("To"), Description("Number of seconds")]
            [UIHint("Time")]
            [DayTimeRangeToValidation, RequiredIf("Additional", true)]
            public DateTime End2 { get; set; }

            [Caption("Additional"), Description("Select to enable an additional time range - This is usually used when closed for lunch")]
            [UIHint("Boolean")]
            public bool Additional { get; set; }

            [Caption("Closed"), Description("Select to indicate when closed all day")]
            [UIHint("Boolean")]
            public bool Closed { get; set; }
        }

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(DayTimeRange model) {

            HtmlBuilder hb = new HtmlBuilder();

            DayTimeRangeUI ts = new DayTimeRangeUI(model??new DayTimeRange());

            hb.Append($"<div id='{ControlId}' class='yt_daytimerange t_edit'>");

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div class='t_from'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Start))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Start))}{ValidationMessage(nameof(DayTimeRangeUI.Start))}
</div>
<div class='t_to'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.End))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.End))}{ValidationMessage(nameof(DayTimeRangeUI.End))}
</div>
<div class='t_from2'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Start2))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Start2))}{ValidationMessage(nameof(DayTimeRangeUI.Start2))}
</div>
<div class='t_to2'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.End2))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.End2))}{ValidationMessage(nameof(DayTimeRangeUI.End2))}
</div>
<div class='t_add'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Additional))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Additional))}
</div>
<div class='t_closed'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Closed))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Closed))}
</div>
");

            }
            hb.Append($"</div>");

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"(new YetaWF_ComponentsHTML.DayTimeRangeComponent('{ControlId}'));");
            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return hb.ToYHtmlString();
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public class DayTimeRangeToValidation : Attribute, YIClientValidation {

            protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public DayTimeRangeToValidation() { }
            public void AddValidation(object container, PropertyData propData, YTagBuilder tag) {
                string msg = __ResStr("dtrTo", "The end time in the field labeled '{0}' must be later than the start time", propData.GetCaption(container));
                tag.MergeAttribute("data-val-daytimerangeto", msg);
                tag.MergeAttribute("data-val", "true");
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public class DayTimeRangeFrom2Validation : Attribute, YIClientValidation {

            protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public DayTimeRangeFrom2Validation() { }
            public void AddValidation(object container, PropertyData propData, YTagBuilder tag) {
                string msg = __ResStr("dtrFrom2", "The starting time in the field labeled '{0}' must be later than the start and end time of the first time range", propData.GetCaption(container));
                tag.MergeAttribute("data-val-daytimerangefrom2", msg);
                tag.MergeAttribute("data-val", "true");
            }
        }
    }
}
