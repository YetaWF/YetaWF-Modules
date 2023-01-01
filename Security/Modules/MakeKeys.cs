/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Security#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Security.Modules {

    public class MakeKeysModuleDataProvider : ModuleDefinitionDataProvider<Guid, MakeKeysModule>, IInstallableModel { }

    [ModuleGuid("{ec1ffd3b-6e5f-4063-b8f2-8ca55d636cf9}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MakeKeysModule : ModuleDefinition {

        public MakeKeysModule() {
            Title = this.__ResStr("modTitle", "RSA Keys");
            Name = this.__ResStr("modName", "RSA Keys");
            Description = this.__ResStr("modSummary", "Creates public/private keys using RSA. These can be used by YetaWF.Core.Security.RSACrypto Encrypt and Decrypt functions to encrypt and decrypt tokens, used to implement data security. Each time the module is displayed, a new key pair is generated. A test page for the RSA Keys Module can be accessed using Tests > Modules > MakeKeys (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MakeKeysModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_DisplayAsync(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = await CustomIconAsync("MakeKeys.png"),
                LinkText = this.__ResStr("displayLink", "Make Keys"),
                MenuText = this.__ResStr("displayText", "Make Keys"),
                Tooltip = this.__ResStr("displayTooltip", "Create a public/private key using RSA"),
                Legend = this.__ResStr("displayLegend", "Creates a public/private key using RSA"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}