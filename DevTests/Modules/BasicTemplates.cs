/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            Title = this.__ResStr("modTitle", "Basic Components Test");
            Name = this.__ResStr("modName", "Basic Components Test");
            Description = this.__ResStr("modSummary", "Test module for many basic built-in YetaWF.Core standard templates. A test page for this module can be found at Tests > Templates > Basic Templates (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BasicTemplatesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("editLink", "Basic Components"),
                MenuText = this.__ResStr("editText", "Basic Components"),
                Tooltip = this.__ResStr("editTooltip", "Used to test all generally available components"),
                Legend = this.__ResStr("editLegend", "Used to test all generally available components"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
