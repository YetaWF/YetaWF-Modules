/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using System.Collections.Generic;
using YetaWF.Core.Extensions;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// The base class for all edit and display components implemented using the YetaWF.ComponentsHTML package.
    /// </summary>
    public abstract class YetaWFComponent : YetaWFComponentBase {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(YetaWFComponent), name, defaultValue, parms); }

        /// <summary>
        /// Defines the HTML field type.
        /// </summary>
        public enum FieldType {
            /// <summary>
            /// An HTML field with a name, without client-side validation.
            /// </summary>
            Normal,
            /// <summary>
            /// An HTML field without name and without client-side validation.
            /// </summary>
            Anonymous,
            /// <summary>
            /// An HTML field with a name and with client-side validation.
            /// </summary>
            Validated,
        }
        /// <summary>
        /// The model used to render a grid.
        /// </summary>
        public class GridModel {
            /// <summary>
            /// Defines the grid.
            /// </summary>
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        /// <summary>
        /// Includes required JavaScript, CSS files when using a display component, for all components in this package.
        /// </summary>
        public override Task IncludeStandardDisplayAsync() { return Task.CompletedTask; }
        /// <summary>
        /// Includes required JavaScript, CSS files when using an edit component, for all components in this package.
        /// </summary>
        public override Task IncludeStandardEditAsync() { return Task.CompletedTask; }

        /// <summary>
        /// Includes required JavaScript, CSS files for this component.
        /// </summary>
        public virtual async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(Package.AreaName, GetTemplateName());
        }

        /// <summary>
        /// Adds a unique ID to the specified tag.
        /// </summary>
        /// <param name="tag">The tag to which the ID is added</param>
        /// <returns>Returns the ID.</returns>
        public string MakeId(YTagBuilder tag) {
            string id = (from a in tag.Attributes where string.Compare(a.Key, "id", true) == 0 select a.Value).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(id)) {
                id = Manager.UniqueId();
                tag.Attributes.Add("id", id);
            }
            return id;
        }

        /// <summary>
        /// Adds HTML attributes and name= attribute to a tag.
        /// </summary>
        /// <param name="tag">The tag to which attributes are added.</param>
        /// <param name="fieldType">The type of the field.</param>
        /// <remarks>This is used for the main tag of a template.
        ///
        /// Also adds validation attributes depending on the field's type.</remarks>
        public void FieldSetup(YTagBuilder tag, FieldType fieldType) {
            if (HtmlAttributes != null)
                tag.MergeAttributes(HtmlAttributes, false);
            switch (fieldType) {
                case FieldType.Anonymous:
                    break;
                case FieldType.Normal:
                    tag.MergeAttribute("name", FieldName, false);
                    break;
                case FieldType.Validated:
                    tag.MergeAttribute("name", FieldName, false);
                    // error state
                    AddErrorClass(tag);
                    // client side validation
                    AddValidation(tag);
                    break;
            }
        }
        private void AddErrorClass(YTagBuilder tagBuilder) {
            string cls = GetErrorClass();
            if (!string.IsNullOrWhiteSpace(cls))
                tagBuilder.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cls));
        }
        internal string GetErrorClass() {
#if MVC6
            ModelStateEntry modelState;
#else
            ModelState modelState;
#endif
            if (HtmlHelper.ModelState.TryGetValue(FieldName, out modelState)) {
                if (modelState.Errors.Count > 0)
                    return "v-valerror";
            }
            return null;
        }
        private void AddValidation(YTagBuilder tagBuilder) {
            //$$$ // add some default validations
            //if (!PropData.ReadOnly) {
            //    if (PropData.PropInfo.PropertyType == typeof(DateTime) || PropData.PropInfo.PropertyType == typeof(DateTime?)) {
            //        tagBuilder.Attributes["data-val-date"] = __ResStr("valDate", "Please enter a valid value for field '{0}'", PropData.GetCaption(Container));
            //        tagBuilder.MergeAttribute("data-val", "true");
            //    } else if (PropData.PropInfo.PropertyType == typeof(int) || PropData.PropInfo.PropertyType == typeof(int?) ||
            //            PropData.PropInfo.PropertyType == typeof(long) || PropData.PropInfo.PropertyType == typeof(long?)) {
            //        tagBuilder.Attributes["data-val-number"] = __ResStr("valNumber", "Please enter a valid number for field '{0}'", PropData.GetCaption(Container));
            //        tagBuilder.MergeAttribute("data-val", "true");
            //    }
            //}
            // Build validation attribute
            List<object> objs = new List<object>();
            foreach (YIClientValidation val in PropData.ClientValidationAttributes) {
                // TODO: GetCaption can fail for redirects (ModuleDefinition) so we can't call it when there are no validation attributes
                // GridAllowedRole and GridAllowedUser use a ResourceRedirectList with a property OUTSIDE of the model. This only works in grids (where it is used)
                // but breaks when used elsewhere (like here) so we only call GetCaption if there is a validation attribute (FOR NOW).
                // That whole resource  redirect business needs to be fixed (old and ugly, and fragile).
                string caption = PropData.GetCaption(Container);
                ValidationBase valBase = val.AddValidation(Container, PropData, caption, tagBuilder);
                if (valBase != null) {
                    string method = valBase.Method;
                    if (string.IsNullOrWhiteSpace(method))
                        throw new InternalError($"No method given ({nameof(ValidationBase)}.{nameof(ValidationBase.Method)})");
                    if (string.IsNullOrWhiteSpace(valBase.Message))
                        throw new InternalError($"No message given ({nameof(ValidationBase)}.{nameof(ValidationBase.Message)})");
                    objs.Add(valBase);
                    method = method.TrimEnd("Attribute");// remove ending ..Attribute
                    method = method.TrimEnd("Validation");// remove ending ..Validation
                    valBase.Method = method.ToLower();
                    if (string.IsNullOrWhiteSpace(method))
                        throw new InternalError($"No method name found after removing Attribute and Validation suffixes");
                }
            }
            if (objs.Count > 0)
                tagBuilder.Attributes.Add("data-v", Utility.JsonSerialize(objs));
        }
        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="fieldName">The HTML field name.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        /// <remarks>
        /// </remarks>
        public string ValidationMessage(string fieldName) {
            // ValidationMessage is always called for a child component within the context of the PARENT
            // component, so we need to prefix the child component field name with the parent field name
            if (!IsContainerComponent)
                fieldName = FieldName + "." + fieldName;
            if (!string.IsNullOrWhiteSpace(FieldNamePrefix))
                fieldName = FieldNamePrefix + "." + fieldName;
            return HtmlHelper.BuildValidationMessage(fieldName);
        }
        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="containerFieldPrefix">The prefix used to build the final field name (for nested fields).</param>
        /// <param name="fieldName">The HTML field name.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        public string ValidationMessage(string containerFieldPrefix, string fieldName) {
            // ValidationMessage is always called for a child component within the context of the PARENT
            // component, so we need to prefix the child component field name with the parent field name
            return HtmlHelper.ValidationMessage(containerFieldPrefix, fieldName);
        }

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
        private int DocCloseParen;

        /// <summary>
        /// Ends a JavaScript code section started by BeginDocumentReady, which contains JavaScript code to execute once
        /// the page is fully loaded (see $YetaWF.addWhenReadyOnce, similar to $(document).ready()).
        /// </summary>
        /// <remarks>This pattern should not be used and will be discontinued.</remarks>
        /// <returns>The Javascript code.</returns>
        protected string EndDocumentReady() {
            return (DocCloseParen > 0 ? "}" : "") + "});";
        }
    }
}