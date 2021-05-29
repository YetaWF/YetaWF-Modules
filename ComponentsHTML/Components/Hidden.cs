/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Hidden component implementation.
    /// </summary>
    public abstract class HiddenComponentBase : YetaWFComponent {

        internal const string TemplateName = "Hidden";

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
    /// Renders the model as a hidden input field &lt;input type="hidden"..&gt;.
    /// </summary>
    /// <remarks>
    /// Even though it's marked with the ReadOnly attribute, this component's value is still modifiable and will be included when a form is submitted.
    /// It never uses validation.
    ///
    /// In most cases the Display version of this component should be used, instead of the Edit component. The Edit component uses validation which is not desirable in most cases.
    /// </remarks>
    /// <example>
    /// [UIHint("Hidden"), ReadOnly]
    /// public int Identity { get; set; }
    /// </example>
    public class HiddenDisplayComponent : HiddenComponentBase, IYetaWFComponent<object?> {

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
        public Task<string> RenderAsync(object? model) {

            string css = string.Empty;
            if (HtmlAttributes.ContainsKey("--NoTemplate"))
                HtmlAttributes.Remove("--NoTemplate");
            else if (HtmlAttributes.ContainsKey("__NoTemplate"))
                HtmlAttributes.Remove("__NoTemplate");
            else
                css = "yt_hidden";
            css = CssManager.CombineCss(css, GetClasses());
            css = string.IsNullOrWhiteSpace(css) ? string.Empty : $" class='{css}'";

            string value = string.Empty;
            if (model != null) {
                if (model.GetType().IsEnum)
                    value = ((int)model).ToString();
                else
                    value = model.ToString() ?? string.Empty;
            }

            // maxlength not allowed on input type=hidden
//            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
//#if NOTYET
//            if (lenAttr == null)
//                throw new InternalError($"No max string length given using StringLengthAttribute - {FieldName}");
//#endif
//            if (lenAttr != null && lenAttr.MaximumLength > 0 && lenAttr.MaximumLength <= 8000)
//                HtmlAttributes.Add("maxlength", lenAttr.MaximumLength.ToString());

            string? id = HtmlBuilder.GetIdCond(HtmlAttributes);
            if (id != null)
                id = $" id='{id}'";
            return Task.FromResult($"<input{id}{FieldSetup(FieldType.Normal)} type='hidden' value='{value}'{css}>");
        }
    }

    /// <summary>
    /// Renders the model as a hidden input field &lt;input type="hidden"..&gt;.
    /// </summary>
    /// <remarks>
    /// In most cases the Display version of this component should be used, instead of the Edit component. The Edit component uses validation which is not desirable in most cases.
    /// </remarks>
    /// <example>
    /// [UIHint("Hidden")]
    /// public int Identity { get; set; }
    /// </example>
    public class HiddenEditComponent : HiddenComponentBase, IYetaWFComponent<object> {

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
        public Task<string> RenderAsync(object model) {

            string css = string.Empty;
            if (HtmlAttributes.ContainsKey("--NoTemplate"))
                HtmlAttributes.Remove("--NoTemplate");
            else if (HtmlAttributes.ContainsKey("__NoTemplate"))
                HtmlAttributes.Remove("__NoTemplate");
            else
                css = "yt_hidden";
            css = CssManager.CombineCss(css, GetClasses());
            css = string.IsNullOrWhiteSpace(css) ? string.Empty : $" class='{css}'";

            string value = string.Empty;
            if (model != null) {
                if (model.GetType().IsEnum)
                    value = ((int)model).ToString();
                else
                    value = model.ToString() ?? string.Empty;
            }

            StringLengthAttribute? lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
#if NOTYET
            if (lenAttr == null)
                throw new InternalError($"No max string length given using StringLengthAttribute - {FieldName}");
#endif
            if (lenAttr != null && lenAttr.MaximumLength > 0 && lenAttr.MaximumLength <= 8000)
                HtmlAttributes.Add("maxlength", lenAttr.MaximumLength.ToString());

            string? id = HtmlBuilder.GetIdCond(HtmlAttributes);
            if (id != null)
                id = $" id='{id}'";
            return Task.FromResult($"<input{id}{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} type='hidden' value='{value}'{css}>");
        }
    }
}
