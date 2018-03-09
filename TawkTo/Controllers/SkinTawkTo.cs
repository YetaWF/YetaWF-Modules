/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.Text;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Modules.TawkTo.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.TawkTo.Controllers {

    public class SkinTawkToModuleController : ControllerImpl<YetaWF.Modules.TawkTo.Modules.SkinTawkToModule> {

        public SkinTawkToModuleController() { }

        public class DisplayModel {
            public string Hash { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> SkinTawkTo() {
            ConfigData config = await ConfigDataProvider.GetConfigAsync();
            if (Manager.EditMode || !config.IsConfigured) return new EmptyResult();
            DisplayModel model = new DisplayModel();

            if (Manager.HaveUser) {
                byte[] key = System.Text.Encoding.UTF8.GetBytes(config.APIKey);
                using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(key)) {
                    model.Hash = ByteArrayToString(hmacsha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Manager.UserEmail)));
                }
            }

            return View(model);
        }

        private string ByteArrayToString(byte[] btes) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < btes.Length; i++)
                sb.Append(btes[i].ToString("X2")); /* hex format */
            return sb.ToString();
        }
    }
}
