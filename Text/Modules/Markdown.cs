/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Text#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Search;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Text.Modules;

public class MarkdownModuleDataProvider : ModuleDefinitionDataProvider<Guid, MarkdownModule>, IInstallableModel { }

public class MarkdownString : MarkdownStringBase {
    [StringLength(0), AdditionalMetadata("EmHeight", 10)]
    public override string? Text { get { return base.Text; } set { base.Text = value; } }
    [StringLength(0)]
    public override string? HTML { get { return base.HTML; } set { base.HTML = value; } }
}

[ModuleGuid("{5EAF62EB-9B05-45a1-A530-4A721D2F1C33}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class MarkdownModule : ModuleDefinition {

    public MarkdownModule() : base() {
        Title = this.__ResStr("modTitle", "Edit");
        Name = this.__ResStr("modName", "Text (Markdown)");
        Description = this.__ResStr("modSummary", "Displays user editable text contents (using Markdown). A sample page is accessible using Tests > Modules > Markdown (standard YetaWF site).");
        Contents = new MarkdownString();
    }

    public override IModuleDefinitionIO GetDataProvider() { return new MarkdownModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return EditorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Contents"), Description("The text contents")]
    [UIHint("Markdown"), AdditionalMetadata("EmHeight", 10)]
    public MarkdownString Contents { get; set; }

    // SEARCH
    // SEARCH
    // SEARCH

    public override void CustomSearch(ISearchWords searchWords) {
        searchWords.AddObjectContents(Contents);
    }

    public class MarkdownEdit : MarkdownString {
        [Required, AdditionalMetadata("EmHeight", 25)]
        public override string? Text { get; set; }
    }
    public class MarkdownDisplay : MarkdownStringBase {
        [ReadOnly, AdditionalMetadata("EmHeight", 25)]
        public override string? Text { get; set; }
    }

    public class ModelEdit {
        [UIHint("Markdown"), AdditionalMetadata("EmHeight", 25)]
        public MarkdownEdit Contents { get; set; }

        public ModelEdit() {
            Contents = new MarkdownEdit();
        }
    }
    public class ModelDisplay {
        [UIHint("Markdown"), ReadOnly]
        public MarkdownDisplay Contents { get; set; }

        public ModelDisplay() {
            Contents = new MarkdownDisplay();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (Manager.EditMode && IsAuthorized(ModuleDefinition.RoleDefinition.Edit)) {
            ModelEdit model = new ModelEdit();
            ObjectSupport.CopyData(Contents, model.Contents);
            return await RenderAsync(model);
        } else {
            ModelDisplay model = new ModelDisplay();
            ObjectSupport.CopyData(Contents, model.Contents);
            return await RenderAsync(model, ViewName: "MarkdownDisplay");
        }
    }

    [Permission("Edit")]//$$$$
    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(ModelEdit model) {
        ObjectSupport.CopyData(model.Contents, Contents);
        await SaveAsync();
        if (IsApply) {
            return await FormProcessedAsync(model, this.__ResStr("saved", "Contents saved"), OnClose: OnCloseEnum.Nothing, OnPopupClose: OnPopupCloseEnum.ReloadModule, OnApply: OnApplyEnum.ReloadModule);
        } else {
            if (Manager.IsInPopup) throw new InternalError("Save & Display not available in a popup window");
            Manager.EditMode = false;
            return await FormProcessedAsync(model, OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
        }
    }
}