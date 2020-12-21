/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TimeSpanDHM component implementation.
    /// </summary>
    public abstract class TimeSpanDHMComponentBase : YetaWFComponent {

        internal const string TemplateName = "TimeSpanDHM";

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
    /// Allows entry of a timespan using days, hours and minutes.
    /// </summary>
    /// <example>
    /// [Caption("Expiration"), Description("The time a cookie will remain valid from the point it is created")]
    /// [UIHint("TimeSpanDHM"), Required]
    /// public TimeSpan ExpireTimeSpan { get; set; }
    /// </example>
    public class TimeSpanDHMEditComponent : TimeSpanDHMComponentBase, IYetaWFComponent<TimeSpan>, IYetaWFComponent<TimeSpan?> {

        internal class TimeSpanUI {
            public TimeSpanUI() { }
            public TimeSpanUI(TimeSpan ts) { Span = ts; }

            private TimeSpan Span = new TimeSpan();

            [Caption("Days"), Description("Number of days")]
            [UIHint("IntValue4"), Required, Range(0, 999999)]
            public int Days { get { return Span.Days; } set { } }
            [Caption("Hours"), Description("Number of hours")]
            [UIHint("IntValue2"), Required, Range(0, 23)]
            public int Hours { get { return Span.Hours; } set { } }
            [Caption("Minutes"), Description("Number of minutes")]
            [UIHint("IntValue2"), Required, Range(0, 59)]
            public int Minutes { get { return Span.Minutes; } set { } }
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(AreaRegistration.CurrentPackage.AreaName, "TimeSpan", ComponentType.Edit);
            await base.IncludeAsync();
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
        public async Task<string> RenderAsync(TimeSpan model) {
            return await RenderAsync((TimeSpan?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(TimeSpan? model) {
            HtmlBuilder hb = new HtmlBuilder();

            TimeSpanUI ts = new TimeSpanUI(model??new TimeSpan());

            hb.Append($@"
<div id='{ControlId}' class='yt_timespan t_edit'>");

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model.ToString(), "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation));

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
    <div class='t_days'>
        {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Days))}{ValidationMessage(nameof(TimeSpanUI.Days))}
        {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Days))}
    </div>
    <div class='t_hours'>
        {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Hours))}{ValidationMessage(nameof(TimeSpanUI.Hours))}
        {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Hours))}
    </div>
    <div class='t_minutes'>
        {await HtmlHelper.ForEditAsync(ts, nameof(TimeSpanUI.Minutes))}{ValidationMessage(nameof(TimeSpanUI.Minutes))}
        {await HtmlHelper.ForLabelAsync(ts, nameof(TimeSpanUI.Minutes))}
    </div>");

            }

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TimeSpanEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}
