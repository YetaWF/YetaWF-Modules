﻿/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class SelectTwoStepAuthModuleDataProvider : ModuleDefinitionDataProvider<Guid, SelectTwoStepAuthModule>, IInstallableModel { }

    [ModuleGuid("{0ffd80d1-69ef-4187-8d2d-aa48e69aa3f8}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SelectTwoStepAuthModule : ModuleDefinition {

        public SelectTwoStepAuthModule() {
            Title = this.__ResStr("modTitle", "Select Desired Two-Step Authentication");
            Name = this.__ResStr("modName", "Select Two-Step Authentication");
            Description = this.__ResStr("modSummary", "Selection of desired Two-Step Authentication");
            WantSearch = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SelectTwoStepAuthModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_SelectTwoStepAuth(string url, int userId, string userName, string userEmail) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { UserId = userId, UserName = userName, UserEmail = userEmail },
                Style = ModuleAction.ActionStyleEnum.ForcePopup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}