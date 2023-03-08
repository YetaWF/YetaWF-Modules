/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Components;

namespace YetaWF.Modules.Identity.Modules;

public class UserIdFilterModuleDataProvider : ModuleDefinitionDataProvider<Guid, UserIdFilterModule>, IInstallableModel { }

[ModuleGuid("{63B188F3-BF70-438e-942C-F397FC0DD88D}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Filter")]
public class UserIdFilterModule : ModuleDefinition2 {

    public UserIdFilterModule() : base() {
        Title = this.__ResStr("modTitle", "User Filter");
        Name = this.__ResStr("modName", "UserId Filter");
        Description = this.__ResStr("modSummary", "Implements a filter for a user id.");
        DefaultViewName = StandardViews.Edit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new UserIdFilterModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Edit() {
        return new ModuleAction() {
            Url = ModulePermanentUrl,
            LinkText = this.__ResStr("editLink", "Filter"),
            MenuText = this.__ResStr("editText", "Filter"),
            Tooltip = this.__ResStr("editTooltip", "Use a filter to limit data shown to a certain user"),
            Legend = this.__ResStr("editLegend", "Limits data shown to a certain user"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.Explicit,
        };
    }

    [Trim]
    public class EditModel {

        public enum FilterOpEnum {
            [EnumDescription("Equal", "Only show users that match the selected user")]
            Equal = 0,
            [EnumDescription("Not Equal", "Only show users that do not match the selected user")]
            NotEqual = 1,
        }

        [Caption("Comparison"), Description("Select the comparison method")]
        [UIHint("Enum")]
        public FilterOpEnum FilterOp { get; set; }

        [Caption("User"), Description("Select the user")]
        [UIHint("YetaWF_Identity_UserId"), AdditionalMetadata("Force", "Grid"), SelectionRequired]
        public int UserId { get; set; }

        public bool __submitShown { get { return true; } }
        public string __submit { get { return this.__ResStr("save", "Save Filter"); } }
        public string __submitTT { get { return this.__ResStr("saveTT", "Click to save the selected user as the new filter setting"); } }

        [UIHint("Hidden"), ReadOnly]
        public string FilterId { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync(string data, string filterId) {
        // string uiHint = Manager.RequestQueryString["UiHint"];

        UserIdDisplayComponent.UserIdFilterData filterData;
        if (data != null)
            filterData = Utility.JsonDeserialize<UserIdDisplayComponent.UserIdFilterData>(data);
        else
            filterData = new UserIdDisplayComponent.UserIdFilterData();

        EditModel model = new EditModel {
            FilterId = filterId,
            FilterOp = filterData.FilterOp == "==" ? EditModel.FilterOpEnum.Equal : EditModel.FilterOpEnum.NotEqual,
            UserId = filterData.UserId,
        };
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {

        UserIdDisplayComponent.UserIdFilterData filterData = null;

        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (model.UserId != 0) {
            filterData = new UserIdDisplayComponent.UserIdFilterData {
                FilterOp = model.FilterOp == EditModel.FilterOpEnum.Equal ? "==" : "!=",
                UserId = model.UserId,
            };
        }
        return await FormProcessedAsync(model, OnPopupClose: OnPopupCloseEnum.ReloadNothing, OnClose: OnCloseEnum.Return, PostSaveJavaScript: $@"YetaWF_ComponentsHTML.Grid.updateComplexFilter('{model.FilterId}', {(filterData != null ? Utility.JsonSerialize(filterData) : null)})");
    }
}
