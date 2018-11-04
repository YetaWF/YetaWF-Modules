/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class Text10DisplayComponent : TextDisplayComponentBase { public Text10DisplayComponent() : base("Text10", "yt_text10") { } }
    public class Text10EditComponent : TextEditComponentBase { public Text10EditComponent() : base("Text10", "yt_text10") { } }
    public class Text20DisplayComponent : TextDisplayComponentBase { public Text20DisplayComponent() : base("Text20", "yt_text20") { } }
    public class Text20EditComponent : TextEditComponentBase { public Text20EditComponent() : base("Text20", "yt_text20") { } }
    public class Text40DisplayComponent : TextDisplayComponentBase { public Text40DisplayComponent() : base("Text40", "yt_text40") { } }
    public class Text40EditComponent : TextEditComponentBase { public Text40EditComponent() : base("Text40", "yt_text40") { } }
    public class Text80DisplayComponent : TextDisplayComponentBase { public Text80DisplayComponent() : base("Text80", "yt_text80") { } }
    public class Text80EditComponent : TextEditComponentBase { public Text80EditComponent() : base("Text80", "yt_text80") { } }
    public class TextDisplayComponent : TextDisplayComponentBase { public TextDisplayComponent() : base("Text", "yt_text") { } }
    public class TextEditComponent : TextEditComponentBase { public TextEditComponent() : base("Text", "yt_text") { } }

    public abstract class TextComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TextEditComponentBase), name, defaultValue, parms); }

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public string TemplateName { get; set; }
        public string TemplateClass { get; set; }
    }

    public abstract class TextDisplayComponentBase : TextComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public TextDisplayComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
        }
        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);
            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass(TemplateClass);
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
                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("Copy.png", Package, "Text");
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, title: __ResStr("ttCopy", "Copy to Clipboard"), alt: __ResStr("altCopy", "Copy to Clipboard"));
                tagImg.AddCssClass("yt_text_copy");
                hb.Append(tagImg.ToString(YTagRenderMode.StartTag));
            }
            return hb.ToYHtmlString();
        }
    }
    public abstract class TextEditComponentBase : TextComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public TextEditComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
            await Manager.AddOnManager.AddTemplateAsync(Controllers.AreaRegistration.CurrentPackage.AreaName, "Text");
        }
        public async Task<YHtmlString> RenderAsync(string model) {
            return await RenderTextAsync(this, model, TemplateClass);
        }
        public static async Task<YHtmlString> RenderTextAsync(YetaWFComponent component, string model, string templateCssClass) {

            await IncludeExplicitAsync();

            HtmlBuilder hb = new HtmlBuilder();

            bool useKendo = true;

            YTagBuilder tag = new YTagBuilder("input");
            if (!string.IsNullOrWhiteSpace(templateCssClass))
                tag.AddCssClass(templateCssClass);
            tag.AddCssClass("yt_text_base");
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
            if (!useKendo)
                tag.AddCssClass("ybrowsercontrols");

            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            if (copy) {
                await Manager.AddOnManager.AddAddOnNamedAsync(component.Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("Copy.png", component.Package, "Text");
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, title: __ResStr("ttCopy", "Copy to Clipboard"), alt: __ResStr("altCopy", "Copy to Clipboard"));
                tagImg.AddCssClass("yt_text_copy");
                hb.Append(tagImg.ToString(YTagRenderMode.StartTag));
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
