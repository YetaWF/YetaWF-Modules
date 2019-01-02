/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Modules {

    public class TinyVisitorsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TinyVisitorsModule>, IInstallableModel { }

    [ModuleGuid("{0180db70-e251-494e-87e9-e97d3796c1ce}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class TinyVisitorsModule : ModuleDefinition {

        public TinyVisitorsModule() {
            Title = this.__ResStr("modTitle", "Visitor Stats");
            Name = this.__ResStr("modName", "Visitor Stats");
            Description = this.__ResStr("modSummary", "Displays today's visitor stats");
            WantFocus = false;
            WantSearch = false;
            ShowTitle = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TinyVisitorsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Visitors Url"), Description("The page shown when the icon is clicked")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local | UrlTypeEnum.Remote)]
        [StringLength(Globals.MaxUrl), Trim]
        public string VisitorsUrl { get; set; }
    }
}