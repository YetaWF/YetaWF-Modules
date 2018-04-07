/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.KeepAlive.Controllers;
using YetaWF.Modules.KeepAlive.DataProvider;

namespace YetaWF.Modules.KeepAlive.Modules {

    public class KeepAliveConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, KeepAliveConfigModule>, IInstallableModel { }

    [ModuleGuid("{e41a6d42-3245-448a-a9ed-310e1c81632e}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class KeepAliveConfigModule : ModuleDefinition {

        public KeepAliveConfigModule() {
            Title = this.__ResStr("modTitle", "Keep Alive Settings");
            Name = this.__ResStr("modName", "Keep Alive Settings");
            Description = this.__ResStr("modSummary", "Edits a YetaWF instance's keep alive settings");
            ShowHelp = true;
            DefaultViewName = StandardViews.Config;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new KeepAliveConfigModuleDataProvider(); }
        public override DataProviderImpl GetConfigDataProvider() { return new KeepAliveConfigDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Config",
                LinkText = this.__ResStr("editLink", "Keep Alive Settings"),
                MenuText = this.__ResStr("editText", "Keep Alive Settings"),
                Tooltip = this.__ResStr("editTooltip", "Edit the YetaWF instance's keep alive settings"),
                Legend = this.__ResStr("editLegend", "Edits the YetaWF instance's keep alive settings"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
        public async Task<ModuleAction> GetAction_RunKeepAliveAsync() {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(KeepAliveConfigModuleController), nameof(KeepAliveConfigModuleController.RunKeepAlive)),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("KeepAlive.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("keepAliveLink", "Keep Alive"),
                MenuText = this.__ResStr("keepAliveMenu", "Keep Alive"),
                Tooltip = this.__ResStr("keepAliveTT", "Access the site to keep it alive"),
                Legend = this.__ResStr("keepAliveLegend", "Accesses the site to keep it alive"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("keepAliveConfirm", "Are you sure you want to access the site to keep it alive?"),
            };
        }
    }
}