using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Content.Modules {

    public class ContentModuleDataProvider : ModuleDefinitionDataProvider<Guid, ContentModule>, IInstallableModel { }

    [ModuleGuid("{cb78208d-f232-4fe2-bf1d-a2b100ad66d1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ContentModule : ModuleDefinition {

        public const int MaxContents = 1024 * 1024;

        public ContentModule() {
            Title = this.__ResStr("modTitle", "Content");
            Name = this.__ResStr("modName", "Content");
            Description = this.__ResStr("modSummary", "Displays an existing content");
            DefaultViewName = StandardViews.Display;
            CompleteContents = this.__ResStr("newContents", "(new)");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ContentModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url, MultiString content) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Content = content },
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Display"),
                MenuText = this.__ResStr("displayText", "Display"),
                Tooltip = this.__ResStr("displayTooltip", "Display the content"),
                Legend = this.__ResStr("displayLegend", "Displays an existing content"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }

        [StringLength(MaxContents)]
        public MultiString CompleteContents { get; set; }

        [Category("General"), Caption("Contents"), Description("The text contents")]
        [UIHint("TextArea")/*$$$$ , AdditionalMetadata("ImageBrowse", true), AdditionalMetadata("FlashBrowse", true), StringLength(MaxContents), AdditionalMetadata("PageBrowse", true)*/]
        [DontSave]
        [NoModelChange]
        public string Contents {
            get {
                return CompleteContents[MultiString.ActiveLanguage];
            }
            set {
                CompleteContents[MultiString.ActiveLanguage] = value;
            }
        }
    }
}
