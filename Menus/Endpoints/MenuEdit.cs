/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menu.Endpoints;

public class MenuEditModuleEndpoints : YetaWFEndpoints {

    public const string EntireMenu = "EntireMenu";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(MenuEditModuleEndpoints), name, defaultValue, parms); }

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
