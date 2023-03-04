/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
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

public class RolesEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, RolesEditModule>, IInstallableModel { }

[ModuleGuid("{e35d6a55-b682-4b4c-9453-04951cc9b9b1}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class RolesEditModule : ModuleDefinition2 {

    public RolesEditModule() : base() {
        Title = this.__ResStr("modTitle", "Edit a Role");
        Name = this.__ResStr("modName", "Edit Role");
        Description = this.__ResStr("modSummary", "Edits an existing role. This is used by the Roles Module to edit a role.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new RolesEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url, string name) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Name = name },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit an existing role"),
            Legend = this.__ResStr("editLegend", "Edits an existing role"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Name"), Description("The name of this role")]
        [UIHint("Text40"), StringLength(DataProvider.RoleDefinition.MaxName), RoleNameValidation, Required, Trim]
        public string Name { get; set; }

        [Caption("Description"), Description("The intended use of the role")]
        [UIHint("Text80"), StringLength(DataProvider.RoleDefinition.MaxDescription)]
        public string Description { get; set; }

        [Caption("Post Login URL"), Description("The URL where a user with this role is redirected after logging on")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local)]
        public string PostLoginUrl { get; set; }

        [UIHint("Hidden")]
        public string OriginalName { get; set; }

        public DataProvider.RoleDefinition GetData(DataProvider.RoleDefinition role) {
            ObjectSupport.CopyData(this, role);
            return role;
        }

        public void SetData(DataProvider.RoleDefinition role) {
            ObjectSupport.CopyData(role, this);
            OriginalName = Name;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        string name = Manager.RequestQueryString["Name"] ?? throw new InternalError("No role name provided");
        using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
            EditModel model = new EditModel { };
            DataProvider.RoleDefinition data = await dataProvider.GetItemAsync(name);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Role \"{0}\" not found."), name);
            model.SetData(data);
            Title = this.__ResStr("modEditTitle", "Role \"{0}\"", name);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        using (RoleDefinitionDataProvider dataProvider = new RoleDefinitionDataProvider()) {
            string originalRole = model.OriginalName;
            DataProvider.RoleDefinition role = await dataProvider.GetItemAsync(originalRole);// get the original item
            if (role == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", originalRole));

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            role = model.GetData(role); // merge new data into original
            model.SetData(role); // and all the data back into model for final display

            switch (await dataProvider.UpdateItemAsync(originalRole, role)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The role named \"{0}\" has been removed and can no longer be updated.", originalRole));
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError(nameof(model.Name), this.__ResStr("alreadyExists", "A role named \"{0}\" already exists.", model.Name));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Role \"{0}\" saved", role.Name), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
