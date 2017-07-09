/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.SyntaxHighlighter.DataProvider;

namespace YetaWF.Modules.SyntaxHighlighter.Modules {

    public class SyntaxHighlighterConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, SyntaxHighlighterConfigModule>, IInstallableModel { }

    [ModuleGuid("{75e0578c-336a-4bd0-9966-b89edf1d3388}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SyntaxHighlighterConfigModule : ModuleDefinition {

        public SyntaxHighlighterConfigModule() {
            Title = this.__ResStr("modTitle", "Syntax Highlighter Settings");
            Name = this.__ResStr("modName", "Syntax Highlighter Settings");
            Description = this.__ResStr("modSummary", "Edits a site's syntax highlighter settings");
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
    }
}
