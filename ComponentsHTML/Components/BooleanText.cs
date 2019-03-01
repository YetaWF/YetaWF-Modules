/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the BooleanText component implementation.
    /// </summary>
    public abstract class BooleanTextComponentBase : YetaWFComponent {

        internal const string TemplateName = "BooleanText";

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
    /// Implementation of the BooleanText display component.
    /// </summary>
    public class BooleanTextDisplayComponent : BooleanTextComponentBase, IYetaWFComponent<bool?> {

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
        public async Task<YHtmlString> RenderAsync(bool model) {
            return await RenderAsync((bool?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<YHtmlString> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_booleantext");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("disabled", "disabled");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");

            string text;
            if (TryGetSiblingProperty($"{PropertyName}_Text", out text))
                return Task.FromResult(new YHtmlString(tag.ToString(YTagRenderMode.StartTag) + HE(text)));
            else
                return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
    /// <summary>
    /// Implementation of the BooleanText edit component.
    /// </summary>
    public class BooleanTextEditComponent : BooleanTextComponentBase, IYetaWFComponent<bool>, IYetaWFComponent<bool?> {

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
        public async Task<YHtmlString> RenderAsync(bool model) {
            return await RenderAsync((bool?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<YHtmlString> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_booleantext");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Anonymous);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("value", "true");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");

            // add a hidden field so we always get "something" for check boxes (that means we have to deal with duplicates names)
            YTagBuilder tagHidden = new YTagBuilder("input");
            FieldSetup(tagHidden, FieldType.Normal);
            tagHidden.Attributes.Add("type", "hidden");
            tagHidden.Attributes.Add("value", "false");

            string text;
            if (TryGetSiblingProperty($"{PropertyName}_Text", out text))
                return Task.FromResult(new YHtmlString(tag.ToString(YTagRenderMode.StartTag) + tagHidden.ToString(YTagRenderMode.StartTag) + HE(text)));
            else
                return Task.FromResult(new YHtmlString(tag.ToYHtmlString(YTagRenderMode.StartTag) + tagHidden.ToString(YTagRenderMode.StartTag)));
        }
    }
}
