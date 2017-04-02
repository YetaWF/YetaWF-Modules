/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Controllers {

    public class SkinVisitorModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.SkinVisitorModule> {

        public SkinVisitorModuleController() { }

        public class DisplayModel {
            public string TrackClickUrl { get; set; }
        }

        [HttpGet]
        public ActionResult SkinVisitor() {
            Module.ShowTitle = Manager.EditMode;// always show title in edit mode and never show in display mode
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (visitorDP.Usable) {
                    if (CallbackRegistered == null) {
                        ErrorHandling.RegisterCallback(AddVisitEntryError);
                        CallbackRegistered = true;
                    }
                    AddVisitEntry(null, Manager, visitorDP);
                }
            }
            // We render a form so we get antiforgery fields used for TrackClick
            DisplayModel model = new DisplayModel {
                TrackClickUrl = YetaWFManager.UrlFor(GetType(), nameof(TrackClick))
            };
            return View(model);
        }

        public static bool? CallbackRegistered = null;
        public static bool InCallback = false;

        [HttpPost]
        // Don't use ConditionalAntiForgeryToken here - We have to prevent malicious cross-site attacks as this could be
        // exploited to flood click tracking and fill up DBs, etc.
        // This means you cannot use tracking on STATIC pages.
        [ValidateAntiForgeryToken]
        public ActionResult TrackClick(string url) {
            string origin = Manager.CurrentRequest.Headers["Origin"];
            if (!string.IsNullOrWhiteSpace(origin))
                return new EmptyResult();
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (visitorDP.Usable) {
                    AddVisitEntry(url, Manager, visitorDP);
                }
            }
            return new EmptyResult();
        }

        public static void AddVisitEntryError(string error) {
            if (!InCallback) {
                InCallback = true;
                using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                    try {
                        AddVisitEntry(null, Manager, visitorDP, error);
                    } catch (Exception) { }
                }
                InCallback = false;
            }
        }

        // because the callback is registered globally (for all sites) errors are always logged for all sites, visitors are only logged if the page has a skinvisitor module reference
        private static void AddVisitEntry(string url, YetaWFManager manager, VisitorEntryDataProvider visitorDP, string error = null) {
            if (manager == null || !manager.HaveCurrentContext) return;
            string sessionKey = AreaRegistration.CurrentPackage.Name + "_Visitor";
            long sessionKeyVal;
            string val = manager.CurrentSession.GetString(sessionKey);
            if (!string.IsNullOrWhiteSpace(val))
                sessionKeyVal = Convert.ToInt64(val);
            else {
                sessionKeyVal = DateTime.Now.Ticks;/*local time*/
                manager.CurrentSession.SetString(sessionKey, sessionKeyVal.ToString());
            }

            GeoLocation geoLocation = new GeoLocation(manager);
            GeoLocation.UserInfo userInfo = geoLocation.GetCurrentUserInfo();

            string referrer;
            string userAgent;
#if MVC6
            if (url == null)
                url = Manager.CurrentRequest.GetDisplayUrl();
            referrer = Manager.CurrentRequest.Headers["Referer"].ToString();
            userAgent = Manager.CurrentRequest.Headers["User-Agent"].ToString();
#else
            if (url == null)
                url = Manager.CurrentRequest.Url.ToString();
            referrer = Manager.CurrentRequest.UrlReferrer != null ? Manager.CurrentRequest.UrlReferrer.ToString() : null;
            userAgent = Manager.CurrentRequest.UserAgent;
#endif


            VisitorEntry visitorEntry = new VisitorEntry {
                SessionKey = sessionKeyVal,
                AccessDateTime = DateTime.UtcNow,
                UserId = manager.UserId,
                IPAddress = userInfo.IPAddress.Truncate(Globals.MaxIP),
                Url = url != null ? url.Truncate(Globals.MaxUrl) : "",
                Referrer = referrer != null ? referrer.Truncate(Globals.MaxUrl) : "",
                UserAgent = userAgent != null ? userAgent.Truncate(VisitorEntry.MaxUserAgent) : "",
                Longitude = userInfo.Longitude,
                Latitude = userInfo.Latitude,
                ContinentCode = userInfo.ContinentCode.Truncate(VisitorEntry.MaxContinentCode),
                CountryCode = userInfo.CountryCode.Truncate(VisitorEntry.MaxCountryCode),
                RegionCode = userInfo.RegionCode.Truncate(VisitorEntry.MaxRegionCode),
                City = userInfo.City.Truncate(VisitorEntry.MaxCity),
                Error = error.Truncate(VisitorEntry.MaxError),
            };
            visitorDP.AddItem(visitorEntry);
        }
    }
}
