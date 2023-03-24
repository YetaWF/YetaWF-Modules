using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

#nullable enable

namespace YetaWF.Modules.Identity.Modules;

public class TemplateResourceUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateResourceUsersModule>, IInstallableModel { }

[ModuleGuid("{fc6b0bad-5416-4fb0-b9d1-e5a02359a7b9}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class TemplateResourceUsersModule : ModuleDefinition {

    public TemplateResourceUsersModule() {
        Title = this.__ResStr("modTitle", "ResourceUsers Test Template");
        Name = this.__ResStr("modName", "Template Test - ResourceUsers");
        Description = this.__ResStr("modSummary", "ResourceUsers test template");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TemplateResourceUsersModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        [Caption("ResourceUsers (Required)"), Description("ResourceUsers (Required)")]
        [UIHint("YetaWF_Identity_ResourceUsers"), Required, Trim]
        public List<User> Prop1Req { get; set; }

        [Caption("ResourceUsers"), Description("ResourceUsers")]
        [UIHint("YetaWF_Identity_ResourceUsers"), Trim]
        public List<User> Prop1 { get; set; }

        [Caption("ResourceUsers (Read/Only)"), Description("ResourceUsers (read/only)")]
        [UIHint("YetaWF_Identity_ResourceUsers"), ReadOnly]
        public List<User> Prop1RO { get; set; }

        public Model() {
            Prop1Req = new List<User>();
            Prop1 = new List<User>();
            Prop1RO = new List<User>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        model.Prop1RO = model.Prop1Req;
        return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
    }
}
