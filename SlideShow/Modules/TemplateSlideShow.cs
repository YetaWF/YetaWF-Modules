/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SlideShow#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.SlideShow.Models;

namespace YetaWF.Modules.SlideShow.Modules {

    public class TemplateSlideShowModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateSlideShowModule>, IInstallableModel { }

    [ModuleGuid("{fc6e7d03-d039-421a-bb0b-d9a7d1eb2e37}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateSlideShowModule : ModuleDefinition {

        public TemplateSlideShowModule() {
            Title = this.__ResStr("modTitle", "SlideShow Test Template");
            Name = this.__ResStr("modName", "Template Test - SlideShow");
            Description = this.__ResStr("modSummary", "SlideShow Test Template");
            SlideShow = new SlideShowInfo();
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateSlideShowModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Caption("Slide Show"), Description("The slide show displayed")]
        [UIHint("YetaWF_SlideShow_SlideShow")]
        public SlideShowInfo SlideShow { get; set; }

        public override void ModuleSaving() {
            SlideShow.Saving("SlideShow", ModuleGuid); // update internal information
        }
    }
}