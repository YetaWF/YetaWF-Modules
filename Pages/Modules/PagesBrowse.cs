/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Pages.Controllers;

namespace YetaWF.Modules.Pages.Modules {

    public class PagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{4AEDC6D5-4655-48fa-A3C1-A1BF2707030D}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PagesBrowseModule : ModuleDefinition {

        public PagesBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Pages");
            Name = this.__ResStr("modName", "Pages");
            Description = this.__ResStr("modSummary", "Displays and manages pages");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PagesBrowseModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return EditorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemovePages",
                        this.__ResStr("roleRemItemsC", "Remove Pages"), this.__ResStr("roleRemItems", "The role has permission to remove pages"),
                        this.__ResStr("userRemItemsC", "Remove Pages"), this.__ResStr("userRemItems", "The user has permission to remove pages")),
                };
            }
        }

        public ModuleAction GetAction_Pages(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Pages"),
                MenuText = this.__ResStr("browseText", "Pages"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage pages"),
                Legend = this.__ResStr("browseLegend", "Displays and manages pages"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_ShowPage(string url) {
            return new ModuleAction(this) {
                Url = url,
                Image = "#Display",
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("displayLink", "Show Page"),
                MenuText = this.__ResStr("displayMenu", "Show Page"),
                Tooltip = this.__ResStr("displayTT", "Display the page in a new window"),
                Legend = this.__ResStr("displayLegend", "Displays the page"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public ModuleAction GetAction_RemoveLink(string pageName) {
            if (!IsAuthorized("RemovePages")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PagesBrowseModuleController), "Remove"),
                QueryArgs = new { PageName = pageName },
                NeedsModuleContext = true,
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Item"),
                MenuText = this.__ResStr("removeMenu", "Remove Item"),
                Legend = this.__ResStr("removeLegend", "Remove the page from the site"),
                Tooltip = this.__ResStr("removeTT", "Removes the page from the site"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove page \"{0}\"?", pageName),
                SaveReturnUrl = true,
            };
        }
    }
}