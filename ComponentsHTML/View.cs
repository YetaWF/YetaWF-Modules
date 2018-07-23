/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
using YetaWF.Core.Addons;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
#else
using System.Web.Routing;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class YetaWFView : YetaWFViewBase {

        protected class JSDocumentReady : IDisposable {

            public JSDocumentReady(HtmlBuilder hb) {
                this.HB = hb;
                DisposableTracker.AddObject(this);
            }
            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) DisposableTracker.RemoveObject(this);
                while (CloseParen > 0) {
                    HB.Append("}");
                    CloseParen = CloseParen - 1;
                }
                HB.Append("});");
            }
            //~JSDocumentReady() { Dispose(false); }
            public HtmlBuilder HB { get; set; }
            public int CloseParen { get; internal set; }
        }
        protected JSDocumentReady DocumentReady(HtmlBuilder hb, string id) {
            hb.Append($@"$YetaWF.addWhenReadyOnce(function (tag) {{ if ($(tag).has('#{id}').length > 0) {{");
            return new JSDocumentReady(hb) { CloseParen = 1 };
        }
        protected JSDocumentReady DocumentReady(HtmlBuilder hb) {
            hb.Append("$YetaWF.addWhenReadyOnce(function (tag) {\n");
            return new JSDocumentReady(hb);
        }

        protected async Task<string> RenderBeginFormAsync(object HtmlAttributes = null, bool SaveReturnUrl = false, bool ValidateImmediately = false, string ActionName = null, string ControllerName = null, bool Pure = false) {

            await YetaWFCoreRendering.Render.AddFormsAddOnsAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF", "Core", "Forms");// standard css, validation strings
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF", "ComponentsHTML", "Forms");

            Manager.NextUniqueIdPrefix();

            if (string.IsNullOrWhiteSpace(ActionName))
                ActionName = GetViewName();
            if (!ActionName.EndsWith(YetaWFViewExtender.PartialSuffix))
                ActionName += YetaWFViewExtender.PartialSuffix;
            if (string.IsNullOrWhiteSpace(ControllerName))
                ControllerName = ModuleBase.Controller;

            IDictionary<string, object> rvd = AnonymousObjectToHtmlAttributes(HtmlAttributes);
            if (SaveReturnUrl)
                rvd.Add(Basics.CssSaveReturnUrl, "");

            if (!Pure) {
                string css = null;
                if (Manager.CurrentSite.FormErrorsImmed)
                    css = YetaWFManager.CombineCss(css, "yValidateImmediately");
                css = YetaWFManager.CombineCss(css, Forms.CssFormAjax);
                rvd.Add("class", css);
            }

            YTagBuilder tagBuilder = new YTagBuilder("form");
            tagBuilder.MergeAttributes(rvd, true);
            string formAction;
#if MVC6
            IServiceProvider services = HtmlHelper.ViewContext.HttpContext.RequestServices;
            IUrlHelper urlHelper = services.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(HtmlHelper.ViewContext);
            formAction = urlHelper.Action(action: ActionName, controller: ControllerName);
#else
            formAction = UrlHelper.GenerateUrl(null /* routeName */, ActionName, ControllerName, null, HtmlHelper.RouteCollection, HtmlHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */);
#endif
            tagBuilder.MergeAttribute("action", formAction, true);
            tagBuilder.MergeAttribute("method", "post", true);

            return tagBuilder.ToString(YTagRenderMode.StartTag);
        }
        protected Task<string> RenderEndFormAsync() {
            return Task.FromResult("</form>");
        }
        private IDictionary<string, object> AnonymousObjectToHtmlAttributes(object htmlAttributes) {
            if (htmlAttributes as RouteValueDictionary != null) return (RouteValueDictionary)htmlAttributes;
            if (htmlAttributes as Dictionary<string, object> != null) return (Dictionary<string, object>)htmlAttributes;
#if MVC6
            return Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
#else
            return HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
#endif
        }
    }
}
