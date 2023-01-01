/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the MenuUL component implementation. Used to load the JavaScript/CSS support for the MenuUL component.
    /// </summary>
    public abstract class MenuULComponentBase : YetaWFComponent {

        internal const string TemplateName = "MenuUL";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Renders nothing. Used to load the JavaScript/CSS support for the MenuUL component.
    /// </summary>
    public class MenuULDisplayComponent : MenuULComponentBase, IYetaWFComponent<object?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(object? model) {
            return Task.FromResult(string.Empty);
        }
    }
}
