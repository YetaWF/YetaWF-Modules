/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class OwinEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, OwinEditModule>, IInstallableModel { }

    [ModuleGuid("{F490DEEB-FF19-4921-894B-4A81D263F97A}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Configuration")]
    public class OwinEditModule : ModuleDefinition {

        public OwinEditModule() : base() {
            Title = this.__ResStr("modTitle", "Login Provider Settings");
            Name = this.__ResStr("modName", "Login Provider Settings");
            Description = this.__ResStr("modSummary", "Used to edit local and external login provider settings like Google, Facebook, etc. The Login Provider Settings Module can be accessed using Admin > Identity Settings > Login Providers (standard YetaWF site).");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new OwinEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_OwinEdit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Login Providers"),
                MenuText = this.__ResStr("editText", "Login Providers"),
                Tooltip = this.__ResStr("editTooltip", "Edit login provider settings"),
                Legend = this.__ResStr("editLegend", "Edits login provider settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
