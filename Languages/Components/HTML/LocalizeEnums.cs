using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Languages.Components {

    public abstract class LocalizeEnumsComponentBase : YetaWFComponent {

        public const string TemplateName = "LocalizeEnums";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class LocalizeEnumsEditComponent : LocalizeEnumsComponentBase, IYetaWFComponent<SerializableList<LocalizationData.EnumData>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(SerializableList<LocalizationData.EnumData> model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_languages_localizeenums t_edit'>");

            int enumIndex = 0;

            foreach (LocalizationData.EnumData strEnum in model) {

                hb.Append($@"
    <div class='t_enum'>
        <div class='t_enumtype'>
            {YetaWFManager.HtmlEncode(strEnum.Name)}
        </div>");

                using (Manager.StartNestedComponent($"{FieldName}[{enumIndex}]")) {

                    hb.Append($@"
        <div class='t_enumentries'>
            {await HtmlHelper.ForEditAsync(strEnum, nameof(strEnum.Name))}");

                    int entryIndex = 0;

                    foreach (LocalizationData.EnumDataEntry entry in strEnum.Entries) {

                        using (Manager.StartNestedComponent($"{Manager.NestedComponentPrefix}.Entries[{entryIndex}]")) {

                            hb.Append($@"
            <div class='t_enumentry'>
                {await HtmlHelper.ForEditAsync(entry, nameof(entry.Name))}
                {await HtmlHelper.ForEditAsync(entry, nameof(entry.Value))}
                <div class='t_name'>
                    {await HtmlHelper.ForLabelAsync(entry, nameof(entry.Name), HtmlAttributes: new { Caption = entry.Name, Description = this.__ResStr("enumTT", "Text found in EnumDescriptionAttribute(...) for {0}", entry.Name) })}
                </div>
                <div class='t_enumvars'>
                    <div class='t_caption'>
                        {await HtmlHelper.ForEditAsync(entry, nameof(entry.Caption))}
                    </div>
                    <div class='t_desc'>
                        {await HtmlHelper.ForEditAsync(entry, nameof(entry.Description))}
                    </div>
                </div>
            </div>
            <div class='y_cleardiv'></div>");

                            ++entryIndex;

                        }
                    }
                    hb.Append($@"
        </div>");
                }

                hb.Append($@"
    </div>");
                ++enumIndex;

            }

            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
}
