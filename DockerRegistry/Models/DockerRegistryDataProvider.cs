/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;

namespace YetaWF.Modules.DockerRegistry.DataProvider {

    public class DockerRegistryEntry {

        public string RegistryName { get; set; }

        public DockerRegistryEntry() { }
    }

    public class DockerTagEntry {

        public string TagName { get; set; }
        public string Digest { get; set; }
        public int Size { get; set; }

        public DockerTagEntry() { }
    }

    public class DockerRegistryDataProvider : IDisposable {

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public DockerRegistryDataProvider() {
            DisposableTracker.AddObject(this);
        }
        public void Dispose() {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        // API
        // API
        // API

        public async Task<DataProviderGetRecords<DockerRegistryEntry>> GetRegistriesAsync(string registryURL, string userName, string userPassword, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            string data;
            using (HttpClient client = new HttpClient()) {
                AddAuthentication(client, userName, userPassword);
                string url = $"{registryURL}/v2/_catalog?n=9999";
                if (YetaWFManager.IsSync()) {
                    using (HttpResponseMessage resp = client.GetAsync(url).Result) {
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errRegs", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                        data = resp.Content.ReadAsStringAsync().Result;
                    }
                } else {
                    using (HttpResponseMessage resp = await client.GetAsync(url)) {
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errRegs", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                        data = await resp.Content.ReadAsStringAsync();
                    }
                }
            }
            RegistryRepositories recs;
            try {
                recs = Utility.JsonDeserialize<RegistryRepositories>(data);
            } catch (Exception exc) {
                throw new Error(this.__ResStr("errRegsData", "The registry data retrieved from {0} is invalid - {1}", registryURL, ErrorHandling.FormatExceptionMessage(exc)));
            }
            return DataProviderImpl<DockerRegistryEntry>.GetRecords((from r in recs.Repositories select (object)new DockerRegistryEntry { RegistryName = r }).ToList(), 0, 0, sort, filters);
        }

        public async Task<DataProviderGetRecords<DockerTagEntry>> GetTagsAsync(string registryURL, string registry, string userName, string userPassword, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            string data;
            using (HttpClient client = new HttpClient()) {

                // retrieve all tags
                AddAuthentication(client, userName, userPassword);
                string url = $"{registryURL}/v2/{registry}/tags/list";
                if (YetaWFManager.IsSync()) {
                    using (HttpResponseMessage resp = client.GetAsync(url).Result) {
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errTags", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                        data = resp.Content.ReadAsStringAsync().Result;
                    }
                } else {
                    using (HttpResponseMessage resp = client.GetAsync(url).Result) {
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errTags", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                        data = await resp.Content.ReadAsStringAsync();
                    }
                }
                RegistryTags recsTag;
                try {
                    recsTag = Utility.JsonDeserialize<RegistryTags>(data);
                } catch (Exception exc) {
                    throw new Error(this.__ResStr("errTagsData", "The tag data retrieved from {0} is invalid - {1}", registryURL, ErrorHandling.FormatExceptionMessage(exc)));
                }
                List<DockerTagEntry> tags = new List<DockerTagEntry>();
                if (recsTag.Tags != null)
                    tags = (from r in recsTag.Tags select new DockerTagEntry { TagName = r }).ToList();

                // retrieve details for each tag
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.docker.distribution.manifest.v2+json");// to get digest - https://github.com/docker/distribution/issues/1565
                foreach (DockerTagEntry tag in tags) {
                    url = $"{registryURL}/v2/{registry}/manifests/{tag.TagName}";
                    string digest = this.__ResStr("unknown", "(unknown)");
                    HttpResponseMessage resp = null;
                    try {
                        try {
                            if (YetaWFManager.IsSync()) {
                                resp = client.GetAsync(url).Result;
                            } else {
                                resp = await client.GetAsync(url);
                            }
                        } catch (Exception exc) {
                            throw new Error(this.__ResStr("errManifest", "An error occurred retrieving tag data from {0} for {1}:{2} - {3}", registryURL, registry, tag.TagName, ErrorHandling.FormatExceptionMessage(exc)));
                        }
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errManifestStatus", "An error occured accessing {0} for {1}:{2} - Error {1}", registryURL, registry, tag.TagName, resp.StatusCode));

                        try {
                            if (YetaWFManager.IsSync()) {
                                data = resp.Content.ReadAsStringAsync().Result;
                            } else {
                                data = await resp.Content.ReadAsStringAsync();
                            }
                        } catch (Exception exc) {
                            throw new Error(this.__ResStr("errManifest", "An error occurred retrieving tag data from {0} for {1}:{2} - {3}", registryURL, registry, tag.TagName, ErrorHandling.FormatExceptionMessage(exc)));
                        }
                        List<string> vals = resp.Headers.GetValues("Docker-Content-Digest").ToList();
                        if (vals.Count == 1)
                            digest = vals[0];
                    } catch (Exception) {
                        throw;
                    } finally {
                        if (resp != null)
                            resp.Dispose();
                    }

                    RegistryTag recTag;
                    try {
                        recTag = Utility.JsonDeserialize<RegistryTag>(data);
                    } catch (Exception exc) {
                        throw new Error(this.__ResStr("errTagsDetails", "The detailed tag data retrieved from {0} for {1}:{2} is invalid - {3}", registryURL, registry, tag.TagName, ErrorHandling.FormatExceptionMessage(exc)));
                    }
                    tag.Digest = digest;
                    tag.Size = recTag.Config.Size + recTag.Layers.Sum((r) => r.Size);
                }
                return DataProviderImpl<DockerTagEntry>.GetRecords((from t in tags select (object)t).ToList(), 0, 0, sort, filters);
            }
        }

        public async Task RemoveTagAsync(string registryURL, string registry, string reference, string userName, string userPassword) {
            using (HttpClient client = new HttpClient()) {
                AddAuthentication(client, userName, userPassword);
                string url = $"{registryURL}/v2/{registry}/manifests/{reference}";
                HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Delete, url);
                if (YetaWFManager.IsSync()) {
                    using (HttpResponseMessage resp = client.SendAsync(reqMsg).Result) {
                    if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errRemove", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                    }
                } else {
                    using (HttpResponseMessage resp = await client.SendAsync(reqMsg)) {
                        if (!resp.IsSuccessStatusCode)
                            throw new Error(this.__ResStr("errRemove", "An error occured accessing {0} - Error {1}", registryURL, resp.StatusCode));
                    }
                }
            }
        }

        public class RegistryRepositories {
            public List<string> Repositories { get; set; }
        }
        public class RegistryTags {
            public string Name { get; set; }
            public List<string> Tags { get; set; }
        }
        public class RegistryTag {
            public class ConfigClass {
                public int Size { get; set; }
                public string Digest { get; set; }
            }
            public class LayerClass {
                public int Size { get; set; }
                public string Digest { get; set; }
            }
            public ConfigClass  Config { get; set; }
            public List<LayerClass> Layers { get; set; }
        }


        private void AddAuthentication(HttpClient client, string userName, string userPassword) {
            if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(userPassword))
                return;
            byte[] byteArray = Encoding.ASCII.GetBytes($"{userName}:{userPassword}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}
