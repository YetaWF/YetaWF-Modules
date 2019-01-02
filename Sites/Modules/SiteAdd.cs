/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Sites.Modules {
    public class SiteAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteAddModule>, IInstallableModel { }

    [ModuleGuid("{c4505071-bc76-4c88-8b01-fe40a8bc273d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SiteAddModule : ModuleDefinition {

        public SiteAddModule() {
            Title = this.__ResStr("modTitle", "Add New Site");
            Name = this.__ResStr("modName", "Add New Site");
            Description = this.__ResStr("modSummary", "Creates a new web site");
            DefaultViewName = StandardViews.Add;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SiteAddModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Add New Site"),
                MenuText = this.__ResStr("addText", "Add New Site"),
                Tooltip = this.__ResStr("addTooltip", "Create a new web site"),
                Legend = this.__ResStr("addLegend", "Creates a new web site"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

