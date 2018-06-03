using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class GridDataContainer : YetaWFComponent, IYetaWFContainer<DataSourceResult> {

        public const string TemplateName = "GridData";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderContainerAsync(DataSourceResult model) {

            ScriptBuilder sb = new ScriptBuilder();

            model.RecordCount = 0;
            Manager.RenderingGridCount = Manager.RenderingGridCount + 1;
            using (Manager.StartNestedComponent($"{model.FieldPrefix}[{model.RecordCount}]")) {

                bool readOnly;
                if (string.IsNullOrWhiteSpace(model.FieldPrefix)) {
                    readOnly = true;
                } else {
                    readOnly = false;
                    //$$$ model.FieldPrefix;
                }

                // determine record type (could use reflection, but this is easier)
                ObjectSupport.ReadGridDictionaryInfo dictInfo = null;
                object firstRecord = model.Data.FirstOrDefault();
                if (firstRecord != null) {
                    Type recordType = firstRecord.GetType();
                    dictInfo = await Grid.LoadGridColumnDefinitionsAsync(recordType);
                }

                // render all records
                sb.Append($@"
{{
    ""records"":{model.Total},
    ""rows"": [");

                foreach (var record in model.Data) {
                    if (model.RecordCount > 0)
                        sb.Append(",");
                    List<PropertyListComponentBase.PropertyListEntry> hiddenProps = GridDisplayComponent.GetHiddenGridProperties(record, dictInfo);
                    List<PropertyListComponentBase.PropertyListEntry> props = GridDisplayComponent.GetGridProperties(record, dictInfo);
                    sb.Append($@"{{ ""id"": ""{model.RecordCount}"", {await GridDisplayComponent.RenderOneRecordAsync(HtmlHelper, record, props, hiddenProps, readOnly)} }}");
                    model.RecordCount = model.RecordCount + 1;
                }
                sb.Append($@"
    ]
}}");

            }
            Manager.RenderingGridCount = Manager.RenderingGridCount - 1;
            return sb.ToYHtmlString();
        }
    }
}
