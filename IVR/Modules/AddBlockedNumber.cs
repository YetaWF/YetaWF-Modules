/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules;

public class AddBlockedNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddBlockedNumberModule>, IInstallableModel { }

[ModuleGuid("{f57c96c9-f3aa-43f7-b9a6-4e30282889e4}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AddBlockedNumberModule : ModuleDefinition {

    public AddBlockedNumberModule() {
        Title = this.__ResStr("modTitle", "Add New Blocked Number");
        Name = this.__ResStr("modName", "Add New Blocked Number");
        Description = this.__ResStr("modSummary", "Adds a new blocked number.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddBlockedNumberModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Add a new blocked number"),
            Legend = this.__ResStr("addLegend", "Adds a new blocked number"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,

        };
    }

    [Trim]
    public class AddModel {

        [Caption("Blocked Number"), Description("Enter the phone number to block")]
        [UIHint("Text20"), StringLength(Globals.MaxPhoneNumber), Required, Trim]
        public string? Number { get; set; }

        [Caption("Description"), Description("Enter an optional description of the blocked number")]
        [UIHint("TextAreaSourceOnly"), StringLength(BlockedNumberEntry.MaxDescription)]
        public string? Description { get; set; }

        public AddModel() { }

        public BlockedNumberEntry GetData() {
            BlockedNumberEntry data = new BlockedNumberEntry();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        ObjectSupport.CopyData(new BlockedNumberEntry(), model);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData())) {
                ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Blocked number already exists"));
                return await PartialViewAsync(model);
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New blocked number saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
