/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the FloatValue component implementation.
    /// </summary>
    public abstract class FloatValueComponentBase : YetaWFComponent {

        internal const string TemplateName = "FloatValue";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
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
    /// Displays the model formatted as a floating point value. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Latitude"), Description("The latitude where the IP address is located")]
    /// [UIHint("FloatValue"), AdditionalMetadata("EmptyIf0", true), SuppressEmpty, ReadOnly]
    /// public float Latitude { get; set; }
    /// </example>
    [UsesAdditional("EmptyIf0", "bool", "false", "If true, the floating point value is not displayed if it is equal to 0. Otherwise it is always displayed.")]
    public class FloatValueDisplayComponent : FloatValueComponentBase, IYetaWFComponent<Single?> {

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
        public Task<string> RenderAsync(Single? model) {

            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                if ((Single)model != 0.0 || !PropData.GetAdditionalAttributeValue("EmptyIf0", false)) {
                    hb.Append(HE(model.ToString()));
                }
            }
            return Task.FromResult(hb.ToString());
        }
    }
}
