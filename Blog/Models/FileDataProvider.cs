/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.DataProvider.File;

public class FileDataProvider : IExternalDataProvider {

    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.BlogCategoryDataProvider), typeof(BlogCategoryDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.BlogEntryDataProvider), typeof(BlogEntryDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.BlogCommentDataProvider), typeof(BlogCommentDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.BlogConfigDataProvider), typeof(BlogConfigDataProvider));
        DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.DisqusConfigDataProvider), typeof(DisqusConfigDataProvider));
    }
    class BlogCategoryDataProvider : FileDataProvider<int, BlogCategory> {
        public BlogCategoryDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Package.AreaName, SiteIdentity.ToString(), "Categories"); }
    }
    class BlogEntryDataProvider : FileDataProvider<int, BlogEntry> {
        public BlogEntryDataProvider(Dictionary<string, object> options) : base(options) { CalculatedPropertyCallbackAsync = GetCalculatedPropertyAsync; }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Package.AreaName, SiteIdentity.ToString(), "Entries"); }
        private async Task<object> GetCalculatedPropertyAsync(string name, object obj) {
            BlogEntry entry = (BlogEntry)obj;
            if (name == "CommentsUnapproved") {
                using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(entry.Identity)) {
                    List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo>() {
                        new DataProviderFilterInfo {
                            Field = nameof(BlogComment.Approved), Operator = "==", Value = false,
                        },
                    };
                    DataProviderGetRecords<BlogComment> commentsUnapproved = await commentDP.GetItemsAsync(0, 0, null, filters);
                    entry.CommentsUnapproved = commentsUnapproved.Total;
                    return obj;
                }
            } else if (name == "Comments") {
                using (DataProvider.BlogCommentDataProvider commentDP = new DataProvider.BlogCommentDataProvider(entry.Identity)) {
                    DataProviderGetRecords<BlogComment> comments = await commentDP.GetItemsAsync(0, 0, null, null);
                    entry.Comments = comments.Total;
                    return obj;
                }
            } else throw new InternalError("Unexpected property {0}", name);
        }
    }
    class BlogCommentDataProvider : FileDataProvider<int, BlogComment> {
        public BlogCommentDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return GetCommentFolder(GetEntryIdentity()); }
        private int GetEntryIdentity() {
            if (Options.ContainsKey("EntryIdentity") && Options["EntryIdentity"] is int)
                return Convert.ToInt32(Options["EntryIdentity"]);
            throw new InternalError("No EntryIdentity found in options");
        }
        private string GetCommentFolderRoot() {
            return Path.Combine(YetaWFManager.DataFolder, Package.AreaName, SiteIdentity.ToString(), "Comments");
        }
        internal string GetCommentFolder(int blogEntry) {
            return Path.Combine(GetCommentFolderRoot(), string.Format("Ent{0}", blogEntry));
        }
        public new async Task<int> RemoveRecordsAsync(List<DataProviderFilterInfo> filters) {
            if (filters != null && filters.Count == 1) {
                DataProviderFilterInfo f = filters.First();
                int entryIdentity = GetEntryIdentity();
                if (f.Field == "EntryIdentity" && f.Operator == "==" && entryIdentity.CompareTo(f.Value) == 0) {
                    await FileSystem.FileSystemProvider.DeleteDirectoryAsync(GetCommentFolder(entryIdentity));
                    return 1;
                }
            }
            return await base.RemoveRecordsAsync(filters);
        }
    }
    class BlogConfigDataProvider : FileDataProvider<int, BlogConfigData> {
        public BlogConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
    class DisqusConfigDataProvider : FileDataProvider<int, DisqusConfigData> {
        public DisqusConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()); }
    }
}
