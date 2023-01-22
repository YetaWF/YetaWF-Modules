/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Packages;
using YetaWF.Modules.Pages.Endpoints;

namespace YetaWF.Modules.Pages {
    /// <inheritdoc/>
    public class AreaRegistration : YetaWF.Core.Controllers.AreaRegistrationBase {
        /// <summary>
        /// Defines the current package, used by applications that need access to the YetaWF.Core.Packages.Package instance.
        /// </summary>
        public static Package CurrentPackage { get { return _CachedCurrentPackage ??= (_CachedCurrentPackage = Package.GetPackageFromAssembly(typeof(AreaRegistration).Assembly)); } }
        private static Package? _CachedCurrentPackage;

        /// <inheritdoc/>
        public override void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {
            endpoints.MapPost($"/{areaName}/{GetEndpoint(nameof(ListOfLocalPagesEndpoint))}/{nameof(ListOfLocalPagesEndpoint.BrowseGridData)}", async (HttpContext context, GridSupport.GridPartialViewData gridPVData) => {
                return await ListOfLocalPagesEndpoint.BrowseGridData(context, gridPVData);
            });
        }
    }
}
