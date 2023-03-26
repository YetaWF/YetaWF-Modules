/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using Softelvdm.Modules.IVR.Models.Attributes;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
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

namespace Softelvdm.Modules.IVR.Modules;

public class EditExtensionModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditExtensionModule>, IInstallableModel { }

[ModuleGuid("{2bd2f6c5-daf4-48c0-bdc4-4eb20f1bca8a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class EditExtensionModule : ModuleDefinition {

    public EditExtensionModule() {
        Title = this.__ResStr("modTitle", "Extension");
        Name = this.__ResStr("modName", "Edit Extension");
        Description = this.__ResStr("modSummary", "Edits an existing extension.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new EditExtensionModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Edit(string? url, string extension) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Extension = extension },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit extension"),
            Legend = this.__ResStr("editLegend", "Edits an existing extension"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    [Trim]
    public class EditModel {

        [Caption("Extension"), Description("Defines the extension (digits)")]
        [UIHint("Text10"), ExtensionValidation, Required, Trim]
        [StringLength(ExtensionEntry.MaxExtension)]
        public string? Extension { get; set; } = null!;

        [Caption("Description"), Description("Describes the extension - This text is used to identify the extension when call screening")]
        [UIHint("Text40"), Required, Trim]
        [StringLength(ExtensionEntry.MaxDescription)]
        public string? Description { get; set; }

        [Caption("Phone Numbers"), Description("Defines the phone numbers to call when this extension is entered - At least one phone number is required")]
        [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), Required]
        public SerializableList<ExtensionPhoneNumber>? PhoneNumbers { get; set; }

        [Caption("Users"), Description("Defines the users that can access voice mails for this extension")]
        [UIHint("YetaWF_Identity_ListOfUserNames")]
        public SerializableList<User>? Users { get; set; }

        [UIHint("Hidden")]
        public string OriginalExtension { get; set; } = null!;

        public ExtensionEntry GetData(ExtensionEntry data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }

        public void SetData(ExtensionEntry data) {
            ObjectSupport.CopyData(data, this);
            OriginalExtension = data.Extension;
        }
        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync(string extension) {
        using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
            EditModel model = new EditModel { };
            ExtensionEntry? data = await dataProvider.GetItemAsync(extension);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Extension \"{0}\" not found"), extension);
            model.SetData(data);
            Title = this.__ResStr("title", "Extension \"{0}\"", extension);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        string originalExtension = model.OriginalExtension;

        using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
            ExtensionEntry? data = await dataProvider.GetItemAsync(originalExtension);// get the original item
            if (data == null) {
                ModelState.AddModelError("Extension", this.__ResStr("alreadyDeleted", "Extension {0} has been removed and can no longer be updated", originalExtension));
                return await PartialViewAsync(model);
            }
            ObjectSupport.CopyData(data, model, ReadOnly: true); // update read only properties in model in case there is an error
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            data = model.GetData(data); // merge new data into original
            model.SetData(data); // and all the data back into model for final display

            switch (await dataProvider.UpdateItemAsync(data)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "Extension {0} has been removed and can no longer be updated", model.OriginalExtension));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Extension {0} already exists.", data.Extension));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Extension saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
