/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
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

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        internal class Setup {

            public enum DateTimeStyleEnum {
                DateTime = 0,
                Date = 1,
                Time = 2,
            }

            public DateTimeStyleEnum Style { get; set; }
            public string InitialCalendarDate { get; set; } = null!;
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public double MinTime { get; set; }
            public double MaxTime { get; set; }
            public string TodayString { get; set; }
            public string Today { get; set; }
            /// <summary>
            /// If a date is entered, it is normalized as UTC midnight, without time component. Otherwise, the time component is left unmodified.
            /// Applies to the Date component only.
            /// </summary>
            public bool UtcMidnight { get; set; }

            public Setup() {
                Style = DateTimeStyleEnum.DateTime;
                MinDate = new DateTime(1900, 1, 1);
                MaxDate = new DateTime(2199, 12, 31);
                MinTime = new TimeSpan(0, 0, 0).TotalMinutes;
                MaxTime = new TimeSpan(23, 59, 0).TotalMinutes;
                TodayString = Formatting.FormatLongDate(DateTime.UtcNow);
                Today = $"{DateTime.UtcNow:o}";
            }
        }

        /// <inheritdoc/>
        public async Task<string> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?)model);
        }
        /// <inheritdoc/>
        public Task<string> RenderAsync(DateTime? model) {

            Setup setup = new Setup {
                Style = Setup.DateTimeStyleEnum.DateTime,
            };
            if (TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeSetup? dateTimeSetup) && dateTimeSetup != null) {
                setup.MinDate = dateTimeSetup.MinDate;
                setup.MaxDate = dateTimeSetup.MaxDate;
                setup.MinTime = dateTimeSetup.MinTime;
                setup.MaxTime = dateTimeSetup.MaxTime;
            }

            // handle min/max date
            // attributes (like MinimumDateAttribute) override setup and defaults
            MinimumDateAttribute? minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
            if (minAttr != null)
                setup.MinDate = minAttr.MinDate;
            MaximumDateAttribute? maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
            if (maxAttr != null)
                setup.MaxDate = maxAttr.MaxDate;

            // model binding error handling
            string internalValue = setup.InitialCalendarDate = model != null ? $"{model:o}" : string.Empty;
            string displayValue = Formatting.FormatDateTime(model);

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{ControlId}' class='yt_datetime t_edit'>
    <input type='hidden' {FieldSetup(FieldType.Validated, new List<YIClientValidation> { new ComponentsHTML_DateTimeValidation() })} value='{HAE(internalValue)}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{HAE(displayValue)}'>
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

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DateTimeEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ComponentsHTML_DateTimeValidation : Attribute, YIClientValidation {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DateTimeComponentBase), name, defaultValue, parms); }

        public ComponentsHTML_DateTimeValidation() { }
        public ValidationBase AddValidation(object container, PropertyData propData, string caption) {
            return new ValidationBase {
                Method = nameof(ComponentsHTML_DateTimeValidation),
                Message = __ResStr("dateTimeVal", "The date/time is invalid (field '{0}')", caption),
            };
        }
    }
}
