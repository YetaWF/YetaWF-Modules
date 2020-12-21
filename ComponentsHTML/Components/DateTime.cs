/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
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
    public class DateTimeEditComponent : DateTimeComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class DateTimeSetup {
            public DateTime Min { get; set; }
            public DateTime Max { get; set; }
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
            return await RenderAsync((DateTime?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(DateTime? model) {

            // handle min/max date
            DateTimeSetup setup = new DateTimeSetup {
                Min = new DateTime(1900, 1, 1),
                Max = new DateTime(2199, 12, 31),
            };
            MinimumDateAttribute minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
            if (minAttr != null)
                setup.Min = minAttr.MinDate;
            MaximumDateAttribute maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
            if (maxAttr != null)
                setup.Max = maxAttr.MaxDate;

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            string hidden = await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation);

            string tags = $"<div id='{DivId}' class='yt_datetime t_edit'>{hidden}<input{FieldSetup(FieldType.Anonymous)} name='dtpicker' value='{(model != null ? HAE(Formatting.FormatDateTime((DateTime)model)) : null)}'></div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DateTimeEditComponent('{DivId}', {Utility.JsonSerialize(setup)});");

            return tags;
        }
    }
}
