/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Search.Modules {

    public class SearchControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchControlModule>, IInstallableModel { }

    [ModuleGuid("{f7202e79-30bc-43ea-8d7a-12218785207b}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SearchControlModule : ModuleDefinition {

        public SearchControlModule() {
            Title = this.__ResStr("modTitle", "Skin Search Results Highlighter");
            Name = this.__ResStr("modName", "Search Results Highlighter (Skin)");
            Description = this.__ResStr("modSummary", "Highlights any search terms found on the current page displayed. It can be included as a skin module or referenced by a site, page or module (see References tab in Page, Module or Site Settings). See Skin Modules for more information.");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SearchControlModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_OnAsync() {
            return new ModuleAction(this) {
                Image = await CustomIconAsync("On.png"),
                LinkText = this.__ResStr("onLink", "On"),
                MenuText = this.__ResStr("onText", "On"),
                Tooltip = this.__ResStr("onTooltip", "Turn on search results highlighting for this page"),
                Legend = this.__ResStr("onLegend", "Turns on search results highlighting for a page"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "On",
                Displayed = false,
            };
        }
        public async Task<ModuleAction> GetAction_OffAsync() {
            return new ModuleAction(this) {
                Image = await CustomIconAsync("Off.png"),
                LinkText = this.__ResStr("offLink", "Off"),
                MenuText = this.__ResStr("offText", "Off"),
                Tooltip = this.__ResStr("offTooltip", "Turn off search results highlighting for this page"),
                Legend = this.__ResStr("offLegend", "Turns off search results highlighting for a page"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Off",
                Displayed = false,
            };
        }
    }
}