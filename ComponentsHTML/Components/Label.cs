/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class LabelDisplayComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "Label";
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder sb = new HtmlBuilder();

            string label;
            if (TryGetSiblingProperty<string>($"{PropertyName}_Label", out label))
                model = label;

            YTagBuilder tagLabel = new YTagBuilder("label");
            FieldSetup(tagLabel, FieldType.Anonymous);
            if (string.IsNullOrEmpty(model)) // we're distinguishing between "" and " "
                tagLabel.InnerHtml = "&nbsp;";
            else
                tagLabel.SetInnerText(model);
            sb.Append(tagLabel.ToString(YTagRenderMode.Normal));

            string helpLink;
            if (TryGetSiblingProperty<string>($"{PropertyName}_HelpLink", out helpLink) && !string.IsNullOrWhiteSpace(helpLink)) {
                YTagBuilder tagA = new YTagBuilder("a");
                tagA.Attributes.Add("href", YetaWFManager.UrlEncodePath(helpLink));
                tagA.Attributes.Add("target", "_blank");
                tagA.MergeAttribute("rel", "noopener noreferrer");
                tagA.AddCssClass(Manager.AddOnManager.CheckInvokedCssModule("yt_extlabel_img"));
                SkinImages skinImages = new SkinImages();
                string imageUrl = await skinImages.FindIcon_TemplateAsync("HelpLink.png", Package, "Label");
                YTagBuilder tagImg = ImageHTML.BuildKnownImageYTag(imageUrl, alt: this.__ResStr("altHelp", "Help"));
                tagA.InnerHtml = tagImg.ToString(YTagRenderMode.StartTag);
                sb.Append(tagA.ToString(YTagRenderMode.Normal));
            }

            return sb.ToYHtmlString();
        }
    }
}
