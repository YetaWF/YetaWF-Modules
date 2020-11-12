/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProvider#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("TwilioProcessorDataProvider")]
[assembly: AssemblyDescription("Data provider for the Softelvdm.TwilioProcessor package.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("TwilioProcessorDataProvider")]
[assembly: AssemblyCopyright("Copyright © 2020 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.3.0.0")]

// This package was originally not part of the YetaWF Open Source Project. For that reason it uses the Softelvdm namespace, but for localization we'll use YetaWF.
[assembly: Package(PackageTypeEnum.Module, "Softelvdm", LanguageDomain: "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProvider",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProviderDataProvider#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/TwilioProcessorDataProviderDataProvider#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]
[assembly: RequiresPackage("YetaWF.Identity")]