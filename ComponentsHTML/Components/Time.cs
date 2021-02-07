/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Time component implementation.
    /// </summary>
    public abstract class TimeComponentBase : YetaWFComponent {

        internal const string TemplateName = "Time";

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
    /// Displays the model formatted as a time localized using the user's selected time zone (see User Settings Module).
    /// </summary>
    /// <remarks>All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// The model value must be specified as UTC. If the model value is null or equal to DateTime.MinValue or DateTime.MaxValue, nothing is rendered.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Time"), Description("The time this feed was published")]
    /// [UIHint("Time"), ReadOnly]
    /// public DateTime? FeedPublishTime { get; set; }
    /// </example>
    public class TimeDisplayComponent : TimeComponentBase, IYetaWFComponent<DateTime?> {

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
            if (model != null && (DateTime)model > DateTime.MinValue && (DateTime)model < DateTime.MaxValue)
                return Task.FromResult($"<div{FieldSetup(FieldType.Anonymous)} class='yt_time t_display'>{HE(YetaWF.Core.Localize.Formatting.FormatTime(model))}</div>");
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Allows entry of a time using local time.
    /// </summary>
    /// <remarks>
    /// All date/time values in YetaWF are internally stored and processed using UTC.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Time"), Description("The time this feed was published")]
    /// [UIHint("Time")]
    /// public DateTime? FeedPublishTime { get; set; }
    /// </example>
    public class TimeEditComponent : TimeComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
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

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            string hidden = await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation);

            string value = string.Empty;
            if (model != null)
                value = Formatting.FormatTime((DateTime)model);// shows time

            string tags = $"<div id='{DivId}' class='yt_time t_edit'>{hidden}<input{FieldSetup(FieldType.Anonymous)} name='dtpicker' value='{value}'></div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.TimeEditComponent('{DivId}');");

            return tags;
        }
    }
}
