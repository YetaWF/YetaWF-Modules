/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

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
using YetaWF.Modules.TawkTo.DataProvider;

namespace YetaWF.Modules.TawkTo.Modules;

public class ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ConfigModule>, IInstallableModel { }

[ModuleGuid("{fdfa95b0-1dfb-4f62-ab07-7328c9d3aff2}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class ConfigModule : ModuleDefinition2 {

    public ConfigModule() {
        Title = this.__ResStr("modTitle", "TawkTo Settings");
        Name = this.__ResStr("modName", "TawkTo Settings");
        Description = this.__ResStr("modSummary", "Implements the TawkTo configuration. It can be accessed using Admin > Settings > TawkTo Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "TawkTo Settings"),
            MenuText = this.__ResStr("editText", "TawkTo Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the tawkto settings"),
            Legend = this.__ResStr("editLegend", "Edits the tawkto settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [TextBelow("The Site ID can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
        [Caption("Site ID"), Description("Defines the account used for the chat window - The Site ID can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
        [UIHint("Text80"), StringLength(DataProvider.ConfigData.MaxAccount), Trim]
        [HelpLink("https://dashboard.tawk.to")]
        [ExcludeDemoMode]
        public string? Account { get; set; }

        [TextBelow("The API Key can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
        [Caption("API Key"), Description("Defines the API Key used for the chat window - The API Key can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
        [UIHint("Text80"), StringLength(DataProvider.ConfigData.MaxAPIKey), RequiredIfSuppliedAttribute(nameof(Account)), Trim]
        [HelpLink("https://dashboard.tawk.to")]
        [ExcludeDemoMode]
        public string? APIKey { get; set; }

        [Caption(" "), Description(" ")]
        [UIHint("String"), ReadOnly]
        public string? Description { get; set; }

        [Caption("Page Css (Exclude)"), Description("Defines the Css classes (space separated) for pages where the Tawk.to chat window is not shown - Pages can be prevented from showing the chat invitation by specifying their Css class found on the <body> tag - If no Css class is specified, all pages show the chat invitation")]
        [UIHint("Text80"), StringLength(DataProvider.ConfigData.MaxCss), Trim]
        public string? ExcludedPagesCss { get; set; }

        [Caption("Page Css (Include)"), Description("Defines the Css classes (space separated) for pages where the Tawk.to chat window is shown - Only pages with one of the defined Css classes will display the chat invitation - If no Css class is specified, all pages show the chat invitation")]
        [UIHint("Text80"), StringLength(DataProvider.ConfigData.MaxCss), Trim]
        public string? IncludedPagesCss { get; set; }

        public ConfigData GetData(ConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(ConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() {
            Description = this.__ResStr("desc", "The Tawk.to (Skin) module is referenced site wide (in Site Settings), in which case all pages show the chat invitation. By using the fields Page Css (Exclude and Include) below, the pages where the chat invitation is shown can be limited. If a chat is already in progress, the chat window is unaffected and is always shown.");
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        await using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            Model model = new Model { };
            ConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The Tawk.to settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        await using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            ConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Tawk.To Settings saved"));
        }
    }
}
