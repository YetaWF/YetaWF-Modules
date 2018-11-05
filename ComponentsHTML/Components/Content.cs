/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ContentComponent : YetaWFComponent {

        public const string TemplateName = "Content";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ContentDisplayComponent : ContentComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(Globals.LazyHTMLOptimization);

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_content");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);

            if (string.IsNullOrWhiteSpace(text))
                text = "&nbsp;"; //so the div is not empty

            tag.InnerHtml = text;

            hb.Append(tag.ToString(YTagRenderMode.Normal));
            hb.Append(Globals.LazyHTMLOptimizationEnd);

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class ContentEditComponent : ContentComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class ContentSetup {

        }

        public async Task<YHtmlString> RenderAsync(object model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "ckeditor5-classic");
            //$$string addonUrl = Manager.AddOnManager.GetAddOnNamedUrl(Package.AreaName, "ckeditor") + "__CUSTOM_FILES/";

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;
            Guid owningGuid = Guid.Empty;

            //$$TryGetSiblingProperty<Guid>($"{PropertyName}_Folder", out owningGuid);
            //if (owningGuid == Guid.Empty && Manager.CurrentModuleEdited != null)
            //    owningGuid = Manager.CurrentModuleEdited.ModuleGuid;
            //if (owningGuid == Guid.Empty) {
            //    owningGuid = Manager.CurrentModule.ModuleGuid;
            //}
            //Guid subFolder = PropData.GetAdditionalAttributeValue("SubFolder", Guid.Empty);
            //if (subFolder == Guid.Empty)
            //    TryGetSiblingProperty<Guid>($"{PropertyName}_SubFolder", out subFolder);

            //$$bool useSave = PropData.GetAdditionalAttributeValue("ContentSave", false);
            //bool useImageBrowsing = PropData.GetAdditionalAttributeValue("ImageBrowse", false);
            //bool useFlashBrowsing = PropData.GetAdditionalAttributeValue("FlashBrowse", false);
            //bool usePageBrowsing = PropData.GetAdditionalAttributeValue("PageBrowse", false);
            //bool restrictedHtml = PropData.GetAdditionalAttributeValue("RestrictedHtml", false);
            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            //$$ int pixHeight = Manager.CharHeight * emHeight;

            //string filebrowserImageBrowseUrl = null;
            //if (useImageBrowsing) {
            //    filebrowserImageBrowseUrl = string.Format("/__CKEditor/ImageBrowseLinkUrl?__FolderGuid={0}&__SubFolder={1}",
            //        owningGuid.ToString(), subFolder.ToString());
            //    filebrowserImageBrowseUrl += "&" + Globals.Link_NoEditMode + "=y";
            //}
            //string filebrowserFlashBrowseUrl = null;
            //if (useFlashBrowsing) {
            //    filebrowserFlashBrowseUrl = string.Format("/__CKEditor/FlashBrowseLinkUrl?__FolderGuid={0}&__SubFolder={1}", owningGuid.ToString(), subFolder.ToString());
            //    filebrowserFlashBrowseUrl += "&" + Globals.Link_NoEditMode + "=y";
            //}
            //string filebrowserPageBrowseUrl = null;
            //if (usePageBrowsing) {
            //    filebrowserPageBrowseUrl = "/__CKEditor/PageBrowseLinkUrl?";
            //    filebrowserPageBrowseUrl += Globals.Link_NoEditMode + "=y";
            //}
            //string url = addonUrl + "full_config.js";
            //if (sourceOnly)
            //    url = addonUrl + "sourceonly_config.js";
            //else if (!useSave)
            //    url = addonUrl + "nosave_config.js";

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("textarea");
            tag.AddCssClass("yt_content");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.Attributes.Add("id", ControlId);
            //$$ tag.Attributes.Add("data-height", pixHeight.ToString());

            tag.SetInnerText(text);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            ContentSetup setup = new ContentSetup();

            hb.Append($@"
<script>
    new YetaWF_ComponentsHTML.ContentEditComponent('{ControlId}', {YetaWFManager.JsonSerialize(setup)});
</script>");

            //if (!string.IsNullOrWhiteSpace(filebrowserImageBrowseUrl)) {
            //    hb.Append($@"
            //        filebrowserImageBrowseUrl: '{YetaWFManager.JserEncode(filebrowserImageBrowseUrl)}',
            //        filebrowserImageBrowseLinkUrl: '{YetaWFManager.JserEncode(filebrowserImageBrowseUrl)}',");
            //}
            //if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
            //    hb.Append($@"
            //        filebrowserFlashBrowseUrl: '{YetaWFManager.JserEncode(filebrowserFlashBrowseUrl)}',");
            //}
            //if (!string.IsNullOrWhiteSpace(filebrowserFlashBrowseUrl)) {
            //    hb.Append($@"
            //        filebrowserBrowseUrl: '{YetaWFManager.JserEncode(filebrowserPageBrowseUrl)}',");
            //}
            //hb.Append($@"
            //    filebrowserWindowFeatures: 'modal=yes,location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,alwaysRaised=yes,resizable=yes,scrollbars=yes'
            //}});
            //// save data in the textarea field when the form is submitted
            //$YetaWF.Forms.addPreSubmitHandler({(Manager.InPartialView? 1 : 0)}, {{
            //    form: $YetaWF.Forms.getForm($YetaWF.getElementById('{ControlId}')),
            //    callback: function(entry) {{
            //        var $ctl = $('#{ControlId}');
            //        var ckEd = CKEDITOR.instances['{ControlId}'];
            //        var data = ckEd.getData();
            //        $ctl.val(data);
            //        //return $ctl[0].name + '&' + encodeURI(data);
            //    }}
            //}});
            //</script>");

            return hb.ToYHtmlString();
        }
    }
}
