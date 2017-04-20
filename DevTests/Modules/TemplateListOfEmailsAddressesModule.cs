/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            Title = this.__ResStr("modTitle", "ListOfEmailAddresses Test Template");
            Name = this.__ResStr("modName", "Template Test - ListOfEmailAddresses");
            Description = this.__ResStr("modSummary", "ListOfEmailAddresses test template");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfEmailAddressesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfEmailAddresses"),
                MenuText = this.__ResStr("displayText", "ListOfEmailAddresses"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfEmailAddresses test template"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfEmailAddresses test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
