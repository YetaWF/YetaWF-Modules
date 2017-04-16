/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Security#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Security;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Security.Controllers {

    public class MakeKeysModuleController : ControllerImpl<YetaWF.Modules.Security.Modules.MakeKeysModule> {

        public MakeKeysModuleController() { }

        public class Model {
            [Caption("Public Key"), Description("The public key")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true)]
            public string PublicKey { get; set; }

            [Caption("Private Key"), Description("The private key")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true)]
            public string PrivateKey { get; set; }
        }

        [HttpGet]
        public ActionResult MakeKeys() {
            string publicKey, privateKey;
            RSACrypto.MakeNewKeys(out publicKey, out privateKey);
            Model model = new Model() { PublicKey = publicKey, PrivateKey = privateKey };
            return View(model);
        }

    }
}

