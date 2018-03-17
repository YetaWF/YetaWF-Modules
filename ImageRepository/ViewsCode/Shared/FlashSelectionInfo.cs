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

    public class FlashSelectionInfo {

        public static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(FlashSelectionInfo), name, defaultValue, parms); }

        public FlashSelectionInfo(ModuleDefinition owningModule, Guid folderGuid, string subFolder, string fileType = "Flash") {
            FolderGuid = folderGuid;
            SubFolder = subFolder;
            FileType = fileType;
            AllowUpload = false;
            PreviewWidth = 200;
            PreviewHeight = 200;
            ClearFlashButton = new ModuleAction(owningModule) {
                Url = YetaWFManager.UrlFor(typeof(FlashSelectionHelperController), "GetFiles"),
                QueryArgs = new { FolderGuid = folderGuid, SubFolder = subFolder, FileType = FileType },
                Image = "ClearImage.png",
                LinkText = __ResStr("clearImage", "Clear"),
                Tooltip = __ResStr("clearImageTT", "Clears the currently selected Flash image (the Flash image itself is NOT removed from the server)"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Clear"
            };
            RemoveFlashButton = new ModuleAction(owningModule) {
                Url = YetaWFManager.UrlFor(typeof(FlashSelectionHelperController), "RemoveSelectedFlashImage"),
                QueryArgs = new { FolderGuid = folderGuid, SubFolder = subFolder, FileType = FileType, Name = "" },
                Image = "#Remove",
                LinkText = __ResStr("removeImage", "Remove"),
                Tooltip = __ResStr("removeImageTT", "Removes the currently selected Flash image from the server"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Remove"
            };
        }

        /// <summary>
        /// The module (guid) for which we're storing Flash images
        /// </summary>
        public Guid FolderGuid { get; set; }
        /// <summary>
        /// Optional subfolder
        /// </summary>
        public string SubFolder { get; set; }
        /// <summary>
        /// File type (defaults to "Flash")
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Whether uploads are allowed
        /// </summary>
        public bool AllowUpload { get; set; }
        /// <summary>
        /// Number of visible entries in list
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Preview width
        /// </summary>
        public int PreviewWidth { get; set; }
        /// <summary>
        /// Preview height
        /// </summary>
        public int PreviewHeight { get; set; }

        public ModuleAction ClearFlashButton { get; set; }
        public ModuleAction RemoveFlashButton { get; set; }
        public ModuleDefinition OwningModule { get; set; }

        public static string StoragePath(Guid folderGuid, string subFolder, string fileType = "Flash") {
            string path = ModuleDefinition.GetModuleDataFolder(folderGuid);
            if (!string.IsNullOrWhiteSpace(subFolder))
                path = Path.Combine(path, subFolder);
            return Path.Combine(path, fileType);
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
            return (from f in files select Path.GetFileName(f) + YetaWF.Core.Image.ImageSupport.ImageSeparator + (File.GetLastWriteTime(f).Ticks / TimeSpan.TicksPerSecond).ToString()).ToList();
        }
        private List<string> ReadFiles() {
            return FlashSelectionInfo.ReadFiles(FolderGuid, SubFolder, FileType);
        }
        private static List<string> ReadFilePaths(Guid folderGuid, string subFolder, string fileType) {
            List<string> files = new List<string>();
            string storagePath = FlashSelectionInfo.StoragePath(folderGuid, subFolder, fileType);
            if (Directory.Exists(storagePath))
                files = Directory.GetFiles(storagePath).ToList();
            return files;
        }
        public string MakeFlashUrl(string filename) {
            return ImageHelper.FormatUrl(FlashSupport.FlashType, string.Format("{0},{1},{2}", FolderGuid.ToString(), SubFolder, FileType), filename);
        }
    }
}