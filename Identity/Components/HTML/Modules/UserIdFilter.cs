/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class UserIdFilterModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserIdFilterModule>, IInstallableModel { }

    [ModuleGuid("{63B188F3-BF70-438e-942C-F397FC0DD88D}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Filter")]
    public class UserIdFilterModule : ModuleDefinition {

        public UserIdFilterModule() : base() {
            Title = this.__ResStr("modTitle", "User Filter");
            Name = this.__ResStr("modName", "UserId Filter");
            Description = this.__ResStr("modSummary", "Implements a filter for a user id.");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UserIdFilterModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit() {
            return new ModuleAction(this) {
                Url = ModulePermanentUrl,
                LinkText = this.__ResStr("editLink", "Filter"),
                MenuText = this.__ResStr("editText", "Filter"),
                Tooltip = this.__ResStr("editTooltip", "Use a filter to limit data shown to a certain user"),
                Legend = this.__ResStr("editLegend", "Limits data shown to a certain user"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.Explicit,
            };
        }
    }
}
