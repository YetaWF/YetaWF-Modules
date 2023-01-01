/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the MaskedEdit component implementation. Used to load the JavaScript/CSS support for the MaskedEdit component.
    /// </summary>
    public abstract class MaskedEditComponentBase : YetaWFComponent {

        internal const string TemplateName = "MaskedEdit";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Renders nothing. Used to load the JavaScript/CSS support for the MaskedEdit component.
    /// </summary>
    public class MaskedEditEditComponent : MaskedEditComponentBase, IYetaWFComponent<object?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(object? model) {
            return Task.FromResult(string.Empty);
        }
    }
}
