/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Email component implementation.
    /// </summary>
    public abstract class EmailComponent : YetaWFComponent {

        internal const string TemplateName = "Email";

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
    /// Displays the model as an email address with a clickable &lt;a&gt; mailto: link. If the model value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Site Admin Email"), Description("The email address of the site's administrator")]
    /// [UIHint("Email"), ReadOnly]
    /// public string AdminEmail { get; set; }
    /// </example>
    public class EmailDisplayComponent : EmailComponent, IYetaWFComponent<string> {

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
            if (!string.IsNullOrWhiteSpace(model))
                hb.Append($@"<div class='yt_email t_display'><a href='mailto: {Utility.HtmlAttributeEncode(model)}'>{Utility.HtmlEncode(model)}</a></div>");
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of an email address.
    /// </summary>
    /// <example>
    /// [Caption("Site Admin Email"), Description("The email address of the site's administrator")]
    /// [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
    /// public string AdminEmail { get; set; }
    /// </example>
    public class EmailEditComponent : EmailComponent, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {
            HtmlAttributes.Add("class", "yt_text40");
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr == null)
                HtmlAttributes.Add("maxlength", Globals.MaxEmail.ToString());
            return await TextEditComponent.RenderTextAsync(this, model?.ToString() ?? "", "yt_email");
        }
    }
}
