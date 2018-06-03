using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Models;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public partial class GridDisplayComponent {

        private async Task<YHtmlString> RenderRecordsAsync(GridDefinition gridDef, ObjectSupport.ReadGridDictionaryInfo dictInfo) {

            HtmlBuilder hb = new HtmlBuilder();
            gridDef.RecordCount = 0;

            Manager.RenderingGridCount = Manager.RenderingGridCount + 1;
            using (Manager.StartNestedComponent($"{FieldName}[{gridDef.RecordCount}]")) {
                foreach (var record in gridDef.Data.Data) {
                    if (gridDef.RecordCount > 0) {
                        hb.Append(",");
                    }
                    List<PropertyListComponentBase.PropertyListEntry> hiddenProps = GetHiddenGridProperties(record, dictInfo);
                    List<PropertyListComponentBase.PropertyListEntry> props = GetGridProperties(record, dictInfo);
                    hb.Append($"{{ 'id': {gridDef.RecordCount}, {await RenderOneRecordAsync(HtmlHelper, record, props, hiddenProps, gridDef.ReadOnly)} }}");
                    gridDef.RecordCount = gridDef.RecordCount + 1;
                }
            }
            Manager.RenderingGridCount = Manager.RenderingGridCount - 1;
            return hb.ToYHtmlString();
        }
    }
}
