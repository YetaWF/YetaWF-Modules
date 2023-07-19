using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class Template$templatetest$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, Template$templatetest$Module>, IInstallableModel { }

[ModuleGuid("{$templatetestmoduleguid$}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class Template$templatetest$Module : ModuleDefinition {

    public Template$templatetest$Module() {
        Title = this.__ResStr("modTitle", "$templatetest$ Test Template");
        Name = this.__ResStr("modName", "Template Test - $templatetest$");
        Description = this.__ResStr("modSummary", "$templatetest$ test template");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new Template$templatetest$ModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        [Caption("$templatetest$ (Required)"), Description("$templatetest$ (Required)")]
        [UIHint("$templatetest$"), Required, Trim]
        public string? Prop1Req { get; set; }

        [Caption("$templatetest$"), Description("$templatetest$")]
        [UIHint("$templatetest$"), Trim]
        public string? Prop1 { get; set; }

        [Caption("$templatetest$ (Read/Only)"), Description("$templatetest$ (read/only)")]
        [UIHint("$templatetest$"), ReadOnly]
        public string? Prop1RO { get; set; }

        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model { };
        return await RenderAsync(model);
    }

    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
    }
}
