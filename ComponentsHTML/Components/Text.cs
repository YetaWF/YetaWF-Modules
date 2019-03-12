/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the Text10 display component.
    /// </summary>
    public class Text10DisplayComponent : TextDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text10DisplayComponent() : base("Text10", "yt_text10") { }
    }
    /// <summary>
    /// Implementation of the Text10 edit component.
    /// </summary>
    public class Text10EditComponent : TextEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text10EditComponent() : base("Text10", "yt_text10") { }
    }
    /// <summary>
    /// Implementation of the Text20 display component.
    /// </summary>
    public class Text20DisplayComponent : TextDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text20DisplayComponent() : base("Text20", "yt_text20") { }
    }
    /// <summary>
    /// Implementation of the Text20 edit component.
    /// </summary>
    public class Text20EditComponent : TextEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text20EditComponent() : base("Text20", "yt_text20") { }
    }
    /// <summary>
    /// Implementation of the Text40 display component.
    /// </summary>
    public class Text40DisplayComponent : TextDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text40DisplayComponent() : base("Text40", "yt_text40") { }
    }
    /// <summary>
    /// Implementation of the Text40 edit component.
    /// </summary>
    public class Text40EditComponent : TextEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text40EditComponent() : base("Text40", "yt_text40") { }
    }
    /// <summary>
    /// Implementation of the Text80 display component.
    /// </summary>
    public class Text80DisplayComponent : TextDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text80DisplayComponent() : base("Text80", "yt_text80") { }
    }
    /// <summary>
    /// Implementation of the Text80 edit component.
    /// </summary>
    public class Text80EditComponent : TextEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Text80EditComponent() : base("Text80", "yt_text80") { }
    }
    /// <summary>
    /// Implementation of the Text display component.
    /// </summary>
    public class TextDisplayComponent : TextDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TextDisplayComponent() : base("Text", "yt_text") { }
    }
    /// <summary>
    /// Implementation of the Text edit component.
    /// </summary>
    public class TextEditComponent : TextEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TextEditComponent() : base("Text", "yt_text") { }
    }

    /// <summary>
    /// Base class for the Text component implementation.
    /// </summary>
    public abstract class TextComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TextEditComponentBase), name, defaultValue, parms); }

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        internal string TemplateName { get; set; }
        internal string TemplateClass { get; set; }
    }

    /// <summary>
    /// Base class for the Text display component implementation.
    /// </summary>
    public abstract class TextDisplayComponentBase : TextComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="templateClass">The CSS class representing the component.</param>
        public TextDisplayComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.UseAsync();// needed for css
            //await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "Text");
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);
            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass(TemplateClass);
            // adding k-textbox to the control makes it look like a kendo maskedtext box without the overhead of actually calling kendoMaskedTextBox
            tag.AddCssClass("k-textbox");
            tag.AddCssClass("t_display");
            tag.AddCssClass("k-state-disabled"); // USE KENDO style
            FieldSetup(tag, FieldType.Anonymous);

            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", model ?? "");
            if (copy || rdonly)
                tag.MergeAttribute("readonly", "readonly");
            else
                tag.MergeAttribute("disabled", "disabled");

            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            if (copy) {
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
                hb.Append(ImageHTML.BuildKnownIcon("#TextCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_text_copy"));
            }
            return hb.ToYHtmlString();
        }
    }

    /// <summary>
    /// Base class for the Text edit component implementation.
    /// </summary>
    public abstract class TextEditComponentBase : TextComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="templateClass">The CSS class representing the component.</param>
        public TextEditComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        /// <summary>
        /// Adds all addons for the DropDownList component to the current page.
        /// </summary>
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            //await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
            await KendoUICore.UseAsync();// needed for css
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "Text");
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(string model) {
            return await RenderTextAsync(this, model, TemplateClass);
        }
        internal static async Task<YHtmlString> RenderTextAsync(YetaWFComponent component, string model, string templateCssClass) {

            await IncludeExplicitAsync();

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("input");
            if (!string.IsNullOrWhiteSpace(templateCssClass))
                tag.AddCssClass(templateCssClass);
            tag.AddCssClass("yt_text_base");
            // adding k-textbox to the control makes it look like a kendo maskedtext box without the overhead of actually calling kendoMaskedTextBox
            tag.AddCssClass("k-textbox");
            tag.AddCssClass("t_edit");
            component.FieldSetup(tag, component.Validation ? FieldType.Validated : FieldType.Normal);
            //string id = null;
            //if (!string.IsNullOrWhiteSpace(mask)) {
            //    id = component.MakeId(tag);
            //}

            bool copy = component.PropData.GetAdditionalAttributeValue<bool>("Copy", false);
            //string mask = component.PropData.GetAdditionalAttributeValue<string>("Mask", null);

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = component.PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (tag.Attributes.ContainsKey("maxlength"))
                    throw new InternalError("Both StringLengthAttribute and maxlength specified - {0}", component.FieldName);
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    tag.MergeAttribute("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !tag.Attributes.ContainsKey("maxlength"))
                throw new InternalError("No max string length given using StringLengthAttribute or maxlength - {0}", component.FieldName);
#endif
            // text
            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", model ?? "");
            tag.MergeAttribute("autocomplete", "on");

            hb.Append($@"{tag.ToString(YTagRenderMode.StartTag)}");

            if (copy) {
                await Manager.AddOnManager.AddAddOnNamedAsync(component.Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
                hb.Append(ImageHTML.BuildKnownIcon("#TextCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_text_copy"));
            }

            //if (!string.IsNullOrWhiteSpace(mask)) {
            //    // if there is a Mask we need to use the KendoMaskedTextBox
            //    await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
            //    ScriptBuilder sb = new ScriptBuilder();
            //    sb.Append("$('#{0}').kendoMaskedTextBox({{ mask: '{1}' }});\n", id, YetaWFManager.JserEncode(mask));
            //    Manager.ScriptManager.AddLastDocumentReady(sb);
            //}
            return hb.ToYHtmlString();
        }
    }
}
