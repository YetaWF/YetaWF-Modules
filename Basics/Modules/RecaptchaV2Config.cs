/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules;

public class RecaptchaV2ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, RecaptchaV2ConfigModule>, IInstallableModel { }

[ModuleGuid("{6256FC1B-6E81-4B2A-9D99-FF79FF425C86}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class RecaptchaV2ConfigModule : ModuleDefinition2 {

    public RecaptchaV2ConfigModule() : base() {
        Title = this.__ResStr("modTitle", "RecaptchaV2 Settings");
        Name = this.__ResStr("modName", "RecaptchaV2 Settings");
        Description = this.__ResStr("modSummary", "Captcha support using Google's https://www.google.com/recaptcha. The Recaptcha configuration can be accessed using Admin > Settings > RecaptchaV2 Settings (standard YetaWF site). Recaptcha is a service provided by Google. Key information can be obtained at https://www.google.com/recaptcha/admin#createsiteRemote. You need a Google account to obtain key information. The settings defined using the RecaptchaV2 Settings Module are used by the RecaptchaV2 component.");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RecaptchaV2ConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new RecaptchaV2ConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "RecaptchaV2 Settings"),
            MenuText = this.__ResStr("editText", "RecaptchaV2 Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the site's RecaptchaV2 settings"),
            Legend = this.__ResStr("editLegend", "Edits the site's RecaptchaV2 settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        public EditModel() {
            GoogleUrl = "https://www.google.com/recaptcha";
        }

        [Caption("Public Key"), Description("The public key used to communicate with the Google/Recaptcha site")]
        [UIHint("Text80"), StringLength(YetaWF.Core.Components.RecaptchaV2Config.MaxPublicKey), Trim]
        [ExcludeDemoMode]
        public string? PublicKey { get; set; }

        [Caption("Private Key"), Description("The private key used to communicate with the Google/Recaptcha site")]
        [UIHint("Text80"), StringLength(YetaWF.Core.Components.RecaptchaV2Config.MaxPrivateKey), RequiredIfSuppliedAttribute(nameof(PublicKey)), Trim]
        [ExcludeDemoMode]
        public string? PrivateKey { get; set; }

        [Caption("Theme"), Description("The theme used for the recaptcha control throughout the site")]
        [UIHint("Enum")]
        public RecaptchaV2Config.ThemeEnum Theme { get; set; }

        [Caption("Size"), Description("The recaptcha control size used throughout the site")]
        [UIHint("Enum")]
        public RecaptchaV2Config.SizeEnum Size { get; set; }

        [Caption("Info"), Description("Visit Google to obtain keys for Recaptcha use on your site")]
        [UIHint("Url"), ReadOnly]
        public string GoogleUrl { get; set; }

        public RecaptchaV2Config GetData() {
            RecaptchaV2Config data = new RecaptchaV2Config();
            ObjectSupport.CopyData(this, data);
            return data;
        }

        public void SetData(RecaptchaV2Config data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (RecaptchaV2ConfigDataProvider dataProvider = new RecaptchaV2ConfigDataProvider()) {
            EditModel model = new EditModel { };
            RecaptchaV2Config data = await dataProvider.GetItemAsync();
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        using (RecaptchaV2ConfigDataProvider dataProvider = new RecaptchaV2ConfigDataProvider()) {
            RecaptchaV2Config data = await dataProvider.GetItemAsync();// get the original item

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            ObjectSupport.CopyData(model, data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display

            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Captcha configuration saved"));
        }
    }
}