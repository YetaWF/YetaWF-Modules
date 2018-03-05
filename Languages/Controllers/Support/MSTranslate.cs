using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using YetaWF.Core.Support;
#if MVC6
#else
using System.Web.Security.AntiXss;
#endif

//$$$ASYNCIFY

namespace YetaWF.Modules.Languages.Controllers.Support {

    internal class MSTranslate {

        internal class MSAuthentication {

            public static readonly string DataMarketAccessUri = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            private string _clientKey;
            private string _token;

            public MSAuthentication(string clientKey) {
                _clientKey = clientKey;
                string request = string.Format("Subscription-Key={0}", YetaWFManager.UrlEncodeArgs(clientKey));
                _token = HttpPost(DataMarketAccessUri, request);
            }
            public string GetAccessToken() {
                return _token;
            }
            private string HttpPost(string dataMarketAccessUri, string requestDetails) {
                WebRequest webRequest = WebRequest.Create(dataMarketAccessUri + "?" + requestDetails);
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = webRequest.GetRequestStream()) {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = webRequest.GetResponse()) {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream())) {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        private string token;

        public MSTranslate(string clientId) {
            MSAuthentication auth = new MSAuthentication(clientId);
            token = auth.GetAccessToken();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Don't care")]
        public List<string> Translate(string from, string to, List<string> strings) {

            string headerValue = "Bearer " + token;
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
            foreach (string s in strings) {
                string encoded;
#if MVC6
                encoded = System.Net.WebUtility.HtmlEncode(s);
#else
                encoded = AntiXssEncoder.XmlEncode(s);
#endif
                hb.Append("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", encoded);
            }
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
