using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class GridDataOneRecordComponent : YetaWFComponent, IYetaWFComponent<object> {

        public const string TemplateName = "GridDataOneRecord";

        public override ComponentType GetComponentType() { return ComponentType.Display; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(object model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append("{");

            ObjectSupport.ReadGridDictionaryInfo dictInfo = null;
            Type recordType = model.GetType();
            dictInfo = await GridHelper.LoadGridColumnDefinitionsAsync(recordType);
            List<Core.Views.Shared.PropertyListEntry> hiddenProps = GridDisplayComponent.GetHiddenGridProperties(model, dictInfo);
            List<Core.Views.Shared.PropertyListEntry> props = GridDisplayComponent.GetGridProperties(model, dictInfo);
            hb.Append(await GridDisplayComponent.RenderOneRecordAsync(HtmlHelper, model, props, hiddenProps, false));

            hb.Append("}");

            return hb.ToYHtmlString();
        }
    }
}
