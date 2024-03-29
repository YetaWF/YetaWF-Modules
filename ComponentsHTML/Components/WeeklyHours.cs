/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the WeeklyHours component implementation.
    /// </summary>
    public abstract class WeeklyHoursComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(WeeklyHoursComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "WeeklyHours";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
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
    /// Allows entry of time ranges for an entire week, two time ranges for each day (typically AM and PM), such as office opening hours, optionally "Closed". The model specifies the selected weekly daytime ranges.
    /// </summary>
    /// <example>
    /// [Category("Hours"), Caption(""), Description("")]
    /// [UIHint("WeeklyHours")]
    /// public WeeklyHours OpeningHours { get; set; }
    /// </example>
    public class WeeklyHoursEditComponent : WeeklyHoursComponentBase, IYetaWFComponent<WeeklyHours?> {

        internal class WeeklyHoursUI {

            public WeeklyHoursUI(WeeklyHours model) {

                Week = model;

                Mondays.ClosedFieldCaption = model.ClosedFieldCaption;
                Mondays.ClosedFieldDescription = model.ClosedFieldDescription;
                Mondays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Mondays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Tuesdays.ClosedFieldCaption = model.ClosedFieldCaption;
                Tuesdays.ClosedFieldDescription = model.ClosedFieldDescription;
                Tuesdays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Tuesdays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Wednesdays.ClosedFieldCaption = model.ClosedFieldCaption;
                Wednesdays.ClosedFieldDescription = model.ClosedFieldDescription;
                Wednesdays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Wednesdays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Thursdays.ClosedFieldCaption = model.ClosedFieldCaption;
                Thursdays.ClosedFieldDescription = model.ClosedFieldDescription;
                Thursdays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Thursdays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Fridays.ClosedFieldCaption = model.ClosedFieldCaption;
                Fridays.ClosedFieldDescription = model.ClosedFieldDescription;
                Fridays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Fridays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Saturdays.ClosedFieldCaption = model.ClosedFieldCaption;
                Saturdays.ClosedFieldDescription = model.ClosedFieldDescription;
                Saturdays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Saturdays.AdditionalFieldDescription = model.AdditionalFieldDescription;
                Sundays.ClosedFieldCaption = model.ClosedFieldCaption;
                Sundays.ClosedFieldDescription = model.ClosedFieldDescription;
                Sundays.AdditionalFieldCaption = model.AdditionalFieldCaption;
                Sundays.AdditionalFieldDescription = model.AdditionalFieldDescription;
            }

            [Caption("Mondays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Mondays { get { return Week.Days[(int)DayOfWeek.Monday]; } set { Week.Days[(int)DayOfWeek.Monday] = value; } }
            [Caption("Tuesdays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Tuesdays { get { return Week.Days[(int)DayOfWeek.Tuesday]; } set { Week.Days[(int)DayOfWeek.Tuesday] = value; } }
            [Caption("Wednesdays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Wednesdays { get { return Week.Days[(int)DayOfWeek.Wednesday]; } set { Week.Days[(int)DayOfWeek.Wednesday] = value; } }
            [Caption("Thursdays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Thursdays { get { return Week.Days[(int)DayOfWeek.Thursday]; } set { Week.Days[(int)DayOfWeek.Thursday] = value; } }
            [Caption("Fridays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Fridays { get { return Week.Days[(int)DayOfWeek.Friday]; } set { Week.Days[(int)DayOfWeek.Friday] = value; } }
            [Caption("Saturdays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Saturdays { get { return Week.Days[(int)DayOfWeek.Saturday]; } set { Week.Days[(int)DayOfWeek.Saturday] = value; } }
            [Caption("Sundays"), Description("")]
            [UIHint("DayTimeRange"), Required]
            public DayTimeRange Sundays { get { return Week.Days[(int)DayOfWeek.Sunday]; } set { Week.Days[(int)DayOfWeek.Sunday] = value; } }

            public WeeklyHours Week { get; set; }

            public string? AdditionalFieldCaption { get { return Week.AdditionalFieldCaption; } }
            public string? AdditionalFieldDescription { get { return Week.AdditionalFieldDescription; } }

            public string? ClosedFieldCaption { get { return Week.ClosedFieldCaption; } }
            public string? ClosedFieldDescription { get { return Week.ClosedFieldDescription; } }
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
        public async Task<string> RenderAsync(WeeklyHours? model) {

            HtmlBuilder hb = new HtmlBuilder();

            WeeklyHoursUI ts = new WeeklyHoursUI(model??new WeeklyHours());

            hb.Append($@"
<div id='{ControlId}' class='yt_weeklyhours t_edit'>");

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
    <div class='t_weekday t_monday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Mondays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Mondays))}{ValidationMessage(nameof(WeeklyHoursUI.Mondays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_tuesday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Tuesdays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Tuesdays))}{ValidationMessage(nameof(WeeklyHoursUI.Tuesdays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_wednesday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Wednesdays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Wednesdays))}{ValidationMessage(nameof(WeeklyHoursUI.Wednesdays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_thursday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Thursdays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Thursdays))}{ValidationMessage(nameof(WeeklyHoursUI.Thursdays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_friday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Fridays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Fridays))}{ValidationMessage(nameof(WeeklyHoursUI.Fridays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_saturday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Saturdays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Saturdays))}{ValidationMessage(nameof(WeeklyHoursUI.Saturdays))}
    </div>");
                hb.Append($@"
    <div class='t_weekday t_sunday'>
        {await HtmlHelper.ForLabelAsync(ts, nameof(WeeklyHoursUI.Sundays))}
        {await HtmlHelper.ForEditAsync(ts, nameof(WeeklyHoursUI.Sundays))}{ValidationMessage(nameof(WeeklyHoursUI.Sundays))}
    </div>");

            }
            hb.Append($@"
</div>");

            return hb.ToString();
        }
    }
}
