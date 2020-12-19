/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.AddThis.DataProvider;

namespace YetaWF.Modules.AddThis.Modules {

    public class SharingSidebarConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SharingSidebarConfigModule>, IInstallableModel { }

    [ModuleGuid("{43cdb837-a2be-4586-ab1c-3ba92c8da482}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SharingSidebarConfigModule : ModuleDefinition {

        public SharingSidebarConfigModule() {
            Title = this.__ResStr("modTitle", "Sharing Sidebar Settings");
            Name = this.__ResStr("modName", "Sharing Sidebar Settings");
            Description = this.__ResStr("modSummary", "Edits a site's AddThis Sharing Sidebar configuration settings. It can be accessed using Admin > Settings > AddThis Sharing SideBar Settings.");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SharingSidebarConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "AddThis Sharing Sidebar Settings"),
                MenuText = this.__ResStr("editText", "AddThis Sharing Sidebar Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the AddThis Sharing Sidebar configuration settings"),
                Legend = this.__ResStr("editLegend", "Edits the AddThis Sharing Sidebar configuration settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}