/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.Controllers.Support {

    internal class MSTranslate {

        public MSTranslate(string? textTranslationUrl, string? region, string clientKey, int reqestLimit) {
            TextTranslationUrl = string.IsNullOrWhiteSpace(textTranslationUrl) ? TextTranslationUrlDefault : textTranslationUrl;
            TextTranslationRegion = region;
            ClientKey = clientKey;
            RequestLimit = reqestLimit;
        }

        private const string TextTranslationUrlDefault = "https://api.cognitive.microsofttranslator.com";

        private string TextTranslationUrl { get; }
        private string? TextTranslationRegion { get; }
        private string ClientKey { get; }
        
        private int RequestLimit { get; }

        private static HttpClient Client = new HttpClient();

        public async Task<List<string>> TranslateAsync(string from, string to, List<string> strings) {

            if (strings.Count == 0) return new List<string>();

            DateTime start = DateTime.UtcNow;

            ScriptBuilder builder = new ScriptBuilder();
            builder.Append("[");
            foreach (string s in strings) {
                builder.Append($@"{{ ""Text"": ""{Utility.JserEncode(s)}"" }},");
            }
            builder.Length = builder.Length - 1;// remove last comma
            builder.Append("]");

            using (HttpRequestMessage request = new HttpRequestMessage()) {
                request.Method = HttpMethod.Post;
                string route = $"/translate?api-version=3.0&from={from}&to={to}";
                request.RequestUri = new Uri(TextTranslationUrl + route);
                request.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", ClientKey);
                if (!string.IsNullOrWhiteSpace(TextTranslationRegion))
                    request.Headers.Add("Ocp-Apim-Subscription-Region", TextTranslationRegion);

                HttpResponseMessage response = await Client.SendAsync(request);
                if (!response.IsSuccessStatusCode) 
                    throw new InternalError($"{nameof(TranslateAsync)} failed - {response.StatusCode}");
                string result = await response.Content.ReadAsStringAsync();
                List<AllTranslations> trans = Utility.JsonDeserialize<List<AllTranslations>>(result);
                List<string> newStrings = (from t in trans select (from te in t.Translations select te.Text).First()).ToList();

                //int requestLength = (from s in strings select s.Length).Sum();
                int requestLength = builder.Length;
                await RequestLimitDelay(requestLength, DateTime.UtcNow.Subtract(start));
                    
                return newStrings;
            }
        }

        private async Task RequestLimitDelay(int requestLength, TimeSpan elapsedTime) {
            if (RequestLimit <= 0) return;
            // This is a naive implementation so the request limit is not exceeded.
            // Given the just processed request, we calculate how long it took versus how much it should have taken given the request limit. 
            // If the request was processed "too fast", we wait for a bit so we can never exceed the request limit.
            double requestLimit = RequestLimit * 0.9; // -10%, just in case
            double maxCharsPerHour = requestLimit * 1000000;// million of characters allowed per hour.
            double maxCharsPerMillisecond = maxCharsPerHour / 60 / 60 / 1000;
            double allowedCharsForRequest = elapsedTime.TotalMilliseconds * maxCharsPerMillisecond;
            if (allowedCharsForRequest >= requestLength) return;// took longer than our request limit permits, so we're not over the request limit
            
            double tooManyChars = requestLength - allowedCharsForRequest;
            double exceededMilliseconds = tooManyChars * maxCharsPerMillisecond;
            double exceededSeconds = (exceededMilliseconds + 999) / 1000;
            // There seems to be an additional undocumented request limit (probably related to many consecutive sub-second requests)
            // which causes a 429 TooManyRequests error. So we round the wait time up to the next second. This seems to work (for now).
            await Task.Delay(((int)exceededSeconds+1)*1000);
        }

        public class AllTranslations {
            [JsonProperty("translations")]
            public List<TranslationEntry> Translations { get; set; }
            public AllTranslations() {
                Translations = new List<TranslationEntry>();
            }
        }
        public class TranslationEntry {
            [JsonProperty("text")]
            public string Text { get; set; } = null!;
        }
    }
}

