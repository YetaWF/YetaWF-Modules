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

public class BrowseVoiceMailsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseVoiceMailsModule>, IInstallableModel { }

[ModuleGuid("{1ab039e0-6e16-4992-8fc9-8bfb0c29824b}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseVoiceMailsModule : ModuleDefinition {

    public BrowseVoiceMailsModule() {
        Title = this.__ResStr("modTitle", "Voice Mail Entries");
        Name = this.__ResStr("modName", "Voice Mail Entries");
        Description = this.__ResStr("modSummary", "Displays and manages voice mail entries.");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseVoiceMailsModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display a voice mail entry customer - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove Voice Mail Entries"), this.__ResStr("roleRemItems", "The role has permission to remove individual voice mail entries"),
                    this.__ResStr("userRemItemsC", "Remove Voice Mail Entries"), this.__ResStr("userRemItems", "The user has permission to remove individual voice mail entries")),
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
            LinkText = this.__ResStr("browseLink", "Voice Mail Entries"),
            MenuText = this.__ResStr("browseText", "Voice Mail Entries"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage voice mail entries"),
            Legend = this.__ResStr("browseLegend", "Displays and manages voice mail entries"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove(int id) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(BrowseVoiceMailsModuleEndpoints), BrowseVoiceMailsModuleEndpoints.Remove),
            QueryArgs = new { Id = id },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove"),
            MenuText = this.__ResStr("removeMenu", "Remove"),
            Tooltip = this.__ResStr("removeTT", "Remove the voice mail entry"),
            Legend = this.__ResStr("removeLegend", "Removes the voice mail entry"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this voice mail entry"),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands { get; set; } = null!;

        public async Task<List<ModuleAction>> __GetCommandsAsync() {
            List<ModuleAction> actions = new List<ModuleAction>();
            actions.New(await DisplayModule.GetAction_DisplayAsync(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(Module.GetAction_Remove(Id), ModuleAction.ActionLocationEnum.GridLinks);
            return actions;
        }

        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int Id { get; set; }

        [Caption("Created"), Description("The date/time the voice mail message was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Heard"), Description("Defines whether the voice mail was already listened to")]
        [UIHint("Boolean"), ReadOnly]
        public bool Heard { get; set; }

        [Caption("Call Sid"), Description("The id used by Twilio to identify the call")]
        [UIHint("String"), ReadOnly]
        public string CallSid { get; set; } = null!;

        [Caption("Phone Number"), Description("The phone number for which the voice mail message is saved")]
        [UIHint("PhoneNumber"), ReadOnly]
        public string? To { get; set; }
        [Caption("Extension"), Description("The extension for which the voice mail message is saved")]
        [UIHint("String"), ReadOnly]
        public string? Extension { get; set; }

        public string RecordingUrl { get; set; } = null!;

        [Caption("From"), Description("The caller's phone number")]
        [UIHint("PhoneNumber"), ReadOnly]
        [ExcludeDemoMode]
        public string? Caller { get; set; }
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

        [Caption("Duration"), Description("The duration of the voice mail message (in seconds)")]
        [UIHint("IntValue"), ReadOnly]
        public int Duration { get; set; }

        private BrowseVoiceMailsModule Module { get; set; }
        private DisplayVoiceMailModule DisplayModule { get; set; }

        public BrowseItem(BrowseVoiceMailsModule module, DisplayVoiceMailModule displayModule, VoiceMailData data) {
            Module = module;
            DisplayModule = displayModule;
            ObjectSupport.CopyData(data, this);
        }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
            //SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseVoiceMailsModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                List<string> extensions = new List<string>();
                if (!Manager.HasSuperUserRole) {
                    using (ExtensionEntryDataProvider extDP = new ExtensionEntryDataProvider()) {
                        extensions = await extDP.GetExtensionsForUserAsync(Manager.UserId);
                    }
                    if (extensions.Count == 0)
                        throw new Error(this.__ResStr("noInbox", "No extension defined for the current user"));
                }
                DisplayVoiceMailModule dispMod = (DisplayVoiceMailModule?)await ModuleDefinition.LoadAsync(ModuleDefinition.GetPermanentGuid(typeof(DisplayVoiceMailModule))) ?? throw new InternalError("Display Module not available");
                using (VoiceMailDataProvider dataProvider = new VoiceMailDataProvider()) {
                    List<DataProviderFilterInfo>? extFilters = null;
                    foreach (string extension in extensions) {
                        extFilters = DataProviderFilterInfo.Join(extFilters, new DataProviderFilterInfo { Field = nameof(ExtensionEntry.Extension), Operator = "==", Value = extension }, SimpleLogic: "||");
                    }
                    if (extFilters != null)
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Filters = extFilters, Logic = "||" });
                    DataProviderGetRecords<VoiceMailData> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, dispMod, s)).ToList<object>(),
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
        Manager.NeedUser();
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
