/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Core.Modules;
using YetaWF.Core.Addons;
#if MVC6
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        /// <summary>
        /// Renders a view.
        /// </summary>
        /// <param name="htmlHelper">The HtmlHelper instance.</param>
        /// <param name="module">The module being rendered in the view.</param>
        /// <param name="viewHtml">The current view contents to be wrapped in the view.</param>
        /// <param name="UsePartialFormCss">Defines whether the partial form CSS should be used.</param>
        /// <returns>Returns the complete view as HTML.</returns>
        public Task<string> RenderViewAsync(YHtmlHelper htmlHelper, ModuleDefinition module, string viewHtml, bool UsePartialFormCss) {

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(Forms.CssFormPartial));
            string divId = null;
            if (Manager.IsPostRequest) {
                divId = Manager.UniqueId();
                tag.Attributes.Add("id", divId);
            } else {
                if (UsePartialFormCss && !Manager.IsInPopup && Manager.ActiveDevice != YetaWFManager.DeviceSelected.Mobile &&
                        !string.IsNullOrWhiteSpace(Manager.SkinInfo.PartialFormCss) && module.UsePartialFormCss)
                    tag.AddCssClass(Manager.SkinInfo.PartialFormCss);
            }
            hb.Append(tag.ToString(YTagRenderMode.StartTag));
            hb.Append(htmlHelper.AntiForgeryToken());
            hb.Append($@"<input name='{Basics.ModuleGuid}' type='hidden' value='{module.ModuleGuid}' />");

            hb.Append(viewHtml);

            hb.Append(tag.ToString(YTagRenderMode.EndTag));

            if (divId != null)
                Manager.ScriptManager.AddLast($"$YetaWF.Forms.initPartialForm('{divId}');");

            return Task.FromResult(hb.ToString());
        }
    }
}
