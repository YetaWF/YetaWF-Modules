/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Modules;
using YetaWF.Core.Localize;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Controllers {

    public class SummaryModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.SummaryModule> {

        public SummaryModuleController() { }

        public class Entry {

            public int Identity { get; set; }

            //[Caption("Title"), Description("The title for this blog entry")]
            //[UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("View"), Description("View the complete blog entry")]
            [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.LinksOnly), ReadOnly]
            public ModuleAction ViewAction { get; set; }

            public Entry(BlogEntry data, EntryDisplayModule dispMod, ModuleAction viewAction) {
                ObjectSupport.CopyData(data, this);
                ViewAction = viewAction;
                ViewAction.LinkText = Title;
                ViewAction.Tooltip = this.__ResStr("viewTT", "Published {0} - {1}", Formatting.FormatDate(data.DatePublished), data.DisplayableSummaryText);
            }
        }

        public class DisplayModel {
            public List<Entry> BlogEntries { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> Summary() {
            //int category;
            //Manager.TryGetUrlArg<int>("BlogCategory", out category);
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            using (BlogEntryDataProvider dataProvider = new BlogEntryDataProvider()) {
                List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                    new DataProviderSortInfo { Field = nameof(BlogEntry.DatePublished), Order = DataProviderSortInfo.SortDirection.Descending },
                };
                List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo>{
                    new DataProviderFilterInfo { Field = nameof(BlogEntry.Published), Operator = "==", Value = true },
                };
                //if (category != 0)
                //    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.CategoryIdentity), Operator = "==", Value = category });
                DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, Module.Entries, sort, filters);
                if (data.Data.Count == 0)
                    return new EmptyResult();

                EntryDisplayModule dispMod = new EntryDisplayModule();
                List<Entry> list = new List<Entry>();
                foreach (BlogEntry d in data.Data) {
                    ModuleAction viewAction = await dispMod.GetAction_DisplayAsync(d.Identity);
                    list.Add(new Entry(d, dispMod, viewAction));
                }
                DisplayModel model = new DisplayModel() {
                    BlogEntries = list,
                };
                return View(model);
            }
        }
    }
}
