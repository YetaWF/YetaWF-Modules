using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Languages.Components {

    public abstract class LocalizeClassesComponentBase : YetaWFComponent {

        public const string TemplateName = "LocalizeClasses";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class LocalizeClassesEditComponent : LocalizeClassesComponentBase, IYetaWFComponent<SerializableList<LocalizationData.ClassData>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(SerializableList<LocalizationData.ClassData> model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_languages_localizeclasses t_edit'>");

            string linkClass = UniqueId();
            int countClasses = model.Count;

            if (countClasses > 1) {
                hb.Append($@"
    <div class='t_links'>");
                for (int i = 0; i < countClasses; ++i) {
                    hb.Append($@"
        <div class='t_link_{i}'>
            <a href='#{linkClass}_{i}'>{YetaWFManager.HtmlEncode(model[i].Name)})</a>
        </div>");
                }
                hb.Append($@"
    </div>");
            }

            int classIndex = 0;

            foreach (LocalizationData.ClassData classData in model) {

                hb.Append($@"
    <div class='t_class'>");

                using (Manager.StartNestedComponent($"{FieldName}[{classIndex}]")) {
                    if (countClasses > 1) {
                        hb.Append($@"
        <div class='t_classname'>
            <a class='t_target' id='{linkClass}_{classIndex}'>{YetaWFManager.HtmlEncode(classData.Name)}</a>
        </div>");
                    }

                    hb.Append($@"
        <div class='t_classinfo'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(classData, nameof(classData.Header))}
            </div>
            <div class='t_header'>
                {await HtmlHelper.ForEditAsync(classData, nameof(classData.Header))}
            </div>
            <div class='y_cleardiv'></div>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(classData, nameof(classData.Footer))}
            </div>
            <div class='t_footer'>
                {await HtmlHelper.ForEditAsync(classData, nameof(classData.Footer))}
            </div>
            <div class='y_cleardiv'></div>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(classData, nameof(classData.Legend))}
            </div>
            <div class='t_legend'>
                {await HtmlHelper.ForEditAsync(classData, nameof(classData.Legend))}
            </div>
            <div class='y_cleardiv'></div>
        </div>");

                    int entryIndex = 0;

                    hb.Append($@"
        <div class='t_propentries'>");

                    foreach (LocalizationData.PropertyData prop in classData.Properties) {
                        using (Manager.StartNestedComponent($"{Manager.NestedComponentPrefix}.Properties[{entryIndex}]")) {

                            hb.Append($@"
            <div class='t_propentry'>
                <div class='t_name'>
                    {await HtmlHelper.ForEditAsync(prop, nameof(prop.Name))}
                </div>
                <div class='t_name'>
                    {await HtmlHelper.ForLabelAsync(prop, nameof(prop.Name), HtmlAttributes: new { Caption = prop.Name, Description = this.__ResStr("propTT", "Text found in attributes for property {0}", prop.Name) })}
                </div>
                <div class='t_propvars'>
                    <div class='t_caption'>
                        {await HtmlHelper.ForEditAsync(prop, nameof(prop.Caption))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtCaption", " (Caption)"))}
                    </div>
                    <div class='t_desc'>
                        {await HtmlHelper.ForEditAsync(prop, nameof(prop.Description))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtDescription", " (Description)"))}
                    </div>
                    <div class='t_helplink'>
                        {await HtmlHelper.ForEditAsync(prop, nameof(prop.HelpLink))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtHelpLink", " (HelpLink)"))}
                    </div>
                    <div class='t_textabove'>
                        {await HtmlHelper.ForEditAsync(prop, nameof(prop.TextAbove))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtTextAbove", " (TextAbove)"))}
                    </div>
                    <div class='t_textbelow'>
                        {await HtmlHelper.ForEditAsync(prop, nameof(prop.TextBelow))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtTextBelow", " (TextBelow)"))}
                    </div>
                </div>
            </div>
            <div class='y_cleardiv'></div>");

                        }

                        ++entryIndex;
                    }

                    hb.Append($@"
        </div>
    </div>");


                }

                ++classIndex;
            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
}
