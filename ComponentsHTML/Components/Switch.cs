/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Switch component implementation.
    /// </summary>
    public abstract class SwitchComponentBase : YetaWFComponent {

        internal string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SwitchComponentBase), name, defaultValue, parms); }

        /// <summary>
        /// UI hint name.
        /// </summary>
        public const string TemplateName = "Switch";

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
    /// [UIHint("Switch"), ReadOnly]
    /// public bool ShowHelp { get; set; }
    /// public bool ShowHelp_Text { get; } = "Hello";
    /// </example>
    [UsesSibling("_Text", "string", "Defines the additional text shown next to the checkbox.")]
    public class SwitchDisplayComponent : SwitchComponentBase, IYetaWFComponent<bool?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(bool? model) {

            string check = string.Empty;
            if (model != null && (bool)model)
                check = " checked='checked'";

            TryGetSiblingProperty($"{PropertyName}_Text", out string? text);
            if (!TryGetSiblingProperty($"{PropertyName}_On", out string? textOn))
                textOn = this.__ResStr("on", "On");
            if (!TryGetSiblingProperty($"{PropertyName}_Off", out string? textOff))
                textOn = this.__ResStr("off", "Off");
            if (TryGetSiblingProperty($"{PropertyName}_Size", out string? size))
                size = $" t_{size}";

            if (TryGetSiblingProperty($"{PropertyName}_Tooltip", out string? tooltip))
                tooltip = $" data-tooltip='{Utility.HAE(tooltip)}'";

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.SwitchComponent('{DivId}');");

            return Task.FromResult($@"
<div id='{DivId}' class='yt_switch t_display{GetClasses()}{size}'>
    <input id='{ControlId}' {FieldSetup(FieldType.Anonymous)} type='checkbox' value='true'{check} disabled>
    <label for='{ControlId}' data-text-on='{textOn}' data-text-off='{textOff}'{tooltip} tabindex='0'></label>
    <div class='t_desc'>{HE(text)}</div>
</div>");

        }
    }
    /// <summary>
    /// Allows selection of a true/false status using a checkbox switch and shows additional text.
    /// </summary>
    /// <remarks>
    /// The additional text shown does not change as the checkbox status is changed by the user.
    /// </remarks>
    [UsesSibling("_Text", "string", "Defines the additional text shown next to the checkbox.")]
    [UsesSibling("_Tooltip", "string", "Defines the tooltip shown for the checkbox.")]
    public class SwitchEditComponent : SwitchComponentBase, IYetaWFComponent<bool?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(bool? model) {

            string check = string.Empty;
            if (model != null && (bool)model)
                check = " checked='checked'";

            TryGetSiblingProperty($"{PropertyName}_Text", out string? text);
            if (!TryGetSiblingProperty($"{PropertyName}_On", out string? textOn))
                textOn = this.__ResStr("on", "On");
            if (!TryGetSiblingProperty($"{PropertyName}_Off", out string? textOff))
                textOn = this.__ResStr("off", "Off");
            if (TryGetSiblingProperty($"{PropertyName}_Size", out string? size))
                size = $" t_{size}";

            if (TryGetSiblingProperty($"{PropertyName}_Tooltip", out string? tooltip))
                tooltip = $" data-tooltip='{Utility.HAE(tooltip)}'";

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.SwitchComponent('{DivId}');");

            return Task.FromResult($@"
<div id='{DivId}' class='yt_switch t_edit{GetClasses()}{size}'>
    <input id='{ControlId}' {FieldSetup(Validation ? FieldType.Validated : FieldType.Anonymous)} type='checkbox' value='true'{check}>
    <label for='{ControlId}' data-text-on='{textOn}' data-text-off='{textOff}'{tooltip} tabindex='0'></label>
    <div class='t_desc'>{HE(text)}</div>
</div>");
        }
    }
}
