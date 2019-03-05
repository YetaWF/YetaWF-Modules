/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
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

    /// <summary>
    /// The base class for all views implemented using the YetaWF.ComponentsHTML package.
    /// </summary>
    public abstract class YetaWFView : YetaWFViewBase {

        /// <summary>
        /// An instance of this class is returned by the DocumentReady method.
        /// </summary>
        /// <remarks>The DocumentReady object is used to generated HTML when the object is disposed.</remarks>
        protected class JSDocumentReady : IDisposable {

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="hb">The HtmlBuilder instance used to generate HTML.</param>
            /// <remarks>
            /// The JSDocumentReady is never directly instantiated. A JSDocumentReady object is made available by calling the DocumentReady method.
            ///
            /// For debugging purposes, instances of this class are tracked using the DisposableTracker class.
            /// </remarks>
            public JSDocumentReady(HtmlBuilder hb) {
                this.HB = hb;
                DisposableTracker.AddObject(this);
            }
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() { Dispose(true); }
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <param name="disposing">true to release the DisposableTracker reference count, false otherwise.</param>
            protected virtual void Dispose(bool disposing) {
                if (disposing) DisposableTracker.RemoveObject(this);
                while (CloseParen > 0) {
                    HB.Append("}");
                    CloseParen = CloseParen - 1;
                }
                HB.Append("});");
            }
            //~JSDocumentReady() { Dispose(false); }
            private HtmlBuilder HB { get; set; }
            /// <summary>
            /// Used to generate the specified number of closing parentheses.
            /// </summary>
            public int CloseParen { get; internal set; }
        }
        /// <summary>
        /// Adds JavaScript code to execute code between DocumentReady and when the JSDocumentReady is disposed which is
        /// executed once the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <param name="hb">The HtmlBuilder instance where the code is generated.</param>
        /// <param name="id">The ID of the HTML element.</param>
        /// <returns>Returns a JSDocumentReady instance.</returns>
        protected JSDocumentReady DocumentReady(HtmlBuilder hb, string id) {
            hb.Append($@"$YetaWF.addWhenReadyOnce(function (tag) {{ if ($(tag).has('#{id}').length > 0) {{");
            return new JSDocumentReady(hb) { CloseParen = 1 };
        }
        /// <summary>
        /// Adds JavaScript code to execute code between DocumentReady and when the JSDocumentReady is disposed which is
        /// executed once the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <param name="hb">The HtmlBuilder instance where the code is generated.</param>
        /// <returns>Returns a JSDocumentReady instance.</returns>
        protected JSDocumentReady DocumentReady(HtmlBuilder hb) {
            hb.Append("$YetaWF.addWhenReadyOnce(function (tag) {\n");
            return new JSDocumentReady(hb);
        }
        /// <summary>
        /// Renders the beginning &lt;form&gt; tag with the specified attributes.
        /// </summary>
        /// <param name="HtmlAttributes">The HTML attributes to add to the &lt;form&gt; tag.</param>
        /// <param name="SaveReturnUrl">Defines whether the return URL is saved when the form is submitted.</param>
        /// <param name="ValidateImmediately">Defines whether client-side validation is immediate (true) or delayed until form submission (false).</param>
        /// <param name="ActionName">Overrides the default action name.</param>
        /// <param name="ControllerName">Overrides the default controller name.</param>
        /// <param name="Pure">TODO: Purpose unclear.</param>
        /// <param name="Method">The method used to submit the form (get/post)</param>
        /// <returns>Returns the HTML with the generated &lt;form&gt; tag.</returns>
        protected async Task<string> RenderBeginFormAsync(object HtmlAttributes = null, bool SaveReturnUrl = false, bool ValidateImmediately = false, string ActionName = null, string ControllerName = null, bool Pure = false, string Method = "post") {

            await YetaWFCoreRendering.Render.AddFormsAddOnsAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Forms");// standard css, validation strings
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_ComponentsHTML", "Forms");
            Manager.ScriptManager.AddLast("$YetaWF.Forms", "$YetaWF.Forms;");// need to evaluate for side effect to initialize forms

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
                    css = CssManager.CombineCss(css, "yValidateImmediately");
                css = CssManager.CombineCss(css, Forms.CssFormAjax);
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
            tagBuilder.MergeAttribute("method", Method, true);

            return tagBuilder.ToString(YTagRenderMode.StartTag);
        }
        /// <summary>
        /// Renders the ending &lt;/form&gt; tag.
        /// </summary>
        /// <returns>Returns the HTML with the generated &lt;/form&gt; tag.</returns>
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
