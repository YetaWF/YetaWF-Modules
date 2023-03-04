/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;

namespace YetaWF.Modules.ModuleEdit.Endpoints;

public class ModuleControlModuleEndpoints : YetaWFEndpoints {

    internal const string MoveUp = "MoveUp";
    internal const string MoveDown = "MoveDown";
    internal const string MoveTop = "MoveTop";
    internal const string MoveBottom = "MoveBottom";
    internal const string MoveToPane = "MoveToPane";
    internal const string Remove = "Remove";
    internal const string RemovePermanent = "RemovePermanent";
    internal const string ExportModuleData = "ExportModuleData";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ModuleControlModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ModuleControlModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken()
            .ExcludeDemoMode();

        group.MapPost(MoveUp, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            return await MoveModule(__ModuleGuid, pageGuid, moduleGuid, pane, moduleIndex, (PageDefinition page) => {
                page.ModuleDefinitions.MoveUp(pane, moduleGuid, moduleIndex);
            });
        });

        group.MapPost(MoveDown, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            return await MoveModule(__ModuleGuid, pageGuid, moduleGuid, pane, moduleIndex, (PageDefinition page) => {
                page.ModuleDefinitions.MoveDown(pane, moduleGuid, moduleIndex);
            });
        });

        group.MapPost(MoveTop, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            return await MoveModule(__ModuleGuid, pageGuid, moduleGuid, pane, moduleIndex, (PageDefinition page) => {
                page.ModuleDefinitions.MoveTop(pane, moduleGuid, moduleIndex);
            });
        });

        group.MapPost(MoveBottom, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            return await MoveModule(__ModuleGuid, pageGuid, moduleGuid, pane, moduleIndex, (PageDefinition page) => {
                page.ModuleDefinitions.MoveBottom(pane, moduleGuid, moduleIndex);
            });
        });

        group.MapPost(MoveToPane, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string oldPane, string newPane) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || oldPane == null || newPane == null)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return Results.Unauthorized();
            page.ModuleDefinitions.MoveToPane(oldPane, moduleGuid, newPane);
            await page.SaveAsync();
            return Reload(ReloadEnum.Page);
        });


        group.MapPost(Remove, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            if (pageGuid == Guid.Empty || pane == null || moduleIndex < 0)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return Results.Unauthorized();
            page.ModuleDefinitions.Remove(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload(ReloadEnum.Page);
        });

        group.MapPost(RemovePermanent, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            if (pageGuid == Guid.Empty || pane == null || moduleIndex < 0)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return Results.Unauthorized();
            page.ModuleDefinitions.Remove(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            await YetaWF.Core.IO.Module.RemoveModuleDefinitionAsync(moduleGuid);
            return Reload(ReloadEnum.Page);
        });

        RouteGroupBuilder groupDownload = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ModuleControlModuleEndpoints)))
            .RequireAuthorization();

        groupDownload.MapGet(ExportModuleData, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid moduleGuid, long cookieToReturn) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            ModuleDefinition mod = await ModuleDefinition.LoadAsync(moduleGuid) ?? throw new InternalError($"{nameof(ExportModuleData)} called for module {moduleGuid} which doesn't exist");
            YetaWFZipFile zipFile = await mod.ExportDataAsync();
            return await ZippedFileResult.ZipFileAsync(context, zipFile, cookieToReturn);
        })
            .ResourceAuthorize(CoreInfo.Resource_ModuleExport);
    }

    private static async Task<IResult> MoveModule(Guid __ModuleGuid, Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex, Action<PageDefinition> action) {
        ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
        if (!module.IsAuthorized()) return Results.Unauthorized();
        if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex < -1)
            throw new ArgumentException();
        PageDefinition page = await LoadPageAsync(pageGuid);
        if (!page.IsAuthorized_Edit())
            return Results.Unauthorized();
        action(page);
        await page.SaveAsync();
        return Reload(ReloadEnum.Page);
    }

    private static async Task<PageDefinition> LoadPageAsync(Guid pageGuid) {
        PageDefinition? page = await PageDefinition.LoadAsync(pageGuid);
        if (page == null)
            throw new Error(__ResStr("pageNotFound", "Page {0} doesn't exist"), pageGuid.ToString());
        return page;
    }
}
