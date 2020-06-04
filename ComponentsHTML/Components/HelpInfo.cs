/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

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
    /// Displays the help file identified by the model. The model contains a file name (without path or extension).
    /// </summary>
    /// <example>
    /// [Caption(""), Description("")]
    /// [UIHint("HelpInfo"), ReadOnly]
    /// [SuppressEmpty]
    /// public HelpInfoDefinition HelpInformation { get { return new HelpInfoDefinition { Package = AreaRegistration.CurrentPackage, Name = typeof(AddBacklinkingDocumentModuleController).FullName }; } }
    /// </example>
    /// <remarks>Help files are located in the specified package's folder ./Addons/_Main/Help and have the extension .html.
    /// If the model is null or the help file doesn't exist, nothing is rendered.
    ///
    /// The help information uses language-specific files based on the user's defined language. The custom folders are searched first.
    /// Help files are cached if the are smaller than 1K of data.
    ///
    /// The search path is as follows (in this example, the user's defined language is DE-de):
    /// ./AddonsCustom/..sitename../..Package.Domain../..Package.Product../_Main/Help/DE-de/..filename...html
    /// ./AddonsCustom/..sitename../..Package.Domain../..Package.Product../_Main/Help/..filename...html
    /// ..package../Addons/_Main/Help/DE-de/..filename...html
    /// ..package../Addons/_Main/Help/..filename...html
    /// </remarks>
    public class HelpInfoDisplayComponent : HelpInfoComponentBase, IYetaWFComponent<HelpInfoDefinition> {

        internal const int MAXSIZE = 1024;

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

            if (model == null)
                return null;
            string contents = await GetHelpFileContentsAsync(model);
            if (contents == null)
                return null;
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div class='yt_helpinfo'>
    {contents}
</div>
            ");
            return hb.ToString();
        }

        private async Task<string> GetHelpFileContentsAsync(HelpInfoDefinition model) {
            string urlBase = $"{VersionManager.GetAddOnPackageUrl(model.Package.AreaName)}Help";
            string urlCustomBase = VersionManager.GetCustomUrlFromUrl(urlBase);
            string file = $"{model.Name}.html";
            string lang = MultiString.ActiveLanguage;

            // try custom language specific
            HelpFileInfo helpCustomLang = await TryHelpFileAsync($"{urlCustomBase}/{lang}/{file}", model.UseCache);
            if (helpCustomLang.Exists)
                return helpCustomLang.Contents;

            // try custom
            HelpFileInfo helpCustom = await TryHelpFileAsync($"{urlCustomBase}/{file}", model.UseCache);
            if (helpCustom.Exists)
                return helpCustom.Contents;

            // try fallback language specific
            HelpFileInfo helpLang = await TryHelpFileAsync($"{urlBase}/{lang}/{file}", model.UseCache);
            if (helpLang.Exists)
                return helpLang.Contents;

            // try fallback
            HelpFileInfo help = await TryHelpFileAsync($"{urlBase}/{file}", model.UseCache);
            if (help.Exists)
                return help.Contents;

            return null;
        }

        private class HelpFileInfo {
            public bool Exists { get; set; }
            public string Contents { get; set; }
        }

        private async Task<HelpFileInfo> TryHelpFileAsync(string url, bool useCache) {

            string file = Utility.UrlToPhysical(url);
            using (ICacheDataProvider cacheDP = YetaWF.Core.IO.Caching.GetStaticSmallObjectCacheProvider()) {

                // Check cache first
                GetObjectInfo<HelpFileInfo> cache = await cacheDP.GetAsync<HelpFileInfo>(file);
                if (cache.Success)
                    return cache.Data;

                // read file
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(file)) {
                    HelpFileInfo noInfo = new HelpFileInfo {
                        Contents = null,
                        Exists = false,
                    };
                    await cacheDP.AddAsync<HelpFileInfo>(file, noInfo);// failure also saved in cache
                    return noInfo;
                }
                string contents = await FileSystem.FileSystemProvider.ReadAllTextAsync(file);

                HelpFileInfo info = new HelpFileInfo {
                    Contents = contents,
                    Exists = true,
                };
                // save in cache
                if (contents.Length < MAXSIZE && useCache)
                    await cacheDP.AddAsync<HelpFileInfo>(file, info);
                return info;
            }
        }
    }
}
