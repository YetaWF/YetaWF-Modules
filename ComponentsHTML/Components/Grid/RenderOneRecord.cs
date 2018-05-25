/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using System.Collections.Generic;
using YetaWF.Core.Models;
using YetaWF.Core.Support;
using YetaWF.Core.Components;
using YetaWF.Core.Views.Shared;
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
                object model, List<PropertyListEntry> props, List<PropertyListEntry> hiddenProps, bool readOnly) {
            HtmlBuilder hb = new HtmlBuilder();

            //$$ GridDefinition.GridEntryDefinition gridEntry = Manager.GetParentModel() as GridDefinition.GridEntryDefinition;
            //$$ DataSourceResult dataSrc = Manager.GetParentModel() as DataSourceResult;

            // check if the grid is readonly or the record supports an "__editable" grid entry property
            bool recordEnabled = !readOnly;
            if (!readOnly)
                ObjectSupport.TryGetPropertyValue<bool>(model, "__editable", out recordEnabled, true);

            //$$} else if (gridEntry != null) {
            //    readOnly = false;
            //    prefix = gridEntry.Prefix;
            //    recordCount = gridEntry.RecNumber;
            //    hiddenProps = await GridHelper.GetHiddenGridPropertiesAsync(model);
            //    props = await GridHelper.GetGridPropertiesAsync(model);
            //}

            int propCount = 0;
            foreach (PropertyListEntry prop in props) {

                //$$using (new HtmlHelperExtender.ControlInfoOverride(meta.AdditionalValues)) { //?? functionality still needed?

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
                        if (htmlHelper.IsSupported(model, prop.Name, UIHint: prop.UIHint)) {
                            output = (await htmlHelper.ForEditComponentAsync(model, prop.Name, prop.Value, prop.UIHint)).ToString();
                            output += YetaWFComponent.ValidationMessage(htmlHelper, Manager.NestedComponentPrefix, prop.Name).ToString();
                        } else {
                            output = $"old template {prop.UIHint} not supported - {prop.Name}";
                        }
                    } else {
                        if (htmlHelper.IsSupported(model, prop.Name, UIHint: prop.UIHint)) {
                            output = (await htmlHelper.ForDisplayComponentAsync(model, prop.Name, prop.Value, prop.UIHint)).ToString();
                        } else {
                            output = $"old template {prop.UIHint} not supported - {prop.Name}";
                        }
                    }
                    output = output.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                    if (string.IsNullOrWhiteSpace(output)) { output = "&nbsp;"; }

                    if (!readOnly && prop.Editable && hiddenProps != null) {
                        // list hidden properties with the first editable field
                        foreach (var h in hiddenProps) {
                            output = (await htmlHelper.ForEditComponentAsync(model, prop.Name, prop.Value, "Hidden")).ToString();
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