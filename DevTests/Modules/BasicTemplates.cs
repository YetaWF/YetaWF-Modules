/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class BasicTemplatesModuleDataProvider : ModuleDefinitionDataProvider<Guid, BasicTemplatesModule>, IInstallableModel { }

    [ModuleGuid("{479f90d6-e15b-41cc-9117-53fb42a10a9e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BasicTemplatesModule : ModuleDefinition {

        public BasicTemplatesModule() {
            Title = this.__ResStr("modTitle", "Basic Templates Test");
            Name = this.__ResStr("modName", "Basic Templates Test");
            Description = this.__ResStr("modSummary", "Used to test all generally available templates");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BasicTemplatesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("editLink", "Basic Templates"),
                MenuText = this.__ResStr("editText", "Basic Templates"),
                Tooltip = this.__ResStr("editTooltip", "Used to test all generally available templates"),
                Legend = this.__ResStr("editLegend", "Used to test all generally available templates"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
