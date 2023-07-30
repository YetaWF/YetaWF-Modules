/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menu.Endpoints;

public class MenuEditModuleEndpoints : YetaWFEndpoints {

    public const string EntireMenu = "EntireMenu";
    public const string CopyPage = "CopyPage";
    public const string SavePage = "SavePage";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(MenuEditModuleEndpoints), name, defaultValue, parms); }

    public class PageData {
        public MultiString MenuText { get; set; } = null!;
        //public string ImagePNG { get; set; }
        public string ImageSVG { get; set; } = null!;
        public MultiString Tooltip { get; set; } = null!;
    }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(MenuEditModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(EntireMenu, async (HttpContext context, Guid menuGuid, long menuVersion) => {

            MenuModule modMenu = await GetModuleAsync<MenuModule>(menuGuid);
            if (menuVersion != modMenu.MenuVersion)
                throw new Error(__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved - Please refresh the current page before proceeding"));

            string entireMenu;
            using (StreamContent streamContent = new StreamContent(context.Request.Body)) {
                entireMenu = await streamContent.ReadAsStringAsync();
            }

            MenuList origMenu = await modMenu.GetMenuAsync();
            MenuList menu = DeserializeFromJSON(entireMenu);
            await modMenu.SaveMenuAsync(menu);

            return Results.Json(new EntireMenuResult {
                NewVersion = modMenu.MenuVersion
            });
        });

        group.MapPost(CopyPage, async (HttpContext context, Guid __ModuleGuid, string page) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();

            PageDefinition? pageDef = await PageDefinition.LoadFromUrlAsync(page);
            if (pageDef == null)
                throw new Error(__ResStr("noPage", "Page {0} doesn't exist.", page));
            return Results.Json(new PageData {
                MenuText = pageDef.Title,
                //ImagePNG = pageDef.FavIcon ?? string.Empty,
                ImageSVG = pageDef.Fav_SVG ?? string.Empty,
                Tooltip = pageDef.Description,
            });
        });

        group.MapPost(SavePage, async (HttpContext context, Guid __ModuleGuid, string page, [FromBody] PageData data) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();

            PageDefinition? pageDef = await PageDefinition.LoadFromUrlAsync(page);
            if (pageDef == null)
                throw new Error(__ResStr("noPage", "Page {0} doesn't exist.", page));
            pageDef.Title = data.MenuText;
            //pageDef.FavIcon = string.IsNullOrWhiteSpace(data.ImagePNG) ? null : data.ImagePNG;
            pageDef.Fav_SVG = string.IsNullOrWhiteSpace(data.ImageSVG) ? null : data.ImageSVG;
            pageDef.Description = data.Tooltip;
            await pageDef.SaveAsync();
            return Results.Ok();
        });
    }

    public class EntireMenuResult {
        public long NewVersion { get; set; }
    }

    private static MenuList DeserializeFromJSON(string menuJSON) {
        List<ModuleAction> actions = (List<ModuleAction>)Utility.JsonDeserialize(menuJSON, typeof(List<ModuleAction>));
        // fix some settings that aren't updated on the browser side
        MenuList menu = new MenuList(actions);
        return menu;
    }
}
