/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
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
    /// Displays the model using a disabled checkbox, indicating the boolean status (checked, unchecked) and additional text.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Show Help"), Description("Defines whether the module help link is shown in Display Mode - The help link is always shown in Edit Mode", Order = -91)]
    /// [UIHint("BooleanText"), ReadOnly]
    /// public bool ShowHelp { get; set; }
    /// public bool ShowHelp_Text { get { return ShowHelp ? "Yes" : "No"; } }
    /// </example>
    [UsesSibling("_Text", "string", "Defines the additional text shown next to the checkbox.")]
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
        public async Task<string> RenderAsync(bool model) {
            return await RenderAsync((bool?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(bool? model) {

            string check = string.Empty;
            if (model != null && (bool)model)
                check = " checked='checked'";

            TryGetSiblingProperty($"{PropertyName}_Text", out string? text);

            return Task.FromResult( $@"<input{FieldSetup(FieldType.Anonymous)} class='yt_booleantext t_display{GetClasses()}' type='checkbox' disabled='disabled'{check}>{HE(text)}");
        }
    }
    /// <summary>
    /// Allows selection of a true/false status using a checkbox and shows additional text.
    /// </summary>
    /// <remarks>
    /// The additional text shown does not change as the checkbox status is changed by the user.
    /// </remarks>
    /// <example>
    /// [Category("Skin"), Caption("Show Help"), Description("Defines whether the module help link is shown in Display Mode - The help link is always shown in Edit Mode", Order = -91)]
    /// [UIHint("BooleanText")]
    /// public bool ShowHelp { get; set; }
    /// public string ShowHelp_Text { get { ShowHelp ? "Yes" : "No" } }
    /// </example>
    [UsesSibling("_Text", "string", "Defines the additional text shown next to the checkbox.")]
    [UsesSibling("_Tooltip", "string", "Defines the tooltip shown for the checkbox.")]
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
        public async Task<string> RenderAsync(bool model) {
            return await RenderAsync((bool?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(bool? model) {

            // add a hidden field so we always get "something" for check boxes (that means we have to deal with duplicates names)

            string check = string.Empty;
            if (model != null && (bool)model)
                check = " checked='checked'";

            TryGetSiblingProperty($"{PropertyName}_Text", out string? text);

            if (TryGetSiblingProperty($"{PropertyName}_Tooltip", out string? tooltip))
                tooltip = $" data-tooltip='{Utility.HAE(tooltip)}'";

            return Task.FromResult($@"
<input id='{ControlId}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Anonymous)} class='yt_booleantext t_edit{GetClasses()}' type='checkbox' value='true'{check}{tooltip}>
{HE(text)}
<input{FieldSetup(FieldType.Normal)} type='hidden' value='false' class='yform-novalidate'>");

            //Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.BooleanTextEditComponent('{id}');");
        }
    }
}
