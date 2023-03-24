/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Modules;

public class SummaryModuleDataProvider : ModuleDefinitionDataProvider<Guid, SummaryModule>, IInstallableModel { }

[ModuleGuid("{2b2c61b6-8f0c-4f39-b927-e09f5e118d86}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Navigation")]
public class SummaryModule : ModuleDefinition {

    public SummaryModule() {
        Title = this.__ResStr("modTitle", "Blog Summary");
        Name = this.__ResStr("modName", "Blog Summary");
        Description = this.__ResStr("modSummary", "Displays a summary of blog entries. The most recent blog entries for all categories are shown. This module is typically used in a sidebar on a page showing blog entries (using the Blog Entries Module).");
        Entries = 20;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SummaryModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Category("General"), Caption("Total Entries"), Description("The maximum number of blog entries shown in the summary display")]
    [UIHint("IntValue4"), Range(1, 9999), Required]
    public int Entries { get; set; }

    public class Entry {

        public int Identity { get; set; }

        //[Caption("Title"), Description("The title for this blog entry")]
        //[UIHint("MultiString"), ReadOnly]
        public MultiString Title { get; set; } = null!;

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
        public List<Entry> BlogEntries { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
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
            DataProviderGetRecords<BlogEntry> data = await dataProvider.GetItemsAsync(0, Entries, sort, filters);
            if (data.Data.Count == 0)
                return ActionInfo.Empty;

            EntryDisplayModule dispMod = new EntryDisplayModule();
            List<Entry> list = new List<Entry>();
            foreach (BlogEntry d in data.Data) {
                ModuleAction viewAction = await dispMod.GetAction_DisplayAsync(d.Identity);
                list.Add(new Entry(d, dispMod, viewAction));
            }
            DisplayModel model = new DisplayModel() {
                BlogEntries = list,
            };
            return await RenderAsync(model);
        }
    }
}
