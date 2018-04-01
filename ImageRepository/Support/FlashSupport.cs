﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Support {

    public class FlashSupport : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        public readonly static string FlashType = "YetaWF_Flash";

        public Task InitializeApplicationStartupAsync() {
            YetaWF.Core.Image.ImageSupport.AddHandler(FlashType, GetAsFileAsync: RetrieveFlashAsync);
            return Task.CompletedTask;
        }

        private Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> RetrieveFlashAsync(string name, string location) {
            Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> fail = Task.FromResult(new Core.Image.ImageSupport.GetImageAsFileInfo());
            if (string.IsNullOrWhiteSpace(location)) return fail;
            if (string.IsNullOrWhiteSpace(name)) return fail;
            string[] parts = location.Split(new char[] { ',' });
            if (parts.Length != 3) return fail;
            string folderGuid = parts[0];
            string subFolder = parts[1];
            string fileType = parts[2];
            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), string.IsNullOrWhiteSpace(subFolder) ? "" : subFolder, fileType);
            return Task.FromResult(new Core.Image.ImageSupport.GetImageAsFileInfo {
                File = Path.Combine(storagePath, name),
                Success = true,
            });
        }
    }
}
