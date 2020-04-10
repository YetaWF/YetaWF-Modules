/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using Softelvdm.Modules.TwilioProcessorDataProvider.DataProvider;
using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.TwilioProcessor.Modules {

    public class TwilioConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, TwilioConfigModule>, IInstallableModel { }

    [ModuleGuid("{876c6c1f-2101-4488-80b8-5262318deea3}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class TwilioConfigModule : ModuleDefinition {

        public TwilioConfigModule() {
            Title = this.__ResStr("modTitle", "TwilioProcessor Settings");
            Name = this.__ResStr("modName", "TwilioProcessor Settings");
            Description = this.__ResStr("modSummary", "Edits a site's TwilioProcessor settings. This can be accessed at Admin > Settings > TwilioProcessor Settings.");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TwilioConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new TwilioConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "TwilioProcessor Settings"),
                MenuText = this.__ResStr("editText", "TwilioProcessor Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the TwilioProcessor settings"),
                Legend = this.__ResStr("editLegend", "Edits the TwilioProcessor settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}