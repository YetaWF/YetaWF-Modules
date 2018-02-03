/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Backups.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.ConfigDataProvider), typeof(ConfigDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.BackupsDataProvider), typeof(BackupsDataProvider));
        }
        class ConfigDataProvider : FileDataProvider<int, ConfigData> {
            public ConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
        }
        internal class BackupsDataProvider : FileDataProvider<string, BackupEntry> {

            public BackupsDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.Manager.SiteFolder, SiteBackup.BackupFolder); }

            internal List<BackupEntry> GetBackups(int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters, out int total) {

                List<BackupEntry> backups = new List<BackupEntry>();
                List<string> files = FileDataProvider<string, BackupEntry>.GetListOfKeys(BaseFolder);
                foreach (string file in files) {
                    DateTime dateTime;
                    string filename = Path.GetFileNameWithoutExtension(file);
                    DateTime.TryParseExact(filename, string.Format(SiteBackup.BackupFileFormat, SiteBackup.BackupDateTimeFormat), new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
                    FileInfo fi = new FileInfo(Path.Combine(BaseFolder, file));
                    long fileSize = fi.Length;
                    BackupEntry backup = new BackupEntry() {
                        FileName = filename,
                        FullFileName = file,
                        Created = dateTime,
                        Size = fileSize,
                    };
                    backups.Add(backup);
                }
                return DataProviderImpl<BackupEntry>.GetRecords(backups, skip, take, sorts, filters, out total);
            }
        }
    }
}
