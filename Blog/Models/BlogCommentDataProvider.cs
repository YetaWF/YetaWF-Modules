/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.DataProvider {

    public class BlogComment {

        public const int MaxName = 80;
        public const int MaxTitle = 120;
        public const int MaxComment = 20000;

        [Data_PrimaryKey, Data_Identity]
        public int Identity { get; set; }
        [Data_Index]
        public int CategoryIdentity { get; set; }
        [Data_Index]
        public int EntryIdentity { get; set; }

        [StringLength(MaxName)]
        public string Name { get; set; }
        [StringLength(Globals.MaxEmail)]
        public string Email { get; set; }

        public bool ShowGravatar { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string Website { get; set; }

        [StringLength(MaxTitle)]
        public string Title { get; set; }
        [StringLength(MaxComment)]
        public string Comment { get; set; }

        public bool Approved { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateCreated { get; set; }

        public BlogComment() {
            ShowGravatar = true;
        }
    }

    public class BlogCommentDataProvider : DataProviderImpl, IInstallableModel {

        public int EntryIdentity { get; private set; }// Blog entry

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BlogCommentDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { EntryIdentity = FileIdentityCount.IDENTITY_SEED; SetDataProvider(CreateDataProvider()); }
        public BlogCommentDataProvider(int blogEntry) : base(YetaWFManager.Manager.CurrentSite.Identity) { EntryIdentity = blogEntry; SetDataProvider(CreateDataProvider());  }
        public BlogCommentDataProvider(int blogEntry, int siteIdentity) : base(siteIdentity) { EntryIdentity = blogEntry; SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, BlogComment> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, BlogComment> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Comments", SiteIdentity: SiteIdentity, Cacheable: true, Parms: new { EntryIdentity = EntryIdentity });
        }

        // API
        // API
        // API

        public BlogComment GetItem(int comment) {
            BlogComment data = DataProvider.Get(comment);
            if (data == null) return null;
            return data;
        }
        public bool AddItem(BlogComment data) {
            data.DateCreated = DateTime.UtcNow;
            data.EntryIdentity = EntryIdentity;
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry entry = entryDP.GetItem(EntryIdentity);
                if (entry == null) throw new InternalError("Entry with id {0} not found", EntryIdentity);
                data.CategoryIdentity = entry.CategoryIdentity;
            }
            using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                BlogCategory cat = categoryDP.GetItem(data.CategoryIdentity);
                if (cat == null)
                    throw new InternalError("Category {0} not found", data.CategoryIdentity);
                if (cat.CommentApproval == BlogCategory.ApprovalType.None)
                    data.Approved = true;
                else if (cat.CommentApproval == BlogCategory.ApprovalType.AnonymousUsers) {
                    if (Manager.HaveUser)
                        data.Approved = true;
                }
            }
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(BlogComment data) {
            return DataProvider.Update(data.Identity, data.Identity, data);
        }
        public bool RemoveItem(int comment) {
            return DataProvider.Remove(comment);
        }
        public List<BlogComment> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "EntryIdentity", Operator = "==", Value = EntryIdentity });
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public bool RemoveAllComments() {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "EntryIdentity", Operator = "==", Value = EntryIdentity });
            DataProvider.RemoveRecords(filters);
            return true;
        }
    }
}
