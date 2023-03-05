/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

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
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules;

public class AlertConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, AlertConfigModule>, IInstallableModel { }

[ModuleGuid("{d2c029c4-6b03-45e4-9b88-1cbda8972738}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class AlertConfigModule : ModuleDefinition2 {

    public AlertConfigModule() {
        Title = this.__ResStr("modTitle", "Alert Settings");
        Name = this.__ResStr("modName", "Alert Settings");
        Description = this.__ResStr("modSummary", "The Alert Settings Module allows configuring the Alert displayed to site visitors and can be accessed using Admin > Settings > Alert Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AlertConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new AlertConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Alert Settings"),
            MenuText = this.__ResStr("editText", "Alert Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the site's Alert settings"),
            Legend = this.__ResStr("editLegend", "Edits the site's Alert settings"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Enabled"), Description("Defines whether message display is enabled")]
        [UIHint("Boolean")]
        public bool Enabled { get; set; }

        [Caption("Message Handling"), Description("Defines how long the message is displayed")]
        [UIHint("Enum")]
        public AlertConfig.MessageHandlingEnum MessageHandling { get; set; }

        [Caption("Message"), Description("Defines the message to display")]
        [UIHint("TextArea"), AdditionalMetadata("EmHeight", 5), StringLength(DataProvider.AlertConfig.MaxMessage), Trim]
        [RequiredIf("Enabled", true)]
        public string? Message { get; set; }

        public AlertConfig GetData(AlertConfig data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(AlertConfig data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {
            Model model = new Model { };
            AlertConfig data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The Alert settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {
            AlertConfig data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Alert settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}
