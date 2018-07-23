/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Pages.Controllers;
using YetaWF.Core.Skins;
using YetaWF.Core.Pages;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Components {

    public abstract class ListOfLocalPagesComponentBase : YetaWFComponent {

        public const string TemplateName = "ListOfLocalPages";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfLocalPagesDisplayComponent : ListOfLocalPagesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridEntryDisplay {

            [Caption("Page"), Description("Shows all page Urls that are part of the Unified Page Set")]
            [UIHint("String"), ReadOnly]
            public string Url { get; set; }

            public GridEntryDisplay(string url) {
                Url = url;
            }
        }

        public async Task<YHtmlString> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_listoflocalpages t_display'>");

            model = model ?? new List<string>();
            List<GridEntryDisplay> list = (from u in model select new GridEntryDisplay(u)).ToList();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEntryDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 20,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid"));

            hb.Append(@"
</div>");
            return hb.ToYHtmlString();
        }
    }

    public class ListOfLocalPagesEditComponent : ListOfLocalPagesComponentBase, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class NewModel {
            [Caption("Page"), Description("Please select a page and click Add to add it to the list of pages")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlTypeEnum.Local), Trim]
            public string NewValue { get; set; }
        }
        [Trim]
        public class GridEntryEdit {

            public GridEntryEdit() { }

            [Caption("Delete"), Description("Click to remove this page from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("Page"), Description("Shows all pages")]
            [UIHint("Url"), ReadOnly]
            public string Url { get { return __Value; } set { __Value = value; } }

            [UIHint("Text80"), StringLength(Globals.MaxUrl), Required, Trim]
            public string __Value { get; set; }
            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return __Value; } }

            public GridEntryEdit(string url) {
                Url = url;
            }
        }
        [Trim]
        public class GridAllEntry {

            public GridAllEntry() { }

            [Caption("Page"), Description("Defines the page name")]
            [UIHint("Url"), ReadOnly]
            public string Url { get; set; }

            [Caption("Title"), Description("The page title which will appear as title in the browser window")]
            [UIHint("MultiString"), ReadOnly]
            public MultiString Title { get; set; }

            [Caption("Page Skin"), Description("The skin used to display the page")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition SelectedSkin { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUrl { get { return Url; } }

            public GridAllEntry(PageDefinition page) {
                ObjectSupport.CopyData(page, this);
            }
        }
        public async Task<YHtmlString> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_listoflocalpages t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{this.__ResStr("dupmsg", "Page {0} has already been added")}'
        data-addedmsg='{this.__ResStr("addedmsg", "Page {0} has been added")}'
        data-remmsg='{this.__ResStr("remmsg", "Page {0} has been removed")}'>");

            model = model ?? new List<string>();
            List<GridEntryEdit> list = (from u in model select new GridEntryEdit(u)).ToList();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEntryEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 0,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = nameof(GridEntryEdit.__TextKey),
                    DisplayProperty = nameof(GridEntryEdit.__TextDisplay)
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            using (Manager.StartNestedComponent(FieldName)) {

                string ajaxUrl = GetSiblingProperty<string>($"{PropertyName}_AjaxUrl");
                if (ajaxUrl == null) throw new InternalError($"{PropertyName}_AjaxUrl property not set");

                NewModel newModel = new NewModel();
                hb.Append("<div class='t_newvalue'>");
                hb.Append(await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue)));
                hb.Append(await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue)));
                hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.HtmlAttributeEncode(ajaxUrl));
                hb.Append("</div>");
            }

            hb.Append(@"
    </div>");

            hb.Append($@"
    <div id='{DivId}_coll'>
        {await ModuleActionHelper.BuiltIn_ExpandAction(this.__ResStr("lblFindPages", "Find Pages"), this.__ResStr("ttFindPages", "Expand to find pages available for this site")).RenderAsNormalLinkAsync() }
    </div>
    <div id='{DivId}_exp' style='display:none'>
        {await ModuleActionHelper.BuiltIn_CollapseAction(this.__ResStr("lblAllPages", "All Pages"), this.__ResStr("ttAllPages", "Shows all designed pages available in the site - Select a page to update the dropdown list above, so the page can be added to the list of pages")).RenderAsNormalLinkAsync() }");

            grid = new GridModel() {
                GridDef = new GridDefinition() {
                    AjaxUrl = YetaWFManager.UrlFor(typeof(ListOfLocalPagesController), nameof(ListOfLocalPagesController.ListOfLocalPagesBrowse_GridData)),
                    Id = DivId + "_listall",
                    RecordType = typeof(GridAllEntry),
                    ShowHeader = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid"));

            hb.Append($@"
    </div>
</div>
<script>
    $YetaWF.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');");

            using (DocumentReady(hb, DivId)) {
        hb.Append($@"
    $('#{tmpltId}_list').jqGrid('sortableRows', {{ connectWith: '#{tmpltId}_list' }});
    YetaWF_Pages_ListOfLocalPages.init($('#{tmpltId}'), $('#{tmpltId}_list'), $('#{DivId}_listall'), $('#{tmpltId} .yt_url.t_edit[data-name$=\'.NewValue\']'));
");
            }

            hb.Append($@"
</script>");
            return hb.ToYHtmlString();
        }
    }
}
