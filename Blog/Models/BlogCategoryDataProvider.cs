/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

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
using YetaWF.DataProvider;

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

        public BlogCategoryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public BlogCategoryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, BlogCategory> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, BlogCategory> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Categories",
                () => { // File
                    return new FileDataProvider<int, BlogCategory>(
                        Path.Combine(YetaWFManager.DataFolder, package.AreaName, SiteIdentity.ToString(), "Categories"),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, BlogCategory>(Dataset, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { Package = Package, Dataset = Dataset, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

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
    }
}
