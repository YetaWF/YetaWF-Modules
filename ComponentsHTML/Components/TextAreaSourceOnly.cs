/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
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
    /// Implementation of the TextAreaSourceOnly display component.
    /// </summary>
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

            bool copy = PropData.GetAdditionalAttributeValue<bool>("Copy", true);

            if (!string.IsNullOrWhiteSpace(text)) {

                int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);

                YTagBuilder tag = new YTagBuilder("textarea");
                tag.AddCssClass("yt_textareasourceonly");
                tag.AddCssClass("t_display");
                tag.AddCssClass("k-textbox"); // USE KENDO style
                //tag.AddCssClass("k-state-disabled"); // USE KENDO style
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add("id", ControlId);
                tag.Attributes.Add("rows", emHeight.ToString());
                if (copy)
                    tag.Attributes.Add("readonly", "readonly");
                else
                    tag.Attributes.Add("disabled", "disabled");
                tag.SetInnerText(text);

                hb.Append(tag.ToString(YTagRenderMode.Normal));

                if (copy)
                    hb.Append(ImageHTML.BuildKnownIcon("#TextAreaSourceOnlyCopy", sprites: Info.PredefSpriteIcons, title: __ResStr("ttCopy", "Copy to Clipboard"), cssClass: "yt_textareasourceonly_copy"));
            }
            if (copy) {
                await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "clipboardjs.com.clipboard");// add clipboard support
            }

            return hb.ToString();
        }
    }

    /// <summary>
    /// Implementation of the TextAreaSourceOnly edit component.
    /// </summary>
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
        public Task<string> RenderAsync(object model) {

            string text;
            if (model is MultiString)
                text = (MultiString)model;
            else
                text = (string)model;

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);

            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("textarea");
            tag.AddCssClass("yt_textareasourceonly");
            tag.AddCssClass("t_edit");
            tag.AddCssClass("k-textbox"); // USE KENDO style
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.Attributes.Add("id", ControlId);
            tag.Attributes.Add("rows", emHeight.ToString());

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr != null) {
#if DEBUG
                if (tag.Attributes.ContainsKey("maxlength"))
                    throw new InternalError($"Both StringLengthAttribute and maxlength specified - {FieldName}");
#endif
                int maxLength = lenAttr.MaximumLength;
                if (maxLength > 0 && maxLength <= 8000)
                    tag.MergeAttribute("maxlength", maxLength.ToString());
            }
#if DEBUG
            if (lenAttr == null && !tag.Attributes.ContainsKey("maxlength"))
                throw new InternalError($"No max string length given using StringLengthAttribute or maxlength - {FieldName}");
#endif

            tag.SetInnerText(text);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.TextAreaSourceOnlyEditComponent('{ControlId}');");

            return Task.FromResult(hb.ToString());
        }
    }
}
