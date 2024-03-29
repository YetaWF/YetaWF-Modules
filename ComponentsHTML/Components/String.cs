/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays the model as an HTML encoded string. If the model value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Email Address"), Description("The user's email address")]
    /// [UIHint("String"), ReadOnly]
    /// public string Email { get; set; }
    /// </example>
    public class StringDisplayComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "String";

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
            HtmlBuilder hb = new HtmlBuilder();

            if (model == null)
                model = "";
            string t = model.ToString();
            if (string.IsNullOrWhiteSpace(t))
                return Task.FromResult<string>(string.Empty);
            t = Utility.HE(t);

            return Task.FromResult(t);
        }
    }
}
