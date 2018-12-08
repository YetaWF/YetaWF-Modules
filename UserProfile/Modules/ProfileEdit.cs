/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/UserProfile#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.UserProfile.Modules {

    public class ProfileEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, ProfileEditModule>, IInstallableModel { }

    [ModuleGuid("{9ba8e8dc-7e04-492c-850d-27f0ca6fa2d3}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ProfileEditModule : ModuleDefinition {

        public ProfileEditModule() {
            Title = this.__ResStr("modTitle", "User Profile");
            Name = this.__ResStr("modName", "Edit User Profile");
            Description = this.__ResStr("modSummary", "Edits an existing user profile");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ProfileEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Post Save Url"), Description("Defines the page to display once the form is saved - If omitted, the Url to return to is determined automatically - This property is ignored when the module is displayed in a popup")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string PostSaveUrl { get; set; }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Profile"),
                MenuText = this.__ResStr("editText", "Profile"),
                Tooltip = this.__ResStr("editTooltip", "Edit your user profile"),
                Legend = this.__ResStr("editLegend", "Edits your user profile"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}
