/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TextArea component implementation.
    /// </summary>
    public abstract class TextAreaComponentBase : YetaWFComponent {

        internal const string TemplateName = "TextArea";

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
    }

    /// <summary>
    /// Implementation of the TextArea display component.
    /// </summary>
    public class TextAreaDisplayComponent : TextAreaComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            bool sourceOnly = PropData.GetAdditionalAttributeValue("SourceOnly", false);
            bool full = PropData.GetAdditionalAttributeValue("Full", false);

            HtmlBuilder hb = new HtmlBuilder();

            if (full || sourceOnly) {

                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "ckeditor");

                int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
                int pixHeight = Manager.CharHeight * emHeight;

                string addonUrl = Manager.AddOnManager.GetAddOnNamedUrl(Package.AreaName, "ckeditor") + "__CUSTOM_FILES/";
                string url = addonUrl + "full_ro_config.js";
                if (sourceOnly)
                    url = addonUrl + "sourceonly_ro_config.js";

                YTagBuilder tag = new YTagBuilder("textarea");
                tag.AddCssClass("yt_textarea");
                tag.AddCssClass("t_edit");
                tag.AddCssClass("t_readonly");
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add("id", ControlId);
                tag.Attributes.Add("data-height", pixHeight.ToString());
                tag.SetInnerText(text);

                hb.Append(tag.ToHtmlString(YTagRenderMode.Normal));

                hb.Append($@"<script>
                    CKEDITOR.replace('{ControlId}', {{
                        customConfig: '{YetaWFManager.JserEncode(Manager.GetCDNUrl(url))}',
                        height: '{pixHeight}px'
                    }});</script>");
            } else {

                if (string.IsNullOrWhiteSpace(text))
                    return new YHtmlString();

                hb.Append(Globals.LazyHTMLOptimization);

                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_textarea");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);

                bool encode = PropData.GetAdditionalAttributeValue("Encode", true);
                if (encode) {
                    tag.SetInnerText(text);
                    text = tag.InnerHtml;
                    text = text.Replace("\r\n", "<br/>");
                    text = text.Replace("\n", "<br/>");
                }
                tag.InnerHtml = text;

                hb.Append(tag.ToString(YTagRenderMode.Normal));
                hb.Append(Globals.LazyHTMLOptimizationEnd);
            }
            return hb.ToYHtmlString();
        }
    }

    /// <summary>
    /// Implementation of the TextArea edit component.
    /// </summary>
    public class TextAreaEditComponent : TextAreaComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderAsync(object model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "ckeditor");
            string addonUrl = Manager.AddOnManager.GetAddOnNamedUrl(Package.AreaName, "ckeditor") + "__CUSTOM_FILES/";

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
            bool useSave = PropData.GetAdditionalAttributeValue("TextAreaSave", false);
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

            YTagBuilder tag = new YTagBuilder("textarea");
            tag.AddCssClass("yt_textarea");
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
        filebrowserImageBrowseUrl: '{YetaWFManager.JserEncode(filebrowserImageBrowseUrl)}',
        filebrowserImageBrowseLinkUrl: '{YetaWFManager.JserEncode(filebrowserImageBrowseUrl)}',");
            }
            if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
                hb.Append($@"
        filebrowserFlashBrowseUrl: '{YetaWFManager.JserEncode(filebrowserFlashBrowseUrl)}',");
            }
            if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
                hb.Append($@"
        filebrowserBrowseUrl: '{YetaWFManager.JserEncode(filebrowserPageBrowseUrl)}',");
            }
            hb.Append($@"
        filebrowserWindowFeatures: 'modal=yes,location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,alwaysRaised=yes,resizable=yes,scrollbars=yes'
    }});
    // save data in the textarea field when the form is submitted
    $YetaWF.Forms.addPreSubmitHandler({(Manager.InPartialView? 1 : 0)}, {{
        form: $YetaWF.Forms.getForm($YetaWF.getElementById('{ControlId}')),
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
