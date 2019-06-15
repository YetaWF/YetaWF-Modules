/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Controllers;
using YetaWF.Modules.ImageRepository.Support;

namespace YetaWF.Modules.ImageRepository.Components {

    public class FlashSelectionInfo {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(FlashSelectionInfo), name, defaultValue, parms); }

        public FlashSelectionInfo(ModuleDefinition owningModule, Guid folderGuid, string subFolder, string fileType = "Flash") {
            OwningModule = owningModule;
            FolderGuid = folderGuid;
            SubFolder = subFolder;
            FileType = fileType;
            AllowUpload = false;
            PreviewWidth = 200;
            PreviewHeight = 200;

            // the upload control
            FileUpload1 = new FileUpload1() {
                SaveURL = Utility.UrlFor(typeof(FlashSelectionController), nameof(FlashSelectionController.SaveFlashImage),
                    new { FolderGuid = FolderGuid, SubFolder = SubFolder, FileType = FileType }),
            };
        }
        public async Task InitAsync() {
            ClearImageButton = new ModuleAction(OwningModule) {
                QueryArgs = new { FolderGuid = FolderGuid, SubFolder = SubFolder, FileType = FileType },
                Image = await new SkinImages().FindIcon_PackageAsync("ClearImage.png", Package.GetCurrentPackage(OwningModule)),
                LinkText = __ResStr("clearImage", "Clear"),
                Tooltip = __ResStr("clearImageTT", "Clears the currently selected image (the image itself is NOT removed from the server)"),
                Style = ModuleAction.ActionStyleEnum.Nothing,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                NeedsModuleContext = true,
                Name = "Clear"
            };
            RemoveImageButton = new ModuleAction(OwningModule) {
                Url = Utility.UrlFor(typeof(FlashSelectionController), nameof(FlashSelectionController.RemoveSelectedFlashImage)),
                QueryArgs = new { FolderGuid = FolderGuid, SubFolder = SubFolder, FileType = FileType, Name = "" },
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
        [UIHint("FileUpload1")]
        public FileUpload1 FileUpload1 { get; set; }

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

        [UIHint("ModuleAction")]
        public ModuleAction ClearImageButton { get; set; }
        [UIHint("ModuleAction")]
        public ModuleAction RemoveImageButton { get; set; }
        public ModuleDefinition OwningModule { get; set; }

        public string MakeFlashUrl(string filename, int width = 0, int height = 0) {
            // always defeat browser caching for image selection
            return ImageHTML.FormatUrl(ImageSupport.ImageType, string.Format("{0},{1},{2}", FolderGuid.ToString(), SubFolder, FileType), filename, width, height, CacheBuster: DateTime.UtcNow.Ticks.ToString());
        }
        public async Task<List<string>> GetFilesAsync() {
            if (_files == null)
                _files = await ImageSelectionInfo.ReadFilesAsync(FolderGuid, SubFolder, FileType);
            return _files;
        }
        List<string> _files;
    }
}