using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class EnumComponent : YetaWFComponent {

        public const string TemplateName = "Enum";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class EnumDisplayComponent : EnumComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(object model) {

            bool showValues = UserSettings.GetProperty<bool>("ShowEnumValue");
            showValues = showValues && PropData.GetAdditionalAttributeValue("ShowEnumValue", true);

            string desc;
            string caption = ObjectSupport.GetEnumDisplayInfo(model, out desc, ShowValue: showValues);

            if (HtmlAttributes.Count > 0 || !string.IsNullOrWhiteSpace(desc)) {
                YTagBuilder tag = new YTagBuilder("span");
                tag.AddCssClass("yt_enum");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add(Basics.CssTooltipSpan, desc);
                tag.SetInnerText(caption);
                return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
            } else {
                return Task.FromResult(new YHtmlString(caption));
            }
        }
    }
    public class EnumEditComponent : EnumComponent, IYetaWFComponent<object> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(object model) {

            List<SelectionItem<int>> list = new List<SelectionItem<int>>();

            Type enumType = model.GetType();
            EnumData enumData = ObjectSupport.GetEnumData(enumType);
            bool showValues = UserSettings.GetProperty<bool>("ShowEnumValue");

            bool showSelect = PropData.GetAdditionalAttributeValue("ShowSelect", false);
            if (showSelect) {
                list.Add(new SelectionItem<int> {
                    Text = this.__ResStr("enumSelect", "(select)"),
                    Value = 0,
                    Tooltip = this.__ResStr("enumPlsSelect", "Please select one of the available options"),
                });
            }
            foreach (EnumDataEntry entry in enumData.Entries) {

                int enumVal = Convert.ToInt32(entry.Value);
                if (enumVal == 0 && showSelect) continue;

                string caption = entry.Caption;
                if (showValues)
                    caption = this.__ResStr("enumFmt", "{0} - {1}", enumVal, caption);

                list.Add(new SelectionItem<int> {
                    Text = caption,
                    Value = enumVal,
                    Tooltip = entry.Description,
                });
            }
            return await DropDownListIntComponent.RenderDropDownListAsync(this, (int)model, list, "yt_enum");
        }
    }
}
