/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

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
using YetaWF.Modules.Logging.DataProvider;

namespace YetaWF.Modules.Logging.Modules;

public class LoggingConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, LoggingConfigModule>, IInstallableModel { }

[ModuleGuid("{5F8435C9-9896-460b-A0CC-26F5C3693B39}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class LoggingConfigModule : ModuleDefinition {

    public LoggingConfigModule() {
        Title = this.__ResStr("modTitle", "Logging Settings");
        Name = this.__ResStr("modName", "Logging Settings");
        Description = this.__ResStr("modSummary", "Used to edit a site's logging settings. This can be accessed using Admin > Settings > Logging Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new LoggingConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new LoggingConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Logging Settings"),
            MenuText = this.__ResStr("editText", "Logging Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the logging settings"),
            Legend = this.__ResStr("editLegend", "Edits the logging settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Retention (Days)"), Description("Defines the number of days logging information is saved - Logging information that is older than the specified number of days is deleted")]
        [UIHint("IntValue"), Range(1, 99999999), Required]
        public int Days { get; set; }

        public LoggingConfigData GetData(LoggingConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(LoggingConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (LoggingConfigDataProvider dataProvider = new LoggingConfigDataProvider()) {
            Model model = new Model { };
            LoggingConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The logging settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (LoggingConfigDataProvider dataProvider = new LoggingConfigDataProvider()) {
            LoggingConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Logging settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}