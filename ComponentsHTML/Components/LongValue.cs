/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the LongValue component implementation.
    /// </summary>
    public abstract class LongValueComponentBase : YetaWFComponent {

        internal const string TemplateName = "LongValue";

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
    /// Displays the specified value formatted as a long value.
    /// </summary>
    /// <example>
    /// [Caption("Memory Limit"), Description("The percentage of physical memory available to the application")]
    /// [UIHint("LongValue"), ReadOnly]
    /// public long EffectivePercentagePhysicalMemoryLimit { get; set; }
    /// </example>
    public class LongValueDisplayComponent : LongValueComponentBase, IYetaWFComponent<long>, IYetaWFComponent<long?> {

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
        public async Task<string> RenderAsync(long model) {
            return await RenderAsync((long?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(long? model) {
            HtmlBuilder hb = new HtmlBuilder();
            string val = model?.ToString() ?? string.Empty;
            hb.Append($@"<div class='yt_longvalue t_display'>{HE(val)}</div>");
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a long value.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    /// </remarks>
    /// <example>
    /// [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
    /// [UIHint("LongValue"), Range(1, 99999999), Required]
    /// public long Days { get; set; }
    /// </example>
    public class LongValueEditComponent : LongValueComponentBase, IYetaWFComponent<long>, IYetaWFComponent<long?> {

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
        public async Task<string> RenderAsync(long model) {
            return await RenderAsync((long?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(long? model) {
            string val = model?.ToString() ?? string.Empty;
            HtmlAttributes.Add("class", "yt_text20");
            HtmlAttributes.Add("maxlength", "30");
            return await TextEditComponent.RenderTextAsync(this, val, "yt_longvalue");
        }
    }
}
