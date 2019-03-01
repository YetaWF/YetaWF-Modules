/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
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
            if (HtmlHelper.ViewData.ModelState.TryGetValue(FieldName, out modelState)) {
                if (modelState.Errors.Count > 0)
#if MVC6
                    return Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper.ValidationInputCssClassName;
#else
                    return HtmlHelper.ValidationInputCssClassName;
#endif
            }
            return null;
        }
        private void AddValidation(YTagBuilder tagBuilder) {
            foreach (YIClientValidation val in PropData.ValidationAttributes) {
                val.AddValidation(Container, PropData, tagBuilder);
            }
            // add some default validations
            if (PropData.PropInfo.PropertyType == typeof(DateTime) || PropData.PropInfo.PropertyType == typeof(DateTime?)) {
                tagBuilder.Attributes["data-val-date"] = __ResStr("valDate", "Please enter a valid value for field '{0}'", PropData.GetCaption(Container));
                tagBuilder.MergeAttribute("data-val", "true");
            } else if (PropData.PropInfo.PropertyType == typeof(int) || PropData.PropInfo.PropertyType == typeof(int?) ||
                    PropData.PropInfo.PropertyType == typeof(long) || PropData.PropInfo.PropertyType == typeof(long?)) {
                tagBuilder.Attributes["data-val-number"] = __ResStr("valNumber", "Please enter a valid number for field '{0}'", PropData.GetCaption(Container));
                tagBuilder.MergeAttribute("data-val", "true");
            }
        }
        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="fieldName">The HTML field name.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        /// <remarks>
        /// </remarks>
        protected YHtmlString ValidationMessage(string fieldName) {
            // ValidationMessage is always called for a child component within the context of the PARENT
            // component, so we need to prefix the child component field name with the parent field name
            if (!IsContainerComponent)
                fieldName = FieldName + "." + fieldName;
            if (!string.IsNullOrWhiteSpace(FieldNamePrefix))
                fieldName = FieldNamePrefix + "." + fieldName;
            return new YHtmlString(HtmlHelper.ValidationMessage(fieldName));
        }
        /// <summary>
        /// Returns the client-side validation message for a component with the specified field name.
        /// </summary>
        /// <param name="htmlHelper">The HtmlHelper instance.</param>
        /// <param name="containerFieldPrefix">The prefix used to build the final field name (for nested fields).</param>
        /// <param name="fieldName">The HTML field name.</param>
        /// <returns>Returns the client-side validation message for the component with the specified field name.</returns>
        public static YHtmlString ValidationMessage(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                 string containerFieldPrefix, string fieldName) {
            // ValidationMessage is always called for a child component within the context of the PARENT
            // component, so we need to prefix the child component field name with the parent field name
            if (!string.IsNullOrEmpty(containerFieldPrefix))
                fieldName = containerFieldPrefix + "." + fieldName;
            return new YHtmlString(htmlHelper.ValidationMessage(fieldName));
        }

        /// <summary>
        /// An instance of this class is returned by the DocumentReady method.
        /// </summary>
        /// <remarks>The DocumentReady object is used to generated HTML when the object is disposed.</remarks>
        protected class JSDocumentReady : IDisposable {

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="hb">The HtmlBuilder instance used to generate HTML.
            ///
            /// The JSDocumentReady is never directly instantiated. A JSDocumentReady object is made available by calling the DocumentReady method.
            ///
            /// For debugging purposes, instances of this class are tracked using the DisposableTracker class.
            /// </param>
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
            hb.Append("$YetaWF.addWhenReadyOnce(function (tag) {");
            return new JSDocumentReady(hb);
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