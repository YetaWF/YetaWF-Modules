/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Languages.Endpoints;

namespace YetaWF.Modules.Languages.Modules;

public class LanguagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguagesBrowseModule>, IInstallableModel { }

[ModuleGuid("{0ce1d3eb-6f43-44ad-acf0-4590652f9012}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class LanguagesBrowseModule : ModuleDefinition2 {

    public LanguagesBrowseModule() : base() {
        Title = this.__ResStr("modTitle", "Languages");
        Name = this.__ResStr("modName", "Languages");
        Description = this.__ResStr("modSummary", "Displays available languages. It is accessible using Admin > Panel > Languages (standard YetaWF site).");
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new LanguagesBrowseModuleDataProvider(); }

    [Category("General"), Caption("Display URL"), Description("The URL to display a language - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Languages(string? url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Languages"),
            MenuText = this.__ResStr("browseText", "Languages"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage languages"),
            Legend = this.__ResStr("browseLegend", "Displays and manages languages"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                LanguageDisplayModule dispMod = new LanguageDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }
        }

        [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
        [UIHint("String"), ReadOnly]
        public string Id { get; set; } = null!;

        [Caption("Name"), Description("The language's short name, which is displayed in language selection controls so the user can select a language")]
        [UIHint("String"), ReadOnly]
        public string ShortName { get; set; } = null!;

        [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
        [UIHint("String"), ReadOnly]
        public string? Description { get; set; }

        private LanguagesBrowseModule Module { get; set; }

        public BrowseItem(LanguagesBrowseModule module, LanguageEntryElement data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    [Header("Languages are defined in the LanguageSettings.json file in the Data folder.")]
    public class BrowseModel {
        [Caption(""), Description("")]
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<LanguagesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                DataProviderGetRecords<LanguageEntryElement> browseItems = DataProviderImpl<LanguageEntryElement>.GetRecords(LanguageSection.Languages, skip, take, sort, filters);
                return Task.FromResult(new DataSourceResult {
                    Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                    Total = browseItems.Total
                });
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
