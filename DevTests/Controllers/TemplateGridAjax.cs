/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.DevTests.Endpoints;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateGridAjaxModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateGridAjaxModule> {

        public TemplateGridAjaxModuleController() { }

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();
                    TemplateGridAjaxModule gridAjaxMod = new TemplateGridAjaxModule();
                    actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);

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
            public string ShortName { get; set; } = null!;

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
            public string Description { get; set; } = null!;

            [Caption("Hidden"), Description("A hidden field")]
            [UIHint("Hidden"), ReadOnly]
            public string Hidden { get; set; } = null!;

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
            public GridDefinition GridDef { get; set; } = null!;
        }
        public class EntryElement {
            public int Id { get; set; }
            public decimal Decimal { get; set; }
            public bool BoolVal { get; set; }
            public string ShortName { get; set; } = null!;
            public string ShortName2 { get { return ShortName; } }
            public string Hidden { get; set; } = null!;
            public string Description { get; set; } = null!;
            public DateTime SomeDateTime { get; set; }
            public DateTime SomeDate { get; set; }
            public Guid Guid { get; set; }
            public ButtonTypeEnum SomeEnum { get; set; }
        }

        internal static GridDefinition GetGridModel(ModuleDefinition module) {

            List<ModuleAction> actions = new List<ModuleAction>();
            TemplateGridAjaxModule gridAjaxMod = new TemplateGridAjaxModule();
            actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);
#if NOT
            actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_Dashboard(), ModuleAction.ActionLocationEnum.GridLinks);
            actions.New(gridAjaxMod.GetAction_User(), ModuleAction.ActionLocationEnum.GridLinks);
#endif
            return new GridDefinition {
                InitialPageSize = 10,
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<TemplateGridAjaxModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<EntryElement> browseItems = DataProviderImpl<EntryElement>.GetRecords(GetRandomData(), skip, take, sort, filters);
                    return Task.FromResult(new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                        Total = browseItems.Total
                    });
                },
                PageSizes = new List<int> { 10, 20, GridDefinition.AllPages },
                PanelHeader = true,
                PanelHeaderTitle = "Test Grid",
                PanelCanMinimize = true,
                PanelHeaderAutoSearch = 300,
                PanelHeaderColumnSelection = true,
                PanelHeaderActions = actions,
                PanelHeaderSearch = true,
                PanelHeaderSearchColumns = new List<string> { nameof(BrowseItem.ShortName), nameof(BrowseItem.Description) },
                PanelHeaderSearchTT = "Enter text to search in the ShortName and Description column",
            };
        }

        [AllowGet]
        public ActionResult TemplateGridAjax() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }

        const int MaxRecords = 100;
        private static List<Guid>? Guids = null;

        private static List<EntryElement> GetRandomData() {
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
