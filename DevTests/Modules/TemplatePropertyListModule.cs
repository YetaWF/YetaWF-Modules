/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplatePropertyListModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplatePropertyListModule>, IInstallableModel { }

    [ModuleGuid("{e356901b-fd5f-49d9-a438-af7f6c491c9e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplatePropertyListModule : ModuleDefinition {

        public TemplatePropertyListModule() {
            Title = this.__ResStr("modTitle", "PropertyList Test Template");
            Name = this.__ResStr("modName", "Template Test - PropertyList");
            Description = this.__ResStr("modSummary", "Test module for the PropertyList component. A test page for this module can be found at Tests > Templates > PropertyList (standard YetaWF site).");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplatePropertyListModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public class SampleSiteDefinition : SiteDefinition {

            public PropertyList.PropertyListStyleEnum Style { get; set; }

            // Override PropertyList definition found in definition file located at ./Addons./_Main/PropertyLists/TemplatePropertyListModule.SampleSiteDefinition
            public Task __PropertyListSetupAsync(PropertyList.PropertyListSetup setup) {
                setup.Style = Style;
                return Task.CompletedTask;
            }
        }

        [Trim]
        public class Model {

            [Caption("Style"), Description("Defines the display style of the property list")]
            [UIHint("Enum")]
            public PropertyList.PropertyListStyleEnum Style { get; set; }

            [Caption(" "), Description(" ")]
            [UIHint("FormButton"), ReadOnly]
            public FormButton ApplyButton { get; set; }

            [Caption("Sample Property List"), Description("")]
            [UIHint("PropertyList")]
            public SampleSiteDefinition Site { get; set; }

            public Model() {
                Site = new SampleSiteDefinition();
                ApplyButton = new FormButton { ButtonType = ButtonTypeEnum.Apply };
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model {
                Style = PropertyList.PropertyListStyleEnum.BoxedWithHeaders,
            };
            ObjectSupport.CopyData(Manager.CurrentSite, model.Site);
            model.Site.Style = model.Style;
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            model.Site.Style = model.Style;
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
