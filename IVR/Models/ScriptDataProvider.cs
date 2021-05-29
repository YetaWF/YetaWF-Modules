/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider {

    public class ScriptParm {
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
    public class ScriptEntry {
        public string Tag { get; set; } = null!;
        public List<ScriptParm> Parms { get; set; }
        public string Text { get; set; } = null!;

        public ScriptEntry() {
            Parms = new List<ScriptParm>();
        }
    }
    public class ExtensionNumber {
        public string Number { get; set; } = null!;
        public bool SendSMSVoiceMail { get; set; }
    }


    public class Extension {
        public string Digits { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<ExtensionNumber> Numbers { get; set; }
        public Extension() {
            Numbers = new List<ExtensionNumber>();
        }
    }

    public class ScriptData {

        public List<Extension> Extensions { get; set; }
        public List<ScriptEntry> Scripts { get; set; }

        public ScriptData() {
            Scripts = new List<ScriptEntry>();
            Extensions = new List<Extension>();
        }

        public Extension? FindExtension(string extension) {
            return (from e in Extensions where e.Digits == extension select e).FirstOrDefault();
        }
    }

    public class ScriptDataProvider : IDisposable {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ScriptDataProvider() {
            DisposableTracker.AddObject(this);
        }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { if (disposing) DisposableTracker.RemoveObject(this); }

        // cached twiml scripts
        private static Dictionary<string, ScriptData> Scripts = new Dictionary<string, ScriptData>();

        // API
        // API
        // API

        public void ClearScript() {
            Scripts = new Dictionary<string, ScriptData>();
        }

        public async Task<ScriptData?> GetScriptAsync(string phoneNumber) {
            if (!Scripts.TryGetValue(phoneNumber, out ScriptData? script)) {
                script = await ReadScriptAsync(phoneNumber);
                if (script != null) {
                    try {
                        Scripts.Add(phoneNumber, script);
                    } catch (Exception) { }
                }
            }
            return script;
        }

        private async Task<ScriptData?> ReadScriptAsync(string phoneNumber) {

            // find the file
            string addonUrl = Package.GetAddOnPackageUrl(Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage.AreaName);
            string scriptPath = Path.Combine(Utility.UrlToPhysical(Package.GetCustomUrlFromUrl(addonUrl)), "Scripts", $"TWIML{phoneNumber}.txt");
            Logging.AddLog($"Trying script at {scriptPath}");
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(scriptPath)) {
                Logging.AddLog($"Script at {scriptPath} not found");
                addonUrl = Package.GetAddOnPackageUrl(Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage.AreaName);
                scriptPath = Path.Combine(Utility.UrlToPhysical(addonUrl), "Scripts", $"TWIML{phoneNumber}.txt");
                Logging.AddLog($"Trying script at {scriptPath}");
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(scriptPath)) {
                    Logging.AddLog($"Script at {scriptPath} not found");
                    return null;
                }
            }

            ScriptData script = new ScriptData();

            using (ExtensionEntryDataProvider extensionDP = new ExtensionEntryDataProvider()) {
                DataProviderGetRecords<ExtensionEntry> data = await extensionDP.GetItemsAsync(0, 0, null, null);
                foreach (ExtensionEntry extension in data.Data) {
                    Extension ext = new Extension {
                        Digits = extension.Extension,
                        Name = extension.Description ?? string.Empty,
                        Numbers = new List<ExtensionNumber>(),
                    };
                    foreach (ExtensionPhoneNumber extPhone in extension.PhoneNumbers) {
                        ext.Numbers.Add(new ExtensionNumber {
                            Number = extPhone.PhoneNumber,
                            SendSMSVoiceMail = extPhone.SendSMS,
                        });
                    }
                    script.Extensions.Add(ext);
                }
            }

            // load the contents
            List<string> lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(scriptPath);
            int total = lines.Count;

            // TWIML Snippets
            int lineIx = 0;
            for ( ; lineIx < total; ++lineIx) {

                string l = lines[lineIx].Trim();
                if (l.Length > 0 && !l.StartsWith("##")) {

                    // tag and parms
                    List<ScriptEntry> entries = new List<ScriptEntry>();
                    for ( ; ; ) {
                        if (l.StartsWith(" ")) {
                            if (entries.Count > 0)
                                break;
                            throw new InternalError($"Tag name expected on line {lineIx + 1} in {scriptPath}");
                        }
                        ScriptEntry entry = new ScriptEntry();
                        string[] tokens = l.Split(new char[] { ' ' });
                        if (tokens.Length < 1 || (tokens.Length % 2) != 1)
                            throw new InternalError($"Unexpected number of arguments on line {lineIx + 1} in {scriptPath}");
                        entry.Tag = tokens[0].ToLower();
                        for (int tokIx = 1; tokIx < tokens.Length; tokIx += 2) {
                            entry.Parms.Add(new ScriptParm {
                                Name = tokens[tokIx],
                                Value = tokens[tokIx + 1],
                            });
                        }
                        ++lineIx;
                        l = lines[lineIx];
                        entries.Add(entry);
                    }

                    // text
                    string text = "";
                    for ( ; lineIx < total; ++lineIx) {
                        l = lines[lineIx];
                        if (l.Length == 0)
                            break;
                        if (!l.StartsWith(" "))
                            throw new InternalError($"Unexpected content (not indented) on line {lineIx + 1} in {scriptPath}");
                        l = l.Trim();
                        text += l + "\r\n";
                    }
                    foreach (ScriptEntry entry in entries)
                        entry.Text = text;

                    script.Scripts.AddRange(entries);
                }
            }
            return script;
        }
    }
}
