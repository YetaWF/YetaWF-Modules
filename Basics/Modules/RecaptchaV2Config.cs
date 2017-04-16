/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class RecaptchaV2ConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, RecaptchaV2ConfigModule>, IInstallableModel { }

    [ModuleGuid("{6256FC1B-6E81-4B2A-9D99-FF79FF425C86}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class RecaptchaV2ConfigModule : ModuleDefinition {

        public RecaptchaV2ConfigModule() : base() {
            Title = this.__ResStr("modTitle", "RecaptchaV2 Settings");
            Name = this.__ResStr("modName", "RecaptchaV2 Settings");
            Description = this.__ResStr("modSummary", "Edits the RecaptchaV2 settings");
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RecaptchaV2ConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new RecaptchaV2ConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "RecaptchaV2 Settings"),
                MenuText = this.__ResStr("editText", "RecaptchaV2 Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the site's RecaptchaV2 settings"),
                Legend = this.__ResStr("editLegend", "Edits the site's RecaptchaV2 settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}