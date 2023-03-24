/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System;
using System.Text;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.TawkTo.DataProvider;

namespace YetaWF.Modules.TawkTo.Modules;

public class SkinTawkToModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinTawkToModule>, IInstallableModel { }

[ModuleGuid("{c063e089-aff3-44e4-ac44-063911853579}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class SkinTawkToModule : ModuleDefinition {

    public SkinTawkToModule() {
        Title = this.__ResStr("modTitle", "Skin Tawk.to");
        Name = this.__ResStr("modName", "Tawk.to (Skin)");
        Description = this.__ResStr("modSummary", "Implements the TawkTo chat window, referenced by your page(s) or site. It is not added to a skin or page, instead it is referenced by a site, page or module. Referencing the TawkTo (Skin) Module causes the TawkTo chat window to be displayed as configured in your https://www.tawk.to account.");
        WantFocus = false;
        ShowTitle = false;
        WantSearch = false;
        Invokable = true;
        InvokeInPopup = false;
        InvokeInAjax = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SkinTawkToModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public class DisplayModel {
        public string Hash { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        ConfigData config = await ConfigDataProvider.GetConfigAsync();
        if (Manager.EditMode || !config.IsConfigured) return ActionInfo.Empty;
        DisplayModel model = new DisplayModel();

        if (Manager.HaveUser) {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(config.APIKey!);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(key)) {
                model.Hash = ByteArrayToString(hmacsha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Manager.UserEmail!)));
            }
        }

        return await RenderAsync(model);
    }

    private string ByteArrayToString(byte[] btes) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < btes.Length; i++)
            sb.Append(btes[i].ToString("X2")); /* hex format */
        return sb.ToString();
    }
}
