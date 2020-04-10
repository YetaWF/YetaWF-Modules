/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateRecaptchaV2ModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateRecaptchaV2Module>, IInstallableModel { }

    [ModuleGuid("{b43ee2f3-dfa7-49c8-8c9e-861901bf0365}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateRecaptchaV2Module : ModuleDefinition {

        public TemplateRecaptchaV2Module() {
            Title = this.__ResStr("modTitle", "RecaptchaV2 Test Component");
            Name = this.__ResStr("modName", "Component Test - RecaptchaV2");
            Description = this.__ResStr("modSummary", "Test module for the RecaptchaV2 component. A test page for this module can be found at Tests > Templates > RecaptchaV2 (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateRecaptchaV2ModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "RecaptchaV2"),
                MenuText = this.__ResStr("displayText", "RecaptchaV2"),
                Tooltip = this.__ResStr("displayTooltip", "Display the RecaptchaV2 test component"),
                Legend = this.__ResStr("displayLegend", "Displays the RecaptchaV2 test component"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
