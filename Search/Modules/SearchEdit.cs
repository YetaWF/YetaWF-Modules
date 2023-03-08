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

public class SearchEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, SearchEditModule>, IInstallableModel { }

[ModuleGuid("{dfa30d65-52b8-4b7f-a2f4-1e4e73477b04}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SearchEditModule : ModuleDefinition2 {

    public SearchEditModule() {
        Title = this.__ResStr("modTitle", "Search Item");
        Name = this.__ResStr("modName", "Edit Search Item");
        Description = this.__ResStr("modSummary", "Used to edit an existing search term. This is used by the Search Keywords Module to edit a search term.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SearchEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Edit(string? url, int searchDataId) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { searchDataId = searchDataId },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit an existing search item"),
            Legend = this.__ResStr("editLegend", "Edits an existing search item"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Search Keyword"), Description("Search Keyword")]
        [UIHint("Text40"), StringLength(SearchData.MaxSearchTerm), Required]
        public string? SearchTerm { get; set; }

        [Caption("Added"), Description("The date/time this keyword was added")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime DateAdded { get; set; }

        [Caption("Url"), Description("The page where this keyword was found")]
        [UIHint("Url"), ReadOnly]
        public string? PageUrl { get; set; }

        [Caption("Language"), Description("The page language where this keyword was found")]
        [UIHint("LanguageId")]
        public string? Language { get; set; }

        [Caption("Count"), Description("The number of times this keyword was found on the page")]
        [UIHint("IntValue"), Range(1, 999999)]
        public int Count { get; set; }

        [Caption("Anonymous Users"), Description("Whether anonymous users can view the page")]
        [UIHint("Boolean")]
        public bool AllowAnonymous { get; set; }
        [Caption("Any Users"), Description("Whether any logged on user can view the page")]
        [UIHint("Boolean")]
        public bool AllowAnyUser { get; set; }

        [Caption("Id"), Description("The internal id this keyword")]
        [UIHint("IntValue"), ReadOnly]
        public int DisplaySearchDataId { get; set; }

        [UIHint("Hidden")]
        public int SearchDataId { get; set; }

        public SearchData GetData(SearchData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(SearchData data) {
            ObjectSupport.CopyData(data, this);
            DisplaySearchDataId = data.SearchDataId;
        }
        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync(int searchDataId) {
        if (!SearchDataProvider.IsUsable) return await RenderAsync(new { }, ViewName: "SearchUnavailable_Edit");
        using (SearchDataProvider dataProvider = new SearchDataProvider()) {
            EditModel model = new EditModel { };
            SearchData? data = await dataProvider.GetItemWithUrlAsync(searchDataId);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Search keyword with id {0} not found."), searchDataId);
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        using (SearchDataProvider dataProvider = new SearchDataProvider()) {
            SearchData? data = await dataProvider.GetItemWithUrlAsync(model.SearchDataId);
            if (data == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The search keyword with id {0} has been removed and can no longer be updated.", model.SearchDataId));

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display

            switch (await dataProvider.UpdateItemAsync(data)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The search keyword with id {0} has been removed and can no longer be updated.", model.SearchDataId));
                case UpdateStatusEnum.NewKeyExists:
                    throw new Error(this.__ResStr("alreadyExists", "A search keyword with id {0} already exists.", model.SearchDataId));
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Search keyword saved"), OnClose: OnCloseEnum.Return, OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}