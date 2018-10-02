/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components
{

    public class GridDataContainer : YetaWFComponent, IYetaWFContainer<GridPartialData> {

        public const string TemplateName = "GridPartialData";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        private class GridPartialResult {
            public int Records { get; set; }
            public string TBody { get; set; }
            public int Pages { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        public async Task<YHtmlString> RenderContainerAsync(GridPartialData model) {

            ScriptBuilder sb = new ScriptBuilder();

            ObjectSupport.ReadGridDictionaryInfo dictInfo = await YetaWF.Core.Components.Grid.LoadGridColumnDefinitionsAsync(model.GridDef.RecordType);

            // render all records
            string data = await GridDisplayComponent.RenderTableHTML(HtmlHelper, model.GridDef, model.Data, model.StaticData, dictInfo, model.FieldPrefix, true, model.Skip, model.Take);

            int pages = 0;
            int page = 0;
            int pageSize = model.Take;
            if (model.Data.Total > 0) {
                if (model.Take == 0) {
                    pages = 1;
                    pageSize = 0;
                } else {
                    pages = Math.Max(1, model.Data.Total / model.Take + (model.Data.Total % model.Take == 0 ? 0 : 1));
                    page = Math.Max(model.Skip / model.Take, 0);
                }
            }

            GridPartialResult result = new GridPartialResult {
                Records = model.Data.Total,
                TBody = data,
                Pages = pages,
                Page = page,
                PageSize = pageSize,
            };

            sb.Append(YetaWFManager.JsonSerialize(result));

            return sb.ToYHtmlString();
        }
    }
}
