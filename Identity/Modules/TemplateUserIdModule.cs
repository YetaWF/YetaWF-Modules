/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class TemplateUserIdModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateUserIdModule>, IInstallableModel { }

    [ModuleGuid("{985c4c49-8103-4b5c-a9ae-2bb108ef58a6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Tools")]
    public class TemplateUserIdModule : ModuleDefinition {

        public TemplateUserIdModule() {
            Title = this.__ResStr("modTitle", "UserId Test Component");
            Name = this.__ResStr("modName", "Component Test - UserId");
            Description = this.__ResStr("modSummary", "Test module for the UserId component (edit and display). A test page for this module can be found at Tests > Templates > UserId (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateUserIdModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "UserId"),
                MenuText = this.__ResStr("displayText", "UserId"),
                Tooltip = this.__ResStr("displayTooltip", "Display the UserId test template"),
                Legend = this.__ResStr("displayLegend", "Displays the UserId test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
