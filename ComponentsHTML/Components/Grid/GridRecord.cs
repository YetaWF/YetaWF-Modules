/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components
{

    public class GridRecordContainer : YetaWFComponent, IYetaWFContainer<GridRecordData> {

        public const string TemplateName = "GridRecord";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        private class GridRecordResult {
            public string TR { get; set; }
            public object StaticData { get; internal set; }
        }

        public async Task<YHtmlString> RenderContainerAsync(GridRecordData model) {

            ScriptBuilder sb = new ScriptBuilder();

            ObjectSupport.ReadGridDictionaryInfo dictInfo = await YetaWF.Core.Components.Grid.LoadGridColumnDefinitionsAsync(model.GridDef.RecordType);

            // render record
            string tr = await GridDisplayComponent.RenderRecordHTMLAsync(HtmlHelper, model.GridDef, dictInfo, model.FieldPrefix, model.Data, 0, 0, false);

            GridRecordResult result = new GridRecordResult {
                TR = tr,
                StaticData = model.Data,
            };
            sb.Append(YetaWFManager.JsonSerialize(result));

            return sb.ToYHtmlString();
        }
    }
}
