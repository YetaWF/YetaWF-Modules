/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Pages.DataProvider {

    public class UnifiedSetData {

        public const int MaxUnifiedSetGuid = 50;
        public const int MaxName = 80;
        public const int MaxDescription = 1000;

        /// <summary>
        /// The internal id for this page set.
        /// </summary>
        [Data_PrimaryKey, StringLength(MaxUnifiedSetGuid)]
        public Guid UnifiedSetGuid { get; set; }

        /// <summary>
        /// User provided name for this page set.
        /// </summary>
        [StringLength(MaxName)]
        public string Name { get; set; }

        /// <summary>
        /// User provided description for this page set.
        /// </summary>
        [StringLength(MaxDescription)]
        public string Description { get; set; }

        /// <summary>
        /// Defines how combined content is rendered.
        /// </summary>
        public PageDefinition.UnifiedModeEnum UnifiedMode { get; set; }
        /// <summary>
        /// The duration of the animation (in milliseconds) when scrolling to content.
        /// </summary>
        /// <remarks>Scrolling to the end of the page would take the specified number of milliseconds.
        /// If the scrolling distance is less than the entire page, the duration is proportionally reduced so the scroll speed would be similar in all cases.
        /// </remarks>
        public int UnifiedAnimation { get; set; }

        /// <summary>
        /// Date/time this entry was created.
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Date/time this entry was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// The list of pages (by Url) which are part of this unified page set.
        /// </summary>
        /// <remarks>They are in the order that content is rendered within a unified page.</remarks>
        [Data_DontSave]
        public List<string> PageList { get; set; }
        /// <summary>
        /// The list of pages (by Guid) which are part of this unified page set.
        /// </summary>
        /// <remarks>They are in the order that content is rendered within a unified page.</remarks>
        [Data_Binary]
        public SerializableList<Guid> PageGuids { get; set; }

        public UnifiedSetData() {
            UnifiedMode = PageDefinition.UnifiedModeEnum.ShowDivs;
            UnifiedAnimation = 1000;
            PageList = new List<string>();
            PageGuids = new SerializableList<Guid>();
        }
    }

    public class UnifiedSetDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

        // IINITIALIZEAPPLICATIONSTARTUP
        // IINITIALIZEAPPLICATIONSTARTUP
        // IINITIALIZEAPPLICATIONSTARTUP

        public void InitializeApplicationStartup() {
            PageDefinition.GetUnifiedPagesFromPageGuid = GetUnifiedPagesFromPageGuid;
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public UnifiedSetDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public UnifiedSetDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<Guid, UnifiedSetData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<Guid, UnifiedSetData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName + "_UnifiedSets", SiteIdentity.ToString()),
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<Guid, UnifiedSetData>(AreaName + "_UnifiedSets", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<Guid, UnifiedSetData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public void DoAction(Guid unifiedSetGuid, Action action) {
            StringLocks.DoAction(LockKey(unifiedSetGuid), () => {
                action();
            });
        }
        private string LockKey(Guid unifiedSetGuid) {
            return string.Format("{0}_{1}", this.AreaName, unifiedSetGuid);
        }
        public UnifiedSetData GetItem(Guid unifiedSetGuid) {
            UnifiedSetData unifiedSet = DataProvider.Get(unifiedSetGuid);
            if (unifiedSet == null) return null;
            unifiedSet.PageList = GetPageListFromGuids(unifiedSet.PageGuids);
            return unifiedSet;
        }
        public bool AddItem(UnifiedSetData unifiedSet) {
            unifiedSet.UnifiedSetGuid = Guid.NewGuid();
            unifiedSet.Created = DateTime.UtcNow;
            unifiedSet.PageGuids = UpdatePageGuids(unifiedSet.UnifiedSetGuid, unifiedSet.PageList);
            if (!DataProvider.Add(unifiedSet)) return false;
            return true;
        }
        public UpdateStatusEnum UpdateItem(UnifiedSetData unifiedSet) {
            unifiedSet.Updated = DateTime.UtcNow;
            unifiedSet.PageGuids = UpdatePageGuids(unifiedSet.UnifiedSetGuid, unifiedSet.PageList);
            UpdateStatusEnum status = DataProvider.Update(unifiedSet.UnifiedSetGuid, unifiedSet.UnifiedSetGuid, unifiedSet);
            if (status != UpdateStatusEnum.OK) return status;
            return status;
        }
        public bool RemoveItem(Guid unifiedSetGuid) {
            if (!DataProvider.Remove(unifiedSetGuid)) return false;
            RemoveGuid(unifiedSetGuid);
            return true;
        }
        public List<UnifiedSetData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }

        // PageList
        // PageList
        // PageList

        private PageDefinition.UnifiedInfo GetUnifiedPagesFromPageGuid(Guid unifiedSetGuid) {
            UnifiedSetData unifiedSet = GetItem(unifiedSetGuid);
            if (unifiedSet == null) return null;
            return new PageDefinition.UnifiedInfo {
                PageGuids = unifiedSet.PageGuids,
                Animation = unifiedSet.UnifiedAnimation,
                Mode = unifiedSet.UnifiedMode,
            };
        }
        private List<string> GetPageListFromGuids(SerializableList<Guid> pageGuids) {
            List<string> pages = new List<string>();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                foreach (Guid pageGuid in pageGuids) {
                    PageDefinition page = pageDP.LoadPageDefinition(pageGuid);
                    if (page != null)
                        pages.Add(page.Url);
                }
            }
            return pages;
        }
        private SerializableList<Guid> UpdatePageGuids(Guid unifiedSetGuid, List<string> pageList) {
            pageList = pageList != null ? pageList : new List<string>();
            SerializableList<Guid> pageGuids = new SerializableList<Guid>();
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                // Get all pages that are currently part of the unified page set
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UnifiedSetGuid", Operator = "==", Value = unifiedSetGuid });
                int total;
                List<PageDefinition> pageDefs = pageDP.GetItems(0, 0, null, filters, out total);
                // translate page list to guid list (preserving order)
                foreach (string page in pageList) {
                    PageDefinition pageDef = pageDP.LoadPageDefinition(page);
                    if (pageDef != null) {
                        pageGuids.Add(pageDef.PageGuid);
                        // check if it's already in the list
                        PageDefinition pageFound = (from p in pageDefs where p.Url == page select p).FirstOrDefault();
                        if (pageFound == null) {
                            // page not in list, add it
                            pageDef.UnifiedSetGuid = unifiedSetGuid;
                            pageDP.SavePageDefinition(pageDef);
                        } else if (pageFound.UnifiedSetGuid != unifiedSetGuid) {
                            // page in list but with the wrong unifiedSetGuid
                            pageDef.UnifiedSetGuid = unifiedSetGuid;
                            pageDP.SavePageDefinition(pageDef);
                            pageDefs.Remove(pageFound);
                        } else {
                            // page already in list
                            pageDefs.Remove(pageFound);
                        }
                    }
                }
                // remove all remaining pages from unified page set, they're no longer in the list
                foreach (PageDefinition pageDef in pageDefs) {
                    pageDef.UnifiedSetGuid = null;
                    pageDP.SavePageDefinition(pageDef);
                }
            }
            return pageGuids;
        }
        private void RemoveGuid(Guid unifiedSetGuid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                // Get all pages that are part of the unified page set
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "UnifiedSetGuid", Operator = "==", Value = unifiedSetGuid });
                int total;
                List<PageDefinition> pageDefs = pageDP.GetItems(0, 0, null, filters, out total);
                // remove all pages from unified page set if they're not within the page list
                foreach (PageDefinition pageDef in pageDefs) {
                    pageDef.UnifiedSetGuid = null;
                    pageDP.SavePageDefinition(pageDef);
                }
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            return DataProvider.InstallModel(errorList);
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
    }
}
