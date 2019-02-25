/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TextAreaSourceOnlyComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TextAreaSourceOnlyComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "TextAreaSourceOnly";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class TextAreaSourceOnlyDisplayComponent : TextAreaSourceOnlyComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            if (string.IsNullOrWhiteSpace(text))
                return new YHtmlString();

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);

            HtmlBuilder hb = new HtmlBuilder();

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            int pixHeight = Manager.CharHeight * emHeight;

            YTagBuilder tag = new YTagBuilder("textarea");
            tag.AddCssClass("yt_textareasourceonly");
            tag.AddCssClass("t_display");
            tag.AddCssClass("k-textbox"); // USE KENDO style
            tag.AddCssClass("k-state-disabled"); // USE KENDO style
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("id", ControlId);
            tag.Attributes.Add("rows", emHeight.ToString());
            if (copy)
                tag.Attributes.Add("readonly", "readonly");
            else
                tag.Attributes.Add("disabled", "disabled");
            tag.SetInnerText(text);

            hb.Append(tag.ToHtmlString(YTagRenderMode.Normal));
            if (copy) {
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
                hb.Append(ImageHTML.BuildKnownIcon("#TextAreaSourceOnlyCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_textareasourceonly_copy"));
            }

            return hb.ToYHtmlString();
        }
    }
    public class TextAreaSourceOnlyEditComponent : TextAreaSourceOnlyComponentBase, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public Task<YHtmlString> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            int pixHeight = Manager.CharHeight * emHeight;

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("textarea");
            tag.AddCssClass("yt_textareasourceonly");
            tag.AddCssClass("t_edit");
            tag.AddCssClass("k-textbox"); // USE KENDO style
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.Attributes.Add("id", ControlId);
            tag.Attributes.Add("rows", emHeight.ToString());

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (tag.Attributes.ContainsKey("maxlength"))
                    throw new InternalError($"Both StringLengthAttribute and maxlength specified - {FieldName}");
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    tag.MergeAttribute("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !tag.Attributes.ContainsKey("maxlength"))
                throw new InternalError($"No max string length given using StringLengthAttribute or maxlength - {FieldName}");
#endif

            tag.SetInnerText(text);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
