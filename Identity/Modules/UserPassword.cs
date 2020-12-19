/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UserPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserPasswordModule>, IInstallableModel { }

    [ModuleGuid("{2ca21dad-34d0-4e2c-83c2-e3f6b31ca630}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Login & Registration")]
    public class UserPasswordModule : ModuleDefinition {

        public UserPasswordModule() : base() {
            Title = this.__ResStr("modTitle", "Change Password");
            Name = this.__ResStr("modName", "Change Password");
            Description = this.__ResStr("modSummary", "Used to edit the current user's account password. The Change Password Module can be accessed using User > Change Password (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
            ShowPasswordRules = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UserPasswordModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Show Password Rules"), Description("Defines whether the password rules are shown")]
        [UIHint("Boolean")]
        [Data_NewValue]
        public bool ShowPasswordRules { get; set; }

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
