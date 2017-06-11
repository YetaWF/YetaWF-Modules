/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.IO;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class StatusModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.StatusModule> {

        public StatusModuleController() { }

        public class DisplayModel {

            [Caption("YetaWF Version"), Description("The YetaWF version installed (Core Package)")]
            [UIHint("String"), ReadOnly]
            public string CoreVersion{ get; set; }

            [Caption("ASP.NET/MVC Version"), Description("The ASP.NET/MVC version used")]
            [UIHint("String"), ReadOnly]
            public string AspNetMvc { get; set; }

            [Caption("Last Restart"), Description("The date and time the site was last restarted")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime LastRestart { get; set; }

            [Caption("Last Deploy"), Description("The date and time the site was deployed - Only shown for deployed sites")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime? LastDeploy { get; set; }

            [Caption("Build"), Description("The type of build that is currently running")]
            [UIHint("String"), ReadOnly]
            public string Build { get; set; }

        }

        [AllowGet]
        public ActionResult Status() {
            DisplayModel model = new DisplayModel {
                LastRestart = YetaWFManager.SiteStart,
            };
            Package corePackage = Package.GetPackageFromPackageName("YetaWF.Core");
            if (corePackage != null)
                model.CoreVersion = corePackage.Version;
            if (Manager.Deployed)
                model.LastDeploy = Directory.GetCreationTimeUtc(Path.Combine(YetaWFManager.RootFolder, Globals.NugetScriptsFolder));
#if DEBUG
            model.Build = "Debug";
#else
            model.Build = "Release";
#endif
            model.AspNetMvc = YetaWFManager.GetAspNetMvcName(YetaWFManager.AspNetMvc);

            return View(model);
        }
    }
}