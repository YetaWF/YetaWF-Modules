/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.IO;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Views.Shared;

namespace YetaWF.Modules.ImageRepository.Support {

    public class ImageSupport : IInitializeApplicationStartup {

        // IInitializeApplicationStartup
        // IInitializeApplicationStartup
        // IInitializeApplicationStartup

        public readonly static string ImageType = "YetaWF_Image";

        public void InitializeApplicationStartup() {
            YetaWF.Core.Image.ImageSupport.AddHandler(ImageType, GetAsFile: RetrieveImage);
        }

        private bool RetrieveImage(string name, string location, out string fileName) {
            fileName = null;
            if (string.IsNullOrWhiteSpace(location)) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            string[] parts = location.Split(new char[] { ',' });
            if (parts.Length != 3) return false;
            string folderGuid = parts[0];
            string subFolder = parts[1];
            string fileType = parts[2];

            string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
            fileName = Path.Combine(storagePath, name);
            return true;
        }
    }
}
