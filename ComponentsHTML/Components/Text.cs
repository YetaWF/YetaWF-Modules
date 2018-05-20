using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;
using YetaWF.Core.Skins;
using YetaWF.Core.Views.Shared;
using YetaWF.Core.Localize;

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

    public abstract class TextDisplayComponentBase : YetaWFComponent, IYetaWFComponent<string> {

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public string TemplateName { get; set; }
        public string TemplateClass { get; set; }

        public TextDisplayComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public override async Task IncludeAsync() {
            //$$ await Manager.AddOnManager.AddTemplateAsync("Text");
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.maskedtextbox.min.js");
        }
        public async Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);
            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass(TemplateClass);
            tag.AddCssClass("t_display");
            tag.MergeAttribute("disabled", "disabled");
            FieldSetup(tag, FieldType.Anonymous);

            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", model ?? "");
            if (copy || rdonly)
                tag.MergeAttribute("readonly", "readonly");
            else
                tag.MergeAttribute("disabled", "disabled");

            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            if (copy) {
                await Manager.AddOnManager.AddAddOnGlobalAsync("clipboardjs.com", "clipboard");// add clipboard support
                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("Copy.png", Package, "Text");
                YTagBuilder tagImg = ImageHelper.BuildKnownImageYTag(imageUrl, title: this.__ResStr("ttCopy", "Copy to Clipboard"), alt: this.__ResStr("altCopy", "Copy to Clipboard"));
                tagImg.AddCssClass("yt_text_copy");
                hb.Append(tagImg.ToString(YTagRenderMode.StartTag));
            }
            return hb.ToYHtmlString();
        }
    }
    public abstract class TextEditComponentBase : YetaWFComponent, IYetaWFComponent<string> {

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public string TemplateName { get; set; }
        public string TemplateClass { get; set; }

        public TextEditComponentBase(string templateName, string templateClass) {
            TemplateName = templateName;
            TemplateClass = templateClass;
        }

        public override async Task IncludeAsync() {
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.maskedtextbox.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            bool useKendo = !Manager.IsRenderingGrid;

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass(TemplateClass);
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            //string id = null;
            //if (!string.IsNullOrWhiteSpace(mask)) {
            //    id = htmlHelper.MakeId(tag);
            //}

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", false);
            //string mask = htmlHelper.GetControlInfo<string>("", "Mask", null);

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (tag.Attributes.ContainsKey("maxlength"))
                    throw new InternalError("Both StringLengthAttribute and maxlength specified - {0}", FieldName);
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    tag.MergeAttribute("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !tag.Attributes.ContainsKey("maxlength"))
                throw new InternalError("No max string length given using StringLengthAttribute or maxlength - {0}", FieldName);
#endif
            // text
            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", model ?? "");
            tag.MergeAttribute("autocomplete", "on");
            if (!useKendo)
                tag.AddCssClass("ybrowsercontrols");

            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            if (copy) {
                await Manager.AddOnManager.AddAddOnGlobalAsync("clipboardjs.com", "clipboard");// add clipboard support
                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("Copy.png", Package, "Text");
                YTagBuilder tagImg = ImageHelper.BuildKnownImageYTag(imageUrl, title: this.__ResStr("ttCopy", "Copy to Clipboard"), alt: this.__ResStr("altCopy", "Copy to Clipboard"));
                tagImg.AddCssClass("yt_text_copy");
                hb.Append(tagImg.ToString(YTagRenderMode.StartTag));
            }

            //if (!string.IsNullOrWhiteSpace(mask)) {
            //    // if there is a Mask we need to use the KendoMaskedTextBox
            //    await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.maskedtextbox.min.js");
            //    ScriptBuilder sb = new ScriptBuilder();
            //    sb.Append("$('#{0}').kendoMaskedTextBox({{ mask: '{1}' }});\n", id, YetaWFManager.JserEncode(mask));
            //    Manager.ScriptManager.AddLastDocumentReady(sb);
            //}
            return hb.ToYHtmlString();
        }
    }
}
