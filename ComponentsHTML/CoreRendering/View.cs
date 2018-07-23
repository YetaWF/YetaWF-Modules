/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Core.Modules;
using YetaWF.Core.Addons;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        public Task<YHtmlString> RenderViewAsync(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                ModuleDefinition module, string viewHtml, bool UsePartialFormCss)
        {
            HtmlBuilder hb = new HtmlBuilder();

            TagBuilder tag = new TagBuilder("div");
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
            hb.Append(tag.ToString(TagRenderMode.StartTag));

            hb.Append(htmlHelper.AntiForgeryToken());
            hb.Append(htmlHelper.Hidden(Basics.ModuleGuid, module.ModuleGuid));
            hb.Append(htmlHelper.Hidden(Forms.UniqueIdPrefix, Manager.UniqueIdPrefix));

            hb.Append(htmlHelper.ValidationSummary());

            hb.Append(viewHtml);

            hb.Append(tag.ToString(TagRenderMode.EndTag));

            if (divId != null)
                Manager.ScriptManager.AddLast($"$YetaWF.Forms.initPartialForm('{divId}');");

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
