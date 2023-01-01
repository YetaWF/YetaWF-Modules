/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("IVR")]
[assembly: AssemblyDescription("IVR - Telephone automated attendant and voice mail system based on Twilio")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("IVR")]
[assembly: AssemblyCopyright("Copyright © 2023 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.5.0.0")]

// This package was originally not part of the YetaWF Open Source Project. For that reason it uses the Softelvdm namespace, but for localization we'll use YetaWF.
[assembly: Package(PackageTypeEnum.Module, "Softelvdm", LanguageDomain: "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/IVR",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/IVR#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/IVR#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")] // needed for HTML components (can be removed if no views/components are implemented by this package)
[assembly: RequiresPackage("Softelvdm.TwilioProcessor")]
