/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Language;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Languages.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Languages.Controllers {

    public class LanguagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LanguagesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    LanguageDisplayModule dispMod = new LanguageDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Id), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("ID"), Description("The language id - this is the same as the culture name used throughout .NET")]
            [UIHint("String"), ReadOnly]
            public string Id { get; set; }

            [Caption("Name"), Description("The language's short name, which is displayed in language selection controls so the user can select a language")]
            [UIHint("String"), ReadOnly]
            public string ShortName { get; set; }

            [Caption("Description"), Description("The description for the language - this is used for informational purposes only")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

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
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult LanguagesBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("LanguagesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> LanguagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            DataProviderGetRecords<LanguageEntryElement> browseItems = DataProviderImpl<LanguageEntryElement>.GetRecords(LanguageSection.Languages, skip, take, sort, filters);
            Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return await GridPartialViewAsync(new DataSourceResult {
                Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                Total = browseItems.Total
            });
        }
    }
}