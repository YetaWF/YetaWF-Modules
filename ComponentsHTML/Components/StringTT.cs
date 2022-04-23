/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the StringTT component implementation.
    /// </summary>
    public abstract class StringTTComponentBase : YetaWFComponent {

        internal const string TemplateName = "StringTT";

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
    /// [Caption("Role"), Description("Role Description")]
    /// [UIHint("StringTT"), ReadOnly]
    /// public StringTT RoleName { get; set; }
    ///
    /// RoleName = new StringTT {
    ///     Text = "Hello",
    ///     Tooltip = "I am a tooltip",
    /// };
    /// </example>
    public class StringTTDisplayComponent : StringTTComponentBase, IYetaWFComponent<StringTT> {

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
        public Task<string> RenderAsync(StringTT model) {
            return RenderStringTTAsync(this, model, null);
        }
        internal static Task<string> RenderStringTTAsync(YetaWFComponent component, StringTT model, string? cssClass) {

            string css = string.Empty;
            if (!string.IsNullOrWhiteSpace(cssClass)) {
                css = CssManager.CombineCss(css, cssClass);
                css = CssManager.CombineCss(css, "t_display");
            }

            string tt = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.Tooltip))
                tt = $" {Basics.CssTooltipSpan}='HAE(model.Tooltip)'";
            return Task.FromResult($"<span{component.FieldSetup(FieldType.Anonymous)}{component.GetClassAttribute(css)}{tt}>{HAE(model.Text)}</span>");
        }
    }

    /// <summary>
    /// This static class implements extension methods to render a StringTT display component.
    /// </summary>
    public static class StringTTExtender {
        /// <summary>
        /// Render a StringTT display component given a HtmlHelper instance.
        /// </summary>
        /// <param name="htmlHelper">The HtmlHelper instance.</param>
        /// <param name="text">The text displayed.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns></returns>
        public static string ForStringTTDisplay(this YHtmlHelper htmlHelper, string text, string tooltip) {
            string tt = string.Empty;
            if (!string.IsNullOrWhiteSpace(tooltip))
                tt = $" {Basics.CssTooltipSpan}='{Utility.HAE(tooltip)}'";
            return $"<span{tt}>{Utility.HE(text)}</span>";
        }
    }
}
