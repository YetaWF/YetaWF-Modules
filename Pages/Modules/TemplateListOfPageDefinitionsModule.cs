/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.Modules {

    public class TemplateListOfLocalPagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfLocalPagesModule>, IInstallableModel { }

    [ModuleGuid("{44977a1b-18bb-4585-9db6-29330c181319}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateListOfLocalPagesModule : ModuleDefinition {

        public TemplateListOfLocalPagesModule() {
            Title = this.__ResStr("modTitle", "ListOfLocalPages Test Template");
            Name = this.__ResStr("modName", "Template Test - ListOfLocalPages");
            Description = this.__ResStr("modSummary", "ListOfLocalPages test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfLocalPagesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfLocalPages"),
                MenuText = this.__ResStr("displayText", "ListOfLocalPages"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfLocalPages test template"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfLocalPages test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
