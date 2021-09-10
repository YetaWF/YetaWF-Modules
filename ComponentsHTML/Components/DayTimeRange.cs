/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the DayTimeRange component implementation.
    /// </summary>
    public abstract class DayTimeRangeComponent : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

        internal const string TemplateName = "DayTimeRange";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays up to two time ranges within a day (typically AM and PM), such as office opening hours, optionally "Closed". The model specifies the selected daytime range.
    /// If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Category("Hours"), Caption("Mondays"), Description("Shows the default working hours")]
    /// [UIHint("DayTimeRange"), ReadOnly]
    /// </example>
    public class DayTimeRangeDisplayComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(DayTimeRange? model) {
            if (model != null) {
                string s;
                if (model.Start != null && model.End != null) {
                    if (model.Start2 != null && model.End2 != null) {
                        s = __ResStr("time2", "{0} - {1}, {2} - {3} ", Formatting.FormatTimeOfDay(model.Start), Formatting.FormatTimeOfDay(model.End), Formatting.FormatTimeOfDay(model.Start2), Formatting.FormatTimeOfDay(model.End2));
                    } else {
                        s = __ResStr("time1", "{0} - {1}", Formatting.FormatTimeOfDay(model.Start), Formatting.FormatTimeOfDay(model.End));
                    }
                } else
                    s = __ResStr("time0", "");
                return Task.FromResult($@"<div{FieldSetup(FieldType.Anonymous)} class='yt_daytimerange t_display{GetClasses()}' >{HAE(s)}</div>");
            }
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Allows entry of up to two time ranges within a day (typically AM and PM), such as office opening hours, and optional "Closed" check box. The model specifies the selected daytime range.
    /// </summary>
    /// <example>
    /// [Category("Hours"), Caption("Mondays"), Description("Please select the default working hours")]
    /// [UIHint("DayTimeRange"), Required]
    /// </example>
    public class DayTimeRangeEditComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange?> {

        internal class DayTimeRangeUI {

            public DayTimeRangeUI(DayTimeRange model) {
                Closed = true;
                Start = model.GetTimeStart();
                End = model.GetTimeEnd();
                Start2 = model.GetTimeStart2();
                End2 = model.GetTimeEnd2();
                if (model.Start != null && model.End != null) {
                    Closed = false;
                    if (model.Start2 != null && model.End2 != null)
                        Additional = true;
                }
            }

            [Caption("From"), Description("Starting time")]
            [UIHint("TimeOfDay")]
            [RequiredIf(nameof(Closed), false)]
            public TimeOfDay Start { get; set; }
            [Caption("To"), Description("Ending time")]
            [UIHint("TimeOfDay")]
            [ComponentsHTML_DayTimeRangeToValidation, RequiredIf(nameof(Closed), false)]
            public TimeOfDay End { get; set; }

            [Caption("From"), Description("Starting time")]
            [UIHint("TimeOfDay")]
            [ComponentsHTML_DayTimeRangeFrom2Validation, RequiredIf(nameof(Additional), true)]
            public TimeOfDay Start2 { get; set; }
            [Caption("To"), Description("Ending time")]
            [UIHint("TimeOfDay")]
            [ComponentsHTML_DayTimeRangeToValidation, RequiredIf(nameof(Additional), true)]
            public TimeOfDay End2 { get; set; }

            [ResourceRedirect(nameof(AdditionalFieldCaption), nameof(AdditionalFieldDescription))]
            [UIHint("Boolean")]
            public bool Additional { get; set; }

            public string? AdditionalFieldCaption { get; set; }
            public string? AdditionalFieldDescription { get; set; }

            [ResourceRedirect(nameof(ClosedFieldCaption), nameof(ClosedFieldDescription))]
            [UIHint("Boolean")]
            public bool Closed { get; set; }

            public string? ClosedFieldCaption { get; set; }
            public string? ClosedFieldDescription { get; set; }
        }

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(DayTimeRange? model) {

            HtmlBuilder hb = new HtmlBuilder();

            DayTimeRangeUI ts = new DayTimeRangeUI(model ?? new DayTimeRange());
            ts.ClosedFieldCaption = model?.ClosedFieldCaption ?? __ResStr("closed", "Closed");
            ts.ClosedFieldDescription = model?.ClosedFieldDescription ?? __ResStr("closedDesc", "Select to indicate when closed all day");
            ts.AdditionalFieldCaption = model?.AdditionalFieldCaption ?? __ResStr("additional", "Additional");
            ts.AdditionalFieldDescription = model?.AdditionalFieldDescription ?? __ResStr("additionalDesc", "Select to enable an additional time range");

            hb.Append($"<div id='{ControlId}' class='yt_daytimerange t_edit'>");

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div class='t_from'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Start))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Start))}{ValidationMessage(FieldName, nameof(DayTimeRangeUI.Start))}
</div>
<div class='t_to'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.End))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.End))}{ValidationMessage(FieldName, nameof(DayTimeRangeUI.End))}
</div>
<div class='t_from2'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.Start2))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.Start2))}{ValidationMessage(FieldName, nameof(DayTimeRangeUI.Start2))}
</div>
<div class='t_to2'>
    {await HtmlHelper.ForLabelAsync(ts, nameof(DayTimeRangeUI.End2))}
    {await HtmlHelper.ForEditAsync(ts, nameof(DayTimeRangeUI.End2))}{ValidationMessage(FieldName, nameof(DayTimeRangeUI.End2))}
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

            Manager.ScriptManager.AddLast($@"(new YetaWF_ComponentsHTML.DayTimeRangeEditComponent('{ControlId}'));");

            return hb.ToString();
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        internal class ComponentsHTML_DayTimeRangeToValidation : Attribute, YIClientValidation {

            internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public ComponentsHTML_DayTimeRangeToValidation() { }
            public ValidationBase AddValidation(object container, PropertyData propData, string caption) {
                return new ValidationBase {
                    Method = nameof(ComponentsHTML_DayTimeRangeToValidation),
                    Message = __ResStr("dtrTo", "The end time in the field labeled '{0}' must be later than the start time", caption),
                };
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        internal class ComponentsHTML_DayTimeRangeFrom2Validation : Attribute, YIClientValidation {

            internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public ComponentsHTML_DayTimeRangeFrom2Validation() { }
            public ValidationBase AddValidation(object container, PropertyData propData, string caption) {
                return new ValidationBase {
                    Method = nameof(ComponentsHTML_DayTimeRangeFrom2Validation),
                    Message = __ResStr("dtrFrom2", "The starting time in the field labeled '{0}' must be later than the start and end time of the first time range", caption),
                };
            }
        }
    }
}
