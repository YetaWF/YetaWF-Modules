/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Pages.Endpoints;

namespace YetaWF.Modules.Pages.Modules {

    public class TemplateListOfLocalPagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateListOfLocalPagesModule>, IInstallableModel { }

    [ModuleGuid("{44977a1b-18bb-4585-9db6-29330c181319}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateListOfLocalPagesModule : ModuleDefinition {

        public TemplateListOfLocalPagesModule() {
            Title = this.__ResStr("modTitle", "ListOfLocalPages Test Component");
            Name = this.__ResStr("modName", "Component - ListOfLocalPages");
            Description = this.__ResStr("modSummary", "Test module for the ListOfLocalPages component (edit and display). A test page for this module can be found at Tests > Templates > ListOfLocalPages (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateListOfLocalPagesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "ListOfLocalPages"),
                MenuText = this.__ResStr("displayText", "ListOfLocalPages"),
                Tooltip = this.__ResStr("displayTooltip", "Display the ListOfLocalPages test template"),
                Legend = this.__ResStr("displayLegend", "Displays the ListOfLocalPages test template"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

            };
        }

        [Trim]
        public class Model {

            [Caption("ListOfLocalPages (Required)"), Description("ListOfLocalPages (Required)")]
            [UIHint("YetaWF_Pages_ListOfLocalPages"), Required]
            public List<string> Prop1Req { get; set; }
            public string Prop1Req_AjaxUrl { get { return Utility.UrlFor<TemplateListOfLocalPagesEndpoints>(TemplateListOfLocalPagesEndpoints.AddPage); } }

            [Caption("ListOfLocalPages"), Description("ListOfLocalPages")]
            [UIHint("YetaWF_Pages_ListOfLocalPages")]
            public List<string> Prop1 { get; set; }
            public string Prop1_AjaxUrl { get { return Utility.UrlFor<TemplateListOfLocalPagesEndpoints>(TemplateListOfLocalPagesEndpoints.AddPage); } }

            [Caption("ListOfLocalPages (Read/Only)"), Description("ListOfLocalPages (read/only)")]
            [UIHint("YetaWF_Pages_ListOfLocalPages"), ReadOnly]
            public List<string> Prop1RO { get; set; }

            public Model() {
                Prop1Req = new List<string>();
                Prop1 = new List<string>();
                Prop1RO = new List<string>();
            }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model { };
            return await RenderAsync(model);
        }

        public async Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            model.Prop1RO = model.Prop1;
            return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
