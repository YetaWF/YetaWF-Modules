/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TimeOfDay component implementation.
    /// </summary>
    public abstract class TimeOfDayComponentBase : YetaWFComponent {

        internal const string TemplateName = "TimeOfDay";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
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
    public class TimeOfDayDisplayComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(TimeOfDay? model) {
            if (model != null)
                return Task.FromResult($"<div{FieldSetup(FieldType.Anonymous)} class='yt_timeofday t_display'>{HE(Formatting.FormatTimeOfDay(model))}</div>");
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
    public class TimeOfDayEditComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, DateTimeEditComponent.TemplateName, ComponentType.Edit);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(TimeOfDay? model) {

            DateTimeEditComponent.Setup setup = new DateTimeEditComponent.Setup {
                Style = DateTimeEditComponent.Setup.DateTimeStyleEnum.Time
            };
            if (TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeEditComponent.DateTimeSetup? dateTimeSetup) && dateTimeSetup != null) {
                setup.MinTime = dateTimeSetup.MinTime;
                setup.MaxTime = dateTimeSetup.MaxTime;
            }

            // model binding error handling
            string internalValue = setup.InitialCalendarDate = (model != null) ? $"{model.AsDateTime():o}" : string.Empty;
            string displayValue = Formatting.FormatTimeOfDay(model != null && model.HasTimeOfDay ? model : null);

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{ControlId}' class='yt_datetime yt_timeofday t_edit'>
    <input type='hidden' {FieldSetup(FieldType.Validated, new List<YIClientValidation> { new ComponentsHTML_TimeOfDayValidation() })} value='{HAE(internalValue)}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{HAE(displayValue)}'>
    <div class='t_sels'>
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
    internal class ComponentsHTML_TimeOfDayValidation : Attribute, YIClientValidation {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TimeOfDayComponentBase), name, defaultValue, parms); }

        public ComponentsHTML_TimeOfDayValidation() { }
        public ValidationBase AddValidation(object container, PropertyData propData, string caption) {
            return new ValidationBase {
                Method = nameof(ComponentsHTML_TimeOfDayValidation),
                Message = __ResStr("timeOfDayVal", "The time is invalid (field '{0}')", caption),
            };
        }
    }
}
