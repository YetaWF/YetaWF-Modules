/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

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
    /// Implementation of the StringTT display component.
    /// </summary>
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
        public Task<YHtmlString> RenderAsync(StringTT model) {
            return RenderStringTTAsync(this, model, null);
        }
        internal static Task<YHtmlString> RenderStringTTAsync(YetaWFComponent component, StringTT model, string cssClass) {
            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("span");
            if (!string.IsNullOrWhiteSpace(cssClass)) {
                tag.AddCssClass(cssClass);
                tag.AddCssClass("t_display");
            }
            component.FieldSetup(tag, FieldType.Anonymous);

            if (!string.IsNullOrWhiteSpace(model.Tooltip))
                tag.Attributes.Add(Basics.CssTooltipSpan, model.Tooltip);
            if (!string.IsNullOrWhiteSpace(model.Text))
                tag.SetInnerText(model.Text);
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
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
#if MVC6
        public static YHtmlString ForStringTTDisplay(this IHtmlHelper htmlHelper, string text, string tooltip)
#else
        public static YHtmlString ForStringTTDisplay(this HtmlHelper htmlHelper, string text, string tooltip)
#endif
        {
            YTagBuilder tag = new YTagBuilder("span");
            if (!string.IsNullOrWhiteSpace(tooltip))
                tag.Attributes.Add(Basics.CssTooltipSpan, tooltip);
            if (!string.IsNullOrWhiteSpace(text))
                tag.SetInnerText(text);
            return tag.ToYHtmlString(YTagRenderMode.Normal);
        }
    }
}
