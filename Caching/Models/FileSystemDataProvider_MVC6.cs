/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

#if MVC6

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class FileSystemDataProvider : FileSystemDataProviderBase, IFileSystem {

        public FileSystemDataProvider(string rootFolder, bool Permanent) : base(rootFolder, Permanent) { }

        // Files

        public override async Task<List<string>> ReadAllLinesAsync(string filePath) {
            if (YetaWFManager.IsSync())
                return File.ReadLines(filePath).ToList();
            else
                return (await File.ReadAllLinesAsync(filePath)).ToList();
        }

        public override Task WriteAllLinesAsync(string filePath, List<string> lines) {
            if (YetaWFManager.IsSync()) {
                File.WriteAllLines(filePath, lines);
                return Task.CompletedTask;
            } else
                return File.WriteAllLinesAsync(filePath, lines);
        }

        public override Task<string> ReadAllTextAsync(string filePath) {
            if (YetaWFManager.IsSync()) {
                string text = File.ReadAllText(filePath);
                return Task.FromResult(text);
            } else
                return File.ReadAllTextAsync(filePath);
        }

        public override Task WriteAllTextAsync(string filePath, string text) {
            if (YetaWFManager.IsSync()) {
                File.WriteAllText(filePath, text);
                return Task.CompletedTask;
            } else
                return File.WriteAllTextAsync(filePath, text);
        }

        public override Task AppendAllTextAsync(string filePath, string text) {
            if (YetaWFManager.IsSync()) {
                File.AppendAllText(filePath, text);
                return Task.CompletedTask;
            } else
                return File.AppendAllTextAsync(filePath, text);
        }

        public override Task AppendAllLinesAsync(string filePath, List<string> lines) {
            if (YetaWFManager.IsSync()) {
                File.AppendAllLines(filePath, lines);
                return Task.CompletedTask;
            } else
                return File.AppendAllLinesAsync(filePath, lines);
        }

        public override Task<byte[]> ReadAllBytesAsync(string filePath) {
            if (YetaWFManager.IsSync()) {
                byte[] data = File.ReadAllBytes(filePath);
                return Task.FromResult(data);
            } else
                return File.ReadAllBytesAsync(filePath);
        }

        public override Task WriteAllBytesAsync(string filePath, byte[] data) {
            if (YetaWFManager.IsSync()) {
                File.WriteAllBytes(filePath, data);
                return Task.CompletedTask;
            } else
                return File.WriteAllBytesAsync(filePath, data);
        }
    }
}

#endif