/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.CurrencyConverter.DataProvider {

    public class ExchangeRateEntry {
        public const int MaxCurrencyName = 50;
        public const int MaxCode = 10;
        [StringLength(MaxCurrencyName)]
        public string CurrencyName { get; set; }
        [StringLength(MaxCode)]
        public string Code { get; set; }
        public decimal Rate { get; set; }
    }

    public class ExchangeRateData {
        [Data_PrimaryKey]
        public int Key { get; set; }

        public DateTime SaveTime { get; set; }
        public SerializableList<ExchangeRateEntry> Rates { get; set; }

        public ExchangeRateData() {
            Rates = new SerializableList<ExchangeRateEntry>();
        }
    }

    public class ExchangeRateDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public const int KEY = 1000;
        public const string JSFile = "ExchangeRates.js";

        public ExchangeRateDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ExchangeRateData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ExchangeRateData> CreateDataProvider() {
            Package package = YetaWF.Modules.CurrencyConverter.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Data", Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<ExchangeRateData> GetItemAsync() {

            ConfigData config = await ConfigDataProvider.GetConfigAsync();

            string jsFileName = GetJSFileName();
            using (ILockObject lockObject = await FileSystem.FileSystemProvider.LockResourceAsync(jsFileName)) {
                ExchangeRateData data = await DataProvider.GetAsync(KEY);
                if (data != null && data.SaveTime.Add(config.RefreshInterval) < DateTime.UtcNow)
                    data = null;
                if (data != null && !await FileSystem.FileSystemProvider.FileExistsAsync(jsFileName))
                    data = null;
                if (data == null)
                    data = await GetExchangeRatesAsync();
                await lockObject.UnlockAsync();
                return data;
            }
        }
        private async Task<ExchangeRateData> GetExchangeRatesAsync() {

            ConfigData config = await ConfigDataProvider.GetConfigAsync();
            if (string.IsNullOrWhiteSpace(config.AppID))
                throw new InternalError("The App ID has not been specified in the Currency Converter Settings (see Admin > Configuration > Currency Converter Settings) - openexchangerates.org requires an app id to be able to retrieve currency exchange rates");

            ExchangeRateData data = new ExchangeRateData();
            data.Key = KEY;
            data.SaveTime = DateTime.UtcNow;

            string url = string.Format("{0}://openexchangerates.org/api/latest.json?app_id={1}", config.UseHttps ? "https" : "http", config.AppID);
            string json = await GetJSONResponseAsync(url);
            CheckForErrors(json);

            url = string.Format("{0}://openexchangerates.org/api/currencies.json?app_id={1}", config.UseHttps ? "https" : "http", config.AppID);
            string jsonCurrencies = await GetJSONResponseAsync(url);
            CheckForErrors(jsonCurrencies);

            // get all currencies
            Dictionary<string, object> currencies = Utility.JsonDeserialize<Dictionary<string, object>>(jsonCurrencies);
            // add all rates
            dynamic jsonObject = Utility.JsonDeserialize(json);
            var rates = jsonObject.rates;
            foreach (var rate in rates) {
                string code = rate.Name;
                object currency;
                if (!currencies.TryGetValue(code, out currency))// replace 3 digit codes by actual name
                    currency = code;
                decimal val = (decimal)rate.Value;
                data.Rates.Add(new ExchangeRateEntry { Code = code, CurrencyName = (string)currency, Rate = val });
            }
            // Save new rates
            UpdateStatusEnum status = await DataProvider.UpdateAsync(KEY, KEY, data);
            if (status != UpdateStatusEnum.OK) {
                if (status != UpdateStatusEnum.RecordDeleted)
                    throw new InternalError("Unexpected status {0}", status);
                if (!await DataProvider.AddAsync(data))
                    throw new InternalError("Unexpected error adding data");
            }
            // Create a javascript file with rates so we can include it in a page
            await SaveRatesJSAsync(data);
            return data;
        }

        private async Task SaveRatesJSAsync(ExchangeRateData data) {
            string file = GetJSFileName();
            ScriptBuilder sb = new ScriptBuilder();
            sb.Append("// Generated file (see ExchangeRateDataProvider) - Do not modify\n");
            sb.Append("YetaWF_CurrencyConverter_Rates = \n");
            sb.Append(Utility.JsonSerialize(data.Rates));
            sb.Append(";\n");
            await FileSystem.FileSystemProvider.WriteAllTextAsync(file, sb.ToString());
        }

        private static string GetJSFileName() {
            string url = VersionManager.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
            string path = Utility.UrlToPhysical(url);
            return Path.Combine(path, JSFile);
        }

        private void CheckForErrors(string json) {
            dynamic jsonObject = Utility.JsonDeserialize(json);
            if (!string.IsNullOrWhiteSpace(jsonObject.error))
                throw new InternalError("An error occurred retrieving exchange rates from openexchangerates.org - {0}: {1}", jsonObject["message"], jsonObject["description"]);
        }
        private async Task<string> GetJSONResponseAsync(string url) {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            System.Net.WebResponse resp;
            try {
                if (YetaWFManager.IsSync())
                    resp = http.GetResponse();
                else
                    resp = await http.GetResponseAsync();
            } catch (Exception exc) {
                throw new InternalError("An error occurred retrieving exchange rates from openexchangerates.org - {0}", ErrorHandling.FormatExceptionMessage(exc));
            }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream())) {
                if (YetaWFManager.IsSync())
                    return sr.ReadToEnd().Trim();
                else
                    return (await sr.ReadToEndAsync()).Trim();
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) { return Task.FromResult(new DataProviderExportChunk { More = false, ObjectList = null }); }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) { return Task.CompletedTask; }
        public new Task LocalizeModelAsync(string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) { return Task.CompletedTask; }
    }
}
