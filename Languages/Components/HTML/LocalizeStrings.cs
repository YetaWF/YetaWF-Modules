/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
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

    /// <summary>
    /// This component is used by the YetaWF.Languages package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class LocalizeStringsEditComponent : LocalizeStringsComponentBase, IYetaWFComponent<SerializableList<LocalizationData.StringData>> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class UIStringData {

            public UIStringData(LocalizationData.StringData entry) {
                ObjectSupport.CopyData(entry, this);
            }

            [UIHint("Hidden"), ResourceRedirect(nameof(NameFieldCaption), nameof(NameFieldDescription)), StringLength(LocalizationData.MaxString)]
            public string Name { get; set; }
            [UIHint("Text80"), ResourceRedirect(nameof(TextFieldCaption), nameof(TextFieldDescription)), StringLength(LocalizationData.MaxString)]
            public string Text { get; set; }

            public string NameFieldCaption { get; set; }
            public string NameFieldDescription { get; set; }
            public string TextFieldCaption { get; set; }
            public string TextFieldDescription { get; set; }
        }

        public async Task<string> RenderAsync(SerializableList<LocalizationData.StringData> model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_languages_localizestrings t_edit'>");

            int index = 0;

            foreach (LocalizationData.StringData strData in model) {

                UIStringData uiData = new UIStringData(strData) {
                    NameFieldCaption = strData.Name,
                    NameFieldDescription = this.__ResStr("strNameTT", "Text found in __ResStr(\"{0}\", ...)", strData.Name),
                    TextFieldCaption = strData.Name,
                    TextFieldDescription = this.__ResStr("strTextTT", "Text found in __ResStr(\"{0}\", ...)", strData.Name),
                };

                hb.Append($@"
    <div class='t_string'>");
                using (Manager.StartNestedComponent($"{FieldName}[{index}]")) {
                    hb.Append(await HtmlHelper.ForEditAsync(uiData, nameof(uiData.Name)));
                    hb.Append($@"<div class='t_name'>");
                    hb.Append(await HtmlHelper.ForLabelAsync(uiData, nameof(uiData.Name)));
                    hb.Append($@"</div>");
                    hb.Append($@"<div class='t_text'>");
                    hb.Append(await HtmlHelper.ForEditAsync(uiData, nameof(uiData.Text)));
                    hb.Append($@"</div>");
                }
                hb.Append($@"
    </div>");

                ++index;
            }
            hb.Append($@"
</div>");

            return hb.ToString();
        }
    }
}
