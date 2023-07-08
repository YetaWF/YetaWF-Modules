/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.TinyLanguage.Modules;

public class TinyLanguageModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyLanguageModule>, IInstallableModel { }

[ModuleGuid("{bb3653e4-6d75-45e2-b998-714b225b5ffa}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class TinyLanguageModule : ModuleDefinition {

    public TinyLanguageModule() : base() {
        Title = this.__ResStr("modTitle", "Tiny Language Selection");
        Name = this.__ResStr("modName", "Tiny Language Selection");
        Description = this.__ResStr("modSummary", "Allows user selection of the site's default language. This module is typically added to every page (as a skin module) so the user can switch the site's default language.");
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TinyLanguageModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    [Trim]
    public class EditModel {

        [Caption("Language"), Description("Select the language to be used for the entire site")]
        [UIHint("LanguageId"), StringLength(LanguageData.MaxId), SubmitFormOnChange]
        public string? LanguageId { get; set; }

        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        EditModel model = new EditModel { LanguageId = Manager.UserLanguage };
        return await RenderAsync(model);
    }

    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        await Manager.SetUserLanguageAsync(model.LanguageId!);
        return await FormProcessedAsync(model, OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceReload: true);
    }
}