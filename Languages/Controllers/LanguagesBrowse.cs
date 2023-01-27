/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Language;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Languages.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Languages.Controllers {

    public class LanguagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LanguagesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    LanguageDisplayModule dispMod = new LanguageDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
            [UIHint("String"), ReadOnly]
            public string Id { get; set; } = null!;

            [Caption("Name"), Description("The language's short name, which is displayed in language selection controls so the user can select a language")]
            [UIHint("String"), ReadOnly]
            public string ShortName { get; set; } = null!;

            [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
            [UIHint("String"), ReadOnly]
            public string? Description { get; set; }

            private LanguagesBrowseModule Module { get; set; }

            public BrowseItem(LanguagesBrowseModule module, LanguageEntryElement data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        [Header("Languages are defined in the LanguageSettings.json file in the Data folder.")]
        public class BrowseModel {
            [Caption(""), Description("")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(LanguagesBrowse_GridData)),
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<LanguageEntryElement> browseItems = DataProviderImpl<LanguageEntryElement>.GetRecords(LanguageSection.Languages, skip, take, sort, filters);
                    return Task.FromResult(new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                        Total = browseItems.Total
                    });
                },
            };
        }

        [AllowGet]
        public ActionResult LanguagesBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> LanguagesBrowse_GridData(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync(GetGridModel(), gridPvData);
        }
    }
}