/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Languages.Components;

public abstract class LocalizeEnumsComponentBase : YetaWFComponent {

    public const string TemplateName = "LocalizeEnums";

    public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    public override string GetTemplateName() { return TemplateName; }
}

/// <summary>
/// This component is used by the YetaWF.Languages package and is not intended for use by an application.
/// </summary>
[PrivateComponent]
public class LocalizeEnumsEditComponent : LocalizeEnumsComponentBase, IYetaWFComponent<SerializableList<LocalizationData.EnumData>> {

    public override ComponentType GetComponentType() { return ComponentType.Edit; }

    public class UIEnumData {
        public UIEnumData(LocalizationData.EnumData entry) {
            ObjectSupport.CopyData(entry, this);
        }
        [UIHint("Hidden"), StringLength(LocalizationData.MaxString)]
        public string Name { get; set; } = null!;
        public SerializableList<LocalizationData.EnumDataEntry> Entries { get; set; } = null!;
    }

    public class UIEnumDataEntry {

        public UIEnumDataEntry(LocalizationData.EnumDataEntry entry) {
            ObjectSupport.CopyData(entry, this);
        }

        [UIHint("Hidden"), ResourceRedirect(nameof(NameFieldCaption), nameof(NameFieldDescription)), StringLength(LocalizationData.MaxString)]
        public string Name { get; set; } = null!;
        [UIHint("Hidden"), StringLength(LocalizationData.MaxString)]
        public string? Value { get; set; }
        [UIHint("Text40"), ResourceRedirect(nameof(CaptionFieldCaption), nameof(CaptionFieldDescription)), StringLength(LocalizationData.MaxString)]
        public string? Caption { get; set; }
        [UIHint("Text80"), ResourceRedirect(nameof(DescriptionFieldCaption), nameof(DescriptionFieldDescription)), StringLength(LocalizationData.MaxString)]
        public string? Description { get; set; }

        public string? NameFieldCaption { get; set; }
        public string? NameFieldDescription { get; set; }
        public string? CaptionFieldCaption { get; set; }
        public string? CaptionFieldDescription { get; set; }
        public string? DescriptionFieldCaption { get; set; }
        public string? DescriptionFieldDescription { get; set; }
    }


    public async Task<string> RenderAsync(SerializableList<LocalizationData.EnumData> model) {
        HtmlBuilder hb = new HtmlBuilder();

        hb.Append($@"
<div class='yt_yetawf_languages_localizeenums t_edit'>");

        int enumIndex = 0;

        foreach (LocalizationData.EnumData strEnum in model) {

            UIEnumData uiEnumData = new UIEnumData(strEnum);

            hb.Append($@"
    <div class='t_enum'>
        <div class='t_enumtype'>
            {Utility.HE(uiEnumData.Name)}
        </div>");

            using (Manager.StartNestedComponent($"{FieldName}[{enumIndex}]")) {

                hb.Append($@"
        <div class='t_enumentries'>
            {await HtmlHelper.ForDisplayAsync(uiEnumData, nameof(uiEnumData.Name))}");

                int entryIndex = 0;

                foreach (LocalizationData.EnumDataEntry entry in uiEnumData.Entries) {

                    UIEnumDataEntry uiEntry = new UIEnumDataEntry(entry) {
                        NameFieldCaption = entry.Name,
                        NameFieldDescription = this.__ResStr("enumNameTT", "Caption and description found in EnumDescriptionAttribute(...) for enum entry {0}", entry.Name),
                        CaptionFieldCaption = entry.Name,
                        CaptionFieldDescription = this.__ResStr("enumCaptTT", "Caption found in EnumDescriptionAttribute(...) for {0}", entry.Name),
                        DescriptionFieldCaption = entry.Name,
                        DescriptionFieldDescription = this.__ResStr("enumDescTT", "Description found in EnumDescriptionAttribute(...) for {0}", entry.Name),
                    };

                    using (Manager.StartNestedComponent($"{Manager.NestedComponentPrefix}.Entries[{entryIndex}]")) {

                        hb.Append($@"
            <div class='t_enumentry'>
                {await HtmlHelper.ForDisplayAsync(uiEntry, nameof(uiEntry.Name))}
                {await HtmlHelper.ForDisplayAsync(uiEntry, nameof(uiEntry.Value))}
                <div class='t_name'>
                    {await HtmlHelper.ForLabelAsync(uiEntry, nameof(uiEntry.Name))}
                </div>
                <div class='t_enumvars'>
                    <div class='t_caption'>
                        {await HtmlHelper.ForEditAsync(uiEntry, nameof(uiEntry.Caption))}
                    </div>
                    <div class='t_desc'>
                        {await HtmlHelper.ForEditAsync(uiEntry, nameof(uiEntry.Description))}
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

        return hb.ToString();
    }
}