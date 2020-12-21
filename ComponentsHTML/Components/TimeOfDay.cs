/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TimeOfDay component implementation.
    /// </summary>
    public abstract class TimeOfDayComponentBase : YetaWFComponent {

        internal const string TemplateName = "TimeOfDay";

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
    /// Displays a time of day (between 00:00:00 hours and 23:59:59). If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Due Time, Morning Tasks"), Description("")]
    /// [UIHint("TimeOfDay"), Required]
    /// public TimeOfDay MorningTaskDueTime { get; set; }
    /// </example>
    public class TimeOfDayDisplayComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay> {

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
        public Task<string> RenderAsync(TimeOfDay model) {
            if (model != null)
                return Task.FromResult($"<div{FieldSetup(FieldType.Anonymous)} class='yt_timeofday t_display'>{HE(Formatting.FormatTime(model.AsDateTime()))}</div>");
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Allows entry of a time of day (between 00:00:00 hours and 23:59:59).
    /// </summary>
    /// <example>
    /// [Caption("Due Time, Morning Tasks"), Description("")]
    /// [UIHint("TimeOfDay"), Required]
    /// public TimeOfDay MorningTaskDueTime { get; set; }
    /// </example>
    public class TimeOfDayEditComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay> {

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
        public async Task<string> RenderAsync(TimeOfDay model) {

            // we're reusing Time component
            await Manager.AddOnManager.AddTemplateFromUIHintAsync("Time", GetComponentType());

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            string hidden = await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation);

            string value = string.Empty;
            if (model != null) {
                DateTime dt = model.AsDateTime();
                value = Formatting.FormatTime(dt);
            }

            string tags = $"<div id='{DivId}' class='yt_time t_edit'>{hidden}<input{FieldSetup(FieldType.Anonymous)} name='dtpicker' value='{value}'></div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.TimeEditComponent('{DivId}');");

            return tags;
        }
    }
}
