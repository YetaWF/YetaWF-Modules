/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;

namespace YetaWF.Modules.BootstrapCarousel.Support {

    public class ImageSupport : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        public const string ImageType = "YetaWF_BootstrapCarousel";

        public Task InitializeApplicationStartupAsync() {
            YetaWF.Core.Image.ImageSupport.AddHandler(ImageType, GetAsFileAsync: RetrieveImageAsync);
            return Task.CompletedTask;
        }
        private Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> RetrieveImageAsync(string name, string location) {
            Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> fail = Task.FromResult(new YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo());
            if (!string.IsNullOrWhiteSpace(location)) return fail;
            if (string.IsNullOrWhiteSpace(name)) return fail;
            string[] parts = name.Split(new char[] { ',' });
            if (parts.Length != 3) return fail;
            string folderGuid = parts[0];
            string propertyName = parts[1];
            string fileGuid = parts[2];
            string path = ModuleDefinition.GetModuleDataFolder(new Guid(folderGuid));
            return Task.FromResult(new YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo {
                File = Path.Combine(path, propertyName, fileGuid),
                Success = true,
            });
        }
    }
}
