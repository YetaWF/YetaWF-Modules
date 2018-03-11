/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
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

        public BlogCategoryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public BlogCategoryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, BlogCategory> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, BlogCategory> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Categories", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public Task<BlogCategory> GetItemAsync(int identity) {
            return DataProvider.GetAsync(identity);
        }
        public Task<bool> AddItemAsync(BlogCategory data) {
            data.DateCreated = DateTime.UtcNow;
            return DataProvider.AddAsync(data);
        }
        public Task<UpdateStatusEnum> UpdateItemAsync(BlogCategory data) {
            return DataProvider.UpdateAsync(data.Identity, data.Identity, data);
        }
        public async Task<bool> RemoveItemAsync(int identity) {
            if (! await DataProvider.RemoveAsync(identity))
                return false;
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                await entryDP.RemoveEntriesAsync(identity);// remove all entries for this category
            }
            return true;
        }
        public Task<DataProviderGetRecords<BlogCategory>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}
