/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Guid component implementation.
    /// </summary>
    public abstract class GuidComponentBase : YetaWFComponent {

        internal const string TemplateName = "Guid";

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
    /// Displays the model. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Owner Guid"), Description("The guid of the Url Service subscriber account")]
    /// [UIHint("Guid"), ReadOnly]
    /// public Guid OwnerGuid { get; set; }
    /// </example>
    public class GuidDisplayComponent : GuidComponentBase, IYetaWFComponent<Guid?> {

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
        public async Task<string> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Guid? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && model != Guid.Empty) {
                hb.Append(HE(((Guid)model).ToString()));
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a Guid.
    /// </summary>
    /// <example>
    /// [Caption("Owner Guid"), Description("The guid of the Url Service subscriber account")]
    /// [UIHint("Guid"), GuidValidation, Required, Trim]
    /// public Guid OwnerGuid { get; set; }
    /// </example>
    public class GuidEditComponent : GuidComponentBase, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

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
        public async Task<string> RenderAsync(Guid model) {
            return await RenderAsync((Guid?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {
            HtmlAttributes.Add("class", "yt_text40");
            HtmlAttributes.Add("maxlength", "40");
            return await TextEditComponent.RenderTextAsync(this, model?.ToString() ?? string.Empty, "yt_guid");
        }
    }
}
