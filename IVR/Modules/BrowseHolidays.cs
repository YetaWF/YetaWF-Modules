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

namespace Softelvdm.Modules.IVR.Modules {

    public class BrowseHolidaysModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseHolidaysModule>, IInstallableModel { }

    [ModuleGuid("{b8096d87-6485-49b3-a831-a73d29472fb0}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseHolidaysModule : ModuleDefinition2 {

        public BrowseHolidaysModule() {
            Title = this.__ResStr("modTitle", "Holidays");
            Name = this.__ResStr("modName", "Holiday Entries");
            Description = this.__ResStr("modSummary", "Displays and manages holiday entries.");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseHolidaysModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new holiday entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? AddUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Holiday Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual holiday entries"),
                        this.__ResStr("userRemItemsC", "Remove Holiday Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual holiday entries")),
                };
            }
        }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            AddHolidayModule mod = new AddHolidayModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction? GetAction_Items(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Holiday Entries"),
                MenuText = this.__ResStr("browseText", "Holiday Entries"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage holiday entries"),
                Legend = this.__ResStr("browseLegend", "Displays and manages holiday entries"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction? GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = $"{Utility.UrlFor(typeof(BrowseHolidaysModuleEndpoints), BrowseHolidaysModuleEndpoints.Remove)}/{id}",
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Holiday Entry"),
                MenuText = this.__ResStr("removeMenu", "Remove Holiday Entry"),
                Tooltip = this.__ResStr("removeTT", "Remove the holiday entry"),
                Legend = this.__ResStr("removeLegend", "Removes the holiday entry"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this holiday entry?"),
            };
        }

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();
                    actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Date"), Description("The date the holiday occurs")]
            [UIHint("Date"), ReadOnly]
            public DateTime HolidayDate { get; set; }

            [Caption("Description"), Description("The description of the holiday")]
            [UIHint("String"), StringLength(HolidayEntry.MaxDescription), ReadOnly]
            public string? Description { get; set; }

            public int Id { get; set; }

            private BrowseHolidaysModule Module { get; set; }

            public BrowseItem(BrowseHolidaysModule module, HolidayEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }
        public GridDefinition GetGridModel() {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = ModuleGuid,
                //SettingsModuleGuid = PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<BrowseHolidaysModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (HolidayEntryDataProvider dataProvider = new HolidayEntryDataProvider()) {
                        DataProviderGetRecords<HolidayEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
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
}
