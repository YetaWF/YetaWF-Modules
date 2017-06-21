/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TestEscapesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TestEscapesModule>, IInstallableModel { }

    [ModuleGuid("{d0071658-35f2-453d-a5ba-2a91bd01ee49}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TestEscapesModule : ModuleDefinition {

        public TestEscapesModule() {
            Title = this.__ResStr("modTitle", "Test Escapes");
            Name = this.__ResStr("modName", "Test Escapes (HTML, Attributes)");
            Description = this.__ResStr("modSummary", "Tests various client-side & server side character translations/escapes");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TestEscapesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Test(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { SomeChars = "TEST < > & @ {0} TEST" },
                Image = "#Display",
                LinkText = this.__ResStr("editLink", "Escapes"),
                MenuText = this.__ResStr("editText", "Escapes"),
                Tooltip = this.__ResStr("editTooltip", "Test various client-side & server side character translations/escapes"),
                Legend = this.__ResStr("editLegend", "Tests various client-side & server side character translations/escapes"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
