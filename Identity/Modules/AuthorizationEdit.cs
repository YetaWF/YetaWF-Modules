/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class AuthorizationEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuthorizationEditModule>, IInstallableModel { }

    [ModuleGuid("{0fd20e73-a4c3-44cb-8b71-0bab64343007}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AuthorizationEditModule : ModuleDefinition {

        public AuthorizationEditModule() : base() {
            Title = this.__ResStr("modTitle", "Edit Resource");
            Name = this.__ResStr("modName", "Edit Resource");
            Description = this.__ResStr("modSummary", "Edits an existing resource");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuthorizationEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url, string resourceName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { ResourceName = resourceName },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Edit"),
                MenuText = this.__ResStr("editText", "Edit"),
                Tooltip = this.__ResStr("editTooltip", "Edit an existing resource"),
                Legend = this.__ResStr("editLegend", "Edits an existing resource"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}