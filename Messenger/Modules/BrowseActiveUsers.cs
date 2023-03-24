/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Modules.Messenger.Endpoints;

namespace YetaWF.Modules.Messenger.Modules;

public class BrowseActiveUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseActiveUsersModule>, IInstallableModel { }

[ModuleGuid("{A48F67F7-AF4A-47cf-AE9F-1859E5FB722C}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseActiveUsersModule : ModuleDefinition {

    public BrowseActiveUsersModule() {
        Title = this.__ResStr("modTitle", "Active Users");
        Name = this.__ResStr("modName", "Browse Active Users");
        Description = this.__ResStr("modSummary", "Displays currently active users. It is accessible using Admin > Dashboard > Active Users (standard YetaWF site).");
        DefaultViewName = StandardViews.Browse;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseActiveUsersModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public class BrowseItem {

        //[Caption("Actions"), Description("The available actions")]
        //[UIHint("ModuleActionsGrid"), ReadOnly]
        //public List<ModuleAction> Commands {
        //    get {
        //        List<ModuleAction> actions = new List<ModuleAction>();
        //        return actions;
        //    }
        //}

        [UIHint("Hidden")]
        public int Key { get; set; }

        [Caption("Connection Id"), Description("The connection id used to identify the active user")]
        [UIHint("String"), ReadOnly]
        public string ConnectionId { get; set; } = null!;

        [Caption("Created"), Description("The date and time the visitor connected to the site")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("User"), Description("The user's email address (if available)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("IP Address"), Description("The IP address of the site visitor")]
        [UIHint("IPAddress"), ReadOnly]
        public string IPAddress { get; set; } = null!;

        public BrowseItem(ActiveUser user) {
            ObjectSupport.CopyData(user, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            InitialPageSize = 20,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseActiveUsersModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (ActiveUsersDataProvider userDP = new ActiveUsersDataProvider()) {
                    DataProviderGetRecords<ActiveUser> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (ActiveUsersDataProvider userDP = new ActiveUsersDataProvider()) {
            if (!userDP.Usable)
                throw new Error(this.__ResStr("notEnabled", "Active users are not tracked - not enabled"));
        }
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}