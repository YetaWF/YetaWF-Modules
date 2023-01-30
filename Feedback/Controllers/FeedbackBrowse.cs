/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.DataProvider;
using YetaWF.Modules.Feedback.Endpoints;
using YetaWF.Modules.Feedback.Modules;

namespace YetaWF.Modules.Feedback.Controllers {

    public class FeedbackBrowseModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    FeedbackDisplayModule dispMod = new FeedbackDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_RemoveFeedback(Key), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }
            [Caption("Created"), Description("The date the message was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Name"), Description("The user's name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;

            [Caption("Email Address"), Description("The user's email address")]
            [UIHint("String"), ReadOnly]
            public string Email { get; set; } = null!;

            [Caption("Subject"), Description("The subject of the message")]
            [UIHint("String"), ReadOnly]
            public string Subject { get; set; } = null!;

            [Caption("IP Address"), Description("The IP address from which the feedback message was sent")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; } = null!;

            [Caption("Message"), Description("The feedback message")]
            [UIHint("String"), ReadOnly]
            public string Message { get; set; } = null!;

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
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor< FeedbackBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
                        DataProviderGetRecords<FeedbackData> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((FeedbackBrowseModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult FeedbackBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}