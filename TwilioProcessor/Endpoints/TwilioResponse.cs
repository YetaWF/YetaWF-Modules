/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;

namespace Softelvdm.Modules.TwilioProcessor.Endpoints;

public class TwilioResponseEndpoints : YetaWFEndpoints {

    internal const string Response = "Response";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(TwilioResponseEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(TwilioResponseEndpoints)));

        group.MapGet(Response, (HttpContext context, string ValidateToNumber, string To, string From, string MessageSid, string MessageStatus, string ErrorCode) => {
            if (ValidateToNumber != To) {
                Logging.AddErrorLog("Response from Twilio: Invalid (to number doesn't match) - {0}", Manager.CurrentRequestUrl);
            } else {
                if (ErrorCode != "0")
                    Logging.AddErrorLog("Response from Twilio: {0} {1} {2} {3} {4} {5}", From, To, ErrorCode, MessageStatus, MessageSid, Manager.CurrentRequestUrl);
                else
                    Logging.AddLog("Response from Twilio: {0} {1} {2} {3} {4} {5}", From, To, ErrorCode, MessageStatus, MessageSid, Manager.CurrentRequestUrl);
            }
            return Results.Ok();
        });
    }
}
