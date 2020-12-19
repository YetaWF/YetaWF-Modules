/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Static class implementing HtmlHelper/IHtmlHelper extension methods.
    /// </summary>
    public static class YHtmlHelperExtender {

        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="containerFieldPrefix">The prefix used to build the final field name (for nested fields). May be null.</param>
        /// <param name="fieldName">The HTML field name.</param>
        /// <param name="htmlHelper">An instance of a YHtmlHelper.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        public static string ValidationMessage(this YHtmlHelper htmlHelper, string containerFieldPrefix, string fieldName) {
            if (!string.IsNullOrEmpty(containerFieldPrefix))
                fieldName = containerFieldPrefix + "." + fieldName;
            return htmlHelper.BuildValidationMessage(fieldName);
        }

        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="fieldName">The HTML field name.</param>
        /// <param name="htmlHelper">An instance of a YHtmlHelper.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        public static string BuildValidationMessage(this YHtmlHelper htmlHelper, string fieldName) {
            var modelState = htmlHelper.ModelState[fieldName];
            string error = null;
            bool hasError = false;
            if (modelState == null) {
                // no errors
            } else {
                IEnumerable<string> errors = (from e in modelState.Errors select e.ErrorMessage);
                hasError = errors.Any();
                if (hasError)
                    error = errors.First();
            }

            YTagBuilder tagBuilder = new YTagBuilder("span");
            tagBuilder.MergeAttribute("data-v-for", fieldName);
            tagBuilder.AddCssClass(hasError ? "v-error" : "v-valid");

            if (hasError) {
                // we're building the same client side in validation.ts, make sure to keep in sync
                // <img src="${$YetaWF.htmlAttrEscape(YConfigs.Forms.CssWarningIconUrl)}" name=${name} class="${YConfigs.Forms.CssWarningIcon}" ${YConfigs.Basics.CssTooltip}="${$YetaWF.htmlAttrEscape(val.M)}"/>
                YTagBuilder tagImg = new YTagBuilder("img");
                tagImg.Attributes.Add("src", Forms.CssWarningIconUrl);
                tagImg.Attributes.Add("name", fieldName);
                tagImg.AddCssClass(Forms.CssWarningIcon);
                tagImg.Attributes.Add(Basics.CssTooltip, error);
                tagBuilder.InnerHtml = tagImg.ToString();
            }
            return tagBuilder.ToString(YTagRenderMode.Normal);
        }
    }
}

