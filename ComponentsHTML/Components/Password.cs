/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Allows entry of a password. The password is masked so it cannot be read.
    /// </summary>
    /// <remarks>
    /// Visually the text box used for the password allows approximately 20 characters to be displayed. The user can enter longer passwords.
    /// </remarks>
    /// <example>
    /// [Caption("Password"), Description("Enter your password")]
    /// [UIHint("Password20"), StringLength(Globals.MaxPswd), Trim]
    /// public string Password { get; set; }
    /// </example>
    [UsesSibling("_PlaceHolder", "string", "Defines the placeholder text shown when control contents are empty.")]
    public class Password20Component : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "Password20";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

            HtmlAttributes.Add("type", "password");
            TryGetSiblingProperty<string>("{PropertyName}_AutoComplete", out string? autocomplete);

            HtmlAttributes.Add("autocomplete", autocomplete ?? "current-password");
            return await TextEditComponent.RenderTextAsync(this, model, "yt_password20");
        }
    }
}
