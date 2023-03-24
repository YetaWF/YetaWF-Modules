/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
using YetaWF.Modules.Dashboard.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Modules;

public class AuditRecordsModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuditRecordsModule>, IInstallableModel { }

[ModuleGuid("{8c1a3287-433f-4354-a3e8-867d5bd87b93}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AuditRecordsModule : ModuleDefinition {

    public AuditRecordsModule() {
        Title = this.__ResStr("modTitle", "Audit Information");
        Name = this.__ResStr("modName", "Audit Information");
        Description = this.__ResStr("modSummary", "Displays and manages audit information. Audit information can be accessed using Admin > Dashboard > Audit Information (standard YetaWF site).");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AuditRecordsModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display an audit record - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Audit Information"), this.__ResStr("roleRemItems", "The role has permission to remove individual audit records"),
                    this.__ResStr("userRemItemsC", "Remove Audit Information"), this.__ResStr("userRemItems", "The user has permission to remove individual audit records")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        return menuList;
    }

    public ModuleAction? GetAction_Items(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Audit Information"),
            MenuText = this.__ResStr("browseText", "Audit Information"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage audit information"),
            Legend = this.__ResStr("browseLegend", "Displays and manages audit information"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int id) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = $"{Utility.UrlFor(typeof(AuditRecordsModuleEndpoints), AuditRecordsModuleEndpoints.Remove)}/{id}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Audit Record"),
            MenuText = this.__ResStr("removeMenu", "Remove Audit Record"),
            Tooltip = this.__ResStr("removeTT", "Remove the audit record"),
            Legend = this.__ResStr("removeLegend", "Removes the audit record"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove audit record \"{0}\"?", id),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();
                AuditDisplayModule dispMod = new AuditDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int Id { get; set; }
        [Caption("Created"), Description("The date/time this record was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Identifier/String"), Description("The identifying string of the record - Identifier String and Type both identify the source of this record")]
        [UIHint("String"), ReadOnly]
        public string IdentifyString { get; set; } = null!;
        [Caption("Identifier/Type"), Description("The type of the record - Identifier String and Type both identify the source of this record")]
        [UIHint("Guid"), ReadOnly]
        public Guid IdentifyGuid { get; set; }

        [Caption("Action"), Description("The action that created this record")]
        [UIHint("String"), ReadOnly]
        public string Action { get; set; } = null!;
        [Caption("Description"), Description("The description for this record")]
        [UIHint("String"), ReadOnly]
        public string? Description { get; set; }
        [Caption("Changes"), Description("The properties that were changed")]
        [UIHint("String"), ReadOnly]
        public string? Changes { get; set; }

        [Caption("Site"), Description("The site that was changed")]
        [UIHint("SiteId"), ReadOnly]
        public int SiteIdentity { get; set; }
        [Caption("User"), Description("The user that made the change")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Restart Pending"), Description("Defines whether this action requires a restart to take effect")]
        [UIHint("Boolean"), ReadOnly]
        public bool RequiresRestart { get; set; }
        [Caption("Expensive Action"), Description("Defines whether this action is an expensive action in a multi-instance site")]
        [UIHint("Boolean"), ReadOnly]
        public bool ExpensiveMultiInstance { get; set; }

        public bool __highlight { get { return RequiresRestart && YetaWF.Core.Support.Startup.MultiInstanceStartTime < Created; ; } }
        public bool __lowlight { get { return YetaWF.Core.Support.Startup.MultiInstanceStartTime > Created; } }

        private AuditRecordsModule Module { get; set; }

        public BrowseItem(AuditRecordsModule module, AuditInfo data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
            Description = Description?.Truncate(100);
            Changes = Changes?.Replace(",", ", ").TruncateWithEllipse(100);
        }
    }

    public class BrowseModel {
        [Caption("Restart Pending"), Description("Defines whether a site restart (all instances) is pending to active all pending changes")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Boolean"), ReadOnly]
        public bool RestartPending { get; set; }

        [Caption("Last Restart"), Description("The date and time the site (all instances) was last restarted")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime LastRestart { get; set; }

        [Caption("Auditing Active"), Description("Defines whether auditing is active (enabled using appsettings.json)")]
        [UIHint("Boolean"), ReadOnly]
        [SuppressIf("AuditingActive", true)]
        public bool AuditingActive { get { return YetaWF.Core.Audit.Auditing.Active; } }

        [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Grid"), ReadOnly]
        [SuppressIf("AuditingActive", false)]
        public GridDefinition GridDef { get; set; } = null!;
    }

    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            InitialPageSize = 20,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<AuditRecordsModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                    DataProviderGetRecords<AuditInfo> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
            BrowseModel model = new BrowseModel {
                RestartPending = YetaWF.Core.Support.Startup.RestartPending || (YetaWF.Core.Audit.Auditing.Active ? await YetaWF.Core.Audit.Auditing.AuditProvider!.HasPendingRestartAsync() : false),
                LastRestart = YetaWF.Core.Support.Startup.MultiInstanceStartTime,
                GridDef = GetGridModel()
            };
            return await RenderAsync(model);
        }
    }
}
