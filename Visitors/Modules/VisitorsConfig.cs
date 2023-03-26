/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

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
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules;

public class VisitorsConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorsConfigModule>, IInstallableModel { }

[ModuleGuid("{2DA557B8-664A-4c23-839B-DC280CECCA47}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class VisitorsConfigModule : ModuleDefinition {

    public VisitorsConfigModule() {
        Title = this.__ResStr("modTitle", "Visitors Settings");
        Name = this.__ResStr("modName", "Visitors Settings");
        Description = this.__ResStr("modSummary", "Used to edit a site's visitors settings. This can be accessed using Admin > Settings > Visitors Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new VisitorsConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new VisitorsConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Visitors Settings"),
            MenuText = this.__ResStr("editText", "Visitors Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the visitors settings"),
            Legend = this.__ResStr("editLegend", "Edits the visitors settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    public class Model {

        [Caption("Retention (Days)"), Description("Defines the number of days visitors information is saved - Visitors information that is older than the specified number of days is deleted")]
        [UIHint("IntValue"), Range(1, 99999999), Required]
        public int Days { get; set; }

        public VisitorsConfigData GetData(VisitorsConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(VisitorsConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (VisitorsConfigDataProvider visitorDP = new VisitorsConfigDataProvider()) {
            if (!await visitorDP.IsInstalledAsync())
                throw new Error(this.__ResStr("noInfo", "Visitor information is not available - See https://yetawf.com/Documentation/YetaWF/Visitors"));
            Model model = new Model { };
            VisitorsConfigData data = await visitorDP.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The visitors settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (VisitorsConfigDataProvider dataProvider = new VisitorsConfigDataProvider()) {
            VisitorsConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Visitors settings saved"));
        }
    }
}