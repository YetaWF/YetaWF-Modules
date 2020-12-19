/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class TemplateListOfUserNamesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfUserNamesModule>, IInstallableModel { }

    [ModuleGuid("{190287E8-EC79-404C-9FCA-6D43607825BC}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Tools")]
    public class TemplateListOfUserNamesModule : ModuleDefinition {

        public TemplateListOfUserNamesModule() {
            Title = this.__ResStr("modTitle", "ListOfUserNames Test Component");
            Name = this.__ResStr("modName", "Component Test - ListOfUserNames");
            Description = this.__ResStr("modSummary", "Test module for the ListOfUserNames component (edit and display). A test page for this module can be found at Tests > Templates > ListOfUserNames (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfUserNamesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfUserNames"),
                MenuText = this.__ResStr("displayText", "ListOfUserNames"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfUserNames test template"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfUserNames test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
