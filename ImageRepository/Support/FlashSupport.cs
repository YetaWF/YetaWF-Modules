﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Support {

    public class FlashSupport : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        public readonly static string FlashType = "YetaWF_Flash";

        public void InitializeApplicationStartup() {
            YetaWF.Core.Image.ImageSupport.AddHandler(FlashType, GetAsFile: RetrieveFlash);
        }

        private bool RetrieveFlash(string name, string location, out string fileName) {
            fileName = null;
            if (string.IsNullOrWhiteSpace(location)) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            string[] parts = location.Split(new char[] { ',' });
            if (parts.Length != 3) return false;
            string folderGuid = parts[0];
            string subFolder = parts[1];
            string fileType = parts[2];

            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), string.IsNullOrWhiteSpace(subFolder) ? "" : subFolder, fileType);
            fileName = Path.Combine(storagePath, name);
            return true;
        }
    }
}
