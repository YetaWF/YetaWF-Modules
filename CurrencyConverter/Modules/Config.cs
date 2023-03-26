/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
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
using YetaWF.Modules.CurrencyConverter.DataProvider;

namespace YetaWF.Modules.CurrencyConverter.Modules;

public class ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfigModule>, IInstallableModel { }

[ModuleGuid("{03950959-d8d7-44bc-a6f7-3162b4db82ac}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class ConfigModule : ModuleDefinition {

    public ConfigModule() {
        Title = this.__ResStr("modTitle", "Currency Converter Settings");
        Name = this.__ResStr("modName", "Currency Converter Settings");
        Description = this.__ResStr("modSummary", "Used to edit the currency converter settings. It is accessible using Admin > Settings > Currency Converter Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Currency Converter Settings"),
            MenuText = this.__ResStr("editText", "Currency Converter Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the currency converter settings"),
            Legend = this.__ResStr("editLegend", "Edits the currency converter settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    public class Model {

        [Caption("App ID"), Description("App ID used by openexchangerates.org to identify your account - an account is needed to retrieve currency exchange rates - This account is used for all sites within this YetaWF instance")]
        [UIHint("Text40"), StringLength(DataProvider.ConfigData.MaxAppID), Trim]
        [ExcludeDemoMode]
        public string? AppID { get; set; }

        [Caption("Use Https"), Description("Use https to access openexchangerates.org (requires a paid account) - This setting is used for all sites within this YetaWF instance")]
        [UIHint("Boolean")]
        public bool UseHttps { get; set; }

        [Caption("Refresh Interval"), Description("Defines at which interval new currency rates are retrieved - Check the maximum allowed under your rate plan at openexchangerates.org")]
        [UIHint("TimeSpan"), Required, TimeSpanRange("00.00:05:00", "30.00:00:00")]
        public TimeSpan RefreshInterval { get; set; }

        [Category("General"), Caption("openexchangerates.org"), Description("Provides a link to openexchangerates.org to set up an account for all your sites within this YetaWF instance")]
        [UIHint("Url"), ReadOnly]
        public string OpenExchangeRatesUrl { get; set; }

        public ConfigData GetData(ConfigData config) {
            ObjectSupport.CopyData(this, config);
            return config;
        }

        public void SetData(ConfigData config) {
            ObjectSupport.CopyData(config, this);
        }
        public Model() {
            OpenExchangeRatesUrl = "http://openexchangerates.org/";
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        await using (ConfigDataProvider configDP = new ConfigDataProvider()) {
            Model model = new Model { };
            ConfigData data = await configDP.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Currency converter configuration not found."));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        await using (ConfigDataProvider configDP = new ConfigDataProvider()) {
            ConfigData data = await configDP.GetItemAsync();// get the original item
            if (data == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The currency converter configuration has been removed and can no longer be updated."));

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display

            await configDP.UpdateConfigAsync(data); // save updated item
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Currency converter configuration saved"));
        }
    }
}