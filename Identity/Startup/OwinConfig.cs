/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using System.Threading.Tasks;

namespace YetaWF.Core.Support {

    /// <summary>
    /// Manages Login Provider settings.
    /// </summary>
    /// <remarks>This class is used exclusively to manage Identity settings.
    ///
    /// It retrieves values and supports saving new values.
    ///
    /// For retrieval, variables embedded in the values are substituted.
    ///
    /// By default, settings for Login Providers are stored in AppSettings.json Application:P:YetaWF_Identity.
    /// This can be redirected to another file by using Application:P:YetaWF_Identity:LoginProviderSettings to define another file for just Login Provider settings.
    /// </remarks>
    public static class OwinConfigHelper {

        private static WebConfigBaseHelper helper = null;

        public static Task InitAsync(string settingsFile) {
            if (settingsFile != null) {
                helper = new WebConfigBaseHelper();
                return helper.InitAsync(settingsFile);
            } else {
                return Task.CompletedTask;
            }
        }

        public static TYPE GetValue<TYPE>(string areaName, string key, TYPE dflt = default(TYPE), bool Package = true, bool Required = false) {
            if (helper != null)
                return helper.GetValue<TYPE>(areaName, key, dflt, Package, Required);
            else
                return WebConfigHelper.GetValue(areaName, key, dflt, Package, Required);
        }

        public static void SetValue<TYPE>(string areaName, string key, TYPE value, bool Package = true) {
            if (helper != null)
                helper.SetValue<TYPE>(areaName, key, value, Package);
            else
                WebConfigHelper.SetValue<TYPE>(areaName, key, value, Package);
        }

        public static Task SaveAsync() {
            if (helper != null)
                return helper.SaveAsync();
            else
                return WebConfigHelper.SaveAsync();
        }
    }
}
