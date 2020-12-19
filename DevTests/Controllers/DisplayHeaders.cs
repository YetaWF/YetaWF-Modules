/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace YetaWF.Modules.DevTests.Controllers {

    public class DisplayHeadersModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.DisplayHeadersModule> {

        public DisplayHeadersModuleController() { }

        public class DisplayModel {

            [Caption("Headers"), Description("Request Headers")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> Headers { get; set; }

            [Caption("Variables"), Description("Various Variables")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> Variables { get; set; }

            public DisplayModel() {
                Headers = new List<string>();
                Variables = new List<string>();
            }
        }

        [AllowGet]
        public ActionResult DisplayHeaders() {
            DisplayModel model = new DisplayModel();
            foreach (var hdr in Request.Headers) {
                model.Headers.Add(hdr.ToString());
            }
            model.Variables.Add($"{nameof(Manager.HostSchemeUsed)} = {Manager.HostSchemeUsed}");
            model.Variables.Add($"{nameof(Manager.HostPortUsed)} = {Manager.HostPortUsed}");
            model.Variables.Add($"{nameof(Manager.HostUsed)} = {Manager.HostUsed}");
            model.Variables.Add($"{nameof(YetaWFManager.IsHTTPSite)} = {YetaWFManager.IsHTTPSite}");
            model.Variables.Add($"{nameof(Manager.IsLocalHost)} = {Manager.IsLocalHost}");
            model.Variables.Add($"{nameof(Manager.CurrentRequestUrl)} = {Manager.CurrentRequestUrl}");
            model.Variables.Add($"UriHelper.GetDisplayUrl(Manager.CurrentRequest) = {UriHelper.GetDisplayUrl(Manager.CurrentRequest)}");

            model.Variables.Add($"{nameof(Manager.CurrentRequest.Scheme)} = {Manager.CurrentRequest.Scheme}");
            model.Variables.Add($"{nameof(Manager.CurrentRequest.IsHttps)} = {Manager.CurrentRequest.IsHttps}");
            model.Variables.Add($"{nameof(Manager.CurrentRequest.Host)} = {Manager.CurrentRequest.Host}");
            return View(model);
        }
    }
}
