/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the DateTime component implementation.
    /// </summary>
    public abstract class DateTimeComponentBase : YetaWFComponent {

        internal const string TemplateName = "DateTime";

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
    /// Displays the model formatted as a date and time localized using the user's selected time zone and time formatting (see User Settings Module).
    /// </summary>
    /// <remarks>All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// The model value must be specified as UTC. If the model value is null or equal to DateTime.MinValue or DateTime.MaxValue, nothing is rendered.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
    /// [UIHint("DateTime"), ReadOnly]
    /// public DateTime? FeedPublishDate { get; set; }
    /// </example>
    public class DateTimeDisplayComponent : DateTimeComponentBase, IYetaWFComponent<DateTime?> {

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
        public async Task<string> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(DateTime? model) {
            if (model != null && (DateTime)model > DateTime.MinValue && (DateTime)model < DateTime.MaxValue) {
                return Task.FromResult($@"<div{FieldSetup(FieldType.Anonymous)} class='yt_date t_display{GetClasses()}'>{HE(YetaWF.Core.Localize.Formatting.FormatDateTime(model))}</div>");
            }
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Allows entry of a date and time using local time.
    /// </summary>
    /// <remarks>
    /// All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// MinimumDateAttribute and MaximumDateAttribute can be used to define the lowest and highest allowable date/time values.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
    /// [UIHint("DateTime")]
    /// public DateTime? FeedPublishDate { get; set; }
    /// </example>
    [UsesSibling("_Setup", nameof(DateTimeSetup), "Defines setup options for the DateTime edit component")]
    public class DateTimeEditComponent : DateTimeComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        /// <summary>
        /// Setup information for DateTime edit component. Setup information is only used foer rendereing. Does not affect validation.
        /// </summary>
        public class DateTimeSetup {
            /// <summary>
            /// The oldest date than can be selected. Default is 1/1/1900.
            /// </summary>
            public DateTime MinDate { get; set; }
            /// <summary>
            /// The latest date than can be selected. Default is 12/31/2199.
            /// </summary>
            public DateTime MaxDate { get; set; }
            /// <summary>
            /// The oldest time than can be selected. Default is 0:00 AM.
            /// </summary>
            public double MinTime { get; set; }
            /// <summary>
            /// The latest time than can be selected. Default is 12:00 AM.
            /// </summary>
            public double MaxTime { get; set; }

            /// <summary>
            ///  Constructor.
            /// </summary>
            public DateTimeSetup() {
                MinDate = new DateTime(1900, 1, 1);
                MaxDate = new DateTime(2199, 12, 31);
                MinTime = new TimeSpan(0, 0, 0).TotalMinutes;
                MaxTime = new TimeSpan(23, 59, 0).TotalMinutes;
            }
        }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class Setup {
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public double MinTime { get; set; }
            public double MaxTime { get; set; }
            public Formatting.DateFormatEnum DateFormat { get; set; }
            public Formatting.TimeFormatEnum TimeFormat { get; set; }
            public List<string> WeekDays { get; set; }
            public List<string> WeekDays2 { get; set; }
            public List<string> Months { get; set; }
            public string TodayString { get; set; }
            public DateTime Today { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.datepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.timepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.datetimepicker.min.js");
            await base.IncludeAsync();
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(DateTime? model) {

            TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeSetup dateTimeSetup);

            // handle min/max date
            Setup setup = new Setup {
                MinDate = dateTimeSetup?.MinDate ?? new DateTime(1900, 1, 1),
                MaxDate = dateTimeSetup?.MaxDate ?? new DateTime(2199, 12, 31),
                MinTime = dateTimeSetup?.MinTime ?? new TimeSpan(0, 0, 0).TotalMinutes,
                MaxTime = dateTimeSetup?.MaxTime ?? new TimeSpan(23, 59, 0).TotalMinutes,
                DateFormat = UserSettings.GetProperty<Formatting.DateFormatEnum>("DateFormat"),
                TimeFormat = UserSettings.GetProperty<Formatting.TimeFormatEnum>("TimeFormat"),
                WeekDays2 = new List<string> {
                    Formatting.GetDayName2Chars(DayOfWeek.Sunday),
                    Formatting.GetDayName2Chars(DayOfWeek.Monday),
                    Formatting.GetDayName2Chars(DayOfWeek.Tuesday),
                    Formatting.GetDayName2Chars(DayOfWeek.Wednesday),
                    Formatting.GetDayName2Chars(DayOfWeek.Thursday),
                    Formatting.GetDayName2Chars(DayOfWeek.Friday),
                    Formatting.GetDayName2Chars(DayOfWeek.Saturday),
                },
                WeekDays = new List<string> {
                    Formatting.GetDayName(DayOfWeek.Sunday),
                    Formatting.GetDayName(DayOfWeek.Monday),
                    Formatting.GetDayName(DayOfWeek.Tuesday),
                    Formatting.GetDayName(DayOfWeek.Wednesday),
                    Formatting.GetDayName(DayOfWeek.Thursday),
                    Formatting.GetDayName(DayOfWeek.Friday),
                    Formatting.GetDayName(DayOfWeek.Saturday),
                },
                Months = new List<string> {
                    Formatting.GetMonthName(1),
                    Formatting.GetMonthName(2),
                    Formatting.GetMonthName(3),
                    Formatting.GetMonthName(4),
                    Formatting.GetMonthName(5),
                    Formatting.GetMonthName(6),
                    Formatting.GetMonthName(7),
                    Formatting.GetMonthName(8),
                    Formatting.GetMonthName(9),
                    Formatting.GetMonthName(10),
                    Formatting.GetMonthName(11),
                    Formatting.GetMonthName(12),
                },
                TodayString = Formatting.FormatLongDate(DateTime.UtcNow),
                Today = DateTime.UtcNow,
            };
            // attributes (like MinimumDateAttribute) override setup and defaults
            MinimumDateAttribute minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
            if (minAttr != null)
                setup.MinDate = minAttr.MinDate;
            MaximumDateAttribute maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
            if (maxAttr != null)
                setup.MaxDate = maxAttr.MaxDate;

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{DivId}' class='yt_datetime t_edit'>
    <input type='hidden'{FieldSetup(FieldType.Validated)} value='{HAE($"{model:o}")}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{HAE(Formatting.FormatDateTime((DateTime)model))}'>
    <div class='t_sels'>
        <div class='t_date'>
            {SkinSVGs.Get(AreaRegistration.CurrentPackage, "far-calendar-alt")}
        </div>
        <div class='t_time'>
            {SkinSVGs.Get(AreaRegistration.CurrentPackage, "far-clock")}
        </div>
    </div>
</div>");

            string tags = hb.ToString();

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DateTimeEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }
}
