/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.PackageAttributes;
using YetaWF.Core.Packages;

[assembly: AssemblyTitle("SyntaxHighlighter")]
[assembly: AssemblyDescription("Syntax Highlighter")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("SyntaxHighlighter")]
[assembly: AssemblyCopyright("Copyright © 2017 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("2.5.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License")]

[assembly: RequiresAddOnGlobal("alexgorbatchev.com", "SyntaxHighlighter", "3.0.83")]
