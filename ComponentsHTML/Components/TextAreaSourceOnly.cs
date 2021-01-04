/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TextAreaSourceOnly component implementation.
    /// </summary>
    public abstract class TextAreaSourceOnlyComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TextAreaSourceOnlyComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "TextAreaSourceOnly";

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
    /// Displays the model rendered as a string using the HTML &lt;textarea&gt; tag. The model is assumed to already be HTML encoded.
    /// </summary>
    /// <example>
    /// [Caption("Message"), Description("The feedback message")]
    /// [UIHint("TextAreaSourceOnly"), ReadOnly]
    /// public string Comment { get; set; }
    /// </example>
    [UsesAdditional("EmHeight", "int", "10", "Defines the height of the component in lines of text.")]
    [UsesAdditional("Copy", "bool", "true", "Defines whether a copy icon is displayed to allow the user to copy the contents to the clipboard. If false is specified, no copy icon is shown.")]
    public class TextAreaSourceOnlyDisplayComponent : TextAreaSourceOnlyComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.UseAsync();// needed for css
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {

            HtmlBuilder hb = new HtmlBuilder();

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;
            text ??= string.Empty;

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);
            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);

            string readOnly = string.Empty;
            string disabled = string.Empty;
            if (copy)
                readOnly = " readonly='readonly'";
            else
                disabled = " disabled='disabled'";

            hb.Append($"<textarea id='{ControlId}'{FieldSetup(FieldType.Anonymous)} rows='{emHeight}' class='yt_textareasourceonly t_display'{readOnly}{disabled}>{HE(text)}</textarea>");

            if (copy) {
                hb.Append(ImageHTML.BuildKnownIcon("#TextAreaSourceOnlyCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_textareasourceonly_copy"));
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
            }

            return hb.ToString();
        }
    }

    /// <summary>
    /// Allows entry of a formatted HTML encoded string using the HTML &lt;textarea&gt; tag. The model is assumed to already be HTML encoded.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the textarea element is limited to the specified number of characters.
    ///
    /// To render a more advanced text editor using CKEditor, use the TextArea Component instead.
    /// </remarks>
    /// <example>
    /// [Caption("Comment"), Description("Enter your comment about this blog entry for others to view")]
    /// [UIHint("TextAreaSourceOnly"), StringLength(BlogComment.MaxComment)]
    /// public string Comment { get; set; }
    /// </example>
    [UsesAdditional("EmHeight", "int", "10", "Defines the height of the component in lines of text.")]
    [UsesAdditional("Spellcheck", "boolean", "true", "Defines whether spell checking is on/off for the text area.")]
    [UsesSibling("_PlaceHolder", "string", "Defines the placeholder text shown when control contents are empty.")]
    [UsesAdditional("Copy", "bool", "false", "Defines whether a copy icon is displayed to allow the user to copy the contents to the clipboard. If false is specified, no copy icon is shown.")]
    public class TextAreaSourceOnlyEditComponent : TextAreaSourceOnlyComponentBase, IYetaWFComponent<object> {

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
        public async Task<string> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);
            bool spellcheck = PropData.GetAdditionalAttributeValue("Spellcheck", true);
            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", false);

            string placeHolder;
            TryGetSiblingProperty<string>($"{PropertyName}_PlaceHolder", out placeHolder);

            if (placeHolder != null)
                placeHolder = $" placeholder='{HAE(placeHolder)}'";

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (HtmlAttributes.ContainsKey("maxlength"))
                    throw new InternalError($"Both StringLengthAttribute and maxlength specified - {FieldName}");
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    HtmlAttributes.Add("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !HtmlAttributes.ContainsKey("maxlength"))
                throw new InternalError($"No max string length given using StringLengthAttribute or maxlength - {FieldName}");
#endif
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($"<textarea id='{ControlId}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} rows='{emHeight}' spellcheck='{(spellcheck ? "true" : "false")}' class='yt_textareasourceonly t_edit'{placeHolder}>{HE(text)}</textarea>");
            if (copy) {
                hb.Append(ImageHTML.BuildKnownIcon("#TextAreaSourceOnlyCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_textareasourceonly_copy"));
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
            }

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.TextAreaSourceOnlyEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}
