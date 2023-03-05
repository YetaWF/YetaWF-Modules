/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules;

public class BrowseBlockedNumbersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseBlockedNumbersModule>, IInstallableModel { }

[ModuleGuid("{9a34582b-631a-4d6f-9557-e2b08228c254}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseBlockedNumbersModule : ModuleDefinition2 {

    public BrowseBlockedNumbersModule() {
        Title = this.__ResStr("modTitle", "Blocked Numbers");
        Name = this.__ResStr("modName", "Blocked Numbers");
        Description = this.__ResStr("modSummary", "Displays and manages blocked numbers.");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseBlockedNumbersModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new blocked number - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a blocked number - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Blocked Numbers"), this.__ResStr("roleRemItems", "The role has permission to remove individual blocked numbers"),
                    this.__ResStr("userRemItemsC", "Remove Blocked Numbers"), this.__ResStr("userRemItems", "The user has permission to remove individual blocked numbers")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        AddBlockedNumberModule mod = new AddBlockedNumberModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Blocked Numbers"),
            MenuText = this.__ResStr("browseText", "Blocked Numbers"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage blocked numbers"),
            Legend = this.__ResStr("browseLegend", "Displays and manages blocked numbers"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int id) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = $"{Utility.UrlFor(typeof(BrowseBlockedNumbersModuleEndpoints), nameof(BrowseBlockedNumbersModuleEndpoints.Remove))}/{id}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove"),
            MenuText = this.__ResStr("removeMenu", "Remove"),
            Tooltip = this.__ResStr("removeTT", "Remove the blocked number"),
            Legend = this.__ResStr("removeLegend", "Removes the blocked number"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this blocked number?"),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                EditBlockedNumberModule editMod = new EditBlockedNumberModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, Number), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Blocked Number"), Description("Shows the blocked phone number")]
        [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), ReadOnly]
        public string Number { get; set; } = null!;

        [Caption("Description"), Description("The description of the blocked number")]
        [UIHint("String"), StringLength(BlockedNumberEntry.MaxDescription), ReadOnly]
        public string? Description { get; set; }

        [Caption("Added"), Description("The date/time the entry was added")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }
        [Caption("Updated"), Description("The date/time the entry was last updated")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? Updated { get; set; }

        public int Id { get; set; }

        private BrowseBlockedNumbersModule Module { get; set; }

        public BrowseItem(BrowseBlockedNumbersModule module, BlockedNumberEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
            data.Description = data.Description?.Truncate(200);
        }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseBlockedNumbersModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
                    DataProviderGetRecords<BlockedNumberEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public class BrowseModel {
        [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
