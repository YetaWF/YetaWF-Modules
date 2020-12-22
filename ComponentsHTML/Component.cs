/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using System.Collections.Generic;
using YetaWF.Core.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using YetaWF.Core.Pages;

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
            await Manager.AddOnManager.AddTemplateAsync(Package.AreaName, GetTemplateName(), GetComponentType());
        }

        /// <summary>
        /// Returns HTML attributes and name= attribute for the component, used on the HTML tag.
        /// </summary>
        /// <param name="fieldType">The type of the field.</param>
        /// <remarks>FieldSetup should be call first, as it adds entries to the HtmlAttributes collection, including additional CSS classes.
        /// This is used for the main tag of a template. Automatically adds the components HtmlAttributes.
        /// 
        /// Adds validation attributes depending on the field's type <paramref name="fieldType"/>.</remarks>
        public string FieldSetup(FieldType fieldType) {
            HtmlBuilder hb = new HtmlBuilder();
            switch (fieldType) {
                case FieldType.Anonymous:
                    break;
                case FieldType.Normal:
                    hb.Append($@" name='{FieldName}'");
                    break;
                case FieldType.Validated:
                    hb.Append($@" name='{FieldName}'");
                    // error state
                    string errClass = GetErrorClass();
                    if (errClass != null)
                        HtmlAttributes.Add("class", errClass);
                    // client side validation
                    string validations = GetValidation();
                    if (validations != null)
                        HtmlAttributes.Add("data-v", validations);
                    break;
            }
            hb.Append(HtmlBuilder.Attributes(HtmlAttributes));
            return hb.ToString();
        }
        /// <summary>
        /// Returns the CSS classes defined for this component.
        /// </summary>
        /// <returns>Returns the CSS classes defined in the HtmlAttributes property. An empty string is returned if no classes are defined.</returns>
        public string GetClasses(string extraCss = null) {
            return HtmlBuilder.GetClasses(HtmlAttributes, extraCss);
        }
        /// <summary>
        /// Returns a complete class= CSS attribute including all classes defined in the HtmlAttributes property.
        /// </summary>
        /// <param name="extraCss">Optional additional CSS classes.</param>
        /// <returns>Returns a complete class= CSS attribute including all classes defined in the HtmlAttributes property. An empty string is returned if no classes are defined.</returns>
        public string GetClassAttribute(string extraCss = null) {
            return HtmlBuilder.GetClassAttribute(HtmlAttributes, extraCss);
        }

        internal string GetErrorClass() {
            ModelStateEntry modelState;
            if (HtmlHelper.ModelState.TryGetValue(FieldName, out modelState)) {
                if (modelState.Errors.Count > 0)
                    return "v-valerror";
            }
            return null;
        }

        private string GetValidation() {
            // Build validation attribute
            List<object> objs = new List<object>();
            foreach (YIClientValidation val in PropData.ClientValidationAttributes) {
                // TODO: GetCaption can fail for redirects (ModuleDefinition) so we can't call it when there are no validation attributes
                // GridAllowedRole and GridAllowedUser use a ResourceRedirectList with a property OUTSIDE of the model. This only works in grids (where it is used)
                // but breaks when used elsewhere (like here) so we only call GetCaption if there is a validation attribute (FOR NOW).
                // That whole resource  redirect business needs to be fixed (old and ugly, and fragile).
                string caption = PropData.GetCaption(Container);
                ValidationBase valBase = val.AddValidation(Container, PropData, caption);
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
            return objs.Count > 0 ? Utility.JsonSerialize(objs) : null;
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
    }
}