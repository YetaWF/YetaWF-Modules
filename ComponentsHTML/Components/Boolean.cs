/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Boolean component implementation.
    /// </summary>
    public abstract class BooleanComponentBase : YetaWFComponent {

        internal const string TemplateName = "Boolean";

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
    /// Displays the model using a disabled checkbox, indicating the boolean status (checked, unchecked).
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Show Help"), Description("Defines whether the module help link is shown in Display Mode - The help link is always shown in Edit Mode", Order = -91)]
    /// [UIHint("Boolean"), ReadOnly]
    /// public bool ShowHelp { get; set; }
    /// </example>
    public class BooleanDisplayComponent : BooleanComponentBase, IYetaWFComponent<bool?> {

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
        public async Task<string> RenderAsync(bool model) {
            return await RenderAsync((bool?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_boolean");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("disabled", "disabled");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");
            return Task.FromResult(tag.ToString(YTagRenderMode.StartTag));

        }
    }
    /// <summary>
    /// Allows selection of a true/false status using a checkbox.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Show Help"), Description("Defines whether the module help link is shown in Display Mode - The help link is always shown in Edit Mode", Order = -91)]
    /// [UIHint("Boolean")]
    /// public bool ShowHelp { get; set; }
    /// </example>
    public class BooleanEditComponent : BooleanComponentBase, IYetaWFComponent<bool>, IYetaWFComponent<bool?> {

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
        public async Task<string> RenderAsync(bool model) {
            return await RenderAsync((bool?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(bool? model) {

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_boolean");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Anonymous);
            tag.Attributes.Add("id", ControlId);
            tag.Attributes.Add("type", "checkbox");
            tag.Attributes.Add("value", "true");
            if (model != null && (bool)model)
                tag.Attributes.Add("checked", "checked");

            // add a hidden field so we always get "something" for check boxes (that means we have to deal with duplicates names)
            YTagBuilder tagHidden = new YTagBuilder("input");
            FieldSetup(tagHidden, FieldType.Normal);
            tagHidden.Attributes.Add("type", "hidden");
            tagHidden.Attributes.Add("value", "false");
            tagHidden.AddCssClass("yform-novalidate");

            //Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.BooleanEditComponent('{ControlId}');");

            return Task.FromResult(tag.ToString(YTagRenderMode.StartTag) + tagHidden.ToString(YTagRenderMode.StartTag));
        }
    }
}
