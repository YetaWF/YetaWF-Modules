/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.Endpoints;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class BrowseCallLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseCallLogModule>, IInstallableModel { }

    [ModuleGuid("{5c9b50ed-b434-451d-b795-59d6a5125c6a}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseCallLogModule : ModuleDefinition {

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
            return new ModuleAction(this) {
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
            return new ModuleAction(this) {
                Url = $"{Utility.UrlFor(typeof(BrowseCallLogModuleEndpoints), BrowseCallLogModuleEndpoints.Remove)}/{id}",
                NeedsModuleContext = true,
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
    }
}
