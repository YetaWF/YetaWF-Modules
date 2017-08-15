﻿/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.DataProvider;
using YetaWF.Core.Site;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    // Standard MVC Controller
    // Standard MVC Controller
    // Standard MVC Controller

    public class StartupPageController : YetaWFController {

        [AllowGet]
        public ActionResult Show() {

            if (SiteDefinition.INITIAL_INSTALL_ENDED)
                return Redirect("/Maintenance/StartupDone.html");
#if MVC6
            return Redirect("/Maintenance/StartupPage6.html");
#else
            return Redirect("/Maintenance/StartupPage5.html");
#endif
        }

        [AllowPost]
        public ActionResult Run() {

            if (!SiteDefinition.INITIAL_INSTALL || SiteDefinition.INITIAL_INSTALL_ENDED)
                return NotAuthorized();

            PackagesDataProvider packagesDP = new PackagesDataProvider();
            QueryHelper qh = new QueryHelper();
            qh.Add("From", "Data");
            packagesDP.InitAll(qh);

            return new EmptyResult();
        }
    }
}