/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Controllers {

    public class SkinVisitorModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.SkinVisitorModule> {

        public SkinVisitorModuleController() { }

        [HttpGet]
        public ActionResult SkinVisitor() {
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (visitorDP.Usable) {
                    if (CallbackRegistered == null) {
                        ErrorHandling.RegisterCallback(AddVisitEntryError);
                        CallbackRegistered = true;
                    }
                    AddVisitEntry(Manager, visitorDP);
                }
            }
            return new EmptyResult();
        }

        public static bool? CallbackRegistered = null;
        public static bool InCallback = false;

        public static void AddVisitEntryError(string error) {
            if (!InCallback) {
                InCallback = true;
                using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                    try {
                        AddVisitEntry(Manager, visitorDP, error);
                    } catch (Exception) { }
                }
                InCallback = false;
            }
        }

        // because the callback is registered globally (for all sites) errors are always logged for all sites, visitors are only logged if the page has a skinvisitor module reference
        private static void AddVisitEntry(YetaWFManager manager, VisitorEntryDataProvider visitorDP, string error = null) {
            if (manager == null || !manager.HaveCurrentContext) return;
            string sessionKey = AreaRegistration.CurrentPackage.Name + "_Visitor";
            long sessionKeyVal;
            if (manager.CurrentSession[sessionKey] != null)
                sessionKeyVal = Convert.ToInt64(manager.CurrentSession[sessionKey]);
            else {
                sessionKeyVal = DateTime.Now.Ticks;/*local time*/
                manager.CurrentSession[sessionKey] = sessionKeyVal.ToString();
            }

            GeoLocation geoLocation = new GeoLocation(manager);
            GeoLocation.UserInfo userInfo = geoLocation.GetCurrentUserInfo();

            VisitorEntry visitorEntry = new VisitorEntry {
                SessionKey = sessionKeyVal,
                AccessDateTime = DateTime.UtcNow,
                UserId = manager.UserId,
                IPAddress = userInfo.IPAddress.Truncate(Globals.MaxIP),
                Url = Manager.CurrentRequest.Url.ToString().Truncate(Globals.MaxUrl),
                Referrer = Manager.CurrentRequest.UrlReferrer != null ? Manager.CurrentRequest.UrlReferrer.ToString().Truncate(Globals.MaxUrl) : "",
                UserAgent = Manager.CurrentRequest.UserAgent.Truncate(VisitorEntry.MaxUserAgent),
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
