using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ImageComponent : YetaWFComponent {

        public const string TemplateName = "Image";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public static string FormatUrl(string imageType, string location, string name, int width = 0, int height = 0,
                string CacheBuster = null, bool ExternalUrl = false, PageDefinition.PageSecurityType SecurityType = PageDefinition.PageSecurityType.Any,
                bool Stretch = false) {
            string url;
            if (width > 0 && height > 0) {
                url = string.Format(YetaWF.Core.Addons.Templates.Image.FormatUrlWithSize, YetaWFManager.UrlEncodeArgs(imageType), YetaWFManager.UrlEncodeArgs(location), YetaWFManager.UrlEncodeArgs(name),
                    width, height, Stretch ? "1" : "0");
            } else {
                url = string.Format(YetaWF.Core.Addons.Templates.Image.FormatUrl, YetaWFManager.UrlEncodeArgs(imageType), YetaWFManager.UrlEncodeArgs(location), YetaWFManager.UrlEncodeArgs(name));
            }
            if (!string.IsNullOrWhiteSpace(CacheBuster))
                url += url.AddUrlCacheBuster(CacheBuster);
            url = Manager.GetCDNUrl(url);
            if (ExternalUrl) {
                // This is a local url, make the final url an external url, i.e., http(s)://
                if (url.StartsWith("/"))
                    url = Manager.CurrentSite.MakeUrl(url, PagePageSecurity: SecurityType);
            }
            return url;
        }
    }

    public class ImageDisplayComponent : ImageComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(string model) {

            string Alt = null;//$$$$

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append("<div class='yt_image t_display'>");

            string imageType = PropData.GetAdditionalAttributeValue<string>("ImageType", null);
            int width = PropData.GetAdditionalAttributeValue("Width", 0);
            int height = PropData.GetAdditionalAttributeValue("Height", 0);

            if (string.IsNullOrWhiteSpace(imageType) && model != null && (model.IsAbsoluteUrl() || model.StartsWith("/"))) {

                if (width != 0 || height != 0) throw new InternalError("Can't use Width or Height with external Urls");

                YTagBuilder img = new YTagBuilder("img");
                img.Attributes.Add("src", model);
                img.Attributes.Add("alt", this.__ResStr("altImage", "{0}", Alt ?? "Image"));
                return img.ToYHtmlString(YTagRenderMode.Normal);

            } else {

                if (string.IsNullOrWhiteSpace(imageType)) throw new InternalError("No ImageType specified");

                bool showMissing = PropData.GetAdditionalAttributeValue("ShowMissing", true);
                if (string.IsNullOrWhiteSpace(model) && !showMissing) return new YHtmlString("");

                string imgTag = RenderImage(imageType, width, height, model, CacheBuster: CacheBuster, Alt: Alt, ExternalUrl: ExternalUrl, SecurityType: SecurityType);

                bool linkToImage = PropData.GetAdditionalAttributeValue("LinkToImage", false);
                if (linkToImage) {
                    YTagBuilder link = new YTagBuilder("a");
                    string imgUrl = FormatUrl(imageType, null, model, CacheBuster: CacheBuster);
                    link.MergeAttribute("href", imgUrl);
                    link.MergeAttribute("target", "_blank");
                    link.MergeAttribute("rel", "noopener noreferrer");
                    link.InnerHtml = imgTag;
                    hb.Append(link.ToYHtmlString(YTagRenderMode.Normal));
                } else
                    hb.Append(imgTag);
            }
            hb.Append("</div>");
            return hb.ToYHtmlString();
        }
    }
    public class ImageEditComponent : ImageComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            await Manager.AddOnManager.AddAddOnGlobalAsync("ckeditor.com", "ckeditor");
            string addonUrl = AddOnManager.GetAddOnGlobalUrl("ckeditor.com", "ckeditor", YetaWF.Core.Addons.AddOnManager.UrlType.Base) + "__CUSTOM_FILES/";

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;
            Guid owningGuid = Guid.Empty;

            TryGetSiblingProperty<Guid>($"{PropertyName}_Folder", out owningGuid);
            if (owningGuid == Guid.Empty && Manager.CurrentModuleEdited != null)
                owningGuid = Manager.CurrentModuleEdited.ModuleGuid;
            if (owningGuid == Guid.Empty) {
                owningGuid = Manager.CurrentModule.ModuleGuid;
            }
            Guid subFolder = PropData.GetAdditionalAttributeValue("SubFolder", Guid.Empty);
            if (subFolder == Guid.Empty)
                TryGetSiblingProperty<Guid>($"{PropertyName}_SubFolder", out subFolder);

            bool sourceOnly = PropData.GetAdditionalAttributeValue("SourceOnly", false);
            bool useSave = PropData.GetAdditionalAttributeValue("ImageSave", false);
            bool useImageBrowsing = PropData.GetAdditionalAttributeValue("ImageBrowse", false);
            bool useFlashBrowsing = PropData.GetAdditionalAttributeValue("FlashBrowse", false);
            bool usePageBrowsing = PropData.GetAdditionalAttributeValue("PageBrowse", false);
            bool restrictedHtml = PropData.GetAdditionalAttributeValue("RestrictedHtml", false);
            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            int pixHeight = Manager.CharHeight * emHeight;

            string filebrowserImageBrowseUrl = null;
            if (useImageBrowsing) {
                filebrowserImageBrowseUrl = string.Format("/__CKEditor/ImageBrowseLinkUrl?__FolderGuid={0}&__SubFolder={1}",
                    owningGuid.ToString(), subFolder.ToString());
                filebrowserImageBrowseUrl += "&" + Globals.Link_NoEditMode + "=y";
            }
            string filebrowserFlashBrowseUrl = null;
            if (useFlashBrowsing) {
                filebrowserFlashBrowseUrl = string.Format("/__CKEditor/FlashBrowseLinkUrl?__FolderGuid={0}&__SubFolder={1}", owningGuid.ToString(), subFolder.ToString());
                filebrowserFlashBrowseUrl += "&" + Globals.Link_NoEditMode + "=y";
            }
            string filebrowserPageBrowseUrl = null;
            if (usePageBrowsing) {
                filebrowserPageBrowseUrl = "/__CKEditor/PageBrowseLinkUrl?";
                filebrowserPageBrowseUrl += Globals.Link_NoEditMode + "=y";
            }
            string url = addonUrl + "full_config.js";
            if (sourceOnly) 
                url = addonUrl + "sourceonly_config.js";
            else if (!useSave)
                url = addonUrl + "nosave_config.js";

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("Image");
            tag.AddCssClass("yt_Image");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.Attributes.Add("id", ControlId);
            tag.Attributes.Add("data-height", pixHeight.ToString());

            tag.SetInnerText(text);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            hb.Append($@"
                <script>
                    CKEDITOR.replace('{ControlId}', {{
                    customConfig: '{Manager.GetCDNUrl(url)}',
                        height: '{pixHeight}px',
                        allowedContent: {(restrictedHtml ? "false" : "true")},");

            if (!string.IsNullOrWhiteSpace(filebrowserImageBrowseUrl)) {
                hb.Append($@"
                    filebrowserImageBrowseUrl: {YetaWFManager.JserEncode(filebrowserImageBrowseUrl)},
                    filebrowserImageBrowseLinkUrl: {YetaWFManager.JserEncode(filebrowserImageBrowseUrl)},");
            }
            if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
                hb.Append($@"
                    filebrowserFlashBrowseUrl: {YetaWFManager.JserEncode(filebrowserFlashBrowseUrl)},");
            }
            if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
                hb.Append($@"
                    filebrowserBrowseUrl: {YetaWFManager.JserEncode(filebrowserPageBrowseUrl)},");
            }
            hb.Append($@"
                filebrowserWindowFeatures: 'modal=yes,location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,alwaysRaised=yes,resizable=yes,scrollbars=yes'
            }});
            // save data in the Image field when the form is submitted
            YetaWF_Forms.addPreSubmitHandler({(Manager.InPartialView? 1 : 0)}, {{
                form: YetaWF_Forms.getForm($('#{ControlId}')),
                callback: function(entry) {{
                    var $ctl = $('#{ControlId}');
                    var ckEd = CKEDITOR.instances['{ControlId}'];
                    var data = ckEd.getData();
                    $ctl.val(data);
                    //return $ctl[0].name + '&' + encodeURI(data);
                }}
            }});
            </script>");

            return hb.ToYHtmlString();
        }
    }
}
