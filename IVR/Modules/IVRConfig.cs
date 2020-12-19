/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.IVR.DataProvider;
using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules {

    public class IVRConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, IVRConfigModule>, IInstallableModel { }

    [ModuleGuid("{4E07DE60-5B6C-41cc-8CA0-AF6F99663D50}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class IVRConfigModule : ModuleDefinition {

        public IVRConfigModule() {
            Title = this.__ResStr("modTitle", "IVR Settings");
            Name = this.__ResStr("modName", "IVR Settings");
            Description = this.__ResStr("modSummary", "Edits a site's IVR settings.");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new IVRConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new IVRConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "IVR Settings"),
                MenuText = this.__ResStr("editText", "IVR Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the IVR settings"),
                Legend = this.__ResStr("editLegend", "Edits the IVR settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}