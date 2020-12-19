/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the FileFolderSize component implementation.
    /// </summary>
    public abstract class FileFolderSizeComponentBase : YetaWFComponent {

        internal const string TemplateName = "FileFolderSize";

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
    /// Displays the model as a size in KB or MB. A tooltip displays the exact size in bytes.
    /// </summary>
    /// <example>
    /// [Caption("Size"), Description("Displays the size of the file")]
    /// [UIHint("FileFolderSize"), ReadOnly]
    /// public long Size { get; set; }
    /// </example>
    public class FileFolderSizeDisplayComponent : FileFolderSizeComponentBase, IYetaWFComponent<long> {

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
        public Task<string> RenderAsync(long model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_filefoldersize t_display' {Basics.CssTooltip}='{Formatting.LongKBDisplay(model, detailed:true)}'>
    {HE(Formatting.FormatFileFolderSize(model))}
</div>
");
            return Task.FromResult(hb.ToString());
        }
    }
}
