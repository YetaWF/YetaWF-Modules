/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#endif

namespace YetaWF.Modules.Panels.Modules {

    public class PagePanelModuleDataProvider : ModuleDefinitionDataProvider<Guid, PagePanelModule>, IInstallableModel { }

    [ModuleGuid("{F8EF23F3-A690-47FC-ABB5-753D8BA9B9DA}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PagePanelModule : ModuleDefinition {

        public PagePanelModule() {
            Title = this.__ResStr("modTitle", "Page Panel");
            Name = this.__ResStr("modName", "Page Panel");
            Description = this.__ResStr("modSummary", "Page Panel - used to display multiple links to pages using the pages' FavIcon");
            PageList = new SerializableList<string>();
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PagePanelModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Pages"), Description("Defines the pages and their order as they are displayed in the Page Panel using their FavIcons and page description")]
        [UIHint("YetaWF_Pages_ListOfLocalPages")]
        [Data_Binary]
        public SerializableList<string> PageList { get; set; }
        public string PageList_AjaxUrl { get { return YetaWFManager.UrlFor(typeof(TemplateListOfLocalPagesModuleController), nameof(TemplateListOfLocalPagesModuleController.AddPage)); } }

        [Category("General"), Caption("Page Pattern"), Description("Defines a Regex pattern - all pages matching this pattern will be included in the Page Panel - for example, ^/Admin/Config/[^/]* would include all pages starting with /Admin/Config, but would not include their child pages")]
        [UIHint("Text40"), Trim]
        [StringLength(500)]
        public string PagePattern { get; set; }

        [Category("General"), Caption("Use Popups"), Description("Defines whether all pages are shown as popups - otherwise full pages are shown")]
        [UIHint("Boolean")]
        public bool UsePopup { get; set; }

        public ModuleAction GetAction_Display(string url) {
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
