/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Security#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Security;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Security.Controllers {

    public class MakeKeysModuleController : ControllerImpl<YetaWF.Modules.Security.Modules.MakeKeysModule> {

        public MakeKeysModuleController() { }

        public class Model {
            [Caption("Public Key"), Description("The public key")]
            [UIHint("TextAreaSourceOnly"), StringLength(0)]
            public string PublicKey { get; set; } = null!;

            [Caption("Private Key"), Description("The private key")]
            [UIHint("TextAreaSourceOnly"), StringLength(0)]
            public string PrivateKey { get; set; } = null!;
        }

        [AllowGet]
        public ActionResult MakeKeys() {
            string publicKey, privateKey;
            RSACrypto.MakeNewKeys(out publicKey, out privateKey);
            Model model = new Model() { PublicKey = publicKey, PrivateKey = privateKey };
            return View(model);
        }

    }
}

