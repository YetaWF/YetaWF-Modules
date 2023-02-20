/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Modules.ImageRepository.Components;

namespace YetaWF.Modules.ImageRepository.Support;

public class ImageSupport : IInitializeApplicationStartup {

    // IInitializeApplicationStartup
    // IInitializeApplicationStartup
    // IInitializeApplicationStartup

    public readonly static string ImageType = "YetaWF_Image";

    public Task InitializeApplicationStartupAsync() {
        YetaWF.Core.Image.ImageSupport.AddHandler(ImageType, GetAsFileAsync: RetrieveImageAsync);
        return Task.CompletedTask;
    }

    private Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> RetrieveImageAsync(string? name, string? location) {
        Task<YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo> fail = Task.FromResult(new YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo());
        if (string.IsNullOrWhiteSpace(location)) return fail;
        if (string.IsNullOrWhiteSpace(name)) return fail;
        string[] parts = location.Split(new char[] { ',' });
        if (parts.Length != 3) return fail;
        string folderGuid = parts[0];
        string subFolder = parts[1];
        string fileType = parts[2];
        string storagePath = ImageSelectionInfo.StoragePath(new Guid(folderGuid), subFolder, fileType);
        return Task.FromResult(new YetaWF.Core.Image.ImageSupport.GetImageAsFileInfo {
            File = Path.Combine(storagePath, name),
            Success = true,
        });
    }
}
