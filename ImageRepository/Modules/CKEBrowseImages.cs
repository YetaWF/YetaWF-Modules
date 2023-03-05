/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.ImageRepository.Components;

namespace YetaWF.Modules.ImageRepository.Modules;

public class CKEBrowseImagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, CKEBrowseImagesModule>, IInstallableModel { }

[ModuleGuid("{079f499e-0ce2-4da0-a7e2-e036bc9c98ee}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class CKEBrowseImagesModule : ModuleDefinition2 {

    public CKEBrowseImagesModule() {
        Title = this.__ResStr("modTitle", "Select Image");
        Name = this.__ResStr("modName", "Select Image (CKEditor)");
        Description = this.__ResStr("modSummary", "Displays a form to upload and select an image.");
        WantSearch = false;
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CKEBrowseImagesModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        [Caption("Image Selection"), Description("Select an image or upload a new image")]
        [UIHint("YetaWF_ImageRepository_ImageSelection"), Required, Trim]
        public string? ImageName { get; set; }

        public ImageSelectionInfo? ImageName_Info { get; set; }

        [UIHint("Hidden")]
        public Guid FolderGuid { get; set; }
        [UIHint("Hidden")]
        public string? SubFolder { get; set; }

        [UIHint("Hidden")]
        public string CKEditor { get; set; } = null!;
        [UIHint("Hidden")]
        public int CKEditorFuncNum { get; set; }
        [UIHint("Hidden")]
        public string LangCode { get; set; } = null!;

        public Model() { }

        public async Task UpdateAsync() {
            ImageName_Info = new ImageSelectionInfo(FolderGuid, SubFolder) {
                AllowUpload = true,
            };
            await ImageName_Info.InitAsync();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!Guid.TryParse(Manager.RequestQueryString["__FolderGuid"], out Guid __FolderGuid)) throw new InternalError($"Argument {nameof(__FolderGuid)} missing");
        if (!Guid.TryParse(Manager.RequestQueryString["__SubFolder"], out Guid __SubFolder)) __SubFolder = Guid.Empty;
        string CKEditor = Manager.RequestQueryString["CKEditor"] ?? throw new InternalError($"Argument {nameof(CKEditor)} missing");
        if (!int.TryParse(Manager.RequestQueryString["CKEditorFuncNum"], out int CKEditorFuncNum)) throw new InternalError($"Argument {nameof(CKEditorFuncNum)} missing");
        string langCode = Manager.RequestQueryString["LangCode"] ?? throw new InternalError($"Argument {nameof(langCode)} missing");
        Model model = new Model {
            FolderGuid = __FolderGuid,
            SubFolder = __SubFolder == Guid.Empty ? null : __SubFolder.ToString(),
            CKEditor = CKEditor,
            CKEditorFuncNum = CKEditorFuncNum,
            LangCode = langCode,
        };
        await model.UpdateAsync();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        await model.UpdateAsync();
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // build javascript to return selected image to CKEditor
        string imageUrl = model.ImageName_Info!.MakeImageUrl(model.ImageName!);
        string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, Utility.JserEncode(imageUrl));

        return await FormProcessedAsync(model, /*this.__ResStr("okSaved", "Image saved"), */ OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, PreSaveJavaScript: js);
    }
}
