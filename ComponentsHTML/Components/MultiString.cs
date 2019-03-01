/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// Implementation of the MultiString10 display component.
    /// </summary>
    public class MultiString10DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString10DisplayComponent() : base("MultiString10", "t_text10") { }
    }
    /// <summary>
    /// Implementation of the MultiString10 edit component.
    /// </summary>
    public class MultiString10EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString10EditComponent() : base("MultiString10", "t_text10") { }
    }
    /// <summary>
    /// Implementation of the MultiString20 display component.
    /// </summary>
    public class MultiString20DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString20DisplayComponent() : base("MultiString20", "t_text20") { }
    }
    /// <summary>
    /// Implementation of the MultiString20 edit component.
    /// </summary>
    public class MultiString20EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString20EditComponent() : base("MultiString20", "t_text20") { }
    }
    /// <summary>
    /// Implementation of the MultiString40 display component.
    /// </summary>
    public class MultiString40DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString40DisplayComponent() : base("MultiString40", "t_text40") { }
    }
    /// <summary>
    /// Implementation of the MultiString40 edit component.
    /// </summary>
    public class MultiString40EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString40EditComponent() : base("MultiString40", "t_text40") { }
    }
    /// <summary>
    /// Implementation of the MultiString80 display component.
    /// </summary>
    public class MultiString80DisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString80DisplayComponent() : base("MultiString80", "t_text80") { }
    }
    /// <summary>
    /// Implementation of the MultiString80 edit component.
    /// </summary>
    public class MultiString80EditComponent : MultiStringEditComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiString80EditComponent() : base("MultiString80", "t_text80") { }
    }
    /// <summary>
    /// Implementation of the MultiString display component.
    /// </summary>
    public class MultiStringDisplayComponent : MultiStringDisplayComponentBase {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiStringDisplayComponent() : base("MultiString", "t_text") { }
    }
    /// <summary>
    /// Implementation of the MultiString edit component.
    /// </summary>
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

        internal string TemplateName { get; set; }
        internal string ExtraClass { get; set; }
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
        public Task<YHtmlString> RenderAsync(MultiString model) {
        HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(HE(model.ToString()));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }

    /// <summary>
    /// Base class for the MultiString edit component implementation.
    /// </summary>
    public abstract class MultiStringEditComponentBase : MultiStringComponentBase, IYetaWFComponent<MultiString> {

        internal class MultiStringUI {
            [UIHint("DropDownList")]
            public string Language { get; set; }
            public List<SelectionItem<string>> Language_List { get; set; }
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
        public async Task<YHtmlString> RenderAsync(MultiString model) {
            return await RenderMultiStringAsync(this, model, ExtraClass);
        }
        private static async Task<YHtmlString> RenderMultiStringAsync(YetaWFComponent component, MultiString model, string extraCssClass) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_multistring t_edit y_inline' id='{component.DivId}'>");

            // use hidden input fields for each language available
            int counter = 0;
            foreach (var lang in MultiString.Languages) {

                hb.Append($@"
    <input type='hidden' name='{component.FieldName}[{counter}].key' value='{lang.Id}'>
    <input type='hidden' name='{component.FieldName}[{counter}].value' value='{model[lang.Id]}'>");

                ++counter;
            }

            // determine which language to select by default (Active or Default)
            // the active language can only be selected if the default language text is available
            string selectLang = MultiString.ActiveLanguage;
            if (string.IsNullOrWhiteSpace(model[MultiString.DefaultLanguage]))
                selectLang = MultiString.DefaultLanguage;

            // generate a textbox for the currently selected language
            component.HtmlAttributes.Add("class", $"yt_multistring_text yt_text_base {Forms.CssFormNoSubmit} " + extraCssClass);
            hb.Append(await TextEditComponent.RenderTextAsync(component, model[selectLang], null));

            // generate a dropdownlist for the available languages
            List<SelectionItem<string>> selectLangList = new List<SelectionItem<string>>();
            foreach (var lang in MultiString.Languages) {
                selectLangList.Add(new SelectionItem<string> { Text = lang.ShortName, Value = lang.Id, Tooltip = lang.Description });
            }
            string idDD = Manager.UniqueId("lng");

            using (Manager.StartNestedComponent(component.FieldName)) {

                MultiStringUI msUI = new MultiStringUI {
                    Language = selectLang,
                    Language_List = selectLangList
                };

                Dictionary<string, object> htmlAttr = new Dictionary<string, object>();
                htmlAttr.Add("id", idDD);
                htmlAttr.Add("class", Forms.CssFormNoSubmit);
                if (!Manager.CurrentSite.Localization || string.IsNullOrEmpty(model.DefaultText))
                    htmlAttr.Add("disabled", "disabled");
                hb.Append(await component.HtmlHelper.ForEditAsync(msUI, nameof(MultiStringUI.Language), HtmlAttributes: htmlAttr, Validation: false));
            }

            //Setup setup = new Setup {
            //    Name = component.FieldName,
            //};

            hb.Append($@"
</div>
<script>
    new YetaWF_ComponentsHTML.MultiStringEditComponent('{component.DivId}');
</script>");

            return hb.ToYHtmlString();
        }
    }
}
