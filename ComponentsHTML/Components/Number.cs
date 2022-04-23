/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;

namespace YetaWF.Modules.ComponentsHTML.Components {

    internal class NumberSetup {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public string? Lead { get; set; }
        public string? Trail { get; set; }
        public int Digits { get; set; }
        public bool Plain { get; set; }
        public string? Currency { get; set; }
        public string? Locale { get; set; }
    }

    /// <summary>
    /// Base class for the Number component implementation. Used to load the JavaScript/CSS support for the Number component.
    /// </summary>
    public abstract class NumberComponentBase : YetaWFComponent {

        internal const string TemplateName = "Number";

        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// Renders nothing. Used to load the JavaScript/CSS support for the Number component.
    /// </summary>
    public class NumberEditComponent : NumberComponentBase, IYetaWFComponent<object?> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public Task<string> RenderAsync(object? model) {
            return Task.FromResult(string.Empty);
        }
    }
}
