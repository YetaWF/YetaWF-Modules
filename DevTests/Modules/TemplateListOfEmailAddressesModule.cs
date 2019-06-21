/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateListOfEmailAddressesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfEmailAddressesModule>, IInstallableModel { }

    [ModuleGuid("{443bfefa-648c-4b4f-832c-25705636565f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateListOfEmailAddressesModule : ModuleDefinition {

        public TemplateListOfEmailAddressesModule() {
            Title = this.__ResStr("modTitle", "ListOfEmailAddresses Test Component");
            Name = this.__ResStr("modName", "Component Test - ListOfEmailAddresses");
            Description = this.__ResStr("modSummary", "ListOfEmailAddresses test component");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfEmailAddressesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfEmailAddresses"),
                MenuText = this.__ResStr("displayText", "ListOfEmailAddresses"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfEmailAddresses test component"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfEmailAddresses test component"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
