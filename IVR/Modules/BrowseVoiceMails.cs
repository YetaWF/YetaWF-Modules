/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.IVR.Modules {

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
        public string DisplayUrl { get; set; }

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

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
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
        public ModuleAction GetAction_Remove(int id) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BrowseVoiceMailsModuleController), nameof(BrowseVoiceMailsModuleController.Remove)),
                NeedsModuleContext = true,
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
    }
}
