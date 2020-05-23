/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    // TODO: This needs to support localization with language specific folders, with a fallback to the non-locale specific file.

    /// <summary>
    /// Base class for the HelpInfo component implementation.
    /// </summary>
    public abstract class HelpInfoComponentBase : YetaWFComponent {

        internal const string TemplateName = "HelpInfo";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
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
    /// Displays the help file identified by the model. The model contains a file name (without path or extension). Help files are located in the specified package's folder ./Addons/_Main/Help and have the extension .html.
    /// If the model is null or the help file doesn't exist, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption(""), Description("")]
    /// [UIHint("HelpInfo"), ReadOnly]
    /// [SuppressEmpty]
    /// public string HelpInformation { get { return "some-file-name"; } }
    public class HelpInfoDisplayComponent : HelpInfoComponentBase, IYetaWFComponent<HelpInfoDefinition> {

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
        public async Task<string> RenderAsync(HelpInfoDefinition model) {

            if (model == null) return null;

            return await GetHelpFileContentsAsync(model);
        }

        public static async Task<string> GetHelpFileContentsAsync(HelpInfoDefinition model) {
            string url = VersionManager.GetAddOnPackageUrl(model.Package.AreaName);
            url = $"{url}Help/{model.Name}.html";
            string file = Utility.UrlToPhysical(url);
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(file)) {
                url = VersionManager.GetCustomUrlFromUrl(url);
                file = Utility.UrlToPhysical(url);
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(file))
                    return null;
            }
            return await FileSystem.FileSystemProvider.ReadAllTextAsync(file);
        }
    }
}
