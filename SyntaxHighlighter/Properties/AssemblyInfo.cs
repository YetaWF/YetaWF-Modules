/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

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

[assembly: AssemblyVersion("1.1.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("http://YetaWF.com/UpdateServer",
    "http://yetawf.com/Documentation/YetaWF/SyntaxHighlighter",
    "http://YetaWF.com/Documentation/YetaWF/Support",
    "http://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#Release%20Notice",
    "http://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License")]

[assembly: RequiresAddOnGlobal("alexgorbatchev.com", "SyntaxHighlighter", "3.0.83")]
