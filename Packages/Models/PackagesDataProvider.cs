/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using YetaWF.Core.Addons;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Packages.DataProvider {
    // not a real data provider - used to clear/create all package data and initial web pages
    public partial class PackagesDataProvider : IInitializeApplicationStartup {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PackagesDataProvider() { }

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public void InitializeApplicationStartup() {
            BuiltinCommands.Add("/$initall", CoreInfo.Resource_BuiltinCommands, InitAll);
            BuiltinCommands.Add("/$initnew", CoreInfo.Resource_BuiltinCommands, InitNew);
            BuiltinCommands.Add("/$restart", CoreInfo.Resource_BuiltinCommands, RestartSite);
            BuiltinCommands.Add("/$initpackage", CoreInfo.Resource_BuiltinCommands, InitPackage);
            BuiltinCommands.Add("/$importdata", CoreInfo.Resource_BuiltinCommands, ImportData);
            BuiltinCommands.Add("/$processtemplate", CoreInfo.Resource_BuiltinCommands, ProcessTemplate);
            BuiltinCommands.Add("/$undotemplate", CoreInfo.Resource_BuiltinCommands, UndoTemplate);
        }

        // RESTART
        // RESTART
        // RESTART

        private void RestartSite(QueryHelper qs) {
            Manager.RestartSite(Manager.CurrentSite.MakeUrl());
        }

        // INITALL
        // INITALL
        // INITALL

        public static string LogFile {
            get { return Path.Combine(YetaWFManager.DataFolder, "InitialInstall.txt"); }
        }
        private static object _lockObject = new object();

        /// <summary>
        /// Installs all packages and builds the initial site from the import data (zip files) or from templates
        /// </summary>
        /// <remarks>
        /// This removes all data for all sites
        /// </remarks>
        public void InitAll(QueryHelper qs) {

            InitialSiteLogging log = new InitialSiteLogging(LogFile, _lockObject);
            Logging.RegisterLogging(log);

            Logging.AddLog("Site initialization starting");

            ClearAll();
            InstallPackages();
            if (qs["From"] == "Data") {
                BuildSiteUsingData(false);
                PermanentManager.ClearAll();// clear any cached objects
                BuildSiteUsingTemplate(Path.Combine(DataFolderName, "Add Site.txt"));
            } else { //if (qs["From"] == "Template") {
                BuildSiteUsingTemplate("InitialSite.txt");
                //BuildSiteUsingTemplate("Custom Site (Initial Site).txt");
            }
            Package.SavePackageMap();

            SiteDefinition.RemoveInitialInstall();
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
            private object _lockObject;
            public InitialSiteLogging(string logFile, object _lockObject) {
                LogFile = logFile;
                this._lockObject = _lockObject;
                lock (_lockObject) {
                    if (File.Exists(LogFile))
                        File.Delete(LogFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
                }
            }
            public Logging.LevelEnum GetLevel() { return Logging.LevelEnum.Info; }
            public void Clear() { }
            public void Flush() { }
            public bool IsInstalled() { return true; }
            public void WriteToLogFile(Logging.LevelEnum level, int relStack, string text) {
                lock (_lockObject) {
                    File.AppendAllText(LogFile, text + "\r\n");
                }
            }
        }

        public static List<string> RetrieveInitialInstallLog(out bool ended) {
            ended = false;
            if (!SiteDefinition.INITIAL_INSTALL || SiteDefinition.INITIAL_INSTALL_ENDED)
                ended = true;
            List<string> lines = new List<string>();
            bool success = false;
            while (!success) {
                try {
                    // This is horrible, polling until the file is no longer in use.
                    // The problem is we can't use statics or some form of caching as this is called by multiple separate requests
                    // and the Package package itself is replaced while we're logging, so we just use a file to hold all data.
                    // unfortunately even the _lockObject is lost when the Package package is replaced. Since this is only used
                    // during an initial install, it's not critical enough to make it perfect...
                    lock (_lockObject) {
                        if (!File.Exists(LogFile))
                            return lines;
                        lines = File.ReadAllLines(LogFile).ToList();
                        success = true;
                    }
                } catch (Exception) { Thread.Sleep(100); }
            }
            if (lines.Count == 0)
                return lines;
            if (lines.Last() == "+++DONE")
                ended = true;
            return lines;
        }

        /// <summary>
        /// Builds the current site (an additional site) using the new site template
        /// </summary>
        /// <param name="template"></param>
        public void InitNew(QueryHelper qs) {
            if (qs["From"] == "Data") {
                BuildSiteUsingData(false);
                BuildSiteUsingTemplate(Path.Combine(DataFolderName, "Add Site.txt"));
            } else { //if (qs["From"] == "Template") {
                BuildSiteUsingTemplate("NewSite.txt");
                //BuildSiteUsingTemplate("Custom Site (Additional Sites).txt");
            }
            // Cache is now invalid so we'll just restart
            Manager.RestartSite(Manager.CurrentSite.MakeUrl(ForceDomain: Manager.CurrentSite.SiteDomain));
        }

        private void InstallPackages() {

            Logging.AddLog("Installing packages");

            // get all packages that are available
            List<Package> neededPackages = Package.GetAvailablePackages();
            // order all available packages by service level
            neededPackages = (from p in neededPackages orderby (int)p.ServiceLevel select p).ToList();

            // keep track of installed packages
            List<Package> installedPackages = new List<Package>();
            List<Package> remainingPackages = (from p in neededPackages select p).ToList();

            // check each package and install it if all dependencies are available
            for (; neededPackages.Count() > installedPackages.Count();) {
                int count = 0;

                foreach (Package package in neededPackages) {
                    List<string> errorList = new List<string>();
                    Logging.AddLog("Installing package {0}", package.Name);
                    if (!installedPackages.Contains(package) && ArePackageDependenciesInstalled(package, installedPackages)) {
                        if (!package.InstallModels(errorList)) {
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
                if (count == 0) // we didn't install any additional packages
                    throw new InternalError("Not all packages could be installed");
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

        private void ClearAll() {

            try {
                // this can fail if there is no session (during initial install)
                YetaWFManager.SetRequestedDomain(null);
            } catch (Exception) { }

            Logging.AddLog("Removing all known tables (if any)");

            LocalizationSupport localizationSupport = new LocalizationSupport();
            localizationSupport.SetUseLocalizationResources(false);// turn off use of localization resources - things are about to be removed

            // turn off logging - things are about to be removed
            YetaWF.Core.Log.Logging.TerminateLogging();

            // SQL - drop all tables
            string sqlDboDefault, connDefault;
            DataProviderImpl.GetSQLInfo(out sqlDboDefault, out connDefault);
            if (!string.IsNullOrWhiteSpace(sqlDboDefault) && !string.IsNullOrWhiteSpace(connDefault)) {
                using (SQLDataProviderImpl sql = new SQLDataProviderImpl(sqlDboDefault, connDefault, null, NoLanguages: true)) {
                    sql.DropAllTables();
                }
            }

            // File - Remove all folders (not files - those could be the sql db)
            if (Directory.Exists(YetaWFManager.DataFolder)) {
                string[] dirs = Directory.GetDirectories(YetaWFManager.DataFolder);
                foreach (string dir in dirs) {
                    Logging.AddLog("Removing folder {0}", dir);
                    DirectoryIO.DeleteFolder(dir);
                }
            }
        }
        /// <summary>
        /// Installs all models for the specified package
        /// </summary>
        public void InitPackage(QueryHelper qs) {
            string packageName = qs["Package"];
            if (string.IsNullOrWhiteSpace(packageName))
                throw new InternalError("Package name missing");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!package.InstallModels(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantInstallModels", "Can't install models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
        }
        /// <summary>
        /// Imports all package data from a zip file (created using Export Data) or from templates
        /// </summary>
        public void ImportData(QueryHelper qs) {
            string zipFileName = qs["ZipFile"];
            if (string.IsNullOrWhiteSpace(zipFileName))
                throw new InternalError("Zip filename missing");
            List<string> errorList = new List<string>();
            if (!Package.ImportData(zipFileName, errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantImportData", "Can't import data from file {0}:(+nl)"), zipFileName);
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
        }
        /// <summary>
        /// Apply a template (creates pages)
        /// </summary>
        public void ProcessTemplate(QueryHelper qs) {
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(templateName);
        }
        /// <summary>
        /// Remove a template (removes pages)
        /// </summary>
        public void UndoTemplate(QueryHelper qs) {
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(templateName, false);
        }
    }
}
