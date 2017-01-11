/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Blog.DataProvider {
    public class BlogCategory {

        public enum ApprovalType {
            [EnumDescription("None", "Comments do not require approval and are immediately viewable")]
            None = 0,
            [EnumDescription("Approve Anonymous Users", "Comments by anonymous users require approval")]
            AnonymousUsers = 1,
            [EnumDescription("Approve All", "All comments require approval")]
            All = 2,
        };

        public const int MaxCategory = 50;
        public const int MaxDescription = 500;
        public const int MaxCopyright = 100;

        [Data_PrimaryKey, Data_Identity]
        public int Identity { get; set; }

        [Data_Index, StringLength(MaxCategory)]
        public MultiString Category { get; set; }
        [StringLength(MaxDescription)]
        public MultiString Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Syndicated { get; set; }
        [StringLength(Globals.MaxEmail)]
        public string SyndicationEmail { get; set; }

        public bool UseCaptcha { get; set; }
        public ApprovalType CommentApproval { get; set; }

        [StringLength(MaxCopyright)]
        public MultiString SyndicationCopyright { get; set; }

        public BlogCategory() {
            UseCaptcha = true;
            Category = new MultiString();
            Description = new MultiString();
            CommentApproval = ApprovalType.AnonymousUsers;
            SyndicationCopyright = new MultiString();
        }
    }

    public class BlogCategoryDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BlogCategoryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public BlogCategoryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, BlogCategory> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, BlogCategory>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString(), "Categories"),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, BlogCategory>(AreaName + "_Categories", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, BlogCategory> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public BlogCategory GetItem(int identity) {
            return DataProvider.Get(identity);
        }
        public bool AddItem(BlogCategory data) {
            data.DateCreated = DateTime.UtcNow;
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(BlogCategory data) {
            return DataProvider.Update(data.Identity, data.Identity, data);
        }
        public bool RemoveItem(int identity) {
            if (!DataProvider.Remove(identity))
                return false;
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                entryDP.RemoveEntries(identity);// remove all entries for this category
            }
            return true;
        }
        public List<BlogCategory> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
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
