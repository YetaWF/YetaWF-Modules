/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

#if MVC6
#else

using System.IO;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Startup {

    public class LanguageSection : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        /// <summary>
        /// Called when any node of a (single- or multi-instance) site is starting up.
        /// </summary>
        public async Task InitializeApplicationStartupAsync() {
            string rootFolder;
#if MVC6
            rootFolder = YetaWFManager.RootFolderWebProject;
#else
            rootFolder = YetaWFManager.RootFolder;
#endif
            await YetaWF.Core.Language.LanguageSection.InitAsync(Path.Combine(rootFolder, Globals.DataFolder, LanguageSettingsFile));
        }

        public const string LanguageSettingsFile = "LanguageSettings.json";
    }
}

#endif