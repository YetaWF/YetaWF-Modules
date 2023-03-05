/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Http;
using Softelvdm.Modules.IVR.DataProvider;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace Softelvdm.Modules.IVR.Modules;

public class AddHolidayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddHolidayModule>, IInstallableModel { }

[ModuleGuid("{0411e732-cf74-4950-8a1e-545566105f7a}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AddHolidayModule : ModuleDefinition2 {

    public AddHolidayModule() {
        Title = this.__ResStr("modTitle", "Add New Holiday");
        Name = this.__ResStr("modName", "Add New Holiday Entry");
        Description = this.__ResStr("modSummary", "Adds a new holiday.");
        DefaultViewName = StandardViews.Add;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddHolidayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Add a new holiday entry"),
            Legend = this.__ResStr("addLegend", "Adds a new holiday entry"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    public class AddModel {

        [Caption("Date"), Description("The date the holiday occurs")]
        [UIHint("Date")]
        public DateTime HolidayDate { get; set; }

        [Caption("Description"), Description("The description of the holiday")]
        [UIHint("Text80"), StringLength(HolidayEntry.MaxDescription), Trim]
        public string? Description { get; set; }

        public AddModel() { }

        public HolidayEntry GetData() {
            HolidayEntry data = new HolidayEntry();
            ObjectSupport.CopyData(this, data);
            return data;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        AddModel model = new AddModel { };
        ObjectSupport.CopyData(new HolidayEntry() {
            HolidayDate = DateTime.UtcNow
        }, model);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(AddModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using (HolidayEntryDataProvider dataProvider = new HolidayEntryDataProvider()) {
            if (!await dataProvider.AddItemAsync(model.GetData())) {
                ModelState.AddModelError(nameof(AddModel.HolidayDate), this.__ResStr("dup", "An entry for {0} already exists", Formatting.FormatDate(model.HolidayDate)));
                return await PartialViewAsync(model);
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New holiday saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
