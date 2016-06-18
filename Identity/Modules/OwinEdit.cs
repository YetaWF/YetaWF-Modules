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

    public class OwinEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, OwinEditModule>, IInstallableModel { }

    [ModuleGuid("{F490DEEB-FF19-4921-894B-4A81D263F97A}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class OwinEditModule : ModuleDefinition {

        public OwinEditModule() : base() {
            Title = this.__ResStr("modTitle", "External Login Provider Settings");
            Name = this.__ResStr("modName", "External Login Provider Settings");
            Description = this.__ResStr("modSummary", "Edits external login providers like Google, Facebook, etc.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new OwinEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_OwinEdit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Login Providers"),
                MenuText = this.__ResStr("editText", "Login Providers"),
                Tooltip = this.__ResStr("editTooltip", "Edit external login providers"),
                Legend = this.__ResStr("editLegend", "Edits external login providers"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}