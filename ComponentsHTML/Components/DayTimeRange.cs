/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Implementation of the TimeRange display component.
    /// </summary>
    public class DayTimeRangeDisplayComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(DayTimeRange model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_daytimerange");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);

                string s = null;
                if (model.Start != null && model.End != null) {
                    if (model.Start2 != null && model.End2 != null) {
                        s = __ResStr("time2", "{0} - {1}, {2} - {3} ", Formatting.FormatTime(model.GetStart()), Formatting.FormatTime(model.GetEnd()), Formatting.FormatTime(model.GetStart2()), Formatting.FormatTime(model.GetEnd2()));
                    } else {
                        s = __ResStr("time1", "{0} - {1}", Formatting.FormatTime(model.GetStart()), Formatting.FormatTime(model.GetEnd()));
                    }
                } else
                    s = __ResStr("time0", "");
                tag.SetInnerText(s);
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Implementation of the TimeRange edit component.
    /// </summary>
    public class DayTimeRangeEditComponent : DayTimeRangeComponent, IYetaWFComponent<DayTimeRange> {

        internal class DayTimeRangeUI {

            public DayTimeRangeUI(DayTimeRange model) {
                Closed = true;
                if (model.Start != null && model.End != null) {
                    Closed = false;
                    if (model.Start2 != null && model.End2 != null) {
                        Start2 = model.GetStart2();
                        End2 = model.GetEnd2();
                        Additional = true;
                    }
                    Start = model.GetStart();
                    End = model.GetEnd();
                }
            }

            [Caption("From"), Description("Starting time")]
            [UIHint("Time")]
            [RequiredIf(nameof(Closed), false)]
            public DateTime Start { get; set; }
            [Caption("To"), Description("Ending time")]
            [UIHint("Time")]
            [DayTimeRangeToValidation, RequiredIf(nameof(Closed), false)]
            public DateTime End { get; set; }

            [Caption("From"), Description("Starting time")]
            [UIHint("Time")]
            [DayTimeRangeFrom2Validation, RequiredIf(nameof(Additional), true)]
            public DateTime Start2 { get; set; }
            [Caption("To"), Description("Ending time")]
            [UIHint("Time")]
            [DayTimeRangeToValidation, RequiredIf(nameof(Additional), true)]
            public DateTime End2 { get; set; }

            [ResourceRedirect(nameof(AdditionalFieldCaption), nameof(AdditionalFieldDescription))]
            [UIHint("Boolean")]
            public bool Additional { get; set; }

            public string AdditionalFieldCaption { get; set; }
            public string AdditionalFieldDescription { get; set; }

            [ResourceRedirect(nameof(ClosedFieldCaption), nameof(ClosedFieldDescription))]
            [UIHint("Boolean")]
            public bool Closed { get; set; }

            public string ClosedFieldCaption { get; set; }
            public string ClosedFieldDescription { get; set; }
        }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(DayTimeRange model) {

            HtmlBuilder hb = new HtmlBuilder();

            DayTimeRangeUI ts = new DayTimeRangeUI(model??new DayTimeRange());
            ts.ClosedFieldCaption = model.ClosedFieldCaption ?? __ResStr("closed", "Closed");
            ts.ClosedFieldDescription = model.ClosedFieldDescription ?? __ResStr("closedDesc", "Select to indicate when closed all day");
            ts.AdditionalFieldCaption = model.AdditionalFieldCaption ?? __ResStr("additional", "Additional");
            ts.AdditionalFieldDescription = model.AdditionalFieldDescription ?? __ResStr("additionalDesc", "Select to enable an additional time range");

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
        internal class DayTimeRangeToValidation : Attribute, YIClientValidation {

            internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public DayTimeRangeToValidation() { }
            public ValidationBase AddValidation(object container, PropertyData propData, string caption, YTagBuilder tag) {
                return new ValidationBase {
                    Method = nameof(DayTimeRangeToValidation),
                    Message = __ResStr("dtrTo", "The end time in the field labeled '{0}' must be later than the start time", caption),
                };
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        internal class DayTimeRangeFrom2Validation : Attribute, YIClientValidation {

            internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayTimeRangeComponent), name, defaultValue, parms); }

            public DayTimeRangeFrom2Validation() { }
            public ValidationBase AddValidation(object container, PropertyData propData, string caption, YTagBuilder tag) {
                return new ValidationBase {
                    Method = nameof(DayTimeRangeFrom2Validation),
                    Message = __ResStr("dtrFrom2", "The starting time in the field labeled '{0}' must be later than the start and end time of the first time range", caption),
                };
            }
        }
    }
}
