/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules;

public class TranslationWarningModuleDataProvider : ModuleDefinitionDataProvider<Guid, TranslationWarningModule>, IInstallableModel { }

[ModuleGuid("{4F420CCB-4E40-4b82-B02F-D06EF12D8500}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Translation Warning")]
public class TranslationWarningModule : ModuleDefinition {

    public TranslationWarningModule() {
        Title = this.__ResStr("modTitle", "Skin Translation Warning");
        Name = this.__ResStr("modName", "Translation Warning");
        Description = this.__ResStr("modSummary", "Skin module displaying a warning about the language translation.");

        Invokable = true;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;

        Warning = this.__ResStr("msgTranslation", "All page content for the currently selected language was machine translated and needs to be \"manually\" perfected. This demonstrates how YetaWF can support multiple languages, down to data stored in files, database tables and strings typically embedded in source code.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TranslationWarningModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Message"), Description("Defines the message that is shown when a non US English language is the current language")]
    [UIHint("MultiString80"), StringLength(1000), Required]
    public MultiString Warning { get; set; }

    public class DisplayModel { }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (Manager.EditMode) return ActionInfo.Empty;
        if (Manager.IsInPopup) return ActionInfo.Empty;
        if (Manager.UserLanguage == null || Manager.UserLanguage == "en-US") return ActionInfo.Empty;

        return await RenderAsync(new DisplayModel());
    }
}
