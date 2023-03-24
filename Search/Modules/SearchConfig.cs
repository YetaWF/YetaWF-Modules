/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

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
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Modules;

public class SearchConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchConfigModule>, IInstallableModel { }

[ModuleGuid("{f27ed8f6-e844-4668-a9fe-1dda07bd7277}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class SearchConfigModule : ModuleDefinition {

    public SearchConfigModule() {
        Title = this.__ResStr("modTitle", "Search Settings");
        Name = this.__ResStr("modName", "Search Settings");
        Description = this.__ResStr("modSummary", "Main configuration module for search settings. It is accessible using Admin > Settings > Search Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SearchConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new SearchConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Search Settings"),
            MenuText = this.__ResStr("editText", "Search Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the site's search settings"),
            Legend = this.__ResStr("editLegend", "Edits the search settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("Smallest Keyword (Mixed Case)"), Description("The smallest mixed case keyword")]
        [UIHint("IntValue2"), Range(2, 10), Required]
        public int SmallestMixedToken { get; set; }

        [Caption("Smallest Keyword (Uppercase)"), Description("The smallest all uppercase keyword")]
        [UIHint("IntValue2"), Range(2, 10), Required]
        public int SmallestUpperCaseToken { get; set; }

        public SearchConfigData GetData(SearchConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(SearchConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
            Model model = new Model { };
            SearchConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The search configuration was not found."));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
            SearchConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Search configuration saved"));
        }
    }
}