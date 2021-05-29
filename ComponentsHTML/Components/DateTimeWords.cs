/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the DateTimeWords component implementation.
    /// </summary>
    public abstract class DateTimeWordsComponentBase : YetaWFComponent {

        internal const string TemplateName = "DateTimeWords";

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
    /// Displays the model formatted in words as a timespan difference between the current date/time and the model's date/time.
    /// Example: a few seconds ago, 2 days ago, last week, 2 months ago, 3 years ago, etc.
    /// The complete long date is available as a tooltip when hovering over the rendered timespan.
    /// </summary>
    /// <remarks>All date/time values in YetaWF are internally stored and processed using UTC.
    ///
    /// The model value must be specified as UTC. If the model value is null or equal to DateTime.MinValue or DateTime.MaxValue, nothing is rendered.
    /// </remarks>
    /// <example>
    /// [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
    /// [UIHint("DateTimeWords"), ReadOnly]
    /// public DateTime? FeedPublishDate { get; set; }
    /// </example>
    [UsesSibling("_EmptyHTML", "string", "If the model value is null or equal to DateTime.MinValue or DateTime.MaxValue, the contents of this property are rendered instead.")]
    public class DateTimeWordsDisplayComponent : DateTimeWordsComponentBase, IYetaWFComponent<DateTime?> {

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
                DateTime last = (DateTime)model;
                TimeSpan diff = last - DateTime.UtcNow;
                string words = HE(Formatting.FormatTimeSpanInWords(diff));
                string wordsTT = Formatting.FormatLongDateTime(last);
                return Task.FromResult($@"<div{FieldSetup(FieldType.Anonymous)}{HtmlBuilder.GetClassAttribute(HtmlAttributes)} {Basics.CssTooltip}='{HAE(wordsTT)}'>{HE(words)}</div>");
            } else {
                if (!TryGetSiblingProperty($"{PropertyName}_EmptyHTML", out string? text))
                    return Task.FromResult<string>(string.Empty);
                return Task.FromResult($@"<div{FieldSetup(FieldType.Anonymous)}{HtmlBuilder.GetClassAttribute(HtmlAttributes)}>{text}</div>");
            }
        }
    }
}
