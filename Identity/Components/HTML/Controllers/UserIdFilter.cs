/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Components;

namespace YetaWF.Modules.Identity.Controllers {

    public class UserIdFilterModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UserIdFilterModule> {

        public UserIdFilterModuleController() { }

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

        [AllowGet]
        public ActionResult UserIdFilter(string data, string filterId, string uiHint) {

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
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult UserIdFilter_Partial(EditModel model) {

            UserIdDisplayComponent.UserIdFilterData filterData = null;

            if (!ModelState.IsValid)
                return PartialView(model);

            if (model.UserId != 0) {
                filterData = new UserIdDisplayComponent.UserIdFilterData {
                    FilterOp = model.FilterOp == EditModel.FilterOpEnum.Equal ? "==" : "!=",
                    UserId = model.UserId,
                };
            }
            return FormProcessed(model, OnPopupClose: OnPopupCloseEnum.ReloadNothing, OnClose: OnCloseEnum.Return, PostSaveJavaScript: $@"YetaWF_ComponentsHTML.Grid.updateComplexFilter('{model.FilterId}', {(filterData != null ? Utility.JsonSerialize(filterData) : null)})");
        }
    }
}
