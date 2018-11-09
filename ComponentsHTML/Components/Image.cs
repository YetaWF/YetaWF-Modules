/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Image;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ImageComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ImageComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Image";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public static string RenderImage(string imageType, int width, int height, string model,
                string CacheBuster = null, string Alt = null, bool ExternalUrl = false, PageDefinition.PageSecurityType SecurityType = PageDefinition.PageSecurityType.Any) {
            string url = ImageHTML.FormatUrl(imageType, null, model, width, height, CacheBuster: CacheBuster, ExternalUrl: ExternalUrl, SecurityType: SecurityType);
            YTagBuilder img = new YTagBuilder("img");
            img.AddCssClass("t_preview");
            img.Attributes.Add("src", url);
            img.Attributes.Add("alt", Alt ?? __ResStr("altImg", "Image"));
            return img.ToString(YTagRenderMode.StartTag);
        }
        public static async Task<string> RenderImageAttributesAsync(string model) {
            if (model == null) return "";
            System.Drawing.Size size = await ImageSupport.GetImageSizeAsync(model);
            if (size.IsEmpty) return "";
            return __ResStr("imgAttr", "{0} x {1} (w x h)", size.Width, size.Height);
        }
    }

    public class ImageDisplayComponent : ImageComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append("<div class='yt_image t_display'>");

            string imageType = PropData.GetAdditionalAttributeValue<string>("ImageType", null);
            int width = PropData.GetAdditionalAttributeValue("Width", 0);
            int height = PropData.GetAdditionalAttributeValue("Height", 0);

            if (string.IsNullOrWhiteSpace(imageType) && model != null && (model.IsAbsoluteUrl() || model.StartsWith("/"))) {

                if (width != 0 || height != 0) throw new InternalError("Can't use Width or Height with external Urls");

                YTagBuilder img = new YTagBuilder("img");
                img.Attributes.Add("src", model);
                if (!img.Attributes.ContainsKey("alt"))
                    img.Attributes.Add("alt", __ResStr("altImage", "Image"));
                hb.Append(img.ToYHtmlString(YTagRenderMode.Normal));

            } else {

                if (string.IsNullOrWhiteSpace(imageType)) throw new InternalError("No ImageType specified");

                bool showMissing = PropData.GetAdditionalAttributeValue("ShowMissing", true);
                if (string.IsNullOrWhiteSpace(model) && !showMissing)
                    return Task.FromResult(new YHtmlString(""));

                string alt = null;
                if (HtmlAttributes.ContainsKey("alt"))
                    alt = (string)HtmlAttributes["alt"];
                string imgTag = ImageComponentBase.RenderImage(imageType, width, height, model, Alt: alt);

                bool linkToImage = PropData.GetAdditionalAttributeValue("LinkToImage", false);
                if (linkToImage) {
                    YTagBuilder link = new YTagBuilder("a");
                    string imgUrl = ImageHTML.FormatUrl(imageType, null, model);
                    link.MergeAttribute("href", imgUrl);
                    link.MergeAttribute("target", "_blank");
                    link.MergeAttribute("rel", "noopener noreferrer");
                    link.InnerHtml = imgTag;
                    hb.Append(link.ToYHtmlString(YTagRenderMode.Normal));
                } else
                    hb.Append(imgTag);
            }

            hb.Append("</div>");
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class ImageEditComponent : ImageComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ImageEditSetup {
            public string UploadId { get; set; }
        }

        public async Task<YHtmlString> RenderAsync(string model) {

            // the upload control
            Core.Components.FileUpload1 setupUpload = new Core.Components.FileUpload1() {
                SaveURL = YetaWFManager.UrlFor(typeof(FileUpload1Controller), nameof(FileUpload1Controller.SaveImage), new { __ModuleGuid = Manager.CurrentModule.ModuleGuid }),
                RemoveURL = YetaWFManager.UrlFor(typeof(FileUpload1Controller), nameof(FileUpload1Controller.RemoveImage), new { __ModuleGuid = Manager.CurrentModule.ModuleGuid }),
            };

            string uploadId = ControlId + "_ul1";

            ImageEditSetup setup = new ImageEditSetup {
                UploadId = uploadId,
            };

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<div class='yt_image t_edit' id='{ControlId}'>
    {(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model, "Hidden", Validation:true)).ToString()}
    <div class='t_image'>
        {(await HtmlHelper.ForDisplayComponentAsync(Container, PropertyName, model, TemplateName, HtmlAttributes: new { alt = __ResStr("imgAlt", "Preview Image") } )).ToString()}
    </div>
    <div class='t_info'>
        {(await RenderImageAttributesAsync(model)).ToString()}
    </div>
    <div class='t_haveimage' {(string.IsNullOrWhiteSpace(model) ? "style='display:none'" : "")}>
        <input type='button' class='t_clear' value='{__ResStr("btnClear", "Clear")}' title='{__ResStr("txtClear", "Click to clear the current image")}' />
    </div>
    {(await HtmlHelper.ForEditContainerAsync(setupUpload, "FileUpload1", HtmlAttributes:new { id = uploadId })).ToString()}
</div>
<script>
    new YetaWF_ComponentsHTML.ImageEditComponent('{ControlId}', {YetaWFManager.JsonSerialize(setup)});
</script>");

            return hb.ToYHtmlString();
        }
    }
}
