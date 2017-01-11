/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Search.Controllers;

namespace YetaWF.Modules.Search.Modules {

    public class SearchControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchControlModule>, IInstallableModel { }

    [ModuleGuid("{f7202e79-30bc-43ea-8d7a-12218785207b}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SearchControlModule : ModuleDefinition {

        public SearchControlModule() {
            Title = this.__ResStr("modTitle", "Skin Search Results Highlighter");
            Name = this.__ResStr("modName", "Search Results Highlighter (Skin)");
            Description = this.__ResStr("modSummary", "Highlights search results in a page");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            Invokable = true;
            InvokeInPopup = true;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SearchControlModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_On(bool shown) {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(SearchControlModuleController), "Switch"), 
                QueryArgs = new { Value = true },
                Image = "On.png",
                LinkText = this.__ResStr("onLink", "On"),
                MenuText = this.__ResStr("onText", "On"),
                Tooltip = this.__ResStr("onTooltip", "Turn on search results highlighting for this page"),
                Legend = this.__ResStr("onLegend", "Turns on search results highlighting for a page"),
                Style = ModuleAction.ActionStyleEnum.Post,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "On",
                Displayed = shown,
            };
        }
        public ModuleAction GetAction_Off(bool shown) {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(SearchControlModuleController), "Switch"),
                QueryArgs = new { Value = false },
                Image = "Off.png",
                LinkText = this.__ResStr("offLink", "Off"),
                MenuText = this.__ResStr("offText", "Off"),
                Tooltip = this.__ResStr("offTooltip", "Turn off search results highlighting for this page"),
                Legend = this.__ResStr("offLegend", "Turns off search results highlighting for a page"),
                Style = ModuleAction.ActionStyleEnum.Post,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Off",
                Displayed = shown,
            };
        }
    }
}