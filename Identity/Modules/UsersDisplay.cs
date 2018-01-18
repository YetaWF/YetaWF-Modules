/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UsersDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersDisplayModule>, IInstallableModel { }

    [ModuleGuid("{e6c98552-d1fa-48aa-a690-e5f933dd71ac}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UsersDisplayModule : ModuleDefinition {

        public UsersDisplayModule() : base() {
            Title = this.__ResStr("modTitle", "User");
            Name = this.__ResStr("modName", "Display a User");
            Description = this.__ResStr("modSummary", "Displays an existing user");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UsersDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, string userName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { UserName = userName },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display account information for user {0}", userName),
                Legend = this.__ResStr("displayLegend", "Displays account information for an existing user"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}