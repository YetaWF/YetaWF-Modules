/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the StringWithToolTip component implementation.
    /// </summary>
    public abstract class StringWithToolTipComponentBase : YetaWFComponent {

        internal const string TemplateName = "StringWithToolTip";

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
    /// Displays the model as an HTML encoded string with a tooltip.
    /// </summary>
    /// <example>
    /// [Caption("Repositories, Projects and Tags Explorer"), Description("")]
    /// [UIHint("StringWithToolTip"), ReadOnly]
    /// public string Text { get; set; }
    /// public string Text_ToolTip { get; set; }
    /// </example>
    [UsesSibling("_ToolTip", "string", "Defines the tooltip to be shown. Can be null.")]
    public class StringWithToolTipDisplayComponent : StringWithToolTipComponentBase, IYetaWFComponent<string> {

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
        public Task<string> RenderAsync(string model) {

            string? toolTip = GetSiblingProperty<string>($"{PropertyName}_ToolTip");
            string tt = string.Empty;
            if (!string.IsNullOrWhiteSpace(toolTip))
                tt = $" {Basics.CssTooltipSpan}='{Utility.HAE(toolTip)}'";

            return Task.FromResult($"<span{FieldSetup(FieldType.Anonymous)}{GetClassAttribute()}{tt}>{Utility.HE(model)}</span>");
        }
    }
}
