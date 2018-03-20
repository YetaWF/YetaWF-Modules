/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

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

        public Task InitializeApplicationStartupAsync(bool firstNode) {
            string rootFolder;
#if MVC6
            rootFolder = YetaWFManager.RootFolderWebProject;
#else
            rootFolder = YetaWFManager.RootFolder;
#endif
            YetaWF.Core.Language.LanguageSection.Init(Path.Combine(rootFolder, Globals.DataFolder, LanguageSettingsFile));
            return Task.CompletedTask;
        }

        public const string LanguageSettingsFile = "LanguageSettings.json";
    }
}

#endif