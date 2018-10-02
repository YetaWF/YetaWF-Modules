/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TimeComponentBase : YetaWFComponent {

        public const string TemplateName = "Time";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class TimeDisplayComponent : TimeComponentBase, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?)model);
        }
        public Task<YHtmlString> RenderAsync(DateTime? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && (DateTime)model > DateTime.MinValue && (DateTime)model < DateTime.MaxValue) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_time");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                tag.SetInnerText(YetaWF.Core.Localize.Formatting.FormatTime(model));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class TimeEditComponent : TimeComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.timepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.datetimepicker.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        public async Task<YHtmlString> RenderAsync(DateTime? model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_time t_edit'>");

            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: HtmlAttributes, Validation: Validation));

            YTagBuilder tag = new YTagBuilder("input");
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add("name", "dtpicker");

                if (model != null)
                    tag.MergeAttribute("value", Formatting.FormatTime((DateTime)model));// shows time
                hb.Append(tag.ToString(YTagRenderMode.SelfClosing));

            hb.Append($"</div>");

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"(new YetaWF_ComponentsHTML.TimeComponent()).init('{ControlId}');");

            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return hb.ToYHtmlString();
        }
    }
}
