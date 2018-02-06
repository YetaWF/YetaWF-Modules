/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Logging.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {
        public void Register() {
            // registration not used - log provider defined in appsettings
        }
    }

    public class LogRecordDataProvider : YetaWF.Modules.Logging.DataProvider.LogRecordDataProvider, IInstallableModel, ILogging {

        static protected object lockObject = new object();

        private readonly string LogfileName = "Logfile.txt";
        string LogFile;

        private const int MAXRECORDS = 1000;// cache # of records
        List<string> LogCache { get; set; }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LogRecordDataProvider() : base() {
            // can't use CurrentPackage as RegisterAllAreas has not yet been called 
            Package package = Package.GetPackageFromAssembly(GetType().Assembly);
            LogFile = Path.Combine(YetaWFManager.DataFolder, package.AreaName, LogfileName);
        }

        // API
        // API
        // API

        public override void Clear() {
            lock (lockObject) {
                try {
                    System.IO.File.Delete(LogFile);
                } catch (Exception) { }
                Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
                LogCache = new List<string>();
            }
        }

        public override void Flush() {
            lock (lockObject) {
                if (LogCache != null)
                    System.IO.File.AppendAllLines(LogFile, LogCache);
                LogCache = new List<string>();
            }
        }

        public override void SaveMessage(LogRecord record) {

            string text = string.Format("{0}-{1}-{2}-{3}-{4}-{5}({6})-{7}: {8},{9},{10},{11} - {12}:{13}",
                DateTime.Now/*Local Time*/, record.SessionId, record.SiteIdentity, record.IPAddress, record.RequestedUrl, record.UserName, record.UserId, record.ReferrerUrl,
                    record.ModuleName,
                    record.Class,
                    record.Method,
                    record.Namespace,
                    record.Level, record.Info);
            text = text.Replace("\n", "\r\n");

            lock (lockObject) {
                LogCache.Add(text);
                if (LogCache.Count >= MAXRECORDS)
                    Flush();
            }
        }

        public override LogRecord GetItem(int key) {
            throw new NotImplementedException();
        }
        public override bool RemoveItem(int key) {
            throw new NotImplementedException();
        }

        public override List<LogRecord> GetItems(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public override List<LogRecord> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            throw new NotImplementedException();
        }
        public override int RemoveItems(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public override bool CanBrowse {
            get {
                return CanImportOrExport;
            }
        }
        public override bool CanImportOrExport {
            get {
                return false;
            }
        }
        public override string GetLogFileName() {
            return LogFile;
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public override bool IsInstalled() {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return false;
            if (_isInstalled == null)
                _isInstalled = Directory.Exists(Path.GetDirectoryName(LogFile));
            return (bool)_isInstalled;
        }
        private bool? _isInstalled;
        public bool InstallModel(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
            YetaWF.Core.Log.Logging.SetupLogging();
            _isInstalled = true;
            return true;
        }
        public bool UninstallModel(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            YetaWF.Core.Log.Logging.TerminateLogging();
            try {
                System.IO.File.Delete(LogFile);
            } catch (Exception) { }
            _isInstalled = false;
            return true;
        }
        public void AddSiteData() { }
        public void RemoveSiteData() { }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we're not exporting any data
            obj = null;
            return false;
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we're not importing any data
        }
    }
}

