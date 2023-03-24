/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.DevTests.Endpoints;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateListOfEmailAddressesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfEmailAddressesModule>, IInstallableModel { }

    [ModuleGuid("{443bfefa-648c-4b4f-832c-25705636565f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateListOfEmailAddressesModule : ModuleDefinition {

        public TemplateListOfEmailAddressesModule() {
            Title = this.__ResStr("modTitle", "ListOfEmailAddresses Test Component");
            Name = this.__ResStr("modName", "Component Test - ListOfEmailAddresses");
            Description = this.__ResStr("modSummary", "Test module for the ListOfEmailAddresses component. A test page for this module can be found at Tests > Templates > ListOfEmailAddresses (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfEmailAddressesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Edit(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfEmailAddresses"),
                MenuText = this.__ResStr("displayText", "ListOfEmailAddresses"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfEmailAddresses test component"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfEmailAddresses test component"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }

        [Trim]
        public class Model {

            [Caption("Email Addresses (Required)"), Description("List of email addresses (Required)")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ListNoDuplicates, EmailValidation, StringLength(Globals.MaxEmail), Required, Trim]
            public List<string> Prop1Req { get; set; } = null!;
            public string Prop1Req_AjaxUrl { get { return Utility.UrlFor(typeof(TemplateListOfEmailAddressesEndpoints), TemplateListOfEmailAddressesEndpoints.AddEmailAddress); } }

            [Caption("Email Addresses"), Description("List of email addresses")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ListNoDuplicates, EmailValidation, StringLength(Globals.MaxEmail), Trim]
            public List<string> Prop1 { get; set; } = null!;
            public string Prop1_AjaxUrl { get { return Utility.UrlFor(typeof(TemplateListOfEmailAddressesEndpoints), TemplateListOfEmailAddressesEndpoints.AddEmailAddress); } }

            [Caption("Email Addresses (Read/Only)"), Description("List of email addresses (read/only)")]
            [UIHint("YetaWF_DevTests_ListOfEmailAddresses"), ReadOnly]
            public List<string>? Prop1RO { get; set; }

            public Model() { }

            public void Update() {
                Prop1RO = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" };
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model {
                Prop1Req = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" },
                Prop1 = new List<string>() { "aa1@somedomain.com", "aa2@somedomain.com", "aa3@somedomain.com", "aa4@somedomain.com", "aa5@somedomain.com", "aa6@somedomain.com", "aa7@somedomain.com", "aa8@somedomain.com", "aa9@somedomain.com", "aa10@somedomain.com" },
            };
            model.Update();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            model.Update();
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
