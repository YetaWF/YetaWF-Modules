/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

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
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules;

public class SchedulerConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SchedulerConfigModule>, IInstallableModel { }

[ModuleGuid("{A43ECFAE-7ED4-4d96-B5A8-CB5116E5A8DF}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class SchedulerConfigModule : ModuleDefinition2 {

    public SchedulerConfigModule() {
        Title = this.__ResStr("modTitle", "Scheduler Settings");
        Name = this.__ResStr("modName", "Scheduler Settings");
        Description = this.__ResStr("modSummary", "Used to edit a site's scheduler settings. This can be accessed using Admin > Settings > Scheduler Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SchedulerConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new SchedulerConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Scheduler Settings"),
            MenuText = this.__ResStr("editText", "Scheduler Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the scheduler settings"),
            Legend = this.__ResStr("editLegend", "Edits the scheduler settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Retention (Days)"), Description("Defines the number of days scheduler logging information is saved - Scheduler logging information that is older than the specified number of days is deleted")]
        [UIHint("IntValue"), Range(1, 99999999), Required]
        public int Days { get; set; }

        public SchedulerConfigData GetData(SchedulerConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(SchedulerConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (SchedulerConfigDataProvider dataProvider = new SchedulerConfigDataProvider()) {
            Model model = new Model { };
            SchedulerConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The logging settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (SchedulerConfigDataProvider dataProvider = new SchedulerConfigDataProvider()) {
            SchedulerConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Logging settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}