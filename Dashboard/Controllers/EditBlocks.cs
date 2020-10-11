/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF2.Middleware;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class EditBlocksModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.EditBlocksModule> {

        public EditBlocksModuleController() { }

        [Trim]
        public class EditModel {

            [Category("Not Authorized"), Caption("URL Path Contains"), Description("List of text fragments, one entry per line - If a text fragment is found in a URL, the request returns 401 Not Authorized - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string NotAuthUrlPathContains { get; set; }

            [Category("Not Authorized"), Caption("URL Path Ends With"), Description("List of text fragments, one entry per line - If a text fragment is found at the end of a URL path, the request returns 401 Not Authorized - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string NotAuthUrlPathEndsWith { get; set; }

            [Category("Not Authorized"), Caption("User Agent Contains"), Description("List of text fragments, one entry per line - If a text fragment is found in a user agent, the request returns 401 Not Authorized - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string NotAuthUserAgentContains { get; set; }

            [Category("Successful"), Caption("URL Path Contains"), Description("List of text fragments, one entry per line - If a text fragment is found in a URL, the request returns 200 OK with an empty response body - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string SuccessUrlPathContains { get; set; }

            [Category("Successful"), Caption("URL Path Ends With"), Description("List of text fragments, one entry per line - If a text fragment is found at the end of a URL path, the request returns 200 OK with an empty response body - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string SuccessUrlPathEndsWith { get; set; }

            [Category("Successful"), Caption("User Agent Contains"), Description("List of text fragments, one entry per line - If a text fragment is found in a user agent, the request returns 200 OK with an empty response body - The comparison is case insensitive")]
            [UIHint("TextAreaSourceOnly"), StringLength(10000), Trim]
            public string SuccessUserAgentContains { get; set; }

            public EditModel() { }
        }

        [AllowGet]
        public ActionResult EditBlocks() {

            BlockSettingsDefinition settings = BlockRequestMiddleware.GetCurrentSettings();

            EditModel model = new EditModel {
                NotAuthUrlPathContains = ToStringLines(settings?.NotAuthorized.UrlPathContains),
                NotAuthUrlPathEndsWith = ToStringLines(settings?.NotAuthorized.UrlPathEndsWith),
                NotAuthUserAgentContains = ToStringLines(settings?.NotAuthorized.UserAgentContains),
                SuccessUrlPathContains = ToStringLines(settings?.Successful.UrlPathContains),
                SuccessUrlPathEndsWith = ToStringLines(settings?.Successful.UrlPathEndsWith),
                SuccessUserAgentContains = ToStringLines(settings?.Successful.UserAgentContains),
            };

            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> EditBlocks_Partial(EditModel model) {

            if (!ModelState.IsValid)
                return PartialView(model);

            BlockSettingsDefinition settings = new BlockSettingsDefinition {
                NotAuthorized = new AuthorizationDefinition {
                    UrlPathContains = ToStringList(model.NotAuthUrlPathContains),
                    UrlPathEndsWith = ToStringList(model.NotAuthUrlPathEndsWith),
                    UserAgentContains = ToStringList(model.NotAuthUserAgentContains),
                },
                Successful = new AuthorizationDefinition {
                    UrlPathContains = ToStringList(model.SuccessUrlPathContains),
                    UrlPathEndsWith = ToStringList(model.SuccessUrlPathEndsWith),
                    UserAgentContains = ToStringList(model.SuccessUserAgentContains),
                },
            };
            await BlockRequestMiddleware.SaveNewSettings(settings);

            return FormProcessed(model, this.__ResStr("okSaved", "Request blocking settings saved"));
        }

        private string ToStringLines(List<string> list) {
            if (list == null) return null;
            return string.Join("\n", list);
        }
        private List<string> ToStringList(string text) {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();
            List<string> list = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            list = (from l in list where l.Trim().Length > 0 select l).ToList();
            return list;
        }
    }
}
