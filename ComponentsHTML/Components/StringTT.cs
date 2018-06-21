/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class StringTTComponentBase : YetaWFComponent {

        public const string TemplateName = "StringTT";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class StringTTDisplayComponent : StringTTComponentBase, IYetaWFComponent<StringTT> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(StringTT model) {
            return RenderStringTTAsync(this, model, null);
        }
        public static Task<YHtmlString> RenderStringTTAsync(YetaWFComponent component, StringTT model, string cssClass) {
            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("span");
            if (!string.IsNullOrWhiteSpace(cssClass)) {
                tag.AddCssClass(cssClass);
                tag.AddCssClass("t_display");
            }
            component.FieldSetup(tag, FieldType.Anonymous);

            if (!string.IsNullOrWhiteSpace(model.Tooltip))
                tag.Attributes.Add(Basics.CssTooltipSpan, model.Tooltip);
            if (!string.IsNullOrWhiteSpace(model.Text))
                tag.SetInnerText(model.Text);
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
        }
    }

    public static class StringTTExtender {
#if MVC6
        public static YHtmlString ForStringTTDisplay(this IHtmlHelper htmlHelper, string text, string tooltip)
#else
        public static YHtmlString ForStringTTDisplay(this HtmlHelper htmlHelper, string text, string tooltip)
#endif
        {
            YTagBuilder tag = new YTagBuilder("span");
            if (!string.IsNullOrWhiteSpace(tooltip))
                tag.Attributes.Add(Basics.CssTooltipSpan, tooltip);
            if (!string.IsNullOrWhiteSpace(text))
                tag.SetInnerText(text);
            return tag.ToYHtmlString(YTagRenderMode.Normal);
        }
    }
}
