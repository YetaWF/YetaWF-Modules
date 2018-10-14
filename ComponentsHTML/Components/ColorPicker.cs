/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class ColorPickerComponent : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ColorPickerComponent), name, defaultValue, parms); }

        public const string TemplateName = "ColorPicker";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class ColorPickerDisplayComponent : ColorPickerComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            int tileSize = PropData.GetAdditionalAttributeValue("TileSize", 24);
            string palette = PropData.GetAdditionalAttributeValue("Palette", "basic");
            int columns = PropData.GetAdditionalAttributeValue("Columns", 6);
            bool preview = PropData.GetAdditionalAttributeValue("Preview", true);

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_colorpicker");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            string style = $"width:{tileSize}px;height:{tileSize}px";
            if (model != null)
                style += $";background-color:{model}";
            tag.MergeAttribute("style", style);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class ColorPickerEditComponent : ColorPickerComponent, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.color.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.slider.min.js");
            await KendoUICore.AddFileAsync("kendo.button.min.js");
            await KendoUICore.AddFileAsync("kendo.colorpicker.min.js");
            await base.IncludeAsync();
        }
        public Task<YHtmlString> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            int tileSize = PropData.GetAdditionalAttributeValue("TileSize", 16);
            string palette = PropData.GetAdditionalAttributeValue("Palette", "basic");
            int columns = PropData.GetAdditionalAttributeValue("Columns", 6);
            bool preview = PropData.GetAdditionalAttributeValue("Preview", true);

            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.MergeAttribute("id", ControlId);
            if (model != null)
                tag.MergeAttribute("value", model);
            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"(new YetaWF_ComponentsHTML.ColorPickerComponent('{ControlId}', {{
                palette: '{JE(palette)}',
                tileSize: {tileSize},
                preview: {JE(preview)},
                messages: {{
                    previewInput: '{JE(__ResStr("editColor", "Edit Color using hex values or names"))}',
                    cancel: '{JE(__ResStr("cancel", "Cancel"))}',
                    apply: '{JE(__ResStr("apply", "Apply"))}'
                }}
            }}));");
            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return Task.FromResult(hb.ToYHtmlString());
        }
    }
}
