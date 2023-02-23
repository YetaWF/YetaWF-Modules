/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Modules;

public class ControlPanelConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, ControlPanelConfigModule>, IInstallableModel { }

[ModuleGuid("{6c41ee8f-fcba-4bbd-90cc-8cae9ccd899e}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class ControlPanelConfigModule : ModuleDefinition2 {

    public ControlPanelConfigModule() {
        Title = this.__ResStr("modTitle", "Control Panel Settings");
        Name = this.__ResStr("modName", "Control Panel Settings");
        Description = this.__ResStr("modSummary", "Used to edit Control Panel settings. It is accessible using Admin > Settings > Control Panel Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ControlPanelConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new ControlPanelConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Control Panel Settings"),
            MenuText = this.__ResStr("editText", "Control Panel Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the Control Panel settings"),
            Legend = this.__ResStr("editLegend", "Edits the Control Panel settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("W3C Validation"), Description("The Url used to validate the current page using a W3C Validation service - Use {0} where the Url is inserted - If no Url is defined, the Control Panel will not display a W3C Validation link")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string? W3CUrl { get; set; }

        [Category("General"), Caption("User List"), Description("List of user accounts that can be used to quickly log into the site")]
        [UIHint("YetaWF_Identity_ResourceUsers")]
        [Data_Binary]
        public SerializableList<User> Users { get; set; }

        public ControlPanelConfigData GetData(ControlPanelConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(ControlPanelConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() {
            Users = new SerializableList<User>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
            Model model = new Model { };
            ControlPanelConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The Control Panel settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
            ControlPanelConfigData origConfig = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            ControlPanelConfigData config = await dataProvider.GetItemAsync();
            config = model.GetData(config); // merge new data into config
            model.SetData(config); // and all the data back into model for final display

            await dataProvider.UpdateConfigAsync(config);

            ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origConfig, config);
            switch (modelDisp) {
                default:
                case ObjectSupport.ModelDisposition.None:
                    return await FormProcessedAsync(model, this.__ResStr("okSaved", "Control Panel settings saved"));
                case ObjectSupport.ModelDisposition.PageReload:
                    return await FormProcessedAsync(model, this.__ResStr("okSaved", "Control Panel settings saved"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect: true);
                case ObjectSupport.ModelDisposition.SiteRestart:
                    return await FormProcessedAsync(model, this.__ResStr("okSavedRestart", "Control Panel settings saved - These settings won't take effect until the site is restarted"));
            }
        }
    }
}