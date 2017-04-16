/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.DataProvider {

    public class LanguageDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup, ILanguages {

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            // The SiteDefinitionDataProvider has two permanent disposable objects
            LanguageInfo.LanguagesAccess = (ILanguages) this;
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LanguageDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public LanguageDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<string, LanguageData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<string, LanguageData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName),
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<string, LanguageData>(AreaName, SQLDbo, SQLConn,
                                NoLanguages: true,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<string, LanguageData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public LanguageData GetItem(string id) {
            return DataProvider.Get(id);
        }
        public bool AddItem(LanguageData data) {
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(LanguageData data) {
            return UpdateItem(data.Id, data);
        }
        public UpdateStatusEnum UpdateItem(string originalId, LanguageData data) {
            return DataProvider.Update(originalId, data.Id, data);
        }
        public bool RemoveItem(string id) {
            return DataProvider.Remove(id);
        }

        public List<LanguageData> GetItems(List<DataProviderFilterInfo> filters) {
            int total;
            return DataProvider.GetRecords(0, 0, null, filters, out total);
        }
        public List<LanguageData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            bool status = DataProvider.InstallModel(errorList);
            if (!status) return false;
            // install default language (need at least one)
            DataProvider.Add(GetDefaultLanguageData());
#if DEBUG
            DataProvider.Add(new LanguageData {
                Id = "de-DE",
                ShortName = "German",
                Description = "German"
            });
            DataProvider.Add(new LanguageData {
                Id = "fr-FR",
                ShortName = "French",
                Description = "French" });
#endif
            return true;
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
        private LanguageData GetDefaultLanguageData() {
            return new LanguageData {
                Id = "en-US",
                ShortName = "US English",
                Description = "US English - default language used throughout YetaWF"
            };
        }

        // ILANGUAGES
        // ILANGUAGES
        // ILANGUAGES

        public List<LanguageData> GetAllLanguages() {
            List<LanguageData> list;
            if (!IsInstalled())
                list = new List<LanguageData>() { GetDefaultLanguageData() };
            else
                list = this.GetItems(null);
            return (from l in list select new LanguageData {
                Id = l.Id,
                Description = l.Description,
                ShortName = l.ShortName,
            }).ToList();
        }
    }
}
