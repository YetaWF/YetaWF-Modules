/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Feedback.DataProvider;
using YetaWF.Modules.Feedback.Modules;

namespace YetaWF.Modules.Feedback.Controllers {

    public class FeedbackBrowseModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    FeedbackDisplayModule dispMod = new FeedbackDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_RemoveFeedback(Key), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }
            [Caption("Created"), Description("The date the message was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Email Address"), Description("The user's email address")]
            [UIHint("String"), ReadOnly]
            public string Email { get; set; }

            [Caption("Subject"), Description("The subject of the message")]
            [UIHint("String"), ReadOnly]
            public string Subject { get; set; }

            [Caption("IP Address"), Description("The IP address from which the feedback message was sent")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }

            [Caption("Message"), Description("The feedback message")]
            [UIHint("String"), ReadOnly]
            public string Message { get; set; }

            private int Key { get; set; }
            private FeedbackBrowseModule Module { get; set; }

            public BrowseItem(FeedbackBrowseModule module, FeedbackData data) {
                Module = module;
                Key = data.Key;
                ObjectSupport.CopyData(data, this);
                if (data.Message.Length > 100)
                    Message = data.Message.Substring(0, 100) + this.__ResStr("more", "...more");
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult FeedbackBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("FeedbackBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeedbackBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (FeedbackDataDataProvider dataProvider = new FeedbackDataDataProvider()) {
                int total;
                List<FeedbackData> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }
        [HttpPost]
        [Permission("RemoveFeedback")]
        [ExcludeDemoMode]
        public ActionResult RemoveFeedback(int key) {
            using (FeedbackDataDataProvider dataProvider = new FeedbackDataDataProvider()) {
                dataProvider.RemoveItem(key);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }
    }
}