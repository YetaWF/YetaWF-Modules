/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Core.Identity;
using YetaWF.PackageAttributes;
using YetaWF.Core.Packages;

[assembly: AssemblyTitle("Identity")]
[assembly: AssemblyDescription("User login, registration and authentication")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("Identity")]
[assembly: AssemblyCopyright("Copyright © 2020 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.3.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://yetawf.com/Documentation/YetaWF/Identity",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://yetawf.com/Documentation/YetaWF/Identity#Release%20Notice",
    "https://yetawf.com/Documentation/YetaWF/Identity#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")]

[assembly: ServiceLevel(ServiceLevelEnum.LowLevelServiceProvider)]

[assembly: Resource(Info.Resource_AllowUserLogon, "Allow logon as another user", Administrator = true, Superuser = true)]
[assembly: Resource(Info.Resource_AllowUserIdAjax, "Allow user list retrieval (Ajax) for UserId template", Administrator = true, Superuser = true)]
[assembly: Resource(Info.Resource_AllowListOfUserNamesAjax, "Allow user list retrieval (Ajax) for ListOfUserNames template", Administrator = true, Superuser = true)]

[assembly: InstallOrder(typeof(YetaWF.Modules.Identity.DataProvider.RoleDefinitionDataProvider))]
[assembly: InstallOrder(typeof(YetaWF.Modules.Identity.DataProvider.SuperuserDefinitionDataProvider))]
[assembly: InstallOrder(typeof(YetaWF.Modules.Identity.DataProvider.UserDefinitionDataProvider))]
[assembly: InstallOrder(typeof(YetaWF.Modules.Identity.DataProvider.AuthorizationDataProvider))]
