/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class DisplayHeadersModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisplayHeadersModule>, IInstallableModel { }

    [ModuleGuid("{41340291-456a-452b-a8d2-581b14a0017c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisplayHeadersModule : ModuleDefinition {

        public DisplayHeadersModule() {
            Title = this.__ResStr("modTitle", "Headers");
            Name = this.__ResStr("modName", "Headers");
            Description = this.__ResStr("modSummary", "Displays request headers. A test page for this module can be found at Tests > Modules > HTTP Request Headers (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisplayHeadersModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int iD) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { ID = iD },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display request headers"),
                Legend = this.__ResStr("displayLegend", "Displays request headers"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
