/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

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

    public class PageBarModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageBarModule>, IInstallableModel { }

    [ModuleGuid("{AF54719E-BEB6-4dda-B724-E0399EB57733}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PageBarModule : ModuleDefinition {

        public PageBarModule() {
            Title = this.__ResStr("modTitle", "Page Bar");
            Name = this.__ResStr("modName", "Page Bar");
            Description = this.__ResStr("modSummary", "Used to display multiple links to pages using the pages' FavIcon, displaying the page contents within the Page Bar Module. A sample page is available at Admin > Settings (standard YetaWF site).");
            PageList = new SerializableList<LocalPage>();
            DefaultImage_Data = new byte[] { };
            ShowTitle = false;
            UsePartialFormCss = false;
            WantSearch = false;
            WantFocus = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PageBarModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Page List"), Description("Defines the pages and their order as they are displayed in the Page Bar using their FavIcons and page description - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
        [UIHint("YetaWF_Panels_ListOfLocalPages")]
        [Data_Binary]
        public SerializableList<LocalPage> PageList { get; set; }
        public string PageList_AjaxUrl { get { return Utility.UrlFor(typeof(PageBarModuleController), nameof(PageBarModuleController.AddPage)); } }

        [Category("General"), Caption("Page Pattern"), Description("Defines a Regex pattern - All pages matching this pattern will be included in the Page Bar - for example, ^/Admin/Config/[^/]*$ would include all pages starting with /Admin/Config/, but would not include their child pages - Pages added to the Page List are shown ahead of pages discovered using the Page Pattern")]
        [UIHint("Text40"), Trim]
        [StringLength(500)]
        public string? PagePattern { get; set; }

        public enum PanelStyleEnum {
            [EnumDescription("Vertical", "Displays the page FavIcon and page title arranged vertically")]
            Vertical = 0,
            [EnumDescription("Horizontal", "Displays the page FavIcon, page title arranged horizontally")]
            Horizontal = 1,
        }
        [Category("General"), Caption("Style"), Description("Defines the appearance of page entries")]
        [UIHint("Enum")]
        public PanelStyleEnum Style { get; set; }

        [Category("General"), Caption("Content Pane"), Description("Defines the pane used to extract page content - leave blank to use the main pane as page content")]
        [UIHint("Text20"), StringLength(20)]
        [Data_NewValue]
        public string? ContentPane { get; set; }

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

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Page Bar"),
                MenuText = this.__ResStr("displayText", "Page Bar"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Page Bar"),
                Legend = this.__ResStr("displayLegend", "Displays the Page Bar"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }
    }
}
