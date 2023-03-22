/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// The base class for all views implemented using the YetaWF.ComponentsHTML package.
    /// </summary>
    public abstract class YetaWFView : YetaWFViewBase {

        // TODO: REMOVE
        /// <summary>
        /// Adds JavaScript code to execute code between BeginDocumentReady and EndDocumentReady which is
        /// executed once the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <param name="id">The ID of the HTML element.</param>
        /// <returns>The Javascript code.</returns>
        protected string BeginDocumentReady(string? id = null) {
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
        /// <param name="ActionName">Overrides the default action name.</param>
        /// <param name="Method">The method used to submit the form (get/post)</param>
        /// <returns>Returns the HTML with the generated &lt;form&gt; tag.</returns>
        protected async Task<string> RenderBeginFormAsync(object? HtmlAttributes = null, bool SaveReturnUrl = false, string? ActionName = null, string Method = "post") {

            await YetaWFCoreRendering.Render.AddFormsAddOnsAsync();
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_Core", "Forms");// standard css, validation strings
            await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF_ComponentsHTML", "Forms");
            Manager.ScriptManager.AddLast("$YetaWF.Forms", "$YetaWF.Forms;");// need to evaluate for side effect to initialize forms

            Manager.NextUniqueIdPrefix();

            if (Method.ToLower() != "post") throw new InternalError("Only POST method is supported");
            string formAction = $"{Utility.UrlFor<ModuleEndpoints>(ModuleEndpoints.Update)}/{ModuleBase.ModuleGuid}";
            if (ActionName != null && ActionName != ModuleBase.Action)
                formAction += $"?Action={ActionName}";

            IDictionary<string, object?> attrs = HtmlBuilder.AnonymousObjectToHtmlAttributes(HtmlAttributes);
            if (SaveReturnUrl)
                attrs.Add(Basics.CssSaveReturnUrl, string.Empty);

            string? css = null;
            if (Manager.CurrentSite.FormErrorsImmed)
                css = CssManager.CombineCss(css, "yValidateImmediately");

            HtmlBuilder hb = new HtmlBuilder();
            string id = HtmlBuilder.GetId(attrs);
            hb.Append($@"
<form id='{id}' class='{Forms.CssForm}{HtmlBuilder.GetClasses(attrs, css)}' autocomplete='{(ModuleBase.FormAutoComplete ? "on" : "off")}' action='{Utility.HAE(formAction)}' method='{Method}'{HtmlBuilder.Attributes(attrs)}>
    {HtmlBuilder.AntiForgeryToken()}");

            return hb.ToString();
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
