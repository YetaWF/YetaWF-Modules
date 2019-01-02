/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
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

        public class UIClassData {

            public UIClassData(LocalizationData.ClassData entry) {
                ObjectSupport.CopyData(entry, this);
            }

            [UIHint("Hidden"), StringLength(LocalizationData.MaxString)]
            public string Name { get; set; }
            [UIHint("Hidden"), StringLength(LocalizationData.MaxString)]
            public string BaseTypeName { get; set; }

            [Caption("Class Header"), Description("Text found in [HeaderAttribute(...)]")]
            [UIHint("Text80"), StringLength(LocalizationData.MaxString)]
            public string Header { get; set; }
            [Caption("Class Footer"), Description("Text found in [FooterAttribute(...)]")]
            [UIHint("Text80"), StringLength(LocalizationData.MaxString)]
            public string Footer { get; set; }
            [Caption("Class Legend"), Description("Text found in [LegendAttribute(...)]")]
            [UIHint("Text80"), StringLength(LocalizationData.MaxString)]
            public string Legend { get; set; }
        }

        public class UIPropertyData {

            public UIPropertyData(LocalizationData.PropertyData entry) {
                ObjectSupport.CopyData(entry, this);
            }

            [UIHint("Hidden"), ResourceRedirect(nameof(NameFieldCaption), nameof(NameFieldDescription)), StringLength(LocalizationData.MaxString)]
            public string Name { get; set; }
            [UIHint("Text40"), ResourceRedirect(nameof(NameFieldCaption)), StringLength(LocalizationData.MaxString)]
            public string Caption { get; set; }
            [UIHint("Text80"), ResourceRedirect(nameof(NameFieldCaption)), StringLength(LocalizationData.MaxString)]
            public string Description { get; set; }
            [UIHint("Text80"), ResourceRedirect(nameof(NameFieldCaption)), StringLength(Globals.MaxUrl)]
            public string HelpLink { get; set; }
            [UIHint("Text80"), ResourceRedirect(nameof(NameFieldCaption)), StringLength(LocalizationData.MaxString)]
            public string TextAbove { get; set; }
            [UIHint("Text80"), ResourceRedirect(nameof(NameFieldCaption)), StringLength(LocalizationData.MaxString)]
            public string TextBelow { get; set; }

            public string NameFieldCaption { get; set; }
            public string NameFieldDescription { get; set; }
        }

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
            <a href='#{linkClass}_{i}'>{YetaWFManager.HtmlEncode(model[i].Name)}</a>
        </div>");
                }
                hb.Append($@"
    </div>");
            }

            int classIndex = 0;

            foreach (LocalizationData.ClassData classData in model) {

                UIClassData uiClassData = new UIClassData(classData);

                hb.Append($@"
    <div class='t_class'>");

                using (Manager.StartNestedComponent($"{FieldName}[{classIndex}]")) {

                    hb.Append(await HtmlHelper.ForDisplayAsync(uiClassData, nameof(uiClassData.Name)));
                    hb.Append(await HtmlHelper.ForDisplayAsync(uiClassData, nameof(uiClassData.BaseTypeName)));

                    if (countClasses > 1) {
                        hb.Append($@"
        <div class='t_classname'>
            <a class='t_target' id='{linkClass}_{classIndex}'>{YetaWFManager.HtmlEncode(uiClassData.Name)}</a>
        </div>");
                    }

                    hb.Append($@"
        <div class='t_classinfo'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(uiClassData, nameof(uiClassData.Header))}
            </div>
            <div class='t_header'>
                {await HtmlHelper.ForEditAsync(uiClassData, nameof(uiClassData.Header))}
            </div>
            <div class='y_cleardiv'></div>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(uiClassData, nameof(uiClassData.Footer))}
            </div>
            <div class='t_footer'>
                {await HtmlHelper.ForEditAsync(uiClassData, nameof(uiClassData.Footer))}
            </div>
            <div class='y_cleardiv'></div>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(uiClassData, nameof(uiClassData.Legend))}
            </div>
            <div class='t_legend'>
                {await HtmlHelper.ForEditAsync(uiClassData, nameof(uiClassData.Legend))}
            </div>
            <div class='y_cleardiv'></div>
        </div>");

                    int entryIndex = 0;

                    hb.Append($@"
        <div class='t_propentries'>");

                    foreach (LocalizationData.PropertyData prop in classData.Properties) {
                        using (Manager.StartNestedComponent($"{Manager.NestedComponentPrefix}.Properties[{entryIndex}]")) {

                            UIPropertyData uiProp = new UIPropertyData(prop) {
                                NameFieldCaption = prop.Name,
                                NameFieldDescription = this.__ResStr("strNameTT", "Text found in attributes for property {0}", prop.Name),
                            };

                            hb.Append($@"
            <div class='t_propentry'>
                <div class='t_name'>
                    {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.Name))}
                </div>
                <div class='t_name'>
                    {await HtmlHelper.ForLabelAsync(uiProp, nameof(uiProp.Name))}
                </div>
                <div class='t_propvars'>
                    <div class='t_caption'>
                        {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.Caption))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtCaption", " (Caption)"))}
                    </div>
                    <div class='t_desc'>
                        {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.Description))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtDescription", " (Description)"))}
                    </div>
                    <div class='t_helplink'>
                        {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.HelpLink))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtHelpLink", " (HelpLink)"))}
                    </div>
                    <div class='t_textabove'>
                        {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.TextAbove))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtTextAbove", " (TextAbove)"))}
                    </div>
                    <div class='t_textbelow'>
                        {await HtmlHelper.ForEditAsync(uiProp, nameof(uiProp.TextBelow))}{YetaWFManager.HtmlEncode(this.__ResStr("cmtTextBelow", " (TextBelow)"))}
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
