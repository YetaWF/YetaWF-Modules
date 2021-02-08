/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Time component implementation.
    /// </summary>
    public abstract class TimeComponentBase : YetaWFComponent {

        internal const string TemplateName = "Time";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, DateTimeEditComponent.TemplateName, ComponentType.Edit);
            await base.IncludeAsync();
        }
        /// <inheritdoc/>
        public async Task<string> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        /// <inheritdoc/>
        public Task<string> RenderAsync(DateTime? model) {

            DateTimeEditComponent.Setup setup = new DateTimeEditComponent.Setup() {
                Style = DateTimeEditComponent.Setup.DateTimeStyleEnum.Time
            };
            if (TryGetSiblingProperty($"{PropertyName}_Setup", out DateTimeEditComponent.DateTimeSetup dateTimeSetup)) {
                setup.MinTime = dateTimeSetup.MinTime;
                setup.MaxTime = dateTimeSetup.MaxTime;
            }

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{DivId}' class='yt_datetime yt_time t_edit'>
    <input type='hidden' id='{ControlId}'{FieldSetup(FieldType.Validated)} value='{(model != null ? HAE($"{model:o}") : null)}'>
    <input type='text'{GetClassAttribute()} maxlength='20' value='{(model != null ? HAE(Formatting.FormatTime((DateTime)model)) : null)}'>
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
