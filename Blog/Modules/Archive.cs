/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class ArchiveModuleDataProvider : ModuleDefinitionDataProvider<Guid, ArchiveModule>, IInstallableModel { }

[ModuleGuid("{2b4f7842-370b-4a03-aa09-4e1341f7b87c}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Navigation")]
public class ArchiveModule : ModuleDefinition {

    public ArchiveModule() {
        Title = this.__ResStr("modTitle", "Blog Archive");
        Name = this.__ResStr("modName", "Archive");
        Description = this.__ResStr("modSummary", "Displays a list of links to blog entries, grouped by month. Add this module to a page for blog navigation.");
        Print = false;
        DefaultViewName = StandardViews.PropertyListEdit;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new ArchiveModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public class DisplayModel {

        [Caption("Archive"), Description("Monthly blog entries")]
        [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
        public List<ModuleAction> Actions { get; set; }

        public DisplayModel() {
            Actions = new List<ModuleAction>();
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(int blogCategory) {

        int category = blogCategory;
        BlogModule blogMod = new BlogModule();
        DisplayModel model = new DisplayModel() { };

        using (BlogEntryDataProvider entryDP = new BlogEntryDataProvider()) {
            List<DataProviderSortInfo> sort = new List<DataProviderSortInfo> {
                new DataProviderSortInfo { Field = nameof(BlogEntry.DatePublished), Order = DataProviderSortInfo.SortDirection.Descending },
            };
            List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo>{
                new DataProviderFilterInfo { Field = nameof(BlogEntry.Published), Operator = "==", Value = true },
            };
            if (category != 0)
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(BlogEntry.CategoryIdentity), Operator = "==", Value = category });

            int year = 0, month = 0, count = 0;

            int totalRecs = 0, start = 0, incr = 100;
            for (; ; ) {

                DataProviderGetRecords<BlogEntry> data = await entryDP.GetItemsAsync(start, incr, sort, filters);
                if (data.Data.Count == 0)
                    return ActionInfo.Empty;

                foreach (BlogEntry entry in data.Data) {
                    if (entry.DatePublished.Month != month || entry.DatePublished.Year != year) {
                        if (count > 0) {
                            DateTime d = new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
                            model.Actions.New(await blogMod.GetAction_BlogAsync(null, category, StartDate: d, Count: count));
                            count = 0;
                        }
                        month = entry.DatePublished.Month;
                        year = entry.DatePublished.Year;
                    }
                    ++count;
                }
                start += incr;
                if (start >= totalRecs) {
                    if (count > 0) {
                        DateTime d = new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
                        model.Actions.New(await blogMod.GetAction_BlogAsync(null, category, StartDate: d, Count: count));
                    }
                    break;
                }
            }
            return await RenderAsync(model);
        }
    }
}
