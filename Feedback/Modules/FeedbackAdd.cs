/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.Feedback.DataProvider;

namespace YetaWF.Modules.Feedback.Modules {
    public class FeedbackAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackAddModule>, IInstallableModel { }

    [ModuleGuid("{30eeace2-f61d-45b7-a430-12c873f78bae}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class FeedbackAddModule : ModuleDefinition {

        public FeedbackAddModule() {
            Title = this.__ResStr("modTitle", "Send Feedback");
            Name = this.__ResStr("modName", "Send Feedback");
            Description = this.__ResStr("modSummary", "User/site feedback");
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new FeedbackAddModuleDataProvider(); }

        [Category("General"), Caption("Default Subject"), Description("The optional default subject of the message when new feedback is entered")]
        [UIHint("Text80"), StringLength(FeedbackData.MaxSubject), Trim]
        public string DefaultSubject { get; set; }

        [Category("General"), Caption("Default Message"), Description("The optional default message when new feedback is entered")]
        [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), StringLength(FeedbackData.MaxMessage), Trim]
        public string DefaultMessage { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Add(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Enter Feedback"),
                MenuText = this.__ResStr("addText", "Enter Feedback"),
                Tooltip = this.__ResStr("addTooltip", "Enter feedback for us"),
                Legend = this.__ResStr("addLegend", "Allows you to enter feedback for us"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}

