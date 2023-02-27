/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.Modules;

public class SyntaxHighlighterConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SyntaxHighlighterConfigModule>, IInstallableModel { }

[ModuleGuid("{75e0578c-336a-4bd0-9966-b89edf1d3388}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Configuration")]
public class SyntaxHighlighterConfigModule : ModuleDefinition2 {

    public SyntaxHighlighterConfigModule() {
        Title = this.__ResStr("modTitle", "Syntax Highlighter Settings");
        Name = this.__ResStr("modName", "Syntax Highlighter Settings");
        Description = this.__ResStr("modSummary", "Configuration module for syntax highlighting. Define the appearance of sections using syntax highlighting. It is accessible using Admin > Settings > Syntax Highlighter Settings (standard YetaWF site).");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SyntaxHighlighterConfigModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new ConfigDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Config",
            LinkText = this.__ResStr("editLink", "Syntax Highlighter Settings"),
            MenuText = this.__ResStr("editText", "Syntax Highlighter Settings"),
            Tooltip = this.__ResStr("editTooltip", "Edit the syntax highlighter settings"),
            Legend = this.__ResStr("editLegend", "Edits the syntax highlighter settings"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    [Header("The settings are used to define syntax highlighting for the entire site. " +
        "If more than one syntax highlighter is available, both can be used, but should not be used on the same page. " +
        "To activate syntax highlighting, the Site, Page or Module Settings must reference one of the available skin modules. " +
        "The <pre> and/or <code> tags must be used to define the desired syntax coloring language.")]
    public class Model {

        [Category("Highlight.js"), Caption("Skin"), Description("The skin used for syntax highlighting")]
        [UIHint("YetaWF_SyntaxHighlighter_HighlightJS"), StringLength(DataProvider.ConfigData.MaxSkinName), AdditionalMetadata("NoDefault", true), Trim]
        public string? HighlightJSSkin { get; set; }

        [TextAbove("The syntax highlighter uses Highlight.js.")]
        [Category("Highlight.js"), Caption("Help"), Description("The syntax highlighter uses Highlight.js")]
        [UIHint("Url"), ReadOnly]
        public string HighlightJSUrl { get; set; }

        [Category("Highlight.js"), Caption("Languages"), Description("Displays help for the languages that can be used by Highlight.js for syntax coloring - Not all may be installed and available")]
        [UIHint("Url"), ReadOnly]
        public string LanguagesHighlightJSUrl { get; set; }

        public ConfigData GetData(ConfigData data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }
        public void SetData(ConfigData data) {
            ObjectSupport.CopyData(data, this);
        }
        public Model() {
            HighlightJSUrl = "https://highlightjs.org";
            LanguagesHighlightJSUrl = "http://highlightjs.readthedocs.io/en/latest/css-classes-reference.html#language-names-and-aliases";
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        await using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            Model model = new Model { };
            ConfigData data = await dataProvider.GetItemAsync();
            if (data == null)
                throw new Error(this.__ResStr("notFound", "The syntax highlighter settings could not be found"));
            model.SetData(data);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        await using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
            ConfigData data = await dataProvider.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display
            await dataProvider.UpdateConfigAsync(data);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Syntax highlighter settings saved"));
        }
    }
}
