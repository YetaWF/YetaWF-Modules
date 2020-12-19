/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
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
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
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
    /// Displays the model as a string or as a read/only CKEditor. If a MultiString is provided, only the text in the user's defined language is shown (see User > Settings, standard YetaWF site).
    /// </summary>
    /// <example>
    /// [Caption("Message"), Description("The feedback message")]
    /// [UIHint("TextArea"), ReadOnly]
    /// [AdditionalMetadata("SourceOnly", true)]
    /// </example>
    [UsesAdditional("SourceOnly", "bool", "false", "Defines whether the text area is rendered using a read/only CKEditor in source mode only. Otherwise, the model is rendered as a string.")]
    [UsesAdditional("Full", "bool", "false", "Defines whether the text area is rendered using a read/only CKEditor, otherwise the model is rendered as a string.")]
    [UsesAdditional("EmHeight", "int", "10", "Defines whether the text area is rendered using a read/only CKEditor in source mode only. Otherwise, the model is rendered as a string.")]
    [UsesAdditional("SourceOnly", "bool", "false", "Defines the approximate height of the CKEditor in line heights based on the page font. The defined height can only be approximated and is by no means meant to be exact. This setting is ignored if the model is rendered as a string.")]
    [UsesAdditional("Encode", "bool", "true", "Defines whether \"\\r\\n\" and \"\\r\" are preserved as new lines using <br/> and the model is encoded when rendered as a string. Otherwise the string is not encoded. This is ignored if a read/only CKEditor is used to render the model.")]
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
        public async Task<string> RenderAsync(object model) {

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
                tag.SetInnerText(text);

                hb.Append(tag.ToString(YTagRenderMode.Normal));

                Manager.ScriptManager.AddLast($@"
CKEDITOR.replace('{ControlId}', {{
    customConfig: '{Utility.JserEncode(Manager.GetCDNUrl(url))}',
    height: '{emHeight}em'
}});");

            } else {

                if (string.IsNullOrWhiteSpace(text))
                    return null;

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
            return hb.ToString();
        }
    }

    /// <summary>
    /// Allows entry of a formatted HTML encoded string and returns the entered text as an HTML encoded string. If a MultiString is provided, only the text in the user's defined language can be modified (see User > Settings, standard YetaWF site).
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the TextArea box is limited to the specified number of characters.
    ///
    /// To render a regular TextArea HTML tag (without CKEditor), use the TextAreaSourceOnly Component instead.
    /// </remarks>
    /// <example>
    /// [Caption("Comment"), Description("Enter your comment about this blog entry for others to view")]
    /// [UIHint("TextArea"), AdditionalMetadata("EmHeight", 10), StringLength(BlogComment.MaxComment)]
    /// [AdditionalMetadata("TextAreaSave", false), AdditionalMetadata("RestrictedHtml", true)]
    /// public string Comment { get; set; }
    /// </example>
    [UsesAdditional("SourceOnly", "bool", "false", "Defines whether the text area is rendered using a CKEditor in source mode only. Otherwise, the full CKEditor feature set is available to create HTML formatted text.")]
    [UsesAdditional("TextAreaSave", "bool", "false", "Defines whether the CKEditor toolbar offers a Save button. Otherwise, the Save button is not shown. This is not currently working (apologies).")]
    [UsesAdditional("ImageBrowse", "bool", "false", "Defines whether image support is available to browse, upload and save images. Otherwise, image support is not available.")]
    [UsesAdditional("PageBrowse", "bool", "false", "Defines whether page selection support is available when adding links. Otherwise, page selection support is not available.")]
    [UsesAdditional("RestrictedHtml", "bool", "false", "Defines whether only an HTML subset is available when creating text. Otherwise, all HTML formatting is available. Restricting HTML tags does not work well with YetaWF and should not be used.")]
    [UsesAdditional("EmHeight", "int", "10", "Defines whether the text area is rendered using a read/only CKEditor in source mode only. Otherwise, the model is rendered as a string.")]
    [UsesSibling("_Folder", "string", "Used for image support to identify the module owning the images. If this property is not found, the current module is used as the image file owner.")]
    [UsesSibling("_SubFolder", "string", "Used for image support to identify the module owning the images. If this property is not found, the current module is used as the image file owner.")]
    public class TextAreaEditComponent : TextAreaComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class TextAreaSetup {
            public bool InPartialView { get; set; }
            public string CDNUrl { get; set; }
            public int EmHeight { get; set; }
            public bool RestrictedHtml { get; set; }
            public string FilebrowserImageBrowseUrl { get; set; }
            public string FilebrowserImageBrowseLinkUrl { get; set; }
            public string FilebrowserPageBrowseUrl { get; set; }
            public string FilebrowserWindowFeatures { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {

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
            bool usePageBrowsing = PropData.GetAdditionalAttributeValue("PageBrowse", false);
            bool restrictedHtml = PropData.GetAdditionalAttributeValue("RestrictedHtml", false);
            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);

            string filebrowserImageBrowseUrl = null;
            if (useImageBrowsing) {
                filebrowserImageBrowseUrl = string.Format("/__CKEditor/ImageBrowseLinkUrl?__FolderGuid={0}&__SubFolder={1}",
                    owningGuid.ToString(), subFolder.ToString());
                filebrowserImageBrowseUrl += "&" + Globals.Link_NoEditMode + "=y";
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

            tag.SetInnerText(text);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            TextAreaSetup setup = new TextAreaSetup {
                InPartialView = Manager.InPartialView,
                CDNUrl = Manager.GetCDNUrl(url),
                EmHeight = emHeight,
                RestrictedHtml = restrictedHtml,
                FilebrowserImageBrowseUrl = filebrowserImageBrowseUrl,
                FilebrowserImageBrowseLinkUrl = filebrowserImageBrowseUrl,
                FilebrowserPageBrowseUrl = filebrowserPageBrowseUrl,
                FilebrowserWindowFeatures = "modal=yes,location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,alwaysRaised=yes,resizable=yes,scrollbars=yes",
            };

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.TextAreaEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
