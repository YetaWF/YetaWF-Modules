/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Dashboard.Modules;

public class StatusModuleDataProvider : ModuleDefinitionDataProvider<Guid, StatusModule>, IInstallableModel { }

[ModuleGuid("{c93657d3-522c-483a-a51a-e20d39d95b6a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class StatusModule : ModuleDefinition {

    public StatusModule() {
        Title = this.__ResStr("modTitle", "Status Information");
        Name = this.__ResStr("modName", "Status Information");
        Description = this.__ResStr("modSummary", "Displays information about the YetaWF instance. Status information can be accessed using Admin > Dashboard > Status Information (standard YetaWF site).");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new StatusModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Status Information"),
            MenuText = this.__ResStr("displayText", "Status Information"),
            Tooltip = this.__ResStr("displayTooltip", "Display YetaWF status information"),
            Legend = this.__ResStr("displayLegend", "Displays YetaWF status information"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("YetaWF Version"), Description("The YetaWF version installed (Core Package)")]
        [UIHint("String"), ReadOnly]
        [HelpLink("https://YetaWF.com")]
        public string CoreVersion { get; set; } = null!;

        [Caption("Operating System"), Description("")]
        [UIHint("String"), ReadOnly]
        public string OSDescription { get; set; } = null!;
        [Caption("Framework"), Description("")]
        [UIHint("String"), ReadOnly]
        public string FrameworkDescription { get; set; } = null!;
        [Caption("Operating System Architecture"), Description("")]
        [UIHint("String"), ReadOnly]
        public string OSArchitecture { get; set; } = null!;
        [Caption("Processor Architecture"), Description("")]
        [UIHint("String"), ReadOnly]
        public string ProcessArchitecture { get; set; } = null!;

        [Caption("Deployment Type"), Description("The deployment type used for the current site")]
        [UIHint("String"), ReadOnly]
        public string BlueGreenDeploy { get; set; } = null!;

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
        public string Build { get; set; } = null!;

        [Caption("LetsEncrypt"), Description("Shows whether LetsEncrypt is enabled")]
        [UIHint("String"), ReadOnly, SuppressEmpty]
        public string LetsEncrypt { get; set; } = null!;

    }

    public async Task<ActionInfo> RenderModuleAsync() {
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
        string healthCheckFile = Utility.UrlToPhysical("/_hc.html");
        string blueGreen = "";
        if (await FileSystem.FileSystemProvider.FileExistsAsync(healthCheckFile)) {
            string contents = await FileSystem.FileSystemProvider.ReadAllTextAsync(healthCheckFile);
            if (contents.Contains("Blue", StringComparison.OrdinalIgnoreCase))
                blueGreen = "Blue";
            else if (contents.Contains("Green", StringComparison.OrdinalIgnoreCase))
                blueGreen = "Green";
            else {
                if (Startup.RunningInContainer)
                    blueGreen = "(Container)";
                else
                    blueGreen = "(No)";
            }
        }
        if (!string.IsNullOrWhiteSpace(blueGreen)) {
            model.BlueGreenDeploy = this.__ResStr("blueGreen5", "{0} - {1}", blueGreen, Manager.HostUsed);
        } else {
            model.BlueGreenDeploy = this.__ResStr("blueGreenNone", "N/A");
        }
        model.LetsEncrypt = YetaWF2.LetsEncrypt.LetsEncrypt.Enabled ? this.__ResStr("letsEncryptEnabled", "Enabled") : string.Empty;

        return await RenderAsync(model);
    }
}
