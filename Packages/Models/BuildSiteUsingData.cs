/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.Packages.DataProvider {
    public partial class PackagesDataProvider {

        /// <summary>
        /// Builds the current site from the zip files in the Data folder
        /// </summary>
        /// <param name="includeNonSiteSpecifics"></param>
        public async Task BuildSiteUsingDataAsync(bool includeNonSiteSpecifics, List<Package> installedPackages = null) {
            Manager.ImportChunksNonSiteSpecifics = includeNonSiteSpecifics;
            List<string> files = await FileSystem.FileSystemProvider.GetFilesAsync(Path.Combine(TemplateFolder, DataFolderName), "*.zip");
            foreach (string file in files) {
                if (installedPackages == null || (from p in installedPackages where file.Contains($"{p.Name}.") select p).Any()) {
                    List<string> errorList = new List<string>();
                    Logging.AddLog("Restoring {0}", file);
                    if (!await Package.ImportDataAsync(file, errorList)) {
                        Logging.AddErrorLog($"Error restoring {file} - {errorList.First()}");
                        //throw new Error(errorList.First());
                    }
                }
            }
            Manager.ImportChunksNonSiteSpecifics = false;
            await Resource.ResourceAccess.ShutTheBackDoorAsync();
        }
    }
}
