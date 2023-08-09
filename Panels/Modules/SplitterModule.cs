/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Search;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Panels.Models;

namespace YetaWF.Modules.Panels.Modules;

public class SplitterModuleDataProvider : ModuleDefinitionDataProvider<Guid, SplitterModule>, IInstallableModel { }

[ModuleGuid("{75C8D7C9-CE51-4d1b-A698-DEEC6757FA03}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SplitterModule : ModuleDefinition, ISearchDynamicUrls, ISiteMapDynamicUrls {

    public SplitterModule() {
        Title = this.__ResStr("modTitle", "Splitter");
        Name = this.__ResStr("modName", "Splitter");
        Description = this.__ResStr("modSummary", "Used to display two modules side by side.");
        Height = 0;
        MinWidth = 100;
        Width = 20;
        ModuleLeft = Guid.Empty;
        ModuleRight = Guid.Empty;
        WantFocus = true;
        WantSearch = true;
        ShowTitle = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SplitterModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }


    [Category("General"), Caption("Left Title"), Description("Defines the title shown in the left pane")]
    [UIHint("Text40"), StringLength(100), Trim]
    [Data_NewValue]
    public string? TitleText { get; set; }
    [Category("General"), Caption("Left Title Tooltip"), Description("Defines the tooltip shown for the title in the left pane")]
    [UIHint("Text80"), StringLength(200), Trim]
    [Data_NewValue]
    public string? TitleTooltip { get; set; }
    [Category("General"), Caption("Collapse Text"), Description("Defines the text shown next to the collapse icon")]
    [UIHint("Text40"), StringLength(100), Trim]
    [Data_NewValue]
    public string? CollapseText { get; set; }
    [Category("General"), Caption("Collapse Tooltip"), Description("Defines the tooltip shown for the collapse text and icon")]
    [UIHint("Text40"), StringLength(200), Trim]
    [Data_NewValue]
    public string? CollapseToolTip { get; set; }
    [Category("General"), Caption("Expand Tooltip"), Description("Defines the tooltip shown for the expand icon")]
    [UIHint("Text40"), StringLength(200), Trim]
    [Data_NewValue]
    public string? ExpandToolTip { get; set; }

    [Category("General"), Caption("Container Selector"), Description("Defines a selector used to locate the container - If found, the height of the splitter module is based on the container height and the Height property is ignored")]
    [UIHint("Text40"), StringLength(100)]
    [Data_NewValue]
    public string? ContainerSelector { get; set; }

    [Category("General"), Caption("Height (Pixels)"), Description("Defines the height of the module in pixels - Set to 0 to calculate the height automatically to fill the remainder of the page - This property is ignored if the ContainerSelector defines a container which can be found")]
    [UIHint("IntValue4"), Range(0, 9999), Required]
    [Data_NewValue]
    public int Height { get; set; }

    [Category("General"), Caption("Module (Left)"), Description("The module rendered on the left side")]
    [UIHint("ModuleSelection"), AdditionalMetadata("New", false), AdditionalMetadata("EditSettings", true), Required]
    public Guid ModuleLeft { get; set; }

    [Category("General"), Caption("Minimum Width (Left, Pixels)"), Description("Defines the minimum width in pixels")]
    [UIHint("IntValue4"), Range(0,9999), Required]
    public int MinWidth { get; set; }

    [Category("General"), Caption("Width (Left, Percentage)"), Description("Defines the desired width in percent of the entire window")]
    [UIHint("IntValue4"), Range(0, 100), Required]
    public int Width { get; set; }

    [Category("General"), Caption("Module (Right)"), Description("The module rendered on the right side, filling the remainder of the available width")]
    [UIHint("ModuleSelection"), AdditionalMetadata("New", false), AdditionalMetadata("EditSettings", true), Required]
    public Guid ModuleRight { get; set; }

    // ISEARCHDYNAMICURLS
    // ISEARCHDYNAMICURLS
    // ISEARCHDYNAMICURLS

    public async Task KeywordsForDynamicUrlsAsync(ISearchWords searchWords) {

        List<PageDefinition> pages = await PageDefinition.GetPagesFromModuleAsync(ModuleGuid);
        if (pages.Count == 0) return;

        ISearchPageDynamicUrls? iSearchLeft = null;
        if (ModuleLeft != Guid.Empty) {
            ModuleDefinition? modLeft = await ModuleDefinition.LoadAsync(ModuleLeft, AllowNone: true);
            if (modLeft != null)
                iSearchLeft = modLeft as ISearchPageDynamicUrls;
        }
        ISearchPageDynamicUrls? iSearchRight = null;
        if (ModuleRight != Guid.Empty) {
            ModuleDefinition? modRight = await ModuleDefinition.LoadAsync(ModuleRight, AllowNone: true);
            if (modRight != null)
                iSearchRight = modRight as ISearchPageDynamicUrls;
        }

        if (iSearchLeft != null || iSearchRight != null) {
            foreach (PageDefinition page in pages) {
                if (searchWords.WantPage(page)) {
                    iSearchLeft?.KeywordsForDynamicUrlsAsync(page, searchWords);
                    iSearchRight?.KeywordsForDynamicUrlsAsync(page, searchWords);
                }
            }
        }
    }

    //// ISITEMAPDYNAMICURLS
    //// ISITEMAPDYNAMICURLS
    //// ISITEMAPDYNAMICURLS

    public async Task FindDynamicUrlsAsync(AddDynamicUrlAsync addDynamicUrlAsync, Func<PageDefinition, bool> validForSiteMap) {

        List<PageDefinition> pages = await PageDefinition.GetPagesFromModuleAsync(ModuleGuid);
        if (pages.Count == 0) return;

        ISiteMapPageDynamicUrls? iSearchLeft = null;
        if (ModuleLeft != Guid.Empty) {
            ModuleDefinition? modLeft = await ModuleDefinition.LoadAsync(ModuleLeft, AllowNone: true);
            if (modLeft != null)
                iSearchLeft = modLeft as ISiteMapPageDynamicUrls;
        }
        ISiteMapPageDynamicUrls? iSearchRight = null;
        if (ModuleRight != Guid.Empty) {
            ModuleDefinition? modRight = await ModuleDefinition.LoadAsync(ModuleRight, AllowNone: true);
            if (modRight != null)
                iSearchRight = modRight as ISiteMapPageDynamicUrls;
        }

        if (iSearchLeft != null || iSearchRight != null) {
            foreach (PageDefinition page in pages) {
                if (validForSiteMap(page)) {
                    iSearchLeft?.FindDynamicUrlsAsync(page, addDynamicUrlAsync, validForSiteMap);
                    iSearchRight?.FindDynamicUrlsAsync(page, addDynamicUrlAsync, validForSiteMap);
                }
            }
        }
    }

    [Trim]
    public class Model {
        public Model() { }
        [UIHint("YetaWF_Panels_SplitterInfo")]
        public SplitterInfo SplitterInfo { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model {
            SplitterInfo = new SplitterInfo {
                TitleText = TitleText,
                TitleTooltip = TitleTooltip,
                CollapseText = CollapseText,
                CollapseToolTip = CollapseToolTip,
                ExpandToolTip = ExpandToolTip,

                ContainerSelector = ContainerSelector,
                Height = Height,
                ModuleLeft = ModuleLeft,
                MinWidth = MinWidth,
                Width = Width,
                ModuleRight = ModuleRight,
            }
        };
        return await RenderAsync(model);
    }
}
