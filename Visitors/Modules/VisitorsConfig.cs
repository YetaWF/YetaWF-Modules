/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class VisitorsConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorsConfigModule>, IInstallableModel { }

    [ModuleGuid("{2DA557B8-664A-4c23-839B-DC280CECCA47}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class VisitorsConfigModule : ModuleDefinition {

        public VisitorsConfigModule() {
            Title = this.__ResStr("modTitle", "Visitors Settings");
            Name = this.__ResStr("modName", "Visitors Settings");
            Description = this.__ResStr("modSummary", "Used to edit a site's visitors settings. This can be accessed using Admin > Settings > Visitors Settings (standard YetaWF site).");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new VisitorsConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new VisitorsConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Visitors Settings"),
                MenuText = this.__ResStr("editText", "Visitors Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the visitors settings"),
                Legend = this.__ResStr("editLegend", "Edits the visitors settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}