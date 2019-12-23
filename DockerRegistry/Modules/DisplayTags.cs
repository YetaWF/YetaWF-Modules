/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.DockerRegistry.Controllers;

namespace YetaWF.Modules.DockerRegistry.Modules {

    public class DisplayTagsModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayTagsModule>, IInstallableModel { }

    [ModuleGuid("{f63c8abb-aa3b-452e-9832-2fb0e72b8498}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayTagsModule : ModuleDefinition {

        public DisplayTagsModule() {
            Title = this.__ResStr("modTitle", "Tags");
            Name = this.__ResStr("modName", "Tags");
            Description = this.__ResStr("modSummary", "Displays tags of a Docker repository");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayTagsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveTags",
                        this.__ResStr("roleRemTagsC", "Remove Images"), this.__ResStr("roleRemTags", "The role has permission to remove tags"),
                        this.__ResStr("userRemTagsC", "Remove Images"), this.__ResStr("userRemTags", "The user has permission to remove tags")),
                };
            }
        }
        public ModuleAction GetAction_Display(string url, int registryId, string repository) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { RegistryId = registryId, Repository = repository },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display Tags"),
                MenuText = this.__ResStr("displayText", "Display Tags"),
                Tooltip = this.__ResStr("displayTooltip", "Display tags"),
                Legend = this.__ResStr("displayLegend", "Displays tags of a Docker repository"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public ModuleAction GetAction_RemoveTag(int registryId, string repository, string reference) {
            if (!IsAuthorized("RemoveTags")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(DisplayTagsModuleController), nameof(DisplayTagsModuleController.Remove)),
                NeedsModuleContext = true,
                QueryArgs = new { RegistryId = registryId, Repository = repository, Reference = reference },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Tag"),
                MenuText = this.__ResStr("removeMenu", "Remove Tag"),
                Tooltip = this.__ResStr("removeTT", "Remove the tag"),
                Legend = this.__ResStr("removeLegend", "Removes the tag"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this tag?"),
            };
        }
    }
}
