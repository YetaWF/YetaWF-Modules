/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.TawkTo.Controllers;
using YetaWF.Modules.TawkTo.DataProvider;
using YetaWF.Modules.TawkTo.Modules;

namespace YetaWF.Modules.TawkTo.Views {

    public class SkinTawkToView : YetaWFView, IYetaWFView<SkinTawkToModule, SkinTawkToModuleController.DisplayModel> {

        public const string ViewName = "SkinTawkTo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public Task<string> RenderViewAsync(SkinTawkToModule module, SkinTawkToModuleController.DisplayModel model) {

            ScriptBuilder sb = new ScriptBuilder();

            ConfigData config = ConfigDataProvider.GetConfigAsync().Result;

            sb.Append($@"
// https://www.tawk.to/javascript-api/
var Tawk_API = Tawk_API || {{}}, Tawk_LoadStart = new Date();");

            if (Manager.HaveUser) {
                sb.Append($@"
Tawk_API.visitor = {{
    name: '{JE(Manager.UserName)}',
    email: '{JE(Manager.UserEmail)}',
    hash: '{JE(model.Hash)}'
}};");
            }

            sb.Append($@"
(function() {{
    var s1=document.createElement('script'),s0=document.getElementsByTagName('script')[0];
    s1.async=true;
    s1.src = '{JE(string.Format("https://embed.tawk.to/{0}/default", Utility.UrlEncodePath(config.Account)))}';
    s1.charset='UTF-8';
    s1.setAttribute('crossorigin','*');
    s0.parentNode.insertBefore(s1,s0);
}})();");

            Manager.ScriptManager.AddLast(sb.ToString());

            return Task.FromResult<string>(string.Empty);
        }
    }
}
