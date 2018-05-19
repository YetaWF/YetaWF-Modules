using System.Threading.Tasks;
using YetaWF.Core.Packages;
using System.Collections.Generic;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
#if MVC6
#else
using System.Web.Mvc;
using System.Web.Routing;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class YetaWFComponent : YetaWFComponentBase {

        public enum FieldType {
            Normal, // with name, not validated
            Anonymous, // no name - no validation
            Validated, // with name, validated
        }

        /// <summary>
        /// Include required JavaScript, Css files for all components in this package.
        /// </summary>
        public override async Task IncludeStandardAsync() {
            await Manager.AddOnManager.AddAddOnGlobalAsync("jquery.com", "jquery");
            await Manager.AddOnManager.AddAddOnGlobalAsync("jqueryui.com", "jqueryui");
        }

        /// <summary>
        /// Include required JavaScript, Css files for this component.
        /// </summary>
        public virtual async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateAsync(Package.Domain, Package.Product, ComponentName);
        }

        /// <summary>
        /// Add HTML attributes and name= attribute to tag.
        /// </summary>
        /// <remarks>This is used for the main tag of a template.
        /// Also adds validation attributes.</remarks>
        protected void FieldSetup(YTagBuilder tag, FieldType fieldType) {
            if (HtmlAttributes != null)
                tag.MergeAttributes(AnonymousObjectToHtmlAttributes(HtmlAttributes), true);
            switch (fieldType) {
                case FieldType.Anonymous:
                    break;
                case FieldType.Normal:
                    tag.MergeAttribute("name", FieldName, true);
                    break;
                case FieldType.Validated:
                    tag.MergeAttribute("name", FieldName, true);
                    // error state
                    AddErrorClass(tag);
                    // client side validation
                    AddValidation(tag);
                    break;
            }
        }
        private IDictionary<string, object> AnonymousObjectToHtmlAttributes(object htmlAttributes) {
            if (htmlAttributes as RouteValueDictionary != null) return (RouteValueDictionary)htmlAttributes;
            return HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }
        private void AddErrorClass(YTagBuilder tagBuilder) {
            string cls = GetErrorClass();
            if (!string.IsNullOrWhiteSpace(cls))
                tagBuilder.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cls));
        }
        public string GetErrorClass() {
#if MVC6
            ModelStateEntry modelState;
#else
            ModelState modelState;
#endif
            if (HtmlHelper.ViewData.ModelState.TryGetValue(FieldName, out modelState)) {
                if (modelState.Errors.Count > 0)
                    return HtmlHelper.ValidationInputCssClassName;
            }
            return null;
        }
        private void AddValidation(YTagBuilder tagBuilder) {
#if MVC6
            ModelExplorer modelExplorer = ExpressionMetadataProvider.FromStringExpression(FieldName, HtmlHelper.ViewData, HtmlHelper.MetadataProvider);
            ValidationHtmlAttributeProvider valHtmlAttrProvider = (ValidationHtmlAttributeProvider)YetaWFManager.ServiceProvider.GetService(typeof(ValidationHtmlAttributeProvider));
            valHtmlAttrProvider.AddAndTrackValidationAttributes(HtmlHelper.ViewContext, modelExplorer, FieldName, tagBuilder.Attributes);
            ModelMetadata metadata = modelExplorer.Metadata;
#else
            ModelMetadata metadata = ModelMetadata.FromStringExpression(FieldName, HtmlHelper.ViewContext.ViewData);
            IDictionary<string, object> attrs = HtmlHelper.GetUnobtrusiveValidationAttributes(FieldName, metadata);
            tagBuilder.MergeAttributes(attrs, replaceExisting: false);
#endif
            // patch up auto-generated "required" validation (added by MVC) and rename our own customrequired validation to required
            if (tagBuilder.Attributes.ContainsKey("data-val-required")) {
                tagBuilder.Attributes.Remove("data-val-required");
            }
            if (tagBuilder.Attributes.ContainsKey("data-val-customrequired")) {
                tagBuilder.Attributes.Add("data-val-required", tagBuilder.Attributes["data-val-customrequired"]);
                tagBuilder.Attributes.Remove("data-val-customrequired");
            }
            // replace type dependent messages (MVC, please, who asked for this?)
            if (tagBuilder.Attributes.ContainsKey("data-val-number"))
                tagBuilder.Attributes["data-val-number"] = this.__ResStr("valNumber", "Please enter a valid number for field '{0}'", AttributeHelper.GetPropertyCaption(metadata));
            if (tagBuilder.Attributes.ContainsKey("data-val-date"))
                tagBuilder.Attributes["data-val-date"] = this.__ResStr("valDate", "Please enter a valid date for field '{0}'", AttributeHelper.GetPropertyCaption(metadata));
        }
    }
}
