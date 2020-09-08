/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
        /// Adds JavaScript code to execute code between BeginDocumentReady and EndDocumentReady which is
        /// executed once the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <param name="id">The ID of the HTML element.</param>
        /// <returns>The Javascript code.</returns>
        protected string BeginDocumentReady(string id = null) {
            if (string.IsNullOrWhiteSpace(id)) {
                DocCloseParen = 0;
                return "$YetaWF.addWhenReadyOnce(function (tag) {";
            } else {
                DocCloseParen = 1;
                return $@"$YetaWF.addWhenReadyOnce(function (tag) {{ if ($(tag).has('#{id}').length > 0) {{";
            }
        }
        /// <summary>
        /// Ends a JavaScript code section started by BeginDocumentReady, which contains JavaScript code to execute once
        /// the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <returns>The Javascript code.</returns>
        protected string EndDocumentReady() {
            return (DocCloseParen > 0 ? "}" : "") + "});";
        }
        private int DocCloseParen;

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
        protected async Task<string> RenderBeginFormAsync(object HtmlAttributes = null, bool SaveReturnUrl = false, bool ValidateImmediately = false, string ActionName = null, string ControllerName = null, string Method = "post") {

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

            IDictionary<string, object> rvd = YHtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes);
            if (SaveReturnUrl)
                rvd.Add(Basics.CssSaveReturnUrl, "");

            string css = null;
            if (Manager.CurrentSite.FormErrorsImmed)
                css = CssManager.CombineCss(css, "yValidateImmediately");
            css = CssManager.CombineCss(css, Forms.CssFormAjax);
            rvd.Add("class", css);

            YTagBuilder tagBuilder = new YTagBuilder("form");
            tagBuilder.MergeAttributes(rvd, true);
            if (ModuleBase.FormAutoComplete)
                tagBuilder.Attributes.Add("autocomplete", "on");
            else
                tagBuilder.Attributes.Add("autocomplete", "off");

            string id = null;
            if (tagBuilder.Attributes.ContainsKey("id")) {
                id = (string)tagBuilder.Attributes["id"];
            } else {
                id = Manager.UniqueId();
                tagBuilder.Attributes.Add("id", id);
            }
            string formAction;
#if MVC6
            System.IServiceProvider services = HtmlHelper.ActionContext.HttpContext.RequestServices;
            IUrlHelper urlHelper = services.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(HtmlHelper.ActionContext);
            formAction = urlHelper.Action(action: ActionName, controller: ControllerName, new { area = HtmlHelper.RouteData.Values["area"] });
#else
            formAction = UrlHelper.GenerateUrl(null /* routeName */, ActionName, ControllerName, null, RouteTable.Routes, HtmlHelper.RequestContext, true /* includeImplicitMvcValues */);
#endif
            tagBuilder.MergeAttribute("action", formAction, true);
            tagBuilder.MergeAttribute("method", Method, true);

            // show errors if already present
            if (!HtmlHelper.ModelState.IsValid) {
                Manager.ScriptManager.AddLast($@"
var f = $YetaWF.getElementById('{id}');
if ($YetaWF.Forms.hasErrors(f))
    $YetaWF.Forms.showErrors(f);
");
            }

            return tagBuilder.ToString(YTagRenderMode.StartTag);
        }
        /// <summary>
        /// Renders the ending &lt;/form&gt; tag.
        /// </summary>
        /// <returns>Returns the HTML with the generated &lt;/form&gt; tag.</returns>
        protected Task<string> RenderEndFormAsync() {
            return Task.FromResult("</form>");
        }
    }
}
