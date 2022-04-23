/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ProgressBar component implementation.
    /// </summary>
    public abstract class ProgressBarComponentBase : YetaWFComponent {

        internal const string TemplateName = "ProgressBar";

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
    /// Renders a horizontal progress bar.
    /// </summary>
    /// <remarks>
    /// A sample ProgressBar implementation is available at Tests > Templates > ProgressBar (standard YetaWF site).
    /// </remarks>
    /// <example>
    /// [UIHint("ProgressBar"), ReadOnly]
    /// public float Value { get; set; }
    /// public float Value_Min { get; set; }
    /// public float Value_Max { get; set; }
    /// </example>
    [UsesSibling("_Min", "float", "The progress bar's optional minimum value. The default is 0.")]
    [UsesSibling("_Max", "float", "The progress bar's optional maximum value. The default is 100.")]
    public class ProgressBarDisplayComponent : ProgressBarComponentBase, IYetaWFComponent<float> {

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
        public Task<string> RenderAsync(float model) {
            HtmlBuilder hb = new HtmlBuilder();

            float min;
            TryGetSiblingProperty<float>($"{PropertyName}_Min", out min);
            float max;
            TryGetSiblingProperty<float>($"{PropertyName}_Max", out max);

            hb.Append($@"
<div id='{DivId}' class='yt_progressbar t_display' role='progressbar' aria-valuemin='{min}' aria-valuemax='{max}' aria-valuenow='{model}'>
    <div class='t_value' style='width:{model}%;'></div>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ProgressBarComponent('{DivId}');");

            return Task.FromResult(hb.ToString());
        }
    }
}
