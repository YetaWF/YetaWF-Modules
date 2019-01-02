/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.Image;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.DataProvider {

    public class BlogConfigData : IInitializeApplicationStartup {

        private string __ResStr(string name, string defaultValue, params object[] parms) {
            return ResourceAccess.GetResourceString(typeof(BlogConfigDataProvider), name, defaultValue, parms);
        }

        // IInitializeApplicationStartup
        public const string ImageType = "YetaWF_Blog_BlogConfigData";

        public Task InitializeApplicationStartupAsync() {
            ImageSupport.AddHandler(ImageType, GetBytesAsync: RetrieveImageAsync);
            return Task.CompletedTask;
        }
        private async Task<ImageSupport.GetImageInBytesInfo> RetrieveImageAsync(string name, string location) {
            ImageSupport.GetImageInBytesInfo fail = new ImageSupport.GetImageInBytesInfo();
            if (!string.IsNullOrWhiteSpace(location)) return fail;
            if (string.IsNullOrWhiteSpace(name)) return fail;
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            if (config.FeedImage_Data == null || config.FeedImage_Data.Length == 0) return fail;
            return new ImageSupport.GetImageInBytesInfo {
                Content = config.FeedImage_Data,
                Success = true,
            };
        }

        public const int MaxFeedTitle = 80;
        public const int MaxFeedSummary = 200;

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string BlogUrl { get; set; }
        public int DefaultCategory { get; set; }
        public int Entries { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string BlogEntryUrl { get; set; }

        public bool ShowGravatar { get; set; }
        public Gravatar.GravatarEnum GravatarDefault { get; set; }
        public Gravatar.GravatarRatingEnum GravatarRating { get; set; }
        public int GravatarSize { get; set; }

        [StringLength(Globals.MaxEmail)]
        public string NotifyEmail { get; set; }

        [Data_NewValue("(0)")]
        public bool NotifyNewComment { get; set; }

        public bool Feed { get; set; }
        [StringLength(MaxFeedTitle)]
        public string FeedTitle { get; set; }
        [StringLength(MaxFeedSummary)]
        public string FeedSummary { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string FeedMainUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string FeedDetailUrl { get; set; }

        [UIHint("Image")]
        [DontSave]
        public string FeedImage {
            get {
                if (_feedImage == null) {
                    if (FeedImage_Data != null && FeedImage_Data.Length > 0)
                        _feedImage = ModuleDefinition.GetPermanentGuid(typeof(BlogConfigModule)).ToString() + ",FeedImage_Data";
                }
                return _feedImage;
            }
            set {
                _feedImage = value;
            }
        }
        private string _feedImage = null;

        [Data_Binary]
        public byte[] FeedImage_Data { get; set; }

        public BlogConfigData() {
            BlogUrl = null;
            Entries = 20;
            BlogEntryUrl = null;
            ShowGravatar = true;
            GravatarDefault = Gravatar.GravatarEnum.wavatar;
            GravatarRating = Gravatar.GravatarRatingEnum.G;
            GravatarSize = 40;
            FeedTitle = this.__ResStr("feedTitle", "(Blog Title)");
            Feed = false;
            FeedSummary = this.__ResStr("feedSummary", "(Blog Summary)");
            FeedMainUrl = null;
            FeedDetailUrl = null;
            FeedImage_Data = new byte[0];
        }

        internal static async Task<string> GetCategoryCanonicalNameAsync(int blogCategory = 0) {
            using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
                string canon = config.BlogUrl;
                if (blogCategory != 0) {
                    BlogCategory cat = await categoryDP.GetItemAsync(blogCategory);
                    if (cat != null)
                        canon = string.Format("{0}/Title/{1}/?BlogCategory={2}", config.BlogUrl, YetaWFManager.UrlEncodeSegment(cat.Category.ToString().Truncate(80)), blogCategory);
                } else {
                    canon = config.BlogUrl;
                }
                return canon;
            }
        }
        public static async Task<string> GetEntryCanonicalNameAsync(int blogEntry) {
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            string canon = string.Format("{0}/?BlogEntry={1}", config.BlogEntryUrl, blogEntry);
            using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
                BlogEntry data = await entryDP.GetItemAsync(blogEntry);
                if (data != null)
                    canon = string.Format("{0}/Title/{1}/?BlogEntry={2}", config.BlogEntryUrl, YetaWFManager.UrlEncodeSegment(data.Title.ToString().Truncate(80)), blogEntry);
                return canon;
            }
        }
    }

    public class BlogConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BlogConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public BlogConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, BlogConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, BlogConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<BlogConfigData> GetConfigAsync() {
            using (BlogConfigDataProvider configDP = new BlogConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<BlogConfigData> GetItemAsync() {
            BlogConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new BlogConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(BlogConfigData data) {
            data.Id = KEY;
            await SaveImagesAsync(ModuleDefinition.GetPermanentGuid(typeof(BlogConfigModule)), data);
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(BlogConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Blog Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(BlogConfigData data) {
            BlogConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            await SaveImagesAsync(ModuleDefinition.GetPermanentGuid(typeof(BlogConfigModule)), data);
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving settings {0}", status);
            await Auditing.AddAuditAsync($"{nameof(BlogConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Blog Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
