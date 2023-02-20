/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
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

public class EditBlockedNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditBlockedNumberModule>, IInstallableModel { }

[ModuleGuid("{6f29aca3-e0c3-4e92-aa65-1d9ca8596bfe}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class EditBlockedNumberModule : ModuleDefinition2 {

    public EditBlockedNumberModule() {
        Title = this.__ResStr("modTitle", "Blocked Number");
        Name = this.__ResStr("modName", "Edit Blocked Number");
        Description = this.__ResStr("modSummary", "Edits an existing blocked number.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new EditBlockedNumberModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Edit(string? url, string number) {
        return new ModuleAction(this) {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Number = number },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit blocked number"),
            Legend = this.__ResStr("editLegend", "Edits an existing blocked number"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class EditModel {

        [Caption("Blocked Number"), Description("Shows the blocked phone number")]
        [UIHint("PhoneNumber"), StringLength(Globals.MaxPhoneNumber), ReadOnly]
        public string? Number { get; set; }

        [Caption("Description"), Description("The description of the blocked number")]
        [UIHint("TextAreaSourceOnly"), StringLength(BlockedNumberEntry.MaxDescription)]
        public string? Description { get; set; }

        public BlockedNumberEntry GetData(BlockedNumberEntry data) {
            ObjectSupport.CopyData(this, data);
            return data;
        }

        [UIHint("Hidden"), ReadOnly]
        public string OriginalNumber { get; set; } = null!;

        public void SetData(BlockedNumberEntry data) {
            ObjectSupport.CopyData(data, this);
            OriginalNumber = data.Number;
        }
        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        string number = Manager.RequestQueryString["Number"] ?? throw new InternalError($"Argument {nameof(number)} missing");
        using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
            BlockedNumberEntry? data = await dataProvider.GetItemAsync(number);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "Blocked phone number \"{0}\" not found"), number);
            EditModel model = new EditModel { };
            model.SetData(data);
            Title = this.__ResStr("modTitle", "Blocked Phone Number \"{0}\"", number);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        using (BlockedNumberDataProvider dataProvider = new BlockedNumberDataProvider()) {
            BlockedNumberEntry? data = await dataProvider.GetItemAsync(model.OriginalNumber);// get the original item
            if (data == null) {
                ModelState.AddModelError("Number", this.__ResStr("alreadyDeleted", "Blocked number {0} has been removed and can no longer be updated", model.OriginalNumber));
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
                    ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "Blocked number {0} has been removed and can no longer be updated", model.OriginalNumber));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "Blocked number {0} already exists", model.Number));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "Blocked number saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
