/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Feedback.DataProvider;

namespace YetaWF.Modules.Feedback.Modules {

    public class FeedbackConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackConfigModule>, IInstallableModel { }

    [ModuleGuid("{933431f8-8ac9-45b8-99e5-05abe2fd8f56}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class FeedbackConfigModule : ModuleDefinition {

        public FeedbackConfigModule() {
            Title = this.__ResStr("modTitle", "Feedback Settings");
            Name = this.__ResStr("modName", "Feedback Settings");
            Description = this.__ResStr("modSummary", "Edits a site's feedback settings");
            ShowHelp = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new FeedbackConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new FeedbackConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Feedback Settings"),
                MenuText = this.__ResStr("editText", "Feedback Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the feedback settings"),
                Legend = this.__ResStr("editLegend", "Edits the feedback settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}