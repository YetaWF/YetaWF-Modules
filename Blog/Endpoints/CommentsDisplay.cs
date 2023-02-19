/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Addons;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Endpoints;

public class CommentsDisplayModuleEndpoints : YetaWFEndpoints {

    internal const string Approve = "Approve";
    internal const string Remove = "Remove";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(CommentsDisplayModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(CommentsDisplayModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken()
            .ExcludeDemoMode()
            .ResourceAuthorize(Info.Resource_AllowManageComments);

        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, int blogEntry, int comment) => {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment? cmt = await dataProvider.GetItemAsync(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                cmt.Approved = true;
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(cmt);
                if (status != UpdateStatusEnum.OK)
                    throw new InternalError("Can't update comment entry - {0}", status);
                return Reload(ReloadEnum.Page);
            }
        });
        group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, int blogEntry, int comment) => {
            using (BlogCommentDataProvider dataProvider = new BlogCommentDataProvider(blogEntry)) {
                BlogComment? cmt = await dataProvider.GetItemAsync(comment);
                if (cmt == null)
                    throw new InternalError("Can't find comment entry {0}", comment);
                if (!await dataProvider.RemoveItemAsync(comment))
                    throw new InternalError("Can't remove comment entry");
                return Reload(ReloadEnum.Page);
            }
        });
    }
}
