/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.ImageRepository.Controllers.Shared;
using YetaWF.Modules.ImageRepository.Support;

namespace YetaWF.Modules.ImageRepository.Views.Shared {

    public class ImageSelectionInfo {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ImageSelectionInfo), name, defaultValue, parms); }

        public ImageSelectionInfo(ModuleDefinition owningModule, Guid folderGuid, string subFolder, string fileType = "Images") {
            FolderGuid = folderGuid;
            SubFolder = subFolder;
            FileType = fileType;
            AllowUpload = false;
            PreviewWidth = 200;
            PreviewHeight = 200;
            ClearImageButton = new ModuleAction(owningModule) {
                Url = YetaWFManager.UrlFor(typeof(ImageSelectionHelperController), "GetFiles"),
                QueryArgs = new { FolderGuid = folderGuid, SubFolder = subFolder, FileType = FileType },
                Image = "ClearImage.png",
                LinkText = __ResStr("clearImage", "Clear"),
                Tooltip = __ResStr("clearImageTT", "Clears the currently selected image (the image itself is NOT removed from the server)"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Clear"
            };
            RemoveImageButton = new ModuleAction(owningModule) {
                Url = YetaWFManager.UrlFor(typeof(ImageSelectionHelperController), "RemoveSelectedImage"),
                QueryArgs = new { FolderGuid = folderGuid, SubFolder = subFolder, FileType = FileType, Name = "" },
                Image = "#Remove",
                LinkText = __ResStr("removeImage", "Remove"),
                Tooltip = __ResStr("removeImageTT", "Removes the currently selected image from the server"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Remove"
            };
        }

        /// <summary>
        /// The module (Guid) for which we're storing images
        /// </summary>
        public Guid FolderGuid { get; set; }
        /// <summary>
        /// Optional subfolder, can be null
        /// </summary>
        public string SubFolder { get; set; }
        /// <summary>
        /// File type (defaults to "Images")
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Whether uploads are allowed
        /// </summary>
        public bool AllowUpload { get; set; }
        /// <summary>
        /// Number of visible entries in selection list
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Preview width in pixels
        /// </summary>
        public int PreviewWidth { get; set; }
        /// <summary>
        /// Preview height in pixels
        /// </summary>
        public int PreviewHeight { get; set; }

        public ModuleAction ClearImageButton { get; set; }
        public ModuleAction RemoveImageButton { get; set; }
        public ModuleDefinition OwningModule { get; set; }

        public static string StoragePath(Guid folderGuid, string subFolder, string fileType = "Images") {
            string path = ModuleDefinition.GetModuleDataFolder(folderGuid);
            if (!string.IsNullOrWhiteSpace(subFolder))
                path = Path.Combine(path, subFolder);
            return Path.Combine(path, fileType);
        }
        // remove folder, then move up and remove all empty parent folders
        public static void RemoveStorage(Guid folderGuid, string subFolder, string fileType = "Images") {
            string path = StoragePath(folderGuid, subFolder, fileType);
            try {
                Directory.Delete(path, true);
            } catch (Exception) {
                return;
            }
            for ( ; ; ) {
                path = Path.GetDirectoryName(path);
                try {
                    Directory.Delete(path, false);
                } catch (Exception) {
                    return;
                }
            }
        }

        public List<string> Files {
            get {
                if (_files == null)
                    _files = ReadFiles();
                return _files;
            }
        }
        List<string> _files;

        public static List<string> ReadFiles(Guid folderGuid, string subFolder, string fileType) {
            List<string> files = ReadFilePaths(folderGuid, subFolder, fileType);
            return (from f in files select Path.GetFileName(f) + YetaWF.Core.Image.ImageSupport.ImageSeparator + (File.GetLastWriteTime(f).Ticks/TimeSpan.TicksPerSecond).ToString() ).ToList();
        }
        private List<string> ReadFiles() {
            return ImageSelectionInfo.ReadFiles(FolderGuid, SubFolder, FileType);
        }
        private static List<string> ReadFilePaths(Guid folderGuid, string subFolder, string fileType) {
            List<string> files = new List<string>();
            string storagePath = ImageSelectionInfo.StoragePath(folderGuid, subFolder, fileType);
            if (Directory.Exists(storagePath))
                files = Directory.GetFiles(storagePath).ToList();
            return files;
        }
        public string MakeImageUrl(string filename, int width = 0, int height = 0) {
            return ImageHelper.FormatUrl(ImageSupport.ImageType, string.Format("{0},{1},{2}", FolderGuid.ToString(), SubFolder, FileType), filename, width, height);
        }
    }
}