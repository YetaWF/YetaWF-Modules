﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            Title = this.__ResStr("modTitle", "RecaptchaV2 Test Template");
            Name = this.__ResStr("modName", "Template Test - RecaptchaV2");
            Description = this.__ResStr("modSummary", "RecaptchaV2 test template");
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
                Tooltip = this.__ResStr("displayTooltip", "Display the RecaptchaV2 test template"),
                Legend = this.__ResStr("displayLegend", "Displays the RecaptchaV2 test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}