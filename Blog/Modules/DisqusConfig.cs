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
using YetaWF.Modules.Blog.Support;

namespace YetaWF.Modules.Blog.Modules;

public class DisqusConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusConfigModule>, IInstallableModel { }

[ModuleGuid("{71583859-baa9-43fa-895b-a6ebd47561a1}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Configuration")]
public class DisqusConfigModule : ModuleDefinition {

    public DisqusConfigModule() {
        Title = this.__ResStr("modTitle", "Disqus Settings");
        Name = this.__ResStr("modName", "Disqus Settings");
        Description = this.__ResStr("modSummary", "Edits a site's Disqus settings. It is accessible using Admin > Settings > Disqus Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisqusConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new DisqusConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Disqus Settings"),
            MenuText = this.__ResStr("editText", "Disqus Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the Disqus settings"),
            Legend = this.__ResStr("editLegend", "Edits the Disqus settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    public class Model {

        [Caption("Shortname"), Description("The Shortname you assigned to your site (at Disqus) - If omitted, Disqus comments are not available")]
        [UIHint("Text40"), StringLength(DisqusConfigData.MaxShortName), ShortNameValidation, Trim]
        [HelpLink("https://disqus.com/admin/settings/general/")]
        public string? ShortName { get; set; }

        [Caption("Avatar Type"), Description("Defines the source for user avatars (using SSO)")]
        [UIHint("Enum")]
        public DisqusConfigData.AvatarTypeEnum AvatarType { get; set; }

        [Caption("Gravatar Default"), Description("Defines the default Gravatar image for visitors leaving comments (used when users have not defined an image at http://www.gravatar.com)")]
        [UIHint("Enum"), RequiredIf("GravatarDefault", true)]
        [ProcessIf("AvatarType", DisqusConfigData.AvatarTypeEnum.Gravatar)]
        public Gravatar.GravatarEnum GravatarDefault { get; set; }

        [Caption("Allowed Gravatar Rating"), Description("Defines the acceptable Gravatar rating for displayed Gravatar images")]
        [UIHint("Enum")]
        [ProcessIf("AvatarType", DisqusConfigData.AvatarTypeEnum.Gravatar)]
        public Gravatar.GravatarRatingEnum GravatarRating { get; set; }

        [Caption("Gravatar Size"), Description("The width and height of the displayed Gravatar images (in pixels)")]
        [UIHint("IntValue4"), Range(16, 100)]
        [ProcessIf("AvatarType", DisqusConfigData.AvatarTypeEnum.Gravatar)]
        public int GravatarSize { get; set; }

        public DisqusConfigData GetData(DisqusConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(DisqusConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
            Model model = new Model { };
            DisqusConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The Disqus settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
            DisqusConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Disqus settings saved"));
        }
    }
}
