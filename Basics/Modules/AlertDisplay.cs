/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules;

public class AlertDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AlertDisplayModule>, IInstallableModel { }

[ModuleGuid("{24b7dc07-e96a-409d-911f-47bffd38d0fc}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class AlertDisplayModule : ModuleDefinition2 {

    public AlertDisplayModule() {
        Title = this.__ResStr("modTitle", "Alert Message (Skin)");
        Name = this.__ResStr("modName", "Alert Message (Skin)");
        Description = this.__ResStr("modSummary", "Can be referenced by sites, pages or modules. Once referenced, an Alert will be displayed (once or until dismissed, depending on Alert Settings).");
        WantFocus = false;
        WantSearch = false;
        ShowTitle = false;
        Print = false;
        Invokable = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AlertDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public AlertConfig.MessageHandlingEnum MessageHandling { get; set; }
        public string Message { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (Manager.EditMode) return ActionInfo.Empty;
        if (Manager.IsInPopup) return ActionInfo.Empty;

        bool done = Manager.SessionSettings.SiteSettings.GetValue<bool>("YetaWF_Basics_AlertDone", false);
        if (done) return ActionInfo.Empty;

        using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {

            AlertConfig config = await dataProvider.GetItemAsync();
            if (config == null || !config.Enabled)
                return ActionInfo.Empty;

            if (config.MessageHandling == AlertConfig.MessageHandlingEnum.DisplayOnce) {
                Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_Basics_AlertDone", true);
                Manager.SessionSettings.SiteSettings.Save();
            }

            DisplayModel model = new DisplayModel() {
                MessageHandling = config.MessageHandling,
                Message = config.Message,
            };
            return await RenderAsync(model);
        }
    }
}
