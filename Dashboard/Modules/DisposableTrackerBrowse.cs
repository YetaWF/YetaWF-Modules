/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Modules {

    public class DisposableTrackerBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisposableTrackerBrowseModule>, IInstallableModel { }

    [ModuleGuid("{56e429a9-2c8d-49dd-8e78-fb1202aefb93}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class DisposableTrackerBrowseModule : ModuleDefinition {

        public DisposableTrackerBrowseModule() {
            Title = this.__ResStr("modTitle", "Disposable Objects");
            Name = this.__ResStr("modName", "Disposable Objects");
            Description = this.__ResStr("modSummary", "Displays and manages disposable objects");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new DisposableTrackerBrowseModuleDataProvider(); }

        [Category("General"), Caption("Display Url"), Description("The Url to display a disposable object - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Disposable Objects"),
                MenuText = this.__ResStr("browseText", "Disposable Objects"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage disposable objects"),
                Legend = this.__ResStr("browseLegend", "Displays and manages disposable objects"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}