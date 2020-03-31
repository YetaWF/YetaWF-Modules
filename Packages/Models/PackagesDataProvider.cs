/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Core.IO;
using YetaWF.Core;

namespace YetaWF.Modules.Packages.DataProvider {
    // not a real data provider - used to clear/create all package data and initial web pages
    public partial class PackagesDataProvider : IInitializeApplicationStartup {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PackagesDataProvider() { }

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public Task InitializeApplicationStartupAsync() {
            BuiltinCommands.Add("/$initall", CoreInfo.Resource_BuiltinCommands, InitAllAsync);
            BuiltinCommands.Add("/$restart", CoreInfo.Resource_BuiltinCommands, RestartSiteAsync);
            BuiltinCommands.Add("/$initpackage", CoreInfo.Resource_BuiltinCommands, InitPackageAsync);
            BuiltinCommands.Add("/$importdata", CoreInfo.Resource_BuiltinCommands, ImportDataAsync);
            BuiltinCommands.Add("/$processtemplate", CoreInfo.Resource_BuiltinCommands, ProcessTemplateAsync);
            BuiltinCommands.Add("/$undotemplate", CoreInfo.Resource_BuiltinCommands, UndoTemplateAsync);
            return Task.CompletedTask;
        }

        // RESTART
        // RESTART
        // RESTART

        private Task RestartSiteAsync(QueryHelper qs) {
            Manager.RestartSite(Manager.CurrentSite.MakeUrl());
            return Task.CompletedTask;
        }

        // INITALL
        // INITALL
        // INITALL

        public static string LogFile {
            get {
                string rootFolder;
#if MVC6
                rootFolder = YetaWFManager.RootFolderWebProject;
#else
                rootFolder = YetaWFManager.RootFolder;
#endif
                string folder = Path.Combine(rootFolder, Globals.DataFolder);
                return Path.Combine(folder, "InitialInstall.txt");
            }
        }

        /// <summary>
        /// Installs all packages and builds the initial site from the import data (zip files) or from templates
        /// </summary>
        /// <remarks>
        /// This removes all data for all sites
        /// </remarks>
        public Task InitAllAsync(QueryHelper qs) {
            return InitAllAsync(qs);
        }

        /// <summary>
        /// Installs all packages and builds the initial site from the import data (zip files) or from templates
        /// </summary>
        /// <remarks>
        /// This removes all data for all sites
        /// </remarks>
        public async Task InitAllAsync(QueryHelper qs, List<string> WantedPackages) {

            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing packages is not possible when distributed caching is enabled");

            InitialSiteLogging log = new InitialSiteLogging(LogFile);
            await Logging.RegisterLoggingAsync(log);

            Logging.AddLog("Site initialization starting");

            //ClearAll();
            List<Package> installedPackages = await InstallPackagesAsync(WantedPackages);
            if (qs["From"] == "Data") {
                await BuildSiteUsingDataAsync(false, installedPackages);
                await BuildSiteUsingTemplateAsync(Path.Combine(DataFolderName, "Add Site.txt"));
            } else { //if (qs["From"] == "Template") {
                await BuildSiteUsingTemplateAsync("InitialSite.txt");
                //BuildSiteUsingTemplate("Custom Site (Initial Site).txt");
            }
            PermanentManager.ClearAll();// clear any cached objects
            await Package.SavePackageMapAsync();

            await SiteDefinition.RemoveInitialInstallAsync();
            Logging.UnregisterLogging(log);

            Logging.AddLog("Site initialization done");

            // Cache is now invalid so we'll just restart
#if MVC6
            //Manager.RestartSite(); // don't restart or redirect (can't)
            // tell user in browser what to do
#else
            // Cache is now invalid so we'll just restart
            Manager.RestartSite();
#endif
        }
        public class InitialSiteLogging : ILogging {

            private string LogFile;

            public InitialSiteLogging(string logFile) {
                LogFile = logFile;
            }
            public async Task InitAsync() {
                using (ILockObject lockObject = await FileSystem.FileSystemProvider.LockResourceAsync(LogFile)) {
                    if (await FileSystem.FileSystemProvider.FileExistsAsync(LogFile))
                        await FileSystem.FileSystemProvider.DeleteFileAsync(LogFile);
                    await FileSystem.FileSystemProvider.CreateDirectoryAsync(Path.GetDirectoryName(LogFile));
                    await lockObject.UnlockAsync();
                }
            }
            public Logging.LevelEnum GetLevel() { return Logging.LevelEnum.Info; }
            public Task ClearAsync() { return Task.CompletedTask; }
            public Task FlushAsync() { return Task.CompletedTask; }
            public Task<bool> IsInstalledAsync() { return Task.FromResult(true); }
            public void WriteToLogFile(string category, Logging.LevelEnum level, int relStack, string text) {
                YetaWFManager.Syncify(async () => { // Logging is sync by definition (this is only used for startup logging)
                    using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync(LogFile)) {
                        await FileSystem.FileSystemProvider.AppendAllTextAsync(LogFile, text + "\r\n");
                        await lockObject.UnlockAsync();
                    }
                });
            }
            /// <summary>
            /// Defines whether the logging data provider is already logging an event.
            /// </summary>
            bool ILogging.IsProcessing { get; set; }
        }

        public class RetrieveInitialInstallLogInfo {
            public bool Ended { get; set; }
            public List<string> Lines { get; set; }
        }

        public static async Task<RetrieveInitialInstallLogInfo> RetrieveInitialInstallLogAsync() {
            RetrieveInitialInstallLogInfo info = new RetrieveInitialInstallLogInfo();
            info.Ended = false;
            if (!SiteDefinition.INITIAL_INSTALL || SiteDefinition.INITIAL_INSTALL_ENDED)
                info.Ended = true;
            bool success = false;
            while (!success) {
                try {
                    // This is horrible, polling until the file is no longer in use.
                    // The problem is we can't use statics or some form of caching as this is called by multiple separate requests
                    // and the Package package itself is replaced while we're logging, so we just use a file to hold all data.
                    // unfortunately even the lockObject is lost when the Package package is replaced. Since this is only used
                    // during an initial install, it's not critical enough to make it perfect...
                    using (ILockObject lockObject = await FileSystem.FileSystemProvider.LockResourceAsync(LogFile)) {
                        if (await FileSystem.FileSystemProvider.FileExistsAsync(LogFile)) {
                            info.Lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(LogFile);
                            success = true;
                        }
                        await lockObject.UnlockAsync();
                    }
                } catch (Exception) {
                    if (YetaWFManager.IsSync())
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50));// wait a while - this is bad, only works because "other" instance has lock
                    else
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 50));// wait a while
                }
            }
            if (info.Lines.Count == 0)
                return info;
            if (info.Lines.Last() == "+++DONE")
                info.Ended = true;
            return info;
        }

        /// <summary>
        /// Builds the current site (an additional site) using the new site template
        /// </summary>
        /// <param name="template"></param>
        public async Task InitNewAsync(bool data = false) {

            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Building a new site is not possible when distributed caching is enabled");

            if (data) {
                await BuildSiteUsingDataAsync(false);
                await BuildSiteUsingTemplateAsync(Path.Combine(DataFolderName, "Add Additional Site.txt"));
            } else {
                await BuildSiteUsingTemplateAsync("NewSite.txt");
            }
            // Cache is now invalid so we need to restart
        }

        private async Task<List<Package>> InstallPackagesAsync(List<string> wantedPackages) {

            Logging.AddLog("Installing packages");

            // get all packages that are available
            List<Package> allPackages = Package.GetAvailablePackages();
            // get packages to install, limit to wanted packages
            List<Package> remPackages = allPackages.ToList();
            if (wantedPackages != null)
                remPackages = (from p in remPackages where wantedPackages.Contains(p.Name) select p).ToList();
            // add all required dependencies from packages even if they were not initially wanted
            foreach (Package remPackage in remPackages.ToList())
                AddDependencies(remPackages, remPackage);

            // order all available packages by service level
            remPackages = (from p in remPackages orderby (int)p.ServiceLevel select p).ToList();

            // keep track of installed packages
            List<Package> installedPackages = new List<Package>();
            List<Package> remainingPackages = (from p in remPackages select p).ToList();

            // check each package and install it if all dependencies are available
            for ( ; remPackages.Count() > installedPackages.Count();) {

                int count = 0;

                foreach (Package package in remPackages) {
                    List<string> errorList = new List<string>();

                    if (!installedPackages.Contains(package)) {
                        Logging.AddLog("Installing package {0}", package.Name);
                        if (ArePackageDependenciesInstalled(package, installedPackages)) {
                            if (!await package.InstallModelsAsync(errorList)) {
                                ScriptBuilder sb = new ScriptBuilder();
                                sb.Append(this.__ResStr("cantInstallPackage", "Can't install package {0}:(+nl)"), package.Name);
                                sb.Append(errorList, LeadingNL: true);
                                throw new Error(sb.ToString());
                            }
                            ++count;
                            installedPackages.Add(package);
                            remainingPackages.Remove(package);
                        }
                    }
                }
                if (count == 0) { // we didn't install any additional packages
                    Logging.AddErrorLog("Not all packages could be installed");
                    foreach (Package package in remainingPackages) {
                        List<string> reqPackages = package.GetRequiredPackages();
                        Logging.AddErrorLog($"Package {package.Name} not installed - Requires {string.Join(", ", reqPackages)}");
                        foreach (string dep in reqPackages) {
                            if (!(from p in installedPackages where p.Name == dep select p).Any())
                                Logging.AddErrorLog($"  Package dependency {dep} not installed and is required by {package.Name}");
                        }
                    }
                    throw new InternalError("Not all packages could be installed");
                }
            }
            return installedPackages;
        }

        private void AddDependencies(List<Package> remPackages, Package remPackage) {
            List<string> reqPackages = remPackage.GetRequiredPackages();
            foreach (string reqPackage in reqPackages) {
                Package depPackage = Package.GetPackageFromPackageName(reqPackage);
                if (depPackage == null)
                    throw new InternalError($"Package {remPackage.Name} lists a dependency package named {reqPackage} which doesn't exist");
                if (!remPackages.Contains(depPackage)) {// add dependent package
                    remPackages.Add(depPackage);
                    AddDependencies(remPackages, depPackage);// add recursive dependencies
                }
            }
        }

        private bool ArePackageDependenciesInstalled(Package package, List<Package> installedPackages) {
            List<string> reqPackages = package.GetRequiredPackages();
            foreach (string reqPackage in reqPackages) {
                bool found = false;
                foreach (Package instPackage in installedPackages) {
                    if (instPackage.Name == reqPackage) {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }

        //private void ClearAll() {

        //    try {
        //        // this can fail if there is no session (during initial install)
        //        YetaWFManager.SetRequestedDomain(null);
        //    } catch (Exception) { }

        //    Logging.AddLog("Removing all known tables (if any)");

        //    LocalizationSupport localizationSupport = new LocalizationSupport();
        //    localizationSupport.SetUseLocalizationResources(false);// turn off use of localization resources - things are about to be removed

        //    // turn off logging - things are about to be removed
        //    YetaWF.Core.Log.Logging.TerminateLogging();

        //    // SQL - drop all tables
        //    string ioModeDefault = WebConfigHelper.GetValue<string>(DataProviderImpl.DefaultString, DataProviderImpl.IOModeString);
        //    if (string.IsNullOrWhiteSpace(ioModeDefault))
        //        throw new InternalError("Default IOMode is missing");
        //    if (ioModeDefault.ToLower() == "sql") {
        //        string sqlDboDefault, connDefault;
        //        DataProviderImpl.GetSQLInfo(out sqlDboDefault, out connDefault);
        //        if (!string.IsNullOrWhiteSpace(sqlDboDefault) && !string.IsNullOrWhiteSpace(connDefault)) {
        //            using (SQLDataProviderImpl sql = new SQLDataProviderImpl(sqlDboDefault, connDefault, null, NoLanguages: true)) {
        //                sql.DropAllTables();
        //            }
        //        }
        //    }
        //    // File - Remove all folders (not files - those could be the sql db)
        //    if (Directory.Exists(YetaWFManager.DataFolder)) {
        //        string[] dirs = Directory.GetDirectories(YetaWFManager.DataFolder);
        //        foreach (string dir in dirs) {
        //            Logging.AddLog("Removing folder {0}", dir);
        //            DirectoryIO.DeleteFolder(dir);
        //        }
        //    }
        //}

        /// <summary>
        /// Installs all models for the specified package
        /// </summary>
        public async Task InitPackageAsync(QueryHelper qs) {

            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new packagesis not possible when distributed caching is enabled");

            string packageName = qs["Package"];
            if (string.IsNullOrWhiteSpace(packageName))
                throw new InternalError("Package name missing");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.InstallModelsAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantInstallModels", "Can't install models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
        }
        /// <summary>
        /// Imports all package data from a zip file (created using Export Data) or from templates
        /// </summary>
        public async Task ImportDataAsync(QueryHelper qs) {
            string zipFileName = qs["ZipFile"];
            if (string.IsNullOrWhiteSpace(zipFileName))
                throw new InternalError("Zip filename missing");
            List<string> errorList = new List<string>();
            if (!await Package.ImportDataAsync(zipFileName, errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantImportData", "Can't import data from file {0}:(+nl)"), zipFileName);
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
        }
        /// <summary>
        /// Apply a template (creates pages)
        /// </summary>
        public async Task ProcessTemplateAsync(QueryHelper qs) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Processing site templates is not possible when distributed caching is enabled");
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            await packagesDP.BuildSiteUsingTemplateAsync(templateName);
        }
        /// <summary>
        /// Remove a template (removes pages)
        /// </summary>
        public async Task UndoTemplateAsync(QueryHelper qs) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Processing site templates is not possible when distributed caching is enabled");
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            await packagesDP.BuildSiteUsingTemplateAsync(templateName, false);
        }
    }
}
