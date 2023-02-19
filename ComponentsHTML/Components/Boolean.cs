/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Boolean component implementation.
    /// </summary>
    public abstract class BooleanComponentBase : YetaWFComponent {

        internal const string TemplateName = "Boolean";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(bool? model) {
            string? check = null;
            if (model != null && (bool)model)
                check = " checked='checked'";
            string tag = $@"<input{FieldSetup(FieldType.Anonymous)} type='checkbox' disabled='disabled'{check} class='yt_boolean t_display{GetClasses()}' >";
            return Task.FromResult(tag);
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
    public class BooleanEditComponent : BooleanComponentBase, IYetaWFComponent<bool?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(bool? model) {

            string? check = null;
            if (model != null && (bool)model)
                check = " checked='checked'";
            string tag = $@"<input id='{ControlId}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Anonymous)} type='checkbox'{check} value='true' class='yt_boolean t_edit{GetClasses()}'>";
            //$$$ add a hidden field so we always get "something" for check boxes (that means we have to deal with duplicates names)
            //$$$ string tagHidden = $@"<input{FieldSetup(FieldType.Normal)} type='hidden' value='false' class='yt_boolean t_edit yform-novalidate'>";

            //Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.BooleanEditComponent('{id}');");

            return Task.FromResult(tag);//$$$$ + tagHidden);
        }
    }
}
