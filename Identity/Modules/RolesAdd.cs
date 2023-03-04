/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Modules;
public class RolesAddModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesAddModule>, IInstallableModel { }

[ModuleGuid("{97285509-fb4e-4f13-a3bc-cd4957f1cff0}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class RolesAddModule : ModuleDefinition2 {

    public RolesAddModule() {
        Title = this.__ResStr("modTitle", "New Role");
        Name = this.__ResStr("modName", "New Role");
        Description = this.__ResStr("modSummary", "Adds a new role. This is used by the Roles Module (Admin > Identity Settings > Roles, standard YetaWF site) to add a new role.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RolesAddModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Add(string url) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Create a new role"),
            Legend = this.__ResStr("addLegend", "Creates a new role"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {

        [Caption("Name"), Description("The role name")]
        [UIHint("Text40"), StringLength(DataProvider.RoleDefinition.MaxName), RoleNameValidation, Required, Trim]
        public string Name { get; set; }

        [Caption("Description"), Description("The intended use of the role")]
        [UIHint("Text80"), StringLength(DataProvider.RoleDefinition.MaxDescription)]
        public string Description { get; set; }

        [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        public string PostLoginUrl { get; set; }

        public AddModel() { }

        public DataProvider.RoleDefinition GetData() {
            DataProvider.RoleDefinition data = new DataProvider.RoleDefinition();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);
        using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData()))
                throw new Error(this.__ResStr("alreadyExists", "A role named \"{0}\" already exists."), model.Name);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New role \"{0}\" saved", model.Name), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}

