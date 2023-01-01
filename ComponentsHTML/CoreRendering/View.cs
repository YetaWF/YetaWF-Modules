/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

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

            string css = Manager.AddOnManager.CheckInvokedCssModule(Forms.CssFormPartial);

            string? id = null;
            string? divId = null;
            if (Manager.IsPostRequest) {
                divId = Manager.UniqueId();
                id = $" id='{divId}'";
            } else {
                if (UsePartialFormCss && !Manager.IsInPopup && Manager.ActiveDevice != YetaWFManager.DeviceSelected.Mobile && !string.IsNullOrWhiteSpace(Manager.SkinInfo.PartialFormCss) && module.UsePartialFormCss)
                    css = CssManager.CombineCss(css, Manager.SkinInfo.PartialFormCss);
            }

            string tags = $@"<div{id} class='{css}'>{HtmlBuilder.AntiForgeryToken()}<input name='{Basics.ModuleGuid}' type='hidden' value='{module.ModuleGuid}' />{viewHtml}</div>";

            if (Manager.IsPostRequest)
                Manager.ScriptManager.AddLast($"$YetaWF.Forms.initPartialForm('{divId}');");

            return Task.FromResult(tags);
        }
    }
}
