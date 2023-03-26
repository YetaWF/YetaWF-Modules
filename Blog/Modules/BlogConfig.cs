/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class BlogConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, BlogConfigModule>, IInstallableModel { }

[ModuleGuid("{8e6986c9-3d25-4479-bce6-a54062efdb15}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Configuration")]
public class BlogConfigModule : ModuleDefinition {

    public BlogConfigModule() {
        Title = this.__ResStr("modTitle", "Blog Settings");
        Name = this.__ResStr("modName", "Blog Settings");
        Description = this.__ResStr("modSummary", "Main configuration module for the blog and defines global blog settings. It is accessible using Admin > Settings > Blog Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BlogConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new BlogConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Blog Settings"),
            MenuText = this.__ResStr("editText", "Blog Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the blog settings"),
            Legend = this.__ResStr("editLegend", "Edits the blog settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    public class Model {

        [Category("Blog"), Caption("Blog Main Url"), Description("Main entry point for the site's blog")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        [StringLength(Globals.MaxUrl), Required, Trim]
        public string? BlogUrl { get; set; }

        [Category("Blog"), Caption("Default Blog Category"), Description("The default blog category displayed when no blog category is selected")]
        [UIHint("YetaWF_Blog_Category"), AdditionalMetadata("ShowAll", true), Required]
        public int DefaultCategory { get; set; }

        [Category("Blog"), Caption("Total Entries"), Description("The maximum number of blog entries shown on the main blog page when no category is selected")]
        [UIHint("IntValue4"), Range(3, 9999), Required]
        public int Entries { get; set; }

        [Category("Blog"), Caption("Blog Entry Url"), Description("URL to display a blog entry with comments")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        [StringLength(Globals.MaxUrl), Required, Trim]
        public string? BlogEntryUrl { get; set; }

        [Category("Comments"), Caption("Show Gravatar"), Description("Defines whether Gravatar images are shown for visitors leaving comments")]
        [HelpLink("http://www.gravatar.com")]
        [UIHint("Boolean"), Trim]
        public bool ShowGravatar { get; set; }

        [Category("Comments"), Caption("Gravatar Default"), Description("Defines the default Gravatar image for visitors leaving comments (used when users have not defined an image at http://www.gravatar.com)")]
        [UIHint("Enum"), RequiredIf("GravatarDefault", true), Trim]
        public Gravatar.GravatarEnum GravatarDefault { get; set; }

        [Category("Comments"), Caption("Allowed Gravatar Rating"), Description("Defines the acceptable Gravatar rating for displayed Gravatar images")]
        [UIHint("Enum"), RequiredIf("ShowGravatar", true), Trim]
        public Gravatar.GravatarRatingEnum GravatarRating { get; set; }

        [Category("Comments"), Caption("Gravatar Size"), Description("The width and height of the displayed Gravatar images (in pixels)")]
        [UIHint("IntValue4"), RequiredIf("ShowGravatar", true), Range(16, 100)]
        public int GravatarSize { get; set; }

        [Category("Notifications"), Caption("Email Address"), Description("The email address where all notifications for blog events are sent")]
        [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
        public string? NotifyEmail { get; set; }

        [Category("Notifications"), Caption("Notify - New Comment"), Description("Defines whether the administrator receives email notification when a new comment has been added to a blog entry")]
        [UIHint("Boolean")]
        public bool NotifyNewComment { get; set; }

        [Category("Rss"), Caption("Feed"), Description("Publish this blog's contents as an Rss feed")]
        [UIHint("Boolean")]
        public bool Feed { get; set; }

        [Category("Rss"), Caption("Feed Title"), Description("The Rss feed's title")]
        [UIHint("Text80"), StringLength(BlogConfigData.MaxFeedTitle), RequiredIf("Feed", true)]
        public string? FeedTitle { get; set; }

        [Category("Rss"), Caption("Feed Summary"), Description("The Rss feed's summary description")]
        [UIHint("Text80"), StringLength(BlogConfigData.MaxFeedSummary), RequiredIf("Feed", true)]
        public string? FeedSummary { get; set; }

        [Category("Rss"), Caption("Feed Main URL"), Description("The optional Rss feed's main URL")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? FeedMainUrl { get; set; }

        [Category("Rss"), Caption("Feed Detail URL"), Description("The optional Rss feed's detail page for a blog entry")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? FeedDetailUrl { get; set; }

        [Category("Rss"), Caption("Feed Image"), Description("The image used for this Rss feed")]
        [UIHint("Image"), AdditionalMetadata("ImageType", BlogConfigData.ImageType)]
        [AdditionalMetadata("Width", 200), AdditionalMetadata("Height", 200)]
        public string? FeedImage { get; set; }

        public BlogConfigData GetData(BlogConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(BlogConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (BlogConfigDataProvider dataProvider = new BlogConfigDataProvider()) {
            Model model = new Model { };
            BlogConfigData? data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The blog settings were not found."));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (BlogConfigDataProvider dataProvider = new BlogConfigDataProvider()) {
            BlogConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Blog settings saved"));
        }
    }
}
