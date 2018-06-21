/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using YetaWF.Core.Models;
using YetaWF.Core.Support;
using YetaWF.Core.Components;
using System.Threading.Tasks;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public partial class GridDisplayComponent {

        internal static async Task<YHtmlString> RenderOneRecordAsync(
#if MVC6
            IHtmlHelper htmlHelper, 
#else
            HtmlHelper htmlHelper,
#endif
                object model, List<PropertyListComponentBase.PropertyListEntry> props, List<PropertyListComponentBase.PropertyListEntry> hiddenProps, bool readOnly) {
            HtmlBuilder hb = new HtmlBuilder();

            // check if the grid is readonly or the record supports an "__editable" grid entry property
            bool recordEnabled = !readOnly;
            if (!readOnly)
                ObjectSupport.TryGetPropertyValue<bool>(model, "__editable", out recordEnabled, true);

            int propCount = 0;
            foreach (PropertyListComponentBase.PropertyListEntry prop in props) {

                //$$using (new HtmlHelperExtender.ControlInfoOverride(meta.AdditionalValues)) { //?? functionality still needed?
                //uses Manager.ControlInfoOverrides

                string output = "";
                if (propCount > 0)
                    hb.Append(",");
                hb.Append("\"{0}\":", prop.Name);

                if (prop.Name == "__highlight") {
                    // check whether the record supports a special "__highlight" property
                    hb.Append("\"{0}\"", prop.Value is bool && (bool)prop.Value == true ? YetaWFManager.JserEncode("<div class='yHighlightGridRow'/>") : "");
                } else if (prop.Name == "__lowlight") {
                    // check whether the record supports a special "__lowlight" property
                    hb.Append("\"{0}\"", prop.Value is bool && (bool)prop.Value == true ? YetaWFManager.JserEncode("<div class='yLowlightGridRow'/>") : "");
                } else {
                    if (recordEnabled && prop.Editable) {
                        output = (await htmlHelper.ForEditComponentAsync(model, prop.Name, prop.Value, prop.UIHint)).ToString();
                        output += YetaWFComponent.ValidationMessage(htmlHelper, Manager.NestedComponentPrefix, prop.Name).ToString();
                    } else {
                        output = (await htmlHelper.ForDisplayComponentAsync(model, prop.Name, prop.Value, prop.UIHint)).ToString();
                    }
                    output = output.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                    if (string.IsNullOrWhiteSpace(output)) { output = "&nbsp;"; }

                    if (!readOnly && prop.Editable && hiddenProps != null) {
                        // list hidden properties with the first editable field
                        foreach (var h in hiddenProps) {
                            output += (await htmlHelper.ForEditComponentAsync(model, h.Name, h.Value, "Hidden")).ToString();
                        }
                        hiddenProps = null;
                    }

                    hb.Append(YetaWFManager.JsonSerialize(output));
                }
                ++propCount;
            }

            return hb.ToYHtmlString();
        }
    }
}