/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class ResetPasswordModuleDataProvider : ModuleDefinitionDataProvider<Guid, ResetPasswordModule>, IInstallableModel { }

    [ModuleGuid("{9FFEAA14-1366-4462-B6F4-1035F1672DBC}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class ResetPasswordModule : ModuleDefinition {

        public ResetPasswordModule() : base() {
            Title = this.__ResStr("modTitle", "Reset Password");
            Name = this.__ResStr("modName", "Reset Password");
            Description = this.__ResStr("modSummary", "Resets a user's password");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ResetPasswordModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_ResetPassword(string url, int userId, string resetKey) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { UserId = userId, ResetKey = resetKey },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Reset Password"),
                MenuText = this.__ResStr("editText", "Reset Password"),
                Tooltip = this.__ResStr("editTooltip", "Reset your account password"),
                Legend = this.__ResStr("editLegend", "Resets your account password"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}