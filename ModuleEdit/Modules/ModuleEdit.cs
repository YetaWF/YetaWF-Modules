/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ModuleEdit.Modules;

public class ModuleEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleEditModule>, IInstallableModel { }

[ModuleGuid("{ACDC1453-32BD-4de2-AB2B-7BF5CE217762}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class ModuleEditModule : ModuleDefinition2 {

    public ModuleEditModule() {
        Name = this.__ResStr("modName", "Module Edit");
        Title = this.__ResStr("modTitle", "Module Editing Features");
        Description = this.__ResStr("modSummary", "Implements editing features used in Site Edit Mode to edit module settings.");
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ModuleEditModuleDataProvider(); }

    public override bool ShowModuleMenu { get { return false; } }
    public override bool ModuleHasSettings { get { return false; } }

    public Task<ModuleAction?> GetAction_SettingsAsync(Guid editGuid) {
        return GetAction_SettingsAsync(editGuid, false);
    }
    public Task<ModuleAction?> GetAction_SettingsGenerateAsync(Guid editGuid) {
        return GetAction_SettingsAsync(editGuid, true);
    }
    private async Task<ModuleAction?> GetAction_SettingsAsync(Guid editGuid, bool force) {
        ModuleDefinition? editMod = await ModuleDefinition.LoadAsync(editGuid, AllowNone: true);
        if (editMod == null) {
            if (!force)
                return null;
            editGuid = Guid.Empty;
        } else {
            if (!editMod.ModuleHasSettings) return null;
        }
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_ModuleSettings))
            return null;
        return new ModuleAction() {
            Url = ModulePermanentUrl,
            QueryArgs = new { ModuleGuid = editGuid },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Settings"),
            MenuText = this.__ResStr("editText", "Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the module's settings"),
            Legend = this.__ResStr("editLegend", "Edits the module's settings"),
            Category = ModuleAction.ActionCategoryEnum.Update,

            Mode = ModuleAction.ActionModeEnum.Any,
            Style = ModuleAction.ActionStyleEnum.PopupEdit,
            Location = ModuleAction.ActionLocationEnum.ModuleMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.GridLinks,
            SaveReturnUrl = true,
        };
    }

    public class ModuleEditModel {

        [UIHint("PropertyList")]
        public ModuleDefinition Dynamic { get; set; } = null!;

        [UIHint("Hidden")]
        public Guid ModuleGuid { get; set; }

        internal async Task UpdateDataAsync() {
            await ObjectSupport.HandlePropertyAsync<List<PageDefinition>>(nameof(ModuleDefinition.Pages), nameof(ModuleDefinition.__GetPagesAsync), Dynamic);
        }
    }

    [ResourceAuthorize(CoreInfo.Resource_ModuleSettings)]
    public async Task<ActionInfo> RenderModuleAsync(Guid moduleGuid) {
        ModuleDefinition module = await ModuleDefinition.LoadAsync(moduleGuid) ?? throw new InternalError($"{nameof(ModuleEdit)} called with {moduleGuid} which doesn't exist");
        ModuleEditModel model = new ModuleEditModel() {
            Dynamic = module,
            ModuleGuid = moduleGuid,
        };
        Title = this.__ResStr("modEditTitle", "Module \"{0}\"", module.Title.ToString());
        await model.UpdateDataAsync();
        Manager.CurrentModuleEdited = module;
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_ModuleSettings)]
    public async Task<IResult> UpdateModuleAsync(ModuleEditModel model, string Dynamic) {

        if (model.ModuleGuid == Guid.Empty)
            throw new InternalError("No moduleGuid provided");

        // we need to find the real type of the module for data binding
        ModuleDefinition origModule = (await ModuleDefinition.LoadAsync(model.ModuleGuid))!;
        Type moduleType = origModule.GetType();
        ModuleDefinition2? module = Utility.JsonDeserialize(Dynamic, moduleType) as ModuleDefinition2;
        if (module is null)
            throw new InternalError($"Model data missing for module {moduleType.FullName}");

        model.Dynamic = module;
        ObjectSupport.CopyData(origModule, module, ReadOnly: true); // update read only properties in model in case there is an error
        ObjectSupport.CopyDataFromOriginal(origModule, module);
        await model.UpdateDataAsync();// in case of validation errors

        await ModelState.ValidateModel(model, null, null, null);

        Manager.CurrentModuleEdited = module;

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        module.CustomValidation(ModelState, $"{nameof(model.Dynamic)}.");
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        await model.UpdateDataAsync();

        // copy/save
        module.Temporary = false;
        await module.SaveAsync();
        return await FormProcessedAsync(model, this.__ResStr("okSaved", "Module settings saved"));
    }
}