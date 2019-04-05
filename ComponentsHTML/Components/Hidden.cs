/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
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
    /// Implementation of the Hidden display component.
    /// </summary>
    public class HiddenDisplayComponent : HiddenComponentBase, IYetaWFComponent<object> {

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
        public Task<string> RenderAsync(object model) {
            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Normal);
            tag.MergeAttribute("type", "hidden");
            if (model != null && model.GetType().IsEnum) {
                model = (int)model;
            }
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToString(YTagRenderMode.StartTag));
        }
    }

    /// <summary>
    /// Implementation of the Hidden edit component.
    /// </summary>
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
        YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.MergeAttribute("type", "hidden");
            if (model != null && model.GetType().IsEnum) {
                model = (int)model;
            }
            tag.MergeAttribute("value", model == null ? "" : model.ToString());
            return Task.FromResult(tag.ToString(YTagRenderMode.StartTag));
        }
    }
}
