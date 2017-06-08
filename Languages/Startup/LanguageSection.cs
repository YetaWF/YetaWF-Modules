/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

#if MVC6
#else

using System.IO;
using YetaWF.Core;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Startup {

    public class LanguageSection : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        public void InitializeApplicationStartup() {
            string rootFolder;
#if MVC6
            rootFolder = YetaWFManager.RootFolderWebProject;
#else
            rootFolder = YetaWFManager.RootFolder;
#endif
            YetaWF.Core.Language.LanguageSection.Init(Path.Combine(rootFolder, Globals.DataFolder, LanguageSettingsFile));
        }

        public const string LanguageSettingsFile = "LanguageSettings.json";
    }
}

#endif