/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules;

public class TemplateRolesSelectorModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateRolesSelectorModule>, IInstallableModel { }

[ModuleGuid("{3dc50ef9-ca0f-4a57-9f56-7bbec59f303b}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Tools")]
public class TemplateRolesSelectorModule : ModuleDefinition2 {

    public TemplateRolesSelectorModule() {
        Title = this.__ResStr("modTitle", "RolesSelector Test Component");
        Name = this.__ResStr("modName", "Component Test - RolesSelector");
        Description = this.__ResStr("modSummary", "Test module for the RolesSelector component (edit and display). A test page for this module can be found at Tests > Templates > RolesSelector (standard YetaWF site).");
        DefaultViewName = StandardViews.EditApply;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new TemplateRolesSelectorModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "RolesSelector"),
            MenuText = this.__ResStr("displayText", "RolesSelector"),
            Tooltip = this.__ResStr("displayTooltip", "Display the RolesSelector test template"),
            Legend = this.__ResStr("displayLegend", "Displays the RolesSelector test template"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class Model {

        [Caption("RolesSelector (Required)"), Description("RolesSelector (Required)")]
        [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", false), AdditionalMetadata("ShowFilter", true), Required]
        public SerializableList<Role> Prop1Req { get; set; }

        [Caption("RolesSelector"), Description("RolesSelector")]
        [UIHint("YetaWF_Identity_RolesSelector"), AdditionalMetadata("ExcludeUser2FA", true), Trim]
        public SerializableList<Role> Prop1 { get; set; }

        [Caption("RolesSelector (Read/Only)"), Description("RolesSelector (read/only)")]
        [UIHint("YetaWF_Identity_RolesSelector"), ReadOnly]
        public SerializableList<Role> Prop1RO { get; set; }

        public Model() {
            Prop1Req = new SerializableList<Role>();
            Prop1 = new SerializableList<Role>();
            List<RoleInfo> allRoles = Resource.ResourceAccess.GetDefaultRoleList();
            Prop1RO = new SerializableList<Role>((from RoleInfo a in allRoles select new Role { RoleId = a.RoleId }).ToList());
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
