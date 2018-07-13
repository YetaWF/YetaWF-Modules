/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

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
using YetaWF.Modules.Panels.Models;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Components {

    public abstract class ListOfLocalPagesComponentBase : YetaWFComponent {

        public const string TemplateName = "ListOfLocalPages";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfLocalPagesEditComponent : ListOfLocalPagesComponentBase, IYetaWFComponent<List<LocalPage>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class NewModel {
            [Caption("Page"), Description("Please select a page and click Add to add it to the list of pages")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), Trim]
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
            public string UrlDisplay { get { return Url; } set { Url = value; } }

            [Caption("Popup"), Description("Defines whether the page is shown in a popup window")]
            [UIHint("Boolean")]
            public bool Popup { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string Url { get; set; } // this is used so we have the pure url (without input field)

            public GridEntryEdit(LocalPage page) {
                Url = page.Url;
                Popup = page.Popup;
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

            [Caption("Popup"), Description("Defines whether the page is shown in a popup window")]
            [UIHint("Boolean"), ReadOnly]
            public bool Popup { get; set; }

            [Caption("Page Skin"), Description("The skin used to display the page")]
            [UIHint("PageSkin"), ReadOnly]
            public SkinDefinition SelectedSkin { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string RawUrl { get { return Url; } }

            public GridAllEntry(PageDefinition page) {
                ObjectSupport.CopyData(page, this);
            }
        }
        public async Task<YHtmlString> RenderAsync(List<LocalPage> model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tmpltId = UniqueId();

            hb.Append($@"
<div class='yt_yetawf_panels_listoflocalpages t_edit' id='{DivId}'>
    <div class='yt_grid_addordelete' id='{tmpltId}'
        data-dupmsg='{this.__ResStr("dupmsg", "Page {0} has already been added")}'
        data-addedmsg='{this.__ResStr("addedmsg", "Page {0} has been added")}'
        data-remmsg='{this.__ResStr("remmsg", "Page {0} has been removed")}'>");

            model = model ?? new List<LocalPage>();
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
                    DeleteProperty = nameof(GridEntryEdit.Url),
                    DisplayProperty = nameof(GridEntryEdit.Url)
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
    YetaWF_Basics.expandCollapseHandling('{DivId}', '{DivId}_coll', '{DivId}_exp');");

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
