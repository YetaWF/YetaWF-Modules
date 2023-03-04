/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;
using YetaWF.Modules.Packages.Modules;

namespace YetaWF.Modules.Packages.Endpoints;

public class PackagesModuleEndpoints : YetaWFEndpoints {

    internal const string LocalizeAllPackages = "LocalizeAllPackages";
    internal const string LocalizePackage = "LocalizePackage";
    internal const string InstallPackageModels = "InstallPackageModels";
    internal const string UninstallPackageModels = "UninstallPackageModels";
    internal const string RemovePackage = "RemovePackage";

    internal const string ExportPackage = "ExportPackage";
    internal const string ExportPackageWithSource = "ExportPackageWithSource";
    internal const string ExportPackageData = "ExportPackageData";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(PackagesModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PackagesModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            PackagesModule module = await GetModuleAsync<PackagesModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost(LocalizeAllPackages, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            List<string> errorList = new List<string>();
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage) {
                    if (!await package.LocalizeAsync(errorList)) {
                        ScriptBuilder sb = new ScriptBuilder();
                        sb.Append(__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), package.Name);
                        sb.Append(errorList, LeadingNL: true);
                        throw new Error(sb.ToString());
                    }
                }
            }
            return Done(__ResStr("generatedAll", "Localization resources for all packages have been successfully generated"));
        })
            .ExcludeDemoMode();

        group.MapPost(LocalizePackage, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Localize")) return Results.Unauthorized();
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.LocalizeAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Done(__ResStr("generated", "Package localization resources successfully generated"), ForcePopup: true);
        })
            .ExcludeDemoMode();

        group.MapPost(InstallPackageModels, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Installs")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.InstallModelsAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(__ResStr("cantInstallModels", "Can't install models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Done(__ResStr("installed", "Package models successfully installed"), ForcePopup: true);
        })
            .ExcludeDemoMode();

        group.MapPost(UninstallPackageModels, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Installs")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.UninstallModelsAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(__ResStr("cantUninstallModels", "Can't uninstall models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Done(__ResStr("removed", "Package models successfully removed"));
        })
            .ExcludeDemoMode();

        group.MapPost(RemovePackage, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Installs")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.RemoveAsync(packageName, errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(__ResStr("cantRemove", "Can't remove package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            string msg;
            if (await package.GetHasSourceAsync())
                msg = __ResStr("okRemovedSrc", "Package successfully removed - These settings won't take effect until the site is restarted(+nl)(+nl)This package includes source code. The source code, project and project references have to be removed manually in the Visual Studio solution for this YetaWF site.");
            else
                msg = __ResStr("okRemoved", "Package successfully removed - These settings won't take effect until the site is restarted");
            return Reload(ReloadEnum.Page, PopupText: msg);
        })
            .ExcludeDemoMode();
        
        // Downloads

        RouteGroupBuilder downloadGroup = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PackagesModuleEndpoints)))
            .RequireAuthorization();

        downloadGroup.MapGet(ExportPackage, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName, [FromQuery] long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = await package.ExportPackageAsync();
            return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
        })
            .ExcludeDemoMode();

        downloadGroup.MapGet(ExportPackageWithSource, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName, [FromQuery] long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = await package.ExportPackageAsync(SourceCode: true);
            return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
        })
            .ExcludeDemoMode();

        downloadGroup.MapGet(ExportPackageData, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string packageName, [FromQuery] long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("Imports")) return Results.Unauthorized();
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = await package.ExportDataAsync();
            return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
        })
            .ExcludeDemoMode();
    }
}
