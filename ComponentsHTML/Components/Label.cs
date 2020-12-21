/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders the model as a &lt;label&gt; tag. If the model is null or "", an &amp;nbsp; placeholder is generated.
    /// </summary>
    /// <example>
    /// [UIHint("Label")]
    /// public string LabelContents { get; set; }
    /// public string LabelContents_HelpLink { get; set; }
    /// </example>
    [UsesSibling("_HelpLink", "string", "Contains a complete URL which is used as a help link for the generated label. If null, no help link is available.")]
    public class LabelDisplayComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "Label";

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

            string text;
            if (string.IsNullOrEmpty(model)) // we're distinguishing between "" and " "
                text = "&nbsp;";
            else
                text = HE(model);
            string label = $"<label{FieldSetup(FieldType.Anonymous)}{HtmlBuilder.GetClassAttribute(HtmlAttributes)}>{text}</label>";

            if (TryGetSiblingProperty<string>($"{PropertyName}_HelpLink", out string helpLink) && !string.IsNullOrWhiteSpace(helpLink)) {
                helpLink = $"<a href='{HAE(helpLink)}' class='yt_extlabel_img' target='_blank' rel='noopener noreferrer'>{ImageHTML.BuildKnownIcon("#Help")}</a>";
                return Task.FromResult(label + helpLink);
            }
            return Task.FromResult(label);
        }
    }
}
