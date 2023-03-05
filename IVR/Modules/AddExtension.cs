/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules;

public class AddExtensionModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddExtensionModule>, IInstallableModel { }

[ModuleGuid("{cb461097-1109-4a5b-8514-5af0260b98c7}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AddExtensionModule : ModuleDefinition2 {

    public AddExtensionModule() {
        Title = this.__ResStr("modTitle", "Add New Extension");
        Name = this.__ResStr("modName", "Add New Extension");
        Description = this.__ResStr("modSummary", "Adds a new extension.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddExtensionModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Add a new extension"),
            Legend = this.__ResStr("addLegend", "Adds a new extension"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {

        [Caption("Extension"), Description("Defines the extension (digits)")]
        [UIHint("Text10"), Required, Trim]
        [StringLength(ExtensionEntry.MaxExtension)]
        public string? Extension { get; set; }

        [Caption("Description"), Description("Describes the extension")]
        [UIHint("Text80"), Required, Trim]
        [StringLength(ExtensionEntry.MaxDescription)]
        public string? Description { get; set; }

        [Caption("Phone Numbers"), Description("Defines the phone numbers to call when this extension is entered - At least one phone number is required")]
        [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), Required]
        public SerializableList<ExtensionPhoneNumber>? PhoneNumbers { get; set; }

        [Caption("Users"), Description("Defines the users that can access voice mails for this extension")]
        [UIHint("YetaWF_Identity_ListOfUserNames")]
        public SerializableList<User>? Users { get; set; }

        public AddModel() { }

        public ExtensionEntry GetData() {
            ExtensionEntry data = new ExtensionEntry();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        ObjectSupport.CopyData(new ExtensionEntry(), model);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (ExtensionEntryDataProvider dataProvider = new ExtensionEntryDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData())) {
                ModelState.AddModelError(nameof(AddModel.Extension), this.__ResStr("dup", "An entry for extension {0} already exists", model.Extension));
                return await PartialViewAsync(model);
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New extension saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
