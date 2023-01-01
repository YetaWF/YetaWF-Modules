/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
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
    public class BooleanTextEditComponent : BooleanTextComponentBase, IYetaWFComponent<bool?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
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
