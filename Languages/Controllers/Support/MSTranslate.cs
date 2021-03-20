/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Controllers.Support {

    internal class MSTranslate {

        internal class MSAuthentication {

            public static readonly string DataMarketAccessUri = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            private string _clientKey;
            private string? _token;

            public MSAuthentication(string clientKey) {
                _clientKey = clientKey;
            }
            public async Task<string> GetAccessTokenAsync() {
                if (_token == null) {
                    string request = string.Format("Subscription-Key={0}", Utility.UrlEncodeArgs(_clientKey));
                    _token = await HttpPostAsync(DataMarketAccessUri, request);
                }
                return _token;
            }
            private async Task<string> HttpPostAsync(string dataMarketAccessUri, string requestDetails) {
                WebRequest webRequest = WebRequest.Create(dataMarketAccessUri + "?" + requestDetails);
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = await webRequest.GetRequestStreamAsync()) {
                    await outputStream.WriteAsync(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = await webRequest.GetResponseAsync()) {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream())) {
                        return await sr.ReadToEndAsync();
                    }
                }
            }
        }

        public MSTranslate(string clientId) {
            _clientId = clientId;
        }

        private string _clientId;
        private string? _token;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Don't care")]
        public async Task<List<string>> TranslateAsync(string from, string to, List<string> strings) {

            if (strings.Count == 0) return new List<string>();

            if (_token == null) {
                MSAuthentication auth = new MSAuthentication(_clientId);
                _token = await auth.GetAccessTokenAsync();
            }

            string headerValue = "Bearer " + _token;
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append("<TranslateArrayRequest>" +
                             "<AppId />" +
                             "<From>{0}</From>" +
                             "<Options>" +
                                 "<Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
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

            using (System.IO.Stream stream = await request.GetRequestStreamAsync()) {
                byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(hb.ToString());
                await stream.WriteAsync(arrBytes, 0, arrBytes.Length);
            }

            // Get the response
            List<string> newStrings = new List<string>();
            WebResponse? response = null;

            using (response = await request.GetResponseAsync()) {
                using (Stream stream = response.GetResponseStream()) {
                    using (StreamReader rdr = new StreamReader(stream, System.Text.Encoding.UTF8)) {
                        string strResponse = await rdr.ReadToEndAsync();
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
