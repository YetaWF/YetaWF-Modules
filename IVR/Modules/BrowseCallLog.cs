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

public class BrowseCallLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseCallLogModule>, IInstallableModel { }

[ModuleGuid("{5c9b50ed-b434-451d-b795-59d6a5125c6a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseCallLogModule : ModuleDefinition2 {

    public BrowseCallLogModule() {
        Title = this.__ResStr("modTitle", "Call Log Entries");
        Name = this.__ResStr("modName", "Call Log Entries");
        Description = this.__ResStr("modSummary", "Displays and manages call log entries.");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseCallLogModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display a call log entry - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Call Log Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual call log entries"),
                    this.__ResStr("userRemItemsC", "Remove Call Log Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual call log entries")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        return menuList;
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Call Log Entries"),
            MenuText = this.__ResStr("browseText", "Call Log Entries"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage call log entries"),
            Legend = this.__ResStr("browseLegend", "Displays and manages call log entries"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int id) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = $"{Utility.UrlFor(typeof(BrowseCallLogModuleEndpoints), BrowseCallLogModuleEndpoints.Remove)}/{id}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove"),
            MenuText = this.__ResStr("removeMenu", "Remove"),
            Tooltip = this.__ResStr("removeTT", "Remove the call log entry"),
            Legend = this.__ResStr("removeLegend", "Removes the call log entry"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove call log entry with id {0}?", id),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                DisplayCallLogModule dispMod = new DisplayCallLogModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int Id { get; set; }

        [Caption("Created"), Description("The date/time the call was received")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("From"), Description("The caller's phone number")]
        [UIHint("PhoneNumber"), ReadOnly]
        [ExcludeDemoMode]
        public string Caller { get; set; } = null!;
        [Caption("From City"), Description("The caller's city (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerCity { get; set; }
        [Caption("From State"), Description("The caller's state (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerState { get; set; }
        [Caption("From Zip Code"), Description("The caller's ZIP code (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerZip { get; set; }
        [Caption("From Country"), Description("The caller's country (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CallerCountry { get; set; }

        [Caption("Phone Number"), Description("The phone number called")]
        [UIHint("PhoneNumber"), ReadOnly]
        public string? To { get; set; }

        private BrowseCallLogModule Module { get; set; }

        public BrowseItem(BrowseCallLogModule module, CallLogEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseCallLogModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (CallLogDataProvider dataProvider = new CallLogDataProvider()) {
                    DataProviderGetRecords<CallLogEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
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
