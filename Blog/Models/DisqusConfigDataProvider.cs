/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Blog.DataProvider {

    public class DisqusConfigData {

        public enum AvatarTypeEnum {
            [EnumDescription("Default", "Avatar as provided by Disqus")]
            Default = 0,
            [EnumDescription("Gravatar", "Avatar as provided by Gravatar, based on email address")]
            Gravatar = 1,
        }

        public const int MaxShortName = 40;
        public const int MaxPublicKey = 200;
        public const int MaxPrivateKey = 200;

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(MaxShortName)]
        public string ShortName { get; set; }

        public bool UseSSO { get; set; }

        [StringLength(MaxPublicKey)]
        public string PublicKey { get; set; }
        [StringLength(MaxPrivateKey)]
        public string PrivateKey { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string LoginUrl { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public AvatarTypeEnum AvatarType { get; set; }
        public Gravatar.GravatarEnum GravatarDefault { get; set; }
        public Gravatar.GravatarRatingEnum GravatarRating { get; set; }
        public int GravatarSize { get; set; }

        public DisqusConfigData() {
            LoginUrl = "/User/Login?CloseOnLogin=true";
            Width = 1200;
            Height = 800;
            AvatarType = AvatarTypeEnum.Default;
            GravatarDefault = Gravatar.GravatarEnum.wavatar;
            GravatarRating = Gravatar.GravatarRatingEnum.G;
            GravatarSize = 48;
        }
    }

    public class DisqusConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public DisqusConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public DisqusConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, DisqusConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, DisqusConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_DisqusConfig", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<DisqusConfigData> GetConfigAsync() {
            using (DisqusConfigDataProvider configDP = new DisqusConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<DisqusConfigData> GetItemAsync() {
            DisqusConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new DisqusConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(DisqusConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(DisqusConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Disqus Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(DisqusConfigData data) {
            DisqusConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(DisqusConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Disqus Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
