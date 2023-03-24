/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;

namespace YetaWF.Modules.Identity.Modules;

public class RolesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesBrowseModule>, IInstallableModel { }

[ModuleGuid("{cc4761a9-977c-438b-880a-3381ab78b4a3}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class RolesBrowseModule : ModuleDefinition {

    public RolesBrowseModule() : base() {
        Title = this.__ResStr("modTitle", "Roles");
        Name = this.__ResStr("modName", "Roles");
        Description = this.__ResStr("modSummary", "Used to display and manage roles. Roles can be managed using Admin > Panel > Identity, Roles tab (standard YetaWF site).");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RolesBrowseModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new role - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string AddUrl { get; set; }
    [Category("General"), Caption("Display Url"), Description("The Url to display a role - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string DisplayUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a role - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveRoles",
                    this.__ResStr("roleRemItemsC", "Remove Roles"), this.__ResStr("roleRemItems", "The role has permission to remove individual roles"),
                    this.__ResStr("userRemItemsC", "Remove Roles"), this.__ResStr("userRemItems", "The user has permission to remove individual roles")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        RolesAddModule mod = new RolesAddModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction GetAction_Roles(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("rolesLink", "Roles"),
            MenuText = this.__ResStr("rolesText", "Roles"),
            Tooltip = this.__ResStr("rolesTooltip", "Display and manages roles"),
            Legend = this.__ResStr("rolesLegend", "Displays and manages roles"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction GetAction_RemoveLink(string name) {
        if (!IsAuthorized("RemoveRoles")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(RolesBrowseModuleEndpoints), RolesBrowseModuleEndpoints.Remove),
            QueryArgs = new { Name = name },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Role"),
            MenuText = this.__ResStr("removeMenu", "Remove Role"),
            Tooltip = this.__ResStr("removeTT", "Remove the role"),
            Legend = this.__ResStr("removeLegend", "Removes the role"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove role \"{0}\"?", name),
        };
    }

    /// <summary>
    /// Used from site template to add a site admin role
    /// </summary>
    public void AddAdministratorRole() {
        YetaWFManager.Syncify(async () => { // super-rare so sync is OK
            using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                await dataProvider.AddAdministratorRoleAsync();
            }
        });
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                RolesDisplayModule dispMod = new RolesDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                RolesEditModule editMod = new RolesEditModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, Name), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(Module.GetAction_RemoveLink(Name), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }
        }

        [Caption("Name"), Description("The role name")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; }

        [Caption("Description"), Description("The intended use of the role")]
        [UIHint("String"), ReadOnly]
        public string Description { get; set; }

        [Caption("Post Login Url"), Description("The Url where a user with this role is redirected after logging on")]
        [UIHint("Url"), ReadOnly]
        public string PostLoginUrl { get; set; }

        [Caption("Role Id"), Description("The internal id of the role")]
        [UIHint("IntValue"), ReadOnly]
        public int RoleId { get; set; }

        private RolesBrowseModule Module { get; set; }

        public BrowseItem(RolesBrowseModule module, DataProvider.RoleDefinition data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<RolesBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
                    DataProviderGetRecords<DataProvider.RoleDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
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
