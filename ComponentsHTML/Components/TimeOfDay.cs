/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
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
    public class TimeOfDayDisplayComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, DateTimeEditComponent.TemplateName, ComponentType.Edit);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(TimeOfDay model) {

            DateTimeEditComponent.Setup setup = new DateTimeEditComponent.Setup {
                Style = DateTimeEditComponent.Setup.DateTimeStyleEnum.Time
            };
            if (TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeEditComponent.DateTimeSetup dateTimeSetup)) {
                setup.MinTime = dateTimeSetup.MinTime;
                setup.MaxTime = dateTimeSetup.MaxTime;
            }

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{DivId}' class='yt_datetime yt_time t_edit'>
    <input type='hidden' id='{ControlId}' {FieldSetup(FieldType.Validated)} value='{(model != null ? HAE($"{model.AsDateTime():o}") : null)}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{(model != null ? HAE(Formatting.FormatTime(model.AsDateTime())) : null)}'>
    <div class='t_sels'>
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
