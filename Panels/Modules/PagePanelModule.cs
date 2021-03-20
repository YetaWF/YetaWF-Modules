/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules {

    public class PagePanelModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagePanelModule>, IInstallableModel { }

    [ModuleGuid("{F8EF23F3-A690-47FC-ABB5-753D8BA9B9DA}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PagePanelModule : ModuleDefinition {

        public PagePanelModule() {
            Title = this.__ResStr("modTitle", "Page Panel");
            Name = this.__ResStr("modName", "Page Panel");
            Description = this.__ResStr("modSummary", "Used to display multiple links to pages using the pages' FavIcon. A sample page is available at /Admin/Tests (standard YetaWF site).");
            PageList = new SerializableList<LocalPage>();
            DefaultImage_Data = new byte[] { };
            UsePartialFormCss = false;
            WantSearch = false;
            WantFocus = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PagePanelModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Page List"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
        [UIHint("YetaWF_Panels_ListOfLocalPages")]
        [Data_Binary]
        public SerializableList<LocalPage> PageList { get; set; }
        public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PagePanelModuleController), nameof(PagePanelModuleController.AddPage)); } }

        [Category("General"), Caption("Page Pattern"), Description("Defines a Regex pattern - All pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
        [UIHint("Text40"), Trim]
        [StringLength(500)]
        public string? PagePattern { get; set; }

        [Category("General"), Caption("Use Popups"), Description("Defines whether pages added automatically using the Page Pattern field are shown as popups - otherwise full pages are shown")]
        [UIHint("Boolean")]
        public bool UsePopup { get; set; }

        public enum PanelStyleEnum {
            [EnumDescription("Default", "Displays the page FavIcon and page title in tiles, arranged horizontally, wrapping around within the available space - A large icon is used")]
            Default = 0,
            [EnumDescription("Small Vertical", "Displays the page FavIcon and page title as a list, arranged vertically, wrapping around within the available space - A small icon is used")]
            SmallVertical = 1,
            [EnumDescription("Small Table", "Displays the page FavIcon, page title and description as a table - A small icon is used")]
            SmallTable = 2,
        }
        [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
        [UIHint("Enum")]
        [Data_NewValue]
        public PanelStyleEnum Style { get; set; }

        [Category("General"), Caption("Default Image"), Description("The default image used when a page doesn't define its own FavIcon")]
        [UIHint("Image"), AdditionalMetadata("ImageType", ModuleImageSupport.ImageType)]
        [AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
        [DontSave]
        public string? DefaultImage {
            get {
                if (_defaultImage == null) {
                    if (DefaultImage_Data != null && DefaultImage_Data.Length > 0)
                        _defaultImage = ModuleGuid.ToString() + ",DefaultImage_Data";
                }
                return _defaultImage;
            }
            set {
                _defaultImage = value;
            }
        }
        private string? _defaultImage = null;

        [Data_Binary, CopyAttribute]
        public byte[]? DefaultImage_Data { get; set; }

        public ModuleAction? GetAction_Display(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Page Panel"),
                MenuText = this.__ResStr("displayText", "Page Panel"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Page Panel"),
                Legend = this.__ResStr("displayLegend", "Displays the Page Panel"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
