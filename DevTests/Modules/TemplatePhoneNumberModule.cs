using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplatePhoneNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplatePhoneNumberModule>, IInstallableModel { }

    [ModuleGuid("{16d600a0-3519-4cb3-a929-665b23a4347f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplatePhoneNumberModule : ModuleDefinition2 {

        public TemplatePhoneNumberModule() {
            Title = this.__ResStr("modTitle", "PhoneNumber Test Template");
            Name = this.__ResStr("modName", "Template Test - PhoneNumber");
            Description = this.__ResStr("modSummary", "PhoneNumber test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplatePhoneNumberModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        [Trim]
        public class Model {

            [TextAbove("National or international phone number.")]
            [Caption("Natl Or Internatl (Required)"), Description("PhoneNumber (Required)")]
            [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), Required, PhoneNumberValidation, Trim]
            public string? Prop1Req { get; set; }

            [TextAbove("National phone number only.")]
            [Caption("National (Required)"), Description("PhoneNumber (Required)")]
            [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), Required, PhoneNumberNationalValidation, Trim]
            public string? Prop2Req { get; set; }

            [Caption("Natl Or Internatl (Read/Only)"), Description("PhoneNumber (read/only)")]
            [UIHint("PhoneNumber"), ReadOnly]
            public string? Prop1RO { get; set; }

            [Caption("National (Read/Only)"), Description("PhoneNumber (read/only)")]
            [UIHint("PhoneNumber"), ReadOnly]
            public string? Prop2RO { get; set; }

            public Model() { }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model { };
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
