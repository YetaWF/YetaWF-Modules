/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the Label display component.
    /// </summary>
    public class LabelDisplayComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "Label";

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

            HtmlBuilder sb = new HtmlBuilder();

            YTagBuilder tagLabel = new YTagBuilder("label");
            FieldSetup(tagLabel, FieldType.Anonymous);
            if (string.IsNullOrEmpty(model)) // we're distinguishing between "" and " "
                tagLabel.InnerHtml = "&nbsp;";
            else
                tagLabel.SetInnerText(model);
            sb.Append(tagLabel.ToString(YTagRenderMode.Normal));

            string helpLink;
            if (TryGetSiblingProperty<string>($"{PropertyName}_HelpLink", out helpLink) && !string.IsNullOrWhiteSpace(helpLink)) {
                YTagBuilder tagA = new YTagBuilder("a");
                tagA.Attributes.Add("href", Utility.UrlEncodePath(helpLink));
                tagA.Attributes.Add("target", "_blank");
                tagA.MergeAttribute("rel", "noopener noreferrer");
                tagA.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule("yt_extlabel_img"));
                tagA.InnerHtml = ImageHTML.BuildKnownIcon("#Help");
                sb.Append(tagA.ToString(YTagRenderMode.Normal));
            }

            return Task.FromResult(sb.ToString());
        }
    }
}
