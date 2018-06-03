using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

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
            dictInfo = await Grid.LoadGridColumnDefinitionsAsync(recordType);
            List<PropertyListComponentBase.PropertyListEntry> hiddenProps = GridDisplayComponent.GetHiddenGridProperties(model, dictInfo);
            List<PropertyListComponentBase.PropertyListEntry> props = GridDisplayComponent.GetGridProperties(model, dictInfo);
            hb.Append(await GridDisplayComponent.RenderOneRecordAsync(HtmlHelper, model, props, hiddenProps, false));

            hb.Append("}");

            return hb.ToYHtmlString();
        }
    }
}
