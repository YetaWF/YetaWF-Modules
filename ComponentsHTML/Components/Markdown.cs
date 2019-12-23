/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// An instance of this class contains markdown text and HTML used with the Markdown component.
    /// </summary>
    public class MarkdownStringBase {
        /// <summary>
        /// The markdown text.
        /// </summary>
        public virtual string Text { get; set; }
        /// <summary>
        /// The HTML rendering of the markdown text.
        /// </summary>
        public virtual string HTML { get; set; }
    }

    /// <summary>
    /// Base class for the Markdown component implementation.
    /// </summary>
    public abstract class MarkdownComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(MarkdownComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Markdown";

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
    /// Implementation of the Markdown display component.
    /// </summary>
    public class MarkdownDisplayComponent : MarkdownComponentBase, IYetaWFComponent<MarkdownStringBase> {

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
        public Task<string> RenderAsync(MarkdownStringBase model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_markdown t_display'>
    {model.HTML}
</div>");

            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Implementation of the Markdown edit component.
    /// </summary>
    public class MarkdownEditComponent : MarkdownComponentBase, IYetaWFComponent<MarkdownStringBase> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(MarkdownStringBase model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "showdownjs.com");

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            int pixHeight = Manager.CharHeight * emHeight;

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tagTextArea = new YTagBuilder("textarea");
            //TODO: Needs fieldsetup for validation
            tagTextArea.Attributes.Add("name", $"{FieldName}.Text");
            tagTextArea.AddCssClass("t_edit");
            tagTextArea.AddCssClass("k-textbox"); // USE KENDO style
            tagTextArea.Attributes.Add("rows", emHeight.ToString());

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (tagTextArea.Attributes.ContainsKey("maxlength"))
                    throw new InternalError($"Both StringLengthAttribute and maxlength specified - {FieldName}");
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    tagTextArea.MergeAttribute("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !tagTextArea.Attributes.ContainsKey("maxlength"))
                throw new InternalError($"No max string length given using StringLengthAttribute or maxlength - {FieldName}");
#endif
            tagTextArea.SetInnerText(model.Text);

            hb.Append($@"
<div id='{ControlId}' class='yt_markdown t_edit'>
    <div class='t_markdown' id='{DivId}'>
        {PropertyListComponentBase.RenderTabStripStart(DivId)}
            {PropertyListComponentBase.RenderTabEntry(DivId, __ResStr("edit", "Edit"), null, 0)}
            {PropertyListComponentBase.RenderTabEntry(DivId, __ResStr("preview", "Preview"), null, 1)}
        {PropertyListComponentBase.RenderTabStripEnd(DivId)}
        {PropertyListComponentBase.RenderTabPaneStart(DivId, 0, "t_edit")}
            {tagTextArea.ToString(YTagRenderMode.Normal)}
        {PropertyListComponentBase.RenderTabPaneEnd(DivId, 0)}
        {PropertyListComponentBase.RenderTabPaneStart(DivId, 1, "t_preview")}
            <input type='hidden' name='{FieldName}.HTML' class='t_html' value='{HAE(model.HTML)}'>
            <div class='t_previewpane' style='min-height:{pixHeight}px'>{model.HTML}</div>
        {PropertyListComponentBase.RenderTabPaneEnd(DivId, 1)}
    </div>
    {await PropertyListComponentBase.RenderTabInitAsync(DivId, null)}
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MarkdownEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}
