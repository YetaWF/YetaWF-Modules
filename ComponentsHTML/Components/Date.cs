/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// Base class for the Date component implementation.
    /// </summary>
    public abstract class DateComponentBase : YetaWFComponent {

        internal const string TemplateName = "Date";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Displays the model formatted as a date localized using the user's selected time zone (see User Settings Module).
    /// </summary>
    /// <remarks>All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// The model value must be specified as UTC. If the model value is null or equal to DateTime.MinValue or DateTime.MaxValue, nothing is rendered.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
    /// [UIHint("Date"), ReadOnly]
    /// public DateTime? FeedPublishDate { get; set; }
    /// </example>
    public class DateDisplayComponent : DateComponentBase, IYetaWFComponent<DateTime?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(DateTime? model) {

            if (model != null && (DateTime)model > DateTime.MinValue && (DateTime)model < DateTime.MaxValue) {

                bool utcMidnight = PropData.GetAdditionalAttributeValue<bool>("UTCMidnight", false);
                string displayValue = utcMidnight ? Formatting.FormatDateOnly(model) : Formatting.FormatDate(model);
                return Task.FromResult($@"<div{FieldSetup(FieldType.Anonymous)} class='yt_datetime t_display{GetClasses()}'>{HE(displayValue)}</div>");
            }
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Allows entry of a date using local time.
    /// </summary>
    /// <remarks>
    /// All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// MinimumDateAttribute and MaximumDateAttribute can be used to define the lowest and highest allowable date/time values.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
    /// [UIHint("Date")]
    /// public DateTime? FeedPublishDate { get; set; }
    /// </example>
    public class DateEditComponent : DateComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class DateSetup {
            public DateTime Min { get; set; }
            public DateTime Max { get; set; }
        }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, DateTimeEditComponent.TemplateName, ComponentType.Edit);
            await base.IncludeAsync();
        }
        /// <inheritdoc/>
        public async Task<string> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        /// <inheritdoc/>
        public Task<string> RenderAsync(DateTime? model) {

            DateTimeEditComponent.Setup setup = new DateTimeEditComponent.Setup {
                Style = DateTimeEditComponent.Setup.DateTimeStyleEnum.Date
            };
            if (TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeEditComponent.DateTimeSetup? dateTimeSetup) && dateTimeSetup != null) {
                setup.MinDate = dateTimeSetup.MinDate;
                setup.MaxDate = dateTimeSetup.MaxDate;
            }

            bool utcMidnight = PropData.GetAdditionalAttributeValue<bool>("UTCMidnight", false);
            setup.UtcMidnight = utcMidnight;

            // handle min/max date
            // attributes (like MinimumDateAttribute) override setup and defaults
            MinimumDateAttribute? minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
            if (minAttr != null)
                setup.MinDate = minAttr.MinDate;
            MaximumDateAttribute? maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
            if (maxAttr != null)
                setup.MaxDate = maxAttr.MaxDate;

            // model binding error handling
            string internalValue;
            if (model != null) {
                internalValue = setup.InitialCalendarDate = utcMidnight ? $"{model.Value.Date:o}" : $"{model:o}";
            } else {
                internalValue = setup.InitialCalendarDate = string.Empty;
            }
            string displayValue = utcMidnight ? Formatting.FormatDateOnly(model) : Formatting.FormatDate(model);

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{ControlId}' class='yt_datetime yt_date t_edit'>
    <input type='hidden' {FieldSetup(FieldType.Validated, new List<YIClientValidation> { new ComponentsHTML_DateValidation() })} value='{HAE(internalValue)}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{HAE(displayValue)}'>
    <div class='t_sels'>
        <div class='t_date'>
            {SkinSVGs.Get(AreaRegistration.CurrentPackage, "far-calendar-alt")}
        </div>
    </div>
</div>");

            string tags = hb.ToString();

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DateTimeEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ComponentsHTML_DateValidation : Attribute, YIClientValidation {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DateComponentBase), name, defaultValue, parms); }

        public ComponentsHTML_DateValidation() { }
        public ValidationBase AddValidation(object container, PropertyData propData, string caption) {
            return new ValidationBase {
                Method = nameof(ComponentsHTML_DateValidation),
                Message = __ResStr("dateVal", "The date is invalid (field '{0}')", caption),
            };
        }
    }
}
