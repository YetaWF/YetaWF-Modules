/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays the model using the user's defined language (see User > Settings, standard YetaWF site). Strings for other languages contained with the model are not displayed.
    /// Should be used for text up to 10 characters in width.
    /// </summary>
    /// <example>
    /// [Caption("Title"), Description("The title for this blog entry")]
    /// [UIHint("MultiString10"), StringLength(BlogEntry.MaxTitle), Required, Trim]
    /// public MultiString Title { get; set; }
    /// </example>
    public class MultiString10DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString10DisplayComponent() : base("MultiString10", "t_text10") { }
    }
    /// <summary>
    /// Allows entry of strings in all site defined languages (see National Language Support). Renders a text box with an adjacent dropdown list showing each available language so the user can enter text for each available language.
    /// Should be used for text up to 10 characters in width.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the Text box is limited to the specified number of characters.
    /// </remarks>
    /// <example>
    /// [Caption("Category"), Description("The name of this blog category")]
    /// [UIHint("MultiString10"), ReadOnly]
    /// public MultiString Category { get; set; }
    /// </example>
    public class MultiString10EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString10EditComponent() : base("MultiString10", "t_text10") { }
    }
    /// <summary>
    /// Displays the model using the user's defined language (see User > Settings, standard YetaWF site). Strings for other languages contained with the model are not displayed.
    /// Should be used for text up to 20 characters in width.
    /// </summary>
    /// <example>
    /// [Caption("Title"), Description("The title for this blog entry")]
    /// [UIHint("MultiString20"), StringLength(BlogEntry.MaxTitle), Required, Trim]
    /// public MultiString Title { get; set; }
    /// </example>
    public class MultiString20DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString20DisplayComponent() : base("MultiString20", "t_text20") { }
    }
    /// <summary>
    /// Allows entry of strings in all site defined languages (see National Language Support). Renders a text box with an adjacent dropdown list showing each available language so the user can enter text for each available language.
    /// Should be used for text up to 20 characters in width.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the Text box is limited to the specified number of characters.
    /// </remarks>
    /// <example>
    /// [Caption("Category"), Description("The name of this blog category")]
    /// [UIHint("MultiString20"), ReadOnly]
    /// public MultiString Category { get; set; }
    /// </example>
    public class MultiString20EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString20EditComponent() : base("MultiString20", "t_text20") { }
    }
    /// <summary>
    /// Displays the model using the user's defined language (see User > Settings, standard YetaWF site). Strings for other languages contained with the model are not displayed.
    /// Should be used for text up to 40 characters in width.
    /// </summary>
    /// <example>
    /// [Caption("Title"), Description("The title for this blog entry")]
    /// [UIHint("MultiString40"), StringLength(BlogEntry.MaxTitle), Required, Trim]
    /// public MultiString Title { get; set; }
    /// </example>
    public class MultiString40DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString40DisplayComponent() : base("MultiString40", "t_text40") { }
    }
    /// <summary>
    /// Allows entry of strings in all site defined languages (see National Language Support). Renders a text box with an adjacent dropdown list showing each available language so the user can enter text for each available language.
    /// Should be used for text up to 40 characters in width.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the Text box is limited to the specified number of characters.
    /// </remarks>
    /// <example>
    /// [Caption("Category"), Description("The name of this blog category")]
    /// [UIHint("MultiString40"), ReadOnly]
    /// public MultiString Category { get; set; }
    /// </example>
    public class MultiString40EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString40EditComponent() : base("MultiString40", "t_text40") { }
    }
    /// <summary>
    /// Displays the model using the user's defined language (see User > Settings, standard YetaWF site). Strings for other languages contained with the model are not displayed.
    /// Should be used for text up to 80 characters in width.
    /// </summary>
    /// <example>
    /// [Caption("Title"), Description("The title for this blog entry")]
    /// [UIHint("MultiString80"), StringLength(BlogEntry.MaxTitle), Required, Trim]
    /// public MultiString Title { get; set; }
    /// </example>
    public class MultiString80DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString80DisplayComponent() : base("MultiString80", "t_text80") { }
    }
    /// <summary>
    /// Allows entry of strings in all site defined languages (see National Language Support). Renders a text box with an adjacent dropdown list showing each available language so the user can enter text for each available language.
    /// Should be used for text up to 80 characters in width.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the Text box is limited to the specified number of characters.
    /// </remarks>
    /// <example>
    /// [Caption("Category"), Description("The name of this blog category")]
    /// [UIHint("MultiString80"), ReadOnly]
    /// public MultiString Category { get; set; }
    /// </example>
    public class MultiString80EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString80EditComponent() : base("MultiString80", "t_text80") { }
    }
    /// <summary>
    /// Displays the model using the user's defined language (see User > Settings, standard YetaWF site). Strings for other languages contained with the model are not displayed.
    /// </summary>
    /// <example>
    /// [Caption("Title"), Description("The title for this blog entry")]
    /// [UIHint("MultiString"), StringLength(BlogEntry.MaxTitle), Required, Trim]
    /// public MultiString Title { get; set; }
    /// </example>
    public class MultiStringDisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiStringDisplayComponent() : base("MultiString", "t_text") { }
    }
    /// <summary>
    /// Allows entry of strings in all site defined languages (see National Language Support). Renders a text box with an adjacent dropdown list showing each available language so the user can enter text for each available language.
    /// </summary>
    /// <remarks>
    /// If the StringLengthAttribute is specified for the model, the Text box is limited to the specified number of characters.
    /// </remarks>
    /// <example>
    /// [Caption("Category"), Description("The name of this blog category")]
    /// [UIHint("MultiString"), ReadOnly]
    /// public MultiString Category { get; set; }
    /// </example>
    public class MultiStringEditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiStringEditComponent() : base("MultiString", "t_text") { }
    }

    /// <summary>
    /// Base class for the MultiString component implementation.
    /// </summary>
    public abstract class MultiStringComponentBase : YetaWFComponent {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(MultiStringComponentBase), name, defaultValue, parms); }

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

        internal string TemplateName { get; set; } = null!;
        internal string? ExtraClass { get; set; }
    }

    /// <summary>
    /// Base class for the MultiString display component implementation.
    /// </summary>
    public abstract class MultiStringDisplayComponentBase : MultiStringComponentBase, IYetaWFComponent<MultiString> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="extraClass">The CSS class representing the component.</param>
        public MultiStringDisplayComponentBase(string templateName, string extraClass) {
            TemplateName = templateName;
            ExtraClass = extraClass;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(MultiString model) {
            if (model != null)
                return Task.FromResult(HE(model.ToString()));
            return Task.FromResult(string.Empty);
        }
    }

    /// <summary>
    /// Base class for the MultiString edit component implementation.
    /// </summary>
    public abstract class MultiStringEditComponentBase : MultiStringComponentBase, IYetaWFComponent<MultiString> {

        internal class MultiStringUI {
            [UIHint("Text")]
            public string? Input { get; set; }
            [UIHint("DropDownList")]
            public string? Language { get; set; }
            public List<SelectionItem<string>>? Language_List { get; set; } = null!;
        }

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        //public class Setup {
        //    public string Name { get; set; }
        //}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <param name="extraClass">The CSS class representing the component.</param>
        public MultiStringEditComponentBase(string templateName, string extraClass) {
            TemplateName = templateName;
            ExtraClass = extraClass;
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(MultiString model) {

            HtmlBuilder hb = new HtmlBuilder();

            // handle StringLengthAttribute as maxlength
            StringLengthAttribute? lenAttr = PropData.TryGetAttribute<StringLengthAttribute>();
            if (lenAttr == null)
                throw new InternalError("No max string length given using StringLengthAttribute - {0}", FieldName);

            hb.Append($@"
<div class='yt_multistring t_edit y_inline' id='{DivId}'>");

            // use hidden input fields for each language available
            int counter = 0;
            foreach (var lang in MultiString.Languages) {

                hb.Append($@"
    <input type='hidden' name='{FieldName}[{counter}].key' value='{lang.Id}'>
    <input type='hidden' name='{FieldName}[{counter}].value' value='{model[lang.Id]}'>");

                ++counter;
            }

            // determine which language to select by default (Active or Default)
            // the active language can only be selected if the default language text is available
            string selectLang = MultiString.ActiveLanguage;
            if (string.IsNullOrWhiteSpace(model[MultiString.DefaultLanguage]))
                selectLang = MultiString.DefaultLanguage;

            // generate a hidden input field for validation (the value in this field is always the default language which is used for validation)
            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, model.DefaultText, "Hidden",
                HtmlAttributes: new { __NoTemplate = true, @class = $"t_multistring_hidden {Forms.CssFormNoSubmit}" }));


            List<SelectionItem<string>>? selectLangList = null;
            if (MultiString.Languages.Count > 1) {
                // generate a dropdownlist for the available languages
                selectLangList = new List<SelectionItem<string>>();
                foreach (var lang in MultiString.Languages) {
                    selectLangList.Add(new SelectionItem<string> { Text = lang.ShortName, Value = lang.Id, Tooltip = lang.Description });
                }
            }
            MultiStringUI msUI = new MultiStringUI {
                Input = model[selectLang],
                Language = selectLang,
                Language_List = selectLangList
            };

            using (Manager.StartNestedComponent(FieldName)) {

                Dictionary<string, object> htmlAttr;

                // generate a textbox for the currently selected language
                htmlAttr = new Dictionary<string, object>();
                htmlAttr.Add("class", $"t_multistring_text {Forms.CssFormNoSubmit} " + ExtraClass);
                if (lenAttr.MaximumLength > 0 && lenAttr.MaximumLength <= 8000)
                    htmlAttr.Add("maxlength", lenAttr.MaximumLength);
                hb.Append(await HtmlHelper.ForEditAsync(msUI, nameof(MultiStringUI.Input), HtmlAttributes: htmlAttr, Validation: false));

                if (msUI.Language_List != null) {
                    // Language dropdown
                    htmlAttr = new Dictionary<string, object>();
                    htmlAttr.Add("class", Forms.CssFormNoSubmit);
                    if (!Manager.CurrentSite.Localization || string.IsNullOrEmpty(model.DefaultText))
                        htmlAttr.Add("disabled", "disabled");
                    hb.Append(await HtmlHelper.ForEditAsync(msUI, nameof(MultiStringUI.Language), HtmlAttributes: htmlAttr, Validation: false));
                }
            }

            //Setup setup = new Setup {
            //    Name = FieldName,
            //};

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MultiStringEditComponent('{DivId}');");

            return hb.ToString();
        }
    }
}
