/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Language;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Languages.Modules;

public class LanguageDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguageDisplayModule>, IInstallableModel { }

[ModuleGuid("{4cf7299c-1217-47d1-99bf-14b214f609b6}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class LanguageDisplayModule : ModuleDefinition {

    public LanguageDisplayModule() : base() {
        Title = this.__ResStr("modTitle", "Language");
        Name = this.__ResStr("modName", "Display Language");
        Description = this.__ResStr("modSummary", "Used to display information about a language. This is used by the Languages Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new LanguageDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, string id) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Id = id },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Displays an existing language"),
            Legend = this.__ResStr("displayLegend", "Displays an existing language"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    public class DisplayModel {

        [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
        [UIHint("String"), ReadOnly]
        public string Id { get; set; } = null!;

        [Caption("Name"), Description("The language's name, which is displayed in language selection controls so the user can select a language")]
        [UIHint("String"), ReadOnly]
        public string ShortName { get; set; } = null!;

        [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
        [UIHint("String"), ReadOnly]
        public string? Description { get; set; }

        public void SetData(LanguageEntryElement data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string id) {
        LanguageEntryElement? language = (from l in LanguageSection.Languages where id == l.Id select l).FirstOrDefault();
        if (language == null)
            throw new Error(this.__ResStr("notFound", "Language \"{0}\" not found"), id);
        DisplayModel model = new DisplayModel();
        model.SetData(language);
        Title = this.__ResStr("title", "Language \"{0}\"", language.ShortName);
        return await RenderAsync(model);
    }
}
