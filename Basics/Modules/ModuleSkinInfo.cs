/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Basics.Modules {

    public class ModuleSkinInfoModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModuleSkinInfoModule>, IInstallableModel { }

    [ModuleGuid("{cfb7fdf4-d62a-4762-a6a0-62cb373053d1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ModuleSkinInfoModule : ModuleDefinition {

        public ModuleSkinInfoModule() {
            Title = this.__ResStr("modTitle", "Module Skin Info");
            Name = this.__ResStr("modName", "Module Skin Info");
            Description = this.__ResStr("modSummary", "Displays a test sample with most HTML elements and tags commonly used. It can be used to verify a skin. It also displays average character height/width, which is used when creating new skins (in a skin's Skin.txt file). Module Skin Info can be accessed using Tests > Module Skin Info (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModuleSkinInfoModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Module Skin Info"),
                MenuText = this.__ResStr("displayText", "Module Skin Info"),
                Tooltip = this.__ResStr("displayTooltip", "Display module skin information"),
                Legend = this.__ResStr("displayLegend", "Displays module skin information"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}