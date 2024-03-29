/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UserAccountModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserAccountModule>, IInstallableModel { }

    [ModuleGuid("{222d53c2-8c9e-41df-8366-96060a4f5b57}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Login & Registration")]
    public class UserAccountModule : ModuleDefinition {

        public UserAccountModule() : base() {
            Title = this.__ResStr("modTitle", "User Account");
            Name = this.__ResStr("modName", "User Account");
            Description = this.__ResStr("modSummary", "Used to edit the current user's account user name or email address and displays additional information. The User Account Module can be accessed using User > Account (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UserAccountModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Account"),
                MenuText = this.__ResStr("editText", "Account"),
                Tooltip = this.__ResStr("editTooltip", "Edit your account information"),
                Legend = this.__ResStr("editLegend", "Edits the current user's account information"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
