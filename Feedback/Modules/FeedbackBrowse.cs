/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Feedback.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Feedback.Modules {

    public class FeedbackBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, FeedbackBrowseModule>, IInstallableModel { }

    [ModuleGuid("{1dc47bb4-bcf9-4615-992a-29e2a4450f32}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class FeedbackBrowseModule : ModuleDefinition {

        public FeedbackBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Feedback");
            Name = this.__ResStr("modName", "Browse Feedback");
            Description = this.__ResStr("modSummary", "Displays and manages user feedback");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new FeedbackBrowseModuleDataProvider(); }

        [Category("General"), Caption("Display URL"), Description("The URL to display a feedback message - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a feedback message - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveFeedback",
                        this.__ResStr("roleRemItemsC", "Remove Feedback"), this.__ResStr("roleRemItems", "The role has permission to remove feedback messages"),
                        this.__ResStr("userRemItemsC", "Remove Feedback"), this.__ResStr("userRemItems", "The user has permission to remove feedback messages")),
                };
            }
        }

        public ModuleAction GetAction_Feedback(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Feedback"),
                MenuText = this.__ResStr("browseText", "Feedback"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage user feedback"),
                Legend = this.__ResStr("browseLegend", "Displays and manages user feedback"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_RemoveFeedback(int key) {
            if (!IsAuthorized("RemoveFeedback")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(FeedbackBrowseModuleController), "RemoveFeedback"),
                NeedsModuleContext = true,
                QueryArgs = new { Key = key},
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Feedback"),
                MenuText = this.__ResStr("removeMenu", "Remove Feedback"),
                Legend = this.__ResStr("removeLegend", "Remove the feedback message"),
                Tooltip = this.__ResStr("removeTT", "Removes the feedback message"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this feedback message?"),
            };
        }
    }
}