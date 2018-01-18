/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Packages.Modules {

    public class SiteTemplateUndoModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteTemplateUndoModule>, IInstallableModel { }

    [ModuleGuid("{E85FCED7-FA21-4697-889C-28DF6632AE0C}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SiteTemplateUndoModule : ModuleDefinition {

        public SiteTemplateUndoModule() {
            Title = this.__ResStr("modTitle", "Undo Site Template");
            Name = this.__ResStr("modName", "Undo Site Template");
            Description = this.__ResStr("modSummary", "Undoes a site template by removing all defined pages and menu entries from the current site");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SiteTemplateUndoModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_UndoSiteTemplate(string url, string fileName) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { FileName = fileName },
                Image = "SiteTemplateUndo.png",
                LinkText = this.__ResStr("undoLink", "Site Template"),
                MenuText = this.__ResStr("undoText", "Site Template"),
                Tooltip = this.__ResStr("undoTooltip", "Undo a site template and add all defined pages to current site"),
                Legend = this.__ResStr("undoLegend", "Undoes a site template by removing all defined pages and menu entries from the current site"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}