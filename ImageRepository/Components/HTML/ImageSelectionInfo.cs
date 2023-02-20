/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Endpoints;
using YetaWF.Modules.ImageRepository.Support;

namespace YetaWF.Modules.ImageRepository.Components;

public class ImageSelectionInfo {

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ImageSelectionInfo), name, defaultValue, parms); }

    public ImageSelectionInfo(ModuleDefinition owningModule, Guid folderGuid, string? subFolder, string fileType = "Images") {
        OwningModule = owningModule;
        FolderGuid = folderGuid;
        SubFolder = subFolder;
        FileType = fileType;
        AllowUpload = false;
        PreviewWidth = 200;
        PreviewHeight = 200;

        // the upload control
        FileUpload1 = new FileUpload1() {
            SaveURL = Utility.UrlFor(typeof(ImageSelectionEndpoints), nameof(ImageSelectionEndpoints.SaveImage),
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
            Url = Utility.UrlFor(typeof(ImageSelectionEndpoints), nameof(ImageSelectionEndpoints.RemoveSelectedImage)),
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
    public string? SubFolder { get; set; }
    /// <summary>
    /// File type (defaults to "Images")
    /// </summary>
    public string FileType { get; set; } = null!;
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
    public ModuleAction ClearImageButton { get; set; } = null!;
    [UIHint("ModuleAction")]
    public ModuleAction RemoveImageButton { get; set; } = null!;
    public ModuleDefinition OwningModule { get; set; }

    public string MakeImageUrl(string filename, int width = 0, int height = 0) {
        // always defeat browser caching for image selection
        return ImageHTML.FormatUrl(ImageSupport.ImageType, string.Format("{0},{1},{2}", FolderGuid.ToString(), SubFolder, FileType), filename, width, height, CacheBuster: DateTime.UtcNow.Ticks.ToString());
    }
    public async Task<List<string>> GetFilesAsync() {
        if (_files == null)
            _files = await ReadFilesAsync(FolderGuid, SubFolder, FileType);
        return _files;
    }
    List<string>? _files;

    // Some of the following is not HTML dependent TODO: %%%%%%%%%%%%%%%%%%% ?

    public static string StoragePath(Guid folderGuid, string? subFolder, string fileType) {
        string path = ModuleDefinition.GetModuleDataFolder(folderGuid);
        if (!string.IsNullOrWhiteSpace(subFolder))
            path = Path.Combine(path, subFolder);
        return Path.Combine(path, fileType);
    }
    // remove folder, then move up and remove all empty parent folders
    public static async Task RemoveStorageAsync(Guid folderGuid, string? subFolder, string fileType) {
        string path = StoragePath(folderGuid, subFolder, fileType);
        try {
            await FileSystem.FileSystemProvider.DeleteDirectoryAsync(path);
        } catch (Exception) {
            return;
        }
        for (;;) {
            path = Path.GetDirectoryName(path)!;
            try {
                await FileSystem.FileSystemProvider.DeleteDirectoryAsync(path);
            } catch (Exception) {
                return;
            }
        }
    }
    public static async Task<List<string>> ReadFilesAsync(Guid folderGuid, string? subFolder, string fileType) {
        List<string> files = await ReadFilePathsAsync(folderGuid, subFolder, fileType);
        List<string> list = new List<string>();
        foreach (string f in files) {
            long cb = (await FileSystem.FileSystemProvider.GetLastWriteTimeUtcAsync(f)).Ticks / TimeSpan.TicksPerSecond;
            list.Add(Path.GetFileName(f) + YetaWF.Core.Image.ImageSupport.ImageSeparator + cb.ToString());
        }
        return list;
    }
    private static async Task<List<string>> ReadFilePathsAsync(Guid folderGuid, string? subFolder, string fileType) {
        List<string> files = new List<string>();
        string storagePath = ImageSelectionInfo.StoragePath(folderGuid, subFolder, fileType);
        if (await FileSystem.FileSystemProvider.DirectoryExistsAsync(storagePath))
            files = await FileSystem.FileSystemProvider.GetFilesAsync(storagePath);
        files = (from f in files where !f.EndsWith(".webp-gen") select f).ToList();
        return files;
    }
}
