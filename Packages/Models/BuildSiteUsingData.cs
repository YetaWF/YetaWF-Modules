/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Identity;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Packages.DataProvider {
    public partial class PackagesDataProvider {

        /// <summary>
        /// Builds the current site from the zip files in the Data folder
        /// </summary>
        /// <param name="includeNonSiteSpecifics"></param>
        public void BuildSiteUsingData(bool includeNonSiteSpecifics) {
            Manager.ImportChunksNonSiteSpecifics = includeNonSiteSpecifics;
            string[] files = Directory.GetFiles(Path.Combine(TemplateFolder, DataFolderName), "*.zip");
            foreach (string file in files) {
                List<string> errorList = new List<string>();
                if (!Package.ImportData(file, errorList)) {
                    throw new Error(errorList.First());
                }
            }
            Manager.ImportChunksNonSiteSpecifics = false;
            Resource.ResourceAccess.ShutTheBackDoor();
        }
    }
}
