/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.DevTests.Endpoints {

    public class TemplateDropDownSearchDataEndpoints : YetaWFEndpoints {

        internal const string GetData = "GetData";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(TemplateDropDownSearchDataEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GetData, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string? search) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                search = search?.ToLower();
                List<SelectionItem<int>> list = GetSampleData();
                if (search != null)
                    list = (from l in list where l.Text.ToString().ToLower().Contains(search) select l).Take(50).ToList();
                else
                    list = list.Take(50).ToList();
                // copy so we get the active language texts
                return Results.Ok((from l in list select new { Text = l.Text?.ToString(), Tooltip = l.Tooltip?.ToString(), Value = l.Value }).ToList());
            });
        }

        public static List<SelectionItem<int>> GetSampleData() {
            List<SelectionItem<int>> list = new List<SelectionItem<int>>();
            for (int i = 1; i < 100; ++i) {
                list.Add(new SelectionItem<int> { Value = i, Text = $"Item {i}", Tooltip = $"Tooltip for item {i}" });
            }
            return list;
        }
    }
}
