/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
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

        // INITALL
        // INITALL
        // INITALL

        private void RestartSite(NameValueCollection qs) {
            Manager.RestartSite();
            Manager.CurrentResponse.Redirect(Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain));
        }

        /// <summary>
        /// Installs all packages and builds the initial site from the import data (zip files) or from templates
        /// </summary>
        /// <remarks>
        /// This removes all data for all sites
        /// </remarks>
        public void InitAll(NameValueCollection qs) {
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
            SiteDefinition.RemoveInitialInstall();

            // Cache is now invalid so we'll just restart
            Manager.RestartSite();
            Manager.CurrentResponse.Redirect(Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain));
        }

        /// <summary>
        /// Builds the current site (an additional site) using the new site template
        /// </summary>
        /// <param name="template"></param>
        public void InitNew(NameValueCollection qs) {
            if (qs["From"] == "Data") {
                BuildSiteUsingData(false);
                BuildSiteUsingTemplate(Path.Combine(DataFolderName, "Add Site.txt"));
            } else { //if (qs["From"] == "Template") {
                BuildSiteUsingTemplate("NewSite.txt");
                //BuildSiteUsingTemplate("Custom Site (Additional Sites).txt");
            }
            // Cache is now invalid so we'll just restart
            Manager.RestartSite();
            Manager.CurrentResponse.Redirect(Manager.CurrentSite.MakeUrl(RealDomain: Manager.CurrentSite.SiteDomain));
        }

        private void InstallPackages() {

            // get all packages that are available
            List<Package> neededPackages = Package.GetAvailablePackages();
            // order all available packages by service level
            neededPackages = (from p in neededPackages orderby (int) p.ServiceLevel select p).ToList();

            // keep track of installed packages
            List<Package> installedPackages = new List<Package>();
            List<Package> remainingPackages = (from p in neededPackages select p).ToList();

            // check each package and install it if all dependencies are available
            for ( ; neededPackages.Count() > installedPackages.Count() ; ) {
                int count = 0;

                foreach (Package package in neededPackages) {
                    List<string> errorList = new List<string>();
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

            YetaWFManager.SetRequestedDomain(null);

            LocalizationSupport localizationSupport = new LocalizationSupport();
            localizationSupport.SetUseLocalizationResources(false);// turn off use of localization resources - things are about to be removed

            // turn off logging - things are about to be removed
            YetaWF.Core.Log.Logging.TerminateLogging();

            // SQL - drop all tables
            string sqlDboDefault, connDefault;
            DataProviderImpl.GetSQLInfo(out sqlDboDefault, out connDefault);
            if (!string.IsNullOrWhiteSpace(sqlDboDefault) && !string.IsNullOrWhiteSpace(connDefault)) {
                SQLDataProviderImpl sql = new SQLDataProviderImpl(sqlDboDefault, connDefault, null, NoLanguages: true);
                sql.DropAllTables();
            }

            // File - Remove all folders (not files - those could be the sql db)
            if (Directory.Exists(YetaWFManager.DataFolder)) {
                string[] dirs = Directory.GetDirectories(YetaWFManager.DataFolder);
                foreach (string dir in dirs) {
                    Directory.Delete(dir, true);
                }
            }
        }
        /// <summary>
        /// Installs all models for the specified package
        /// </summary>
        public void InitPackage(NameValueCollection qs) {
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
        public void ImportData(NameValueCollection qs) {
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
        public void ProcessTemplate(NameValueCollection qs) {
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(templateName);
        }
        /// <summary>
        /// Remove a template (removes pages)
        /// </summary>
        public void UndoTemplate(NameValueCollection qs) {
            string templateName = qs["Template"];
            if (string.IsNullOrWhiteSpace(templateName))
                throw new InternalError("Template name missing");
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            packagesDP.BuildSiteUsingTemplate(templateName, false);
        }
    }
}
