using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.DevTests.Components {

    public abstract class ListOfEmailAddressesComponent : YetaWFComponent {

        public const string TemplateName = "ListOfEmailAddresses";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ListOfEmailAddressesDisplayComponent : ListOfEmailAddressesComponent, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }
        public class GridDisplay {
            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("String"), ReadOnly]
            public string EmailAddress { get; set; }

            public GridDisplay(string text) {
                EmailAddress = text;
            }
        }
        public async Task<YHtmlString> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div class='yt_listofemailaddresses t_display'>");

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            List<GridDisplay> list = new List<GridDisplay>();
            if (model != null)
                list = (from u in model select new GridDisplay(u)).ToList();

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridDisplay),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = true,
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            hb.Append($"</div>");
            return hb.ToYHtmlString();
        }
    }
    public class ListOfEmailAddressesEditComponent : ListOfEmailAddressesComponent, IYetaWFComponent<List<string>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class GridModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }
        public class NewModel {
            [Caption("Email Address"), Description("Please enter a new email address and click Add")]
            [UIHint("Text80"), StringLength(80), Trim]
            public string NewValue { get; set; }
        }
        [Trim]
        public class GridEdit {

            public GridEdit() { }

            [Caption("Delete"), Description("Click to remove this email address from the list")]
            [UIHint("GridDeleteEntry")]
            public int DeleteMe { get; set; }

            [Caption("Email Address"), Description("Shows all defined email addresses")]
            [UIHint("Text80"), StringLength(Globals.MaxEmail), EmailValidation, ListNoDuplicates, Required, Trim]
            public string __Value { get; set; }

            [UIHint("Raw"), ReadOnly]
            public string __TextKey { get { return __Value; } }
            [UIHint("Raw"), ReadOnly]
            public string __TextDisplay { get { return __Value; } }

            public GridEdit(string text) {
                __Value = text;
            }
        }
        public async Task<YHtmlString> RenderAsync(List<string> model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_listofemailaddresses yt_grid_addordelete t_edit' id='{DivId}'
        data-dupmsg='{this.__ResStr("dupmsg", "Email address {0} has already been added")}'
        data-addedmsg='{this.__ResStr("addedmsg", "Email address {0} has been added")}'
        data-remmsg='{this.__ResStr("remmsg", "Email address {0} has been removed")}'>");

            List<GridEdit> list = new List<GridEdit>();
            if (model != null)
                list = (from u in model select new GridEdit(u)).ToList();

            bool header = PropData.GetAdditionalAttributeValue("Header", true);

            DataSourceResult data = new DataSourceResult {
                Data = list.ToList<object>(),
                Total = list.Count,
            };
            GridModel grid = new GridModel() {
                GridDef = new GridDefinition() {
                    RecordType = typeof(GridEdit),
                    Data = data,
                    SupportReload = false,
                    PageSizes = new List<int>(),
                    InitialPageSize = 5,
                    ShowHeader = header,
                    ReadOnly = false,
                    CanAddOrDelete = true,
                    DeleteProperty = "__TextKey",
                    DisplayProperty = "__TextDisplay"
                }
            };

            hb.Append(await HtmlHelper.ForDisplayAsAsync(Container, PropertyName, FieldName, grid, nameof(grid.GridDef), grid.GridDef, "Grid", HtmlAttributes: HtmlAttributes));

            using (Manager.StartNestedComponent(FieldName)) {

                string ajaxUrl = GetSiblingProperty<string>($"{PropertyName}_AjaxUrl");

                NewModel newModel = new NewModel();
                hb.Append("<div class='t_newvalue'>");
                hb.Append(await HtmlHelper.ForLabelAsync(newModel, nameof(newModel.NewValue)));
                hb.Append(await HtmlHelper.ForEditAsync(newModel, nameof(newModel.NewValue)));
                hb.Append("<input name='btnAdd' type='button' value='Add' data-ajaxurl='{0}' />", YetaWFManager.HtmlAttributeEncode(ajaxUrl));
                hb.Append("</div>");

            }
            hb.Append($@"
</div>
<script>");        

    using (DocumentReady(hb, DivId)) {
        hb.Append($@"YetaWF_Grid.setAddButtonStatus($('#{DivId}'));");
    }

    hb.Append($@"
</script>");
            return hb.ToYHtmlString();
        }
    }
}
