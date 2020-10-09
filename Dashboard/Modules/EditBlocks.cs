/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules {

    public class EditBlocksModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditBlocksModule>, IInstallableModel { }

    [ModuleGuid("{9492c9e8-8df1-45a0-9c0f-18370cbcb6de}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class EditBlocksModule : ModuleDefinition {

        public EditBlocksModule() {
            Title = this.__ResStr("modTitle", "Request Blocking");
            Name = this.__ResStr("modName", "Request Blocking");
            Description = this.__ResStr("modSummary", "Edits request blocking settings, used to stop bots from common exploit scanning, scraping");
            DefaultViewName = StandardViews.Edit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EditBlocksModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
