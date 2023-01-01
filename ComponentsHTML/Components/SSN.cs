/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SSN component implementation.
    /// </summary>
    public abstract class SSNComponentBase : YetaWFComponent {

        internal const string TemplateName = "SSN";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        internal class Setup {
            public string Mask { get; set; } = null!;
        }
    }

    /// <summary>
    /// Displays the model formatted as a Social Security Number.
    /// </summary>
    public class SSNDisplayComponent : SSNComponentBase, IYetaWFComponent<string?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(string? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && model.Length == 9) {
                hb.Append($@"
<div class='yt_ssn yt_maskededit_base t_display'>
    {HE(SSNValidationAttribute.GetDisplay(model))}
</div>");
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a Social Security Number.
    /// </summary>
    public class SSNEditComponent : SSNComponentBase, IYetaWFComponent<string?> {

        Setup setup = new Setup {
            Mask = "NNN-NN-NNNN",
        };

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "MaskedEdit", ComponentType.Edit);
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "Text", ComponentType.Edit);
            await base.IncludeAsync();
        }
        /// <inheritdoc/>
        public Task<string> RenderAsync(string? model) {

            string value = string.Empty;
            if (model != null)
                value = model;

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div id='{ControlId}' class='yt_ssn yt_maskededit_base t_edit'>
    <input type='text' value='{HAE(value)}'{HtmlBuilder.GetClassAttribute(HtmlAttributes, "yt_text_base t_edit")}{HtmlBuilder.Attributes(HtmlAttributes)}>
    <input type='hidden' value='{HAE(value)}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} maxlength='200'>
</div>");

            string tags = hb.ToString();

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.SSNEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }
}
