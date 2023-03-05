/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
using YetaWF.Modules.Dashboard.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules;

public class AuditConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditConfigModule>, IInstallableModel { }

[ModuleGuid("{3C45FB8E-123A-45f6-89BD-75596473F70B}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class AuditConfigModule : ModuleDefinition2 {

    public AuditConfigModule() {
        Title = this.__ResStr("modTitle", "Audit Settings");
        Name = this.__ResStr("modName", "Audit Settings");
        Description = this.__ResStr("modSummary", "Used to edit a site's audit settings. This can be accessed using Admin > Settings > Audit Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AuditConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new AuditConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Audit Settings"),
            MenuText = this.__ResStr("editText", "Audit Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the audit settings"),
            Legend = this.__ResStr("editLegend", "Edits the audit settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Retention (Days)"), Description("Defines the number of days audit information is saved - Audit information that is older than the specified number of days is deleted")]
        [UIHint("IntValue"), Range(1, 99999999), Required]
        public int Days { get; set; }

        public AuditConfigData GetData(AuditConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(AuditConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (AuditConfigDataProvider dataProvider = new AuditConfigDataProvider()) {
            Model model = new Model { };
            AuditConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The audit settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (AuditConfigDataProvider dataProvider = new AuditConfigDataProvider()) {
            AuditConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Audit settings saved"), NextPage: Manager.ReturnToUrl);
        }
    }
}
