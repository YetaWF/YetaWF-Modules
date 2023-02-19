/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Backups.DataProvider;

namespace YetaWF.Modules.Backups.Modules;

public class BackupConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, BackupConfigModule>, IInstallableModel { }

[ModuleGuid("{84b5bc7e-e5d9-4ab1-9535-8ba729d66649}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class BackupConfigModule : ModuleDefinition2 {

    public BackupConfigModule() {
        Title = this.__ResStr("modTitle", "Backup Settings");
        Name = this.__ResStr("modName", "Backup Settings");
        Description = this.__ResStr("modSummary", "Edits a site's backup settings.");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BackupConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Backup Settings"),
            MenuText = this.__ResStr("editText", "Backup Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the backup settings"),
            Legend = this.__ResStr("editLegend", "Edits the backup settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
        [UIHint("IntValue"), Range(1, 99999999), Required]
        public int Days { get; set; }

        [Caption("Data Only"), Description("Defines whether only data is backed up (scheduled backups) - Otherwise all source and binaries (complete packages) are backed up - For production sites it is generally sufficient to back up the data")]
        [UIHint("Boolean")]
        public bool DataOnly { get; set; }

        public ConfigData GetData(ConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(ConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            Model model = new Model { };
            ConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The backup settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            ConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Backup settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}
