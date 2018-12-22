/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class UrlComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(UrlComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Url";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class UrlDisplayComponent : UrlComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            if (string.IsNullOrWhiteSpace(model)) return Task.FromResult(new YHtmlString(""));

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append("<div class='yt_url t_display'>");

            string hrefUrl;
            if (!TryGetSiblingProperty($"{PropertyName}_Url", out hrefUrl))
                hrefUrl = model;

            if (string.IsNullOrWhiteSpace(hrefUrl)) {
                // no link
                YTagBuilder tag = new YTagBuilder("span");
                FieldSetup(tag, FieldType.Anonymous);

                string cssClass = PropData.GetAdditionalAttributeValue("CssClass", "");
                if (!string.IsNullOrWhiteSpace(cssClass))
                    tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));

                tag.SetInnerText(model);
                hb.Append(tag.ToString(YTagRenderMode.Normal));

            } else {
                // link
                YTagBuilder tag = new YTagBuilder("a");
                FieldSetup(tag, FieldType.Anonymous);

                string cssClass = PropData.GetAdditionalAttributeValue("CssClass", "");
                if (!string.IsNullOrWhiteSpace(cssClass))
                    tag.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule(cssClass));

                tag.MergeAttribute("href", hrefUrl);
                tag.MergeAttribute("target", "_blank");
                tag.MergeAttribute("rel", "nofollow noopener noreferrer");
                string text;
                if (!TryGetSiblingProperty($"{PropertyName}_Text", out text))
                    text = model;
                tag.SetInnerText(text);
                string tooltip = null;
                TryGetSiblingProperty($"{PropertyName}_ToolTip", out tooltip);
                if (!string.IsNullOrWhiteSpace(tooltip))
                    tag.MergeAttribute(Basics.CssTooltip, tooltip);

                // image
                if (PropData.GetAdditionalAttributeValue("ShowImage", true)) {
                    tag.InnerHtml = tag.InnerHtml + ImageHTML.BuildKnownIcon("#UrlRemote", sprites: Info.PredefSpriteIcons);
                }
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            hb.Append("</div>");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class UrlEditComponent : UrlComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class UrlEditSetup {
            public UrlTypeEnum Type { get; set; }
            public string Url { get; set; }
        }
        public class UrlUI {

            [UIHint("Hidden")]
            public string Url { get; set; }

            [UIHint("UrlType")]
            public UrlTypeEnum UrlType { get; set; }
            [UIHint("UrlDesignedPage")]
            public string _Local { get; set; }
            [UIHint("UrlRemotePage")]
            public string _Remote { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            UrlTypeEnum type = PropData.GetAdditionalAttributeValue("UrlType", UrlTypeEnum.Remote);

            UrlUI ui = new UrlUI {
                UrlType = type,
                _Local = model,
                _Remote = model,
            };

            hb.Append($@"
<div id='{ControlId}' class='yt_url t_edit'>");

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("t_hidden");
            tag.Attributes["type"] = "hidden";
            FieldSetup(tag, FieldType.Validated);
            tag.MergeAttribute("value", model);
            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
    {await HtmlHelper.ForEditAsync(ui, nameof(ui.UrlType), Validation: false)}
");

                if ((type & UrlTypeEnum.Local) != 0) {
                    hb.Append($@"
    <div class='t_local'>
        {await HtmlHelper.ForEditAsync(ui, nameof(ui._Local), Validation: false)}
    </div>");
                }
                if ((type & UrlTypeEnum.Remote) != 0) {
                    hb.Append($@"
    <div class='t_remote'>
        {await HtmlHelper.ForEditAsync(ui, nameof(ui._Remote), Validation: false)}
    </div>");
                }
            }

            // link
            tag = new YTagBuilder("a");
            tag.MergeAttribute("href", YetaWFManager.UrlEncodePath(model));
            tag.MergeAttribute("target", "_blank");
            tag.MergeAttribute("rel", "nofollow noopener noreferrer");

            tag.InnerHtml = tag.InnerHtml + ImageHTML.BuildKnownIcon("#UrlRemote", sprites: Info.PredefSpriteIcons);
            string link = tag.ToString(YTagRenderMode.Normal);

            UrlEditSetup setup = new UrlEditSetup {
                Type = type,
                Url = model,
            };

            hb.Append($@"
    <div class='t_link'>
        {link}
    </div>
</div>
<script>
    new YetaWF_ComponentsHTML.UrlEditComponent('{ControlId}', {YetaWFManager.JsonSerialize(setup)});
</script>");
            return hb.ToYHtmlString();
        }
    }
}
