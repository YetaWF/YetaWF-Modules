/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Modules;
using System.Linq;
using System;
using YetaWF.Modules.DevTests.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateGridAjaxModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateGridAjaxModule> {

        public TemplateGridAjaxModuleController() { }

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    TemplateGridModule gridMod = new TemplateGridModule();
                    TemplateGridAjaxModule gridAjaxMod = new TemplateGridAjaxModule();
                    actions.New(gridMod.GetAction_Display(null), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(gridAjaxMod.GetAction_Display(null), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("Id"), Description("Some id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }

            [Caption("Bool"), Description("Some bool")]
            [UIHint("Boolean"), ReadOnly]
            public bool BoolVal { get; set; }

            [Caption("Decimal"), Description("Some decimal")]
            [UIHint("Decimal"), ReadOnly]
            public decimal Decimal { get; set; }

            [Caption("ShortName"), Description("Some string")]
            [UIHint("String"), ReadOnly]
            public string ShortName { get; set; }

            [Caption("Date/Time"), Description("Some date and time")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime SomeDateTime { get; set; }

            [Caption("Date"), Description("Some date")]
            [UIHint("Date"), ReadOnly]
            public DateTime SomeDate { get; set; }

            [Caption("Enum"), Description("Some Enum")]
            [UIHint("Enum"), ReadOnly]
            public ButtonTypeEnum SomeEnum { get; set; }

            [Caption("Guid"), Description("Some Guid")]
            [UIHint("Guid"), ReadOnly]
            public Guid Guid { get; set; }

            [Caption("Description"), Description("Description")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Hidden"), Description("A hidden field")]
            [UIHint("Hidden"), ReadOnly]
            public string Hidden { get; set; }

            [Caption("ShortName 2"), Description("Some string")]
            [UIHint("String"), ReadOnly]
            public string ShortName2 { get { return ShortName; } }

            public BrowseItem(EntryElement data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [Caption(""), Description("")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        public class EntryElement {
            public int Id { get; set; }
            public decimal Decimal { get; set; }
            public bool BoolVal { get; set; }
            public string ShortName { get; set; }
            public string ShortName2 { get { return ShortName; } }
            public string Hidden { get; set; }
            public string Description { get; set; }
            public DateTime SomeDateTime { get; set; }
            public DateTime SomeDate { get; set; }
            public Guid Guid { get; set; }
            public ButtonTypeEnum SomeEnum { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(TemplateGridAjax_GridData)),
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<EntryElement> browseItems = DataProviderImpl<EntryElement>.GetRecords(GetRandomData(), skip, take, sort, filters);
                    return Task.FromResult(new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                        Total = browseItems.Total
                    });
                },
            };
        }

        [AllowGet]
        public ActionResult TemplateGridAjax() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> TemplateGridAjax_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        const int MaxRecords = 300;
        private static List<Guid> Guids = null;

        private List<EntryElement> GetRandomData() {
            if (Guids == null) {
                Guids = new List<Guid>();
                for (int i = 0; i < MaxRecords; ++i) {
                    Guids.Add(Guid.NewGuid());
                }
            }
            List<EntryElement> elements = new List<EntryElement>();
            for (int i = 0; i < MaxRecords; ++i) {
                elements.Add(new EntryElement {
                    Id = i,
                    Decimal = i,
                    ShortName = $"Name {i}",
                    Hidden = i.ToString(),
                    Description = $"Longer description for item {i}",
                    SomeDateTime = DateTime.UtcNow.AddDays(i),
                    SomeDate = DateTime.UtcNow.AddDays(i),
                    Guid = Guids[i],
                    SomeEnum = (ButtonTypeEnum)(i % 5),
                });
            }
            return elements;
        }
    }
}
