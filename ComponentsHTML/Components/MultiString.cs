using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class MultiString10DisplayComponent : MultiStringDisplayComponentBase { public MultiString10DisplayComponent() : base("MultiString10", "t_text10") { } }
    public class MultiString10EditComponent : MultiStringEditComponentBase { public MultiString10EditComponent() : base("MultiString10", "t_text10") { } }
    public class MultiString20DisplayComponent : MultiStringDisplayComponentBase { public MultiString20DisplayComponent() : base("MultiString20", "t_text20") { } }
    public class MultiString20EditComponent : MultiStringEditComponentBase { public MultiString20EditComponent() : base("MultiString20", "t_text20") { } }
    public class MultiString40DisplayComponent : MultiStringDisplayComponentBase { public MultiString40DisplayComponent() : base("MultiString40", "t_text40") { } }
    public class MultiString40EditComponent : MultiStringEditComponentBase { public MultiString40EditComponent() : base("MultiString40", "t_text40") { } }
    public class MultiString80DisplayComponent : MultiStringDisplayComponentBase { public MultiString80DisplayComponent() : base("MultiString80", "t_text80") { } }
    public class MultiString80EditComponent : MultiStringEditComponentBase { public MultiString80EditComponent() : base("MultiString80", "t_text80") { } }
    public class MultiStringDisplayComponent : MultiStringDisplayComponentBase { public MultiStringDisplayComponent() : base("MultiString", "t_text") { } }
    public class MultiStringEditComponent : MultiStringEditComponentBase { public MultiStringEditComponent() : base("MultiString", "t_text") { } }

    public abstract class MultiStringDisplayComponentBase : YetaWFComponent, IYetaWFComponent<MultiString> {

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public string TemplateName { get; set; }
        public string ExtraClass { get; set; }

        public MultiStringDisplayComponentBase(string templateName, string extraClass) {
            TemplateName = templateName;
            ExtraClass = extraClass;
        }

        public Task<YHtmlString> RenderAsync(MultiString model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(model.ToString());
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public abstract class MultiStringEditComponentBase : YetaWFComponent, IYetaWFComponent<MultiString> {

        public class MultiStringUI {
            [UIHint("DropDownList")]
            public string Language { get; set; }
            public List<SelectionItem<string>> Language_List { get; set; }
        }

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(MultiStringEditComponentBase), name, defaultValue, parms); }

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public string TemplateName { get; set; }
        public string ExtraClass { get; set; }

        public MultiStringEditComponentBase(string templateName, string extraClass) {
            TemplateName = templateName;
            ExtraClass = extraClass;
        }

        public async Task<YHtmlString> RenderAsync(MultiString model) {
            return await RenderMultiStringAsync(model, this, ExtraClass);
        }
        private static async Task<YHtmlString> RenderMultiStringAsync(MultiString model, YetaWFComponent component, string extraCssClass) {

            HtmlBuilder hb = new HtmlBuilder();

            // <div class="yt_multistring t_edit" data-name="@Html.FieldName("")" id="@DivId" class="@Html.GetErrorClass("")" ...validation...>
            YTagBuilder tagDiv = new YTagBuilder("div");
            tagDiv.AddCssClass("yt_multistring");
            tagDiv.AddCssClass("t_edit");
            tagDiv.AddCssClass("y_inline");
            tagDiv.Attributes["data-name"] = component.FieldName;
            tagDiv.Attributes["id"] = component.DivId;

            // use hidden input fields for each language available
            int counter = 0;
            foreach (var lang in MultiString.Languages) {
                YTagBuilder tag = new YTagBuilder("input");
                tag.MergeAttribute("type", "hidden");
                string n = string.Format("{0}[{1}].key", component.FieldName, counter);
                tag.MergeAttribute("name", n, true);
                tag.MergeAttribute("value", lang.Id);
                hb.Append(tag.ToString(YTagRenderMode.StartTag));

                tag = new YTagBuilder("input");
                tag.MergeAttribute("type", "hidden");
                n = string.Format("{0}[{1}].value", component.FieldName, counter);
                tag.MergeAttribute("name", n, true);
                tag.MergeAttribute("value", model[lang.Id]);
                hb.Append(tag.ToString(YTagRenderMode.StartTag));

                ++counter;
            }

            // determine which language to select by default (Active or Default)
            // the active language can only be selected if the default language text is available
            string selectLang = MultiString.ActiveLanguage;
            if (string.IsNullOrWhiteSpace(model[MultiString.DefaultLanguage]))
                selectLang = MultiString.DefaultLanguage;

            // generate a textbox for the currently selected language
            component.HtmlAttributes.Add("class", "yt_multistring_text yt_text_base " + extraCssClass);
            hb.Append(await TextEditComponent.RenderTextAsync(model[selectLang], component, null));

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
                if (!Manager.CurrentSite.Localization)
                    htmlAttr.Add("disabled", "disabled");
                hb.Append(await component.HtmlHelper.ForEditAsync(msUI, nameof(MultiStringUI.Language), HtmlAttributes: htmlAttr, Validation: false));
            }
            tagDiv.InnerHtml = hb.ToString();
            return tagDiv.ToYHtmlString(YTagRenderMode.Normal);
        }
    }
}
