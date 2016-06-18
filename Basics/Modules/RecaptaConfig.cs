/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class RecaptchaConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, RecaptchaConfigModule>, IInstallableModel { }

    [ModuleGuid("{e5196b95-304d-4fc3-a748-1202291cd676}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class RecaptchaConfigModule : ModuleDefinition {

        public RecaptchaConfigModule() : base() {
            Title = this.__ResStr("modTitle", "Recaptcha Settings");
            Name = this.__ResStr("modName", "Recaptcha Settings");
            Description = this.__ResStr("modSummary", "Edits the Recaptcha settings");
            SameAsPage = false;
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RecaptchaConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new RecaptchaConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Recaptcha Settings"),
                MenuText = this.__ResStr("editText", "Recaptcha Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the site's Recaptcha settings"),
                Legend = this.__ResStr("editLegend", "Edits the site's Recaptcha settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}