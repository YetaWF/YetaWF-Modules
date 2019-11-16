/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
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
            [HelpLink("https://YetaWF.com")]
            public string CoreVersion{ get; set; }

            [Caption("ASP.NET/MVC Version"), Description("The ASP.NET/MVC version used")]
            [UIHint("String"), ReadOnly]
            public string AspNetMvc { get; set; }

            [Caption("Operating System"), Description("")]
            [UIHint("String"), ReadOnly]
            public string OSDescription { get; set; }
            [Caption("Framework"), Description("")]
            [UIHint("String"), ReadOnly]
            public string FrameworkDescription { get; set; }
            [Caption("Operating System Architecture"), Description("")]
            [UIHint("String"), ReadOnly]
            public string OSArchitecture { get; set; }
            [Caption("Processor Architecture"), Description("")]
            [UIHint("String"), ReadOnly]
            public string ProcessArchitecture { get; set; }

            [Caption("Deployment Type"), Description("The deployment type used for the current site")]
            [UIHint("String"), ReadOnly]
            public string BlueGreenDeploy { get; set; }

            [Caption("Multi-Instance Enabled"), Description("Defines whether multiple running instances (container/webfarm/webgarden) support is enabled using shared caching")]
            [UIHint("Boolean"), ReadOnly]
            public bool MultiInstance { get; set; }

            [Caption("Last Restart"), Description("The date and time the site (all instances) was last restarted")]
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
        public async Task<ActionResult> Status() {
            DisplayModel model = new DisplayModel {
                LastRestart = YetaWF.Core.Support.Startup.MultiInstanceStartTime,
                MultiInstance = YetaWF.Core.Support.Startup.MultiInstance,
                OSArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString(),
                FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                ProcessArchitecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(),
            };

            Package corePackage = Package.GetPackageFromPackageName("YetaWF.Core");
            if (corePackage != null)
                model.CoreVersion = corePackage.Version;
            if (YetaWFManager.Deployed)
                model.LastDeploy = await FileSystem.FileSystemProvider.GetCreationTimeUtcAsync(Path.Combine(YetaWFManager.RootFolderWebProject, Globals.NodeModulesFolder));
#if DEBUG
            model.Build = "Debug";
#else
            model.Build = "Release";
#endif
            model.AspNetMvc = Utility.GetAspNetMvcName(Utility.AspNetMvc);

            string healthCheckFile = Utility.UrlToPhysical("/_hc.html");
            string blueGreen = "";
            if (await FileSystem.FileSystemProvider.FileExistsAsync(healthCheckFile)) {
                string contents = await FileSystem.FileSystemProvider.ReadAllTextAsync(healthCheckFile);
                if (contents.Contains("Blue"))
                    blueGreen = "Blue";
                else if (contents.Contains("Green"))
                    blueGreen = "Green";
                else {
                    if (Startup.RunningInContainer)
                        blueGreen = "(Container)";
                    else
                        blueGreen = "(No)";
                }
            }
            if (!string.IsNullOrWhiteSpace(blueGreen)) {
#if MVC6
                model.BlueGreenDeploy = this.__ResStr("blueGreen5", "{0} - {1}", blueGreen,  Manager.HostUsed);
#else
                model.BlueGreenDeploy = this.__ResStr("blueGreen4", "{0} - {1}:{2}", blueGreen, Manager.HostUsed, Manager.HostPortUsed);
#endif
            } else {
                model.BlueGreenDeploy = this.__ResStr("blueGreenNone", "N/A");
            }

            return View(model);
        }
    }
}