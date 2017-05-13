/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UserPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserPasswordModule>, IInstallableModel { }

    [ModuleGuid("{2ca21dad-34d0-4e2c-83c2-e3f6b31ca630}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class UserPasswordModule : ModuleDefinition {

        public UserPasswordModule() : base() {
            Title = this.__ResStr("modTitle", "Change Password");
            Name = this.__ResStr("modName", "Change Password");
            Description = this.__ResStr("modSummary", "Changes a user's password");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UserPasswordModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_UserPassword(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Change Password"),
                MenuText = this.__ResStr("editText", "Change Password"),
                Tooltip = this.__ResStr("editTooltip", "Change your account password"),
                Legend = this.__ResStr("editLegend", "Changes your account password"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}