﻿/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Backups.DataProvider {

    public class BackupEntry {
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public long Size { get; set; }
        public DateTime Created { get; set; }
    }

    public class BackupsDataProvider : DataProviderImpl {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BackupsDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        // File

        private IDataProvider<string, BackupEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, BackupEntry> CreateDataProvider() {
            return new FileDataProvider<string, BackupEntry>(
                Path.Combine(Manager.SiteFolder, SiteBackup.BackupFolder));
        }

        // API
        // API
        // API

        public List<BackupEntry> GetBackups(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total)
        {
            List<BackupEntry> backups = new List<BackupEntry>();
            List<string> files = DataProvider.GetKeyList();
            foreach (string file in files) {
                DateTime dateTime;
                string filename = Path.GetFileNameWithoutExtension(file);
                DateTime.TryParseExact(filename, string.Format(SiteBackup.BackupFileFormat, SiteBackup.BackupDateTimeFormat), new DateTimeFormatInfo(), System.Globalization.DateTimeStyles.None, out dateTime);
                FileInfo fi = new FileInfo(Path.Combine(Manager.SiteFolder, SiteBackup.BackupFolder, file));
                long fileSize = fi.Length;
                BackupEntry backup = new BackupEntry() {
                    FileName = filename,
                    FullFileName = file,
                    Created = dateTime,
                    Size = fileSize,
                };
                backups.Add(backup);
            }
            return DataProviderImpl<BackupEntry>.GetRecords(backups, skip, take, sort, filters, out total);
        }

    }
}
