/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using Google.Cloud.Translation.V2;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YetaWF.Modules.Languages.Support
{

    internal class GoogleTranslate
    {

        public GoogleTranslate(string appName, string appKey)
        {
            AppName = appName;
            AppKey = appKey;
        }

        private string AppName { get; }
        private string? AppKey { get; }

        public async Task<List<string>> TranslateAsync(string from, string to, List<string> strings)
        {

            if (strings.Count == 0) return new List<string>();

            using (TranslationClient client = TranslationClient.CreateFromApiKey(AppKey))
            {
                IList<TranslationResult> results = await client.TranslateTextAsync(strings.ToArray(), to);
                List<string> newStrings = (from t in results select t.TranslatedText).ToList();
                return newStrings;
            }
        }
    }
}

