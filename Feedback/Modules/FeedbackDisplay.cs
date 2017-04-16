/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Feedback.Modules {

    public class FeedbackDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackDisplayModule>, IInstallableModel { }

    [ModuleGuid("{a29a50e9-9457-46f4-9bce-77a967b1671e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class FeedbackDisplayModule : ModuleDefinition {

        public FeedbackDisplayModule() : base() {
            Title = this.__ResStr("modTitle", "Feedback Message");
            Name = this.__ResStr("modName", "Feedback Message");
            Description = this.__ResStr("modSummary", "Displays a feedback message");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new FeedbackDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, int key) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Key = key },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the feedback message"),
                Legend = this.__ResStr("displayLegend", "Displays a feedback message"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}