/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Languages.Components {

    public abstract class LocalizeStringsComponentBase : YetaWFComponent {

        public const string TemplateName = "LocalizeStrings";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class LocalizeStringsEditComponent : LocalizeStringsComponentBase, IYetaWFComponent<SerializableList<LocalizationData.StringData>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(SerializableList<LocalizationData.StringData> model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_languages_localizestrings t_edit'>");

            int index = 0;

            foreach (LocalizationData.StringData strData in model) {

                hb.Append($@"
    <div class='t_string'>");
                using (Manager.StartNestedComponent($"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForEditAsync(strData, nameof(strData.Name)));
                    hb.Append($@"<div class='t_name'>");
                    hb.Append(await HtmlHelper.ForLabelAsync(strData, nameof(strData.Name), HtmlAttributes: new { Caption = strData.Name, Description = this.__ResStr("captTT", "Text found in __ResStr(\"{0}\", ...)", strData.Name) }));
                    hb.Append($@"</div>");
                    hb.Append($@"<div class='t_text'>");
                    hb.Append(await HtmlHelper.ForEditAsync(strData, nameof(strData.Text)));
                    hb.Append($@"</div>");
                }
                hb.Append($@"
    </div>");

                ++index;
            }
            hb.Append($@"
</div>");

            return hb.ToYHtmlString();
        }
    }
}
