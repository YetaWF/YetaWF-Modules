/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Text.Controllers;
using YetaWF.Core.Search;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Text.Modules {

    public class TextModuleDataProvider : ModuleDefinitionDataProvider<Guid, TextModule>, IInstallableModel { }

    [ModuleGuid("{408CA15D-14B0-443b-A66A-14CC6B9EBE38}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TextModule : ModuleDefinition {

        public const int MaxContents = 1024 * 1024;

        public TextModule() : base() {
            Title = this.__ResStr("modTitle", "Edit");
            Name = this.__ResStr("modName", "Text (Full Editor, HTML)");
            Description = this.__ResStr("modSummary", "Displays user editable text contents (full wysiwyg editor, using CKEditor). A sample page is accessible using Tests > Modules > Text (standard YetaWF site).");
            CompleteContents = this.__ResStr("newContents", "(new)");
            EditOnPage = true;
            ShowTitleActions = true;
            FeedImage_Data = new byte[0];
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TextModuleDataProvider(); }

        [StringLength(MaxContents)]
        public MultiString CompleteContents { get; set; }

        [Category("General"), Caption("Edit On Page"), Description("Enables editing on the page instead of as part of module settings")]
        [UIHint("Boolean")]
        public bool EditOnPage { get; set; }

        [Category("Rss"), Caption("Feed"), Description("Publish this module's contents as an Rss feed")]
        [UIHint("Boolean")]
        public bool Feed { get; set; }

        [Category("Rss"), Caption("Feed Title"), Description("The Rss feed's title")]
        [UIHint("Text80"), StringLength(80), RequiredIf("Feed", true)]
        public string? FeedTitle { get; set; }

        [Category("Rss"), Caption("Feed Summary"), Description("The Rss feed's summary description")]
        [UIHint("Text80"), StringLength(200), RequiredIf("Feed", true)]
        public string? FeedSummary { get; set; }

        [Category("Rss"), Caption("Feed Main URL"), Description("The optional Rss feed's main URL")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? FeedMainUrl { get; set; }

        [Category("Rss"), Caption("Feed Detail URL"), Description("The optional Rss feed's detail page for this Text module")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? FeedDetailUrl { get; set; }

        [Category("Rss"), Caption("Feed Publish Date"), Description("The date this feed was published")]
        [UIHint("DateTime"), RequiredIf("Feed", true)]
        public DateTime? FeedPublishDate { get; set; }

        [Category("Rss"), Caption("Feed Update Date"), Description("The date this feed was last updated")]
        [UIHint("DateTime"), RequiredIf("Feed", true)]
        public DateTime? FeedUpdateDate { get; set; }

        [Category("Rss"), Caption("Feed Image"), Description("The image used for this Rss feed")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
        [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
        [DontSave]
        public string? FeedImage {
            get {
                if (_feedImage == null) {
                    if (FeedImage_Data != null && FeedImage_Data.Length > 0)
                        _feedImage = ModuleGuid.ToString() + ",FeedImage_Data";
                }
                return _feedImage;
            }
            set {
                _feedImage = value;
            }
        }
        private string? _feedImage = null;

        [Data_Binary, CopyAttribute]
        public byte[] FeedImage_Data { get; set; }

        [Category("General"), Caption("Contents"), Description("The text contents")]
        [UIHint("TextArea"), AdditionalMetadata("ImageBrowse", true), StringLength(MaxContents), AdditionalMetadata("PageBrowse", true)]
        [DontSave]
        [NoModelChange]
        public string? Contents {
            get {
                return CompleteContents[MultiString.ActiveLanguage];
            }
            set {
                CompleteContents[MultiString.ActiveLanguage] = value??string.Empty;
            }
        }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return EditorLevel_DefaultAllowedRoles; } }

        public override async Task<List<ModuleAction>> RetrieveModuleActionsAsync() {
            List<ModuleAction> actions = await base.RetrieveModuleActionsAsync();
            if (Feed)
                actions.New(await GetAction_RssFeedAsync(ModuleGuid));
            return actions;
        }

        public async Task<ModuleAction?> GetAction_RssFeedAsync(Guid moduleGuid) {
            TextModule? mod = (TextModule?)await ModuleDefinition.LoadAsync(moduleGuid, AllowNone: true);
            if (mod == null) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(RssController), "RssFeed"),
                QueryArgs = new { ModuleGuid = moduleGuid, },
                QueryArgsHR = new { Title = mod.Title.ToString() },
                Image = await CustomIconAsync("RssFeed.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("rssLink", "RSS Feed"),
                MenuText = this.__ResStr("rssMenu", "RSS Feed"),
                Tooltip = this.__ResStr("rssTT", "Display the RSS Feed"),
                Legend = this.__ResStr("rssLegend", "Displays the RSS Feed"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
        public async Task<ModuleAction?> GetAction_RssDetailAsync(string? url, Guid moduleGuid, string? AnchorId = null) {
            TextModule? mod = (TextModule?)await ModuleDefinition.LoadAsync(moduleGuid, AllowNone: true);
            if (mod == null) return null;
            if (string.IsNullOrWhiteSpace(url))
                url = Manager.CurrentSite.MakeUrl(GetModuleUrl(moduleGuid));
            return new ModuleAction(this) {
                Url = url,
                QueryArgs = new { ModuleGuid = moduleGuid, },
                QueryArgsHR = new { Title = mod.Title.ToString().Truncate(80) },
                AnchorId = AnchorId,
                Image = await CustomIconAsync("RssFeed.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("rssDetailLink", "RSS Feed Entry"),
                MenuText = this.__ResStr("rssDetailMenu", "RSS Feed Entry"),
                Tooltip = this.__ResStr("rssDetailTT", "Display the RSS feed entry"),
                Legend = this.__ResStr("rssDetailLegend", "Displays the RSS feed entry"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }

        // SEARCH
        // SEARCH
        // SEARCH

        public override void CustomSearch(ISearchWords searchWords) {
            searchWords.AddContent(CompleteContents);
        }

        // VALIDATION
        // VALIDATION
        // VALIDATION

        public override void CustomValidation(ModelStateDictionary modelState, string modelPrefix) {
            if (FeedMainUrl == "/")
                modelState.AddModelError(modelPrefix+"FeedMainUrl", this.__ResStr("urlMain", "The site's home page can't be used as the feed's main URL (due to the use of human readable querystring arguments, which could be misinterpreted as the page name)"));
            if (FeedDetailUrl == "/")
                modelState.AddModelError(modelPrefix + "FeedDetailUrl", this.__ResStr("urlDetail", "The site's home page can't be used as the feed's detail URL (due to the use of human readable querystring arguments, which could be misinterpreted as the page name)"));

            if (FeedUpdateDate == null)
                FeedUpdateDate = FeedPublishDate;
            if (FeedPublishDate != null && FeedUpdateDate != null && (DateTime)FeedUpdateDate < (DateTime)FeedPublishDate)
                modelState.AddModelError(modelPrefix + "FeedUpdateDate", this.__ResStr("dateFeedUpdate", "The last update date can't be earlier than the date this item was published"));
        }
    }
}