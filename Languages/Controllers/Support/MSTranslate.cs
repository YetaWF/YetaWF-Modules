using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Security.AntiXss;
using System.Xml.Linq;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Controllers.Support {

    [DataContract]
    internal class AdmAccessToken {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    internal class MSTranslate {

        internal class AdmAuthentication {

            public static readonly string DataMarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

            private string _clientId;
            private string _clientSecret;
            private string _request;
            private AdmAccessToken _token;

            public AdmAuthentication(string clientId, string clientSecret) {
                _clientId = clientId;
                _clientSecret = clientSecret;
                _request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", YetaWFManager.UrlEncodeArgs(clientId), YetaWFManager.UrlEncodeArgs(clientSecret));
                _token = HttpPost(DataMarketAccessUri, _request);
            }
            public AdmAccessToken GetAccessToken() {
                return this._token;
            }
            private AdmAccessToken HttpPost(string dataMarketAccessUri, string requestDetails) {
                WebRequest webRequest = WebRequest.Create(dataMarketAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = webRequest.GetRequestStream()) {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = webRequest.GetResponse()) {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                    AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                    return token;
                }
            }
        }

        private AdmAccessToken _admToken;

        public MSTranslate(string clientId, string clientSecret) {
            AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);
            _admToken = admAuth.GetAccessToken();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Don't care")]
        public List<string> Translate(string from, string to, List<string> strings) {

            string headerValue = "Bearer " + _admToken.access_token;
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append("<TranslateArrayRequest>" +
                             "<AppId />" +
                             "<From>{0}</From>" +
                             "<Options>" +
                                " <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">text/plain</ContentType>" +
                                 "<ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                             "</Options>" +
                             "<Texts>", from);
            foreach (string s in strings)
                hb.Append("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", AntiXssEncoder.XmlEncode(s));
            hb.Append("</Texts>" +
                    "<To>{0}</To>" +
                    "</TranslateArrayRequest>", to);

            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", headerValue);
            request.ContentType = "text/xml";
            request.Method = "POST";

            using (System.IO.Stream stream = request.GetRequestStream()) {
                byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(hb.ToString());
                stream.Write(arrBytes, 0, arrBytes.Length);
            }

            // Get the response
            List<string> newStrings = new List<string>();
            WebResponse response = null;

            using (response = request.GetResponse()) {
                using (Stream stream = response.GetResponseStream()) {
                    using (StreamReader rdr = new StreamReader(stream, System.Text.Encoding.UTF8)) {
                        string strResponse = rdr.ReadToEnd();
                        XDocument doc = XDocument.Parse(strResponse);
                        XNamespace ns = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2";
                        foreach (XElement xe in doc.Descendants(ns + "TranslateArrayResponse")) {
                            foreach (var node in xe.Elements(ns + "TranslatedText"))
                                newStrings.Add(node.Value);
                        }
                    }
                }
            }
            return newStrings;
        }
    }
}
