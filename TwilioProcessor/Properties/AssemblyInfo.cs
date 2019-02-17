/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("TwilioProcessor")]
[assembly: AssemblyDescription("Twilio - Build apps that communicate with everyone in the world. Voice & Video, Messaging, and Authentication APIs for every application.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("TwilioProcessor")]
[assembly: AssemblyCopyright("Copyright © 2019 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("4.2.0.0")]

// This package was originally not part of the YetaWF Open Source Project. For that reason it uses the Softelvdm namespace, but for localization we'll use YetaWF.
[assembly: Package(PackageTypeEnum.Module, "Softelvdm", LanguageDomain: "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessor",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessor#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License")]

[assembly: RequiresPackage("YetaWF.Identity")]