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
using YetaWF.Core.Identity;
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

public class BrowseExtensionsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseExtensionsModule>, IInstallableModel { }

[ModuleGuid("{c90d1c0b-7ed3-4584-8e1e-561714cf7c57}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseExtensionsModule : ModuleDefinition {

    public BrowseExtensionsModule() {
        Title = this.__ResStr("modTitle", "Extensions");
        Name = this.__ResStr("modName", "Extensions");
        Description = this.__ResStr("modSummary", "Displays and manages extensions.");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseExtensionsModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new extension - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a extension - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Extension Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual extensions"),
                    this.__ResStr("userRemItemsC", "Remove Extension Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual extensions")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        AddExtensionModule mod = new AddExtensionModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Extensions"),
            MenuText = this.__ResStr("browseText", "Extensions"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage extensions"),
            Legend = this.__ResStr("browseLegend", "Displays and manages extensions"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int id) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = $"{Utility.UrlFor(typeof(BrowseExtensionsModuleEndpoints), BrowseExtensionsModuleEndpoints.Remove)}/{id}",
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove Extension"),
            MenuText = this.__ResStr("removeMenu", "Remove Extension"),
            Tooltip = this.__ResStr("removeTT", "Remove the extension"),
            Legend = this.__ResStr("removeLegend", "Removes the extension"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove extension with id \"{0}\"?", id),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                EditExtensionModule editMod = new EditExtensionModule();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, Extension), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

        [Caption("Extension"), Description("Defines the extension (digits)")]
        [UIHint("String"), ReadOnly]
        [StringLength(ExtensionEntry.MaxExtension)]
        public string Extension { get; set; } = null!;

        [Caption("Description"), Description("Describes the extension")]
        [UIHint("String"), ReadOnly]
        [StringLength(ExtensionEntry.MaxDescription)]
        public string? Description { get; set; }

        [Caption("Phone Numbers / SMS"), Description("Shows the phone numbers to call when this extension is entered and the text messaging selection")]
        [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), AdditionalMetadata("UseSkinFormatting", false), AdditionalMetadata("Header", false), AdditionalMetadata("Pager", false), ReadOnly]
        public SerializableList<ExtensionPhoneNumber>? PhoneNumbers { get; set; }

        [Caption("Users"), Description("Shows the users that can access voice mails for this extension")]
        [UIHint("YetaWF_Identity_ListOfUserNames"), AdditionalMetadata("UseSkinFormatting", false), AdditionalMetadata("Header", false), AdditionalMetadata("Pager", false), ReadOnly]
        public SerializableList<User>? Users { get; set; }

        [Caption("Created"), Description("The date/time the extension was added")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }
        [Caption("Updated"), Description("The date/time the extension was last updated")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? Updated { get; set; }

        public int Id { get; set; }
        private BrowseExtensionsModule Module { get; set; }

        public BrowseItem(BrowseExtensionsModule module, ExtensionEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseExtensionsModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
                    DataProviderGetRecords<ExtensionEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
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
