/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TextAreaSourceOnlyComponent : YetaWFComponent {

        public const string TemplateName = "TextAreaSourceOnly";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class TextAreaSourceOnlyDisplayComponent : TextAreaSourceOnlyComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

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
            tag.Attributes.Add("disabled", "disabled");
            tag.SetInnerText(text);

            hb.Append(tag.ToHtmlString(YTagRenderMode.Normal));

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class TextAreaSourceOnlyEditComponent : TextAreaSourceOnlyComponent, IYetaWFComponent<object> {

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
