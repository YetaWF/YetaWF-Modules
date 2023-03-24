/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.Identity;
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

namespace YetaWF.Modules.Identity.Modules;

public class AuthorizationEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuthorizationEditModule>, IInstallableModel { }

[ModuleGuid("{0fd20e73-a4c3-44cb-8b71-0bab64343007}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Configuration")]
public class AuthorizationEditModule : ModuleDefinition {

    public AuthorizationEditModule() : base() {
        Title = this.__ResStr("modTitle", "Edit Resource");
        Name = this.__ResStr("modName", "Edit Resource");
        Description = this.__ResStr("modSummary", "Used to edit a resource. This module is used by the Resources Module");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AuthorizationEditModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit(string url, string resourceName) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { ResourceName = resourceName },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit an existing resource"),
            Legend = this.__ResStr("editLegend", "Edits an existing resource"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Resource Name"), Description("The name of this resource")]
        [UIHint("String"), ReadOnly]
        public string ResourceName { get; set; }

        [Caption("Resource Description"), Description("The permissions granted if a user or role has access to this resource")]
        [UIHint("TextAreaSourceOnly"), StringLength(Authorization.MaxResourceDescription)]
        public string ResourceDescription { get; set; }

        [Caption("Allowed Roles"), Description("The roles that are permitted to access this resource")]
        [UIHint("YetaWF_Identity_ResourceRoles")]
        public SerializableList<Role> AllowedRoles { get; set; }

        [Caption("Allowed Users"), Description("The users that are permitted to access this resource")]
        [UIHint("YetaWF_Identity_ResourceUsers")]
        public SerializableList<User> AllowedUsers { get; set; }

        [UIHint("Hidden")]
        public string OriginalName { get; set; }

        public Authorization GetData(Authorization data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }

        public void SetData(Authorization data) {
            ObjectSupport.CopyData(data, this);
            OriginalName = ResourceName;
        }

        public EditModel() { }

    }

    public async Task<ActionInfo> RenderModuleAsync(string resourceName) {
        using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
            EditModel model = new EditModel { };
            Authorization data = await dataProvider.GetItemAsync(resourceName);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Resource \"{0}\" not found."), resourceName);
            model.SetData(data);
            Title = this.__ResStr("editTitle", "Resource \"{0}\"", resourceName);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        string originalName = model.OriginalName;

        using (AuthorizationDataProvider dataProvider = new AuthorizationDataProvider()) {
            Authorization data = await dataProvider.GetItemAsync(originalName);// get the original resource
            if (data == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The resource named \"{0}\" has been removed and can no longer be updated.", originalName));

            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display

            switch (await dataProvider.UpdateItemAsync(data)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The resource named \"{0}\" has been removed and can no longer be updated.", originalName));
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError(nameof(model.ResourceName), this.__ResStr("alreadyExists", "A resource named \"{0}\" already exists.", model.ResourceName));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Resource \"{0}\" saved", model.ResourceName), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
