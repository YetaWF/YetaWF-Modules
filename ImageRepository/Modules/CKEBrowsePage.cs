/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ImageRepository.Modules;

public class CKEBrowsePageModuleDataProvider : ModuleDefinitionDataProvider<Guid, CKEBrowsePageModule>, IInstallableModel { }

[ModuleGuid("{3645C411-EF04-4fd3-8B87-9E37B803C4B5}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class CKEBrowsePageModule : ModuleDefinition2 {

    public CKEBrowsePageModule() {
        Title = this.__ResStr("modTitle", "Select Page");
        Name = this.__ResStr("modName", "Select Page (CKEditor)");
        Description = this.__ResStr("modSummary", "Displays a form to select a local page URL.");
        WantSearch = false;
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new CKEBrowsePageModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        [Caption("Local Url"), Description("Local Url")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        [StringLength(Globals.MaxUrl), Required, Trim]
        public string? PageUrl { get; set; }

        [UIHint("Hidden")]
        public string CKEditor { get; set; } = null!;
        [UIHint("Hidden")]
        public int CKEditorFuncNum { get; set; }
        [UIHint("Hidden")]
        public string LangCode { get; set; } = null!;

        public Model() { }

        public void Update(ModuleDefinition module) { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        string CKEditor = Manager.RequestQueryString["CKEditor"] ?? throw new InternalError($"Argument {nameof(CKEditor)} missing");
        if (!int.TryParse(Manager.RequestQueryString["CKEditorFuncNum"], out int CKEditorFuncNum)) throw new InternalError($"Argument {nameof(CKEditorFuncNum)} missing");
        string langCode = Manager.RequestQueryString["LangCode"] ?? throw new InternalError($"Argument {nameof(langCode)} missing");
        Model model = new Model {
            CKEditor = CKEditor,
            CKEditorFuncNum = CKEditorFuncNum,
            LangCode = langCode,
        };
        model.Update(this);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        model.Update(this);
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        // build javascript to return selected Page image to CKEditor
        string js = string.Format("window.opener.CKEDITOR.tools.callFunction({0}, '{1}');", model.CKEditorFuncNum, Utility.JserEncode(model.PageUrl));
        return await FormProcessedAsync(model, OnClose: OnCloseEnum.CloseWindow, OnPopupClose: OnPopupCloseEnum.ReloadNothing, PreSaveJavaScript: js);
    }
}
