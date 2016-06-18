/* Copyright � 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Visitors#License */

using System.Net;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Visitors.Controllers {

    public class IPAddressLookupModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.IPAddressLookupModule> {

        public IPAddressLookupModuleController() { }

        public class Model {

            [Caption("IP Address"), Description("The IP address")]
            [UIHint("String"), ReadOnly]
            public string IPAddress{ get; set; }

            [Caption("Host Name"), Description("The host name")]
            [UIHint("String"), ReadOnly]
            public string HostName { get; set; }

            [Caption("Latitude"), Description("The latitude where the IP address is located")]
            [UIHint("floatValue"), AdditionalMetadata("EmptyIf0", true), SuppressEmpty, ReadOnly]
            public float Latitude { get; set; }
            [Caption("Longitude"), Description("The longitude where the IP address is located")]
            [UIHint("floatValue"), AdditionalMetadata("EmptyIf0", true), SuppressEmpty, ReadOnly]
            public float Longitude { get; set; }
            [Caption("Region"), Description("The region where the IP address is located")]
            [UIHint("String"), SuppressEmpty, ReadOnly]
            public string RegionName { get; set; }
            [Caption("City"), Description("The city where the IP address is located")]
            [UIHint("String"), SuppressEmpty, ReadOnly]
            public string City { get; set; }
            [Caption("Country"), Description("The country where the IP address is located")]
            [UIHint("String"), SuppressEmpty, ReadOnly]
            public string CountryName { get; set; }
            [Caption("Continent"), Description("The continent where the IP address is located")]
            [UIHint("String"), SuppressEmpty, ReadOnly]
            public string ContinentCode { get; set; }
        }

        [HttpGet]
        public ActionResult IPAddressLookup(string ipAddress, bool geoData) {
            if (string.IsNullOrWhiteSpace(ipAddress)) throw new InternalError("IP address not specified");
            Model model = new Model();
            model.IPAddress = ipAddress;

            if (geoData) {
                GeoLocation geoLocation = new GeoLocation(Manager);
                GeoLocation.UserInfo info = geoLocation.GetUserInfo(ipAddress);
                ObjectSupport.CopyData(info, model);
            }
            try {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                model.HostName = hostEntry.HostName;
            } catch { }

            return View(model);
        }
    }
}