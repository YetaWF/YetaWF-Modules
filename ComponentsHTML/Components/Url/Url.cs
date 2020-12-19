/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Url component implementation.
    /// </summary>
    public abstract class UrlComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(UrlComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Url";

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
    /// Displays the model as a URL with a link to visit the URL in a new window. If the model is null or empty, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("URL"), Description("The requested URL")]
    /// [UIHint("Url"), ReadOnly]
    /// public string RequestedUrl { get; set; }
    ///
    /// [Caption("Product")]
    /// [UIHint("Url"), ReadOnly]
    /// public string Description { get; set; }
    /// public string Description_Url { get; set; }
    /// </example>
    [UsesAdditional("CssClass", "string", null, "Defines an optional CSS class added to the URL link.")]
    [UsesSibling("_Url", "string", "If this property is specified, the model is used as the link text and this property is used as the actual URL.")]
    public class UrlDisplayComponent : UrlComponentBase, IYetaWFComponent<string> {

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
        public Task<string> RenderAsync(string model) {

            if (string.IsNullOrWhiteSpace(model)) return Task.FromResult<string>(null);

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append("<div class='yt_url t_display'>");

            string hrefUrl;
            if (!TryGetSiblingProperty($"{PropertyName}_Url", out hrefUrl))
                hrefUrl = model;

            if (string.IsNullOrWhiteSpace(hrefUrl) || (!hrefUrl.StartsWith("http://") && !hrefUrl.StartsWith("https://") && !hrefUrl.StartsWith("/"))) {
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
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows selection of a designed page URL or entering a local or remote URL.
    /// </summary>
    /// <example>
    /// [Category("Urls"), Caption("Redirect To Page"), Description("If this page is accessed, it is redirected to the Url defined here - Redirection is not active in Site Edit Mode")]
    /// [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local | UrlHelperEx.UrlTypeEnum.Remote)]
    /// [StringLength(Globals.MaxUrl), Trim]
    /// public string RedirectToPageUrl { get; set; }
    /// </example>
    [UsesAdditional("UrlType", "YetaWF.Core.Models.Attributes.UrlHelperEx.UrlTypeEnum", "UrlHelperEx.UrlTypeEnum.Remote", "Defines the type of URL that can be entered. UrlHelperEx.UrlTypeEnum is used as a flag so multiple types can be entered.")]
    public class UrlEditComponent : UrlComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class UrlEditSetup {
            public UrlTypeEnum Type { get; set; }
            public string Url { get; set; }
        }
        internal class UrlUI {

            [UIHint("Hidden")]
            public string Url { get; set; }

            [UIHint("UrlType")]
            public UrlTypeEnum UrlType { get; set; }
            [UIHint("UrlDesignedPage")]
            public string _Local { get; set; }
            [UIHint("UrlRemotePage")]
            public string _Remote { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            model = model ?? "";

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
            tag.MergeAttribute("href", Utility.UrlEncodePath(model));
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
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.UrlEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
