/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class DateComponent : YetaWFComponent {

        public const string TemplateName = "Date";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class DateDisplayComponent : DateComponent, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?)model);
        }
        public Task<YHtmlString> RenderAsync(DateTime? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && (DateTime)model > DateTime.MinValue && (DateTime)model < DateTime.MaxValue) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_datetime");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                tag.SetInnerText(YetaWF.Core.Localize.Formatting.FormatDate(model));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class DateEditComponent : DateComponent, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.datepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.timepicker.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        public async Task<YHtmlString> RenderAsync(DateTime? model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_date t_edit'>");

            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, "", "Hidden", HtmlAttributes: HtmlAttributes, Validation: Validation));

            YTagBuilder tag = new YTagBuilder("input");
                FieldSetup(tag, FieldType.Anonymous);
                tag.Attributes.Add("name", "dtpicker");

                // handle min/max date
                MinimumDateAttribute minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
                MaximumDateAttribute maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
                if (minAttr != null) {
                    tag.MergeAttribute("data-min-y", minAttr.MinDate.Year.ToString());
                    tag.MergeAttribute("data-min-m", minAttr.MinDate.Month.ToString());
                    tag.MergeAttribute("data-min-d", minAttr.MinDate.Day.ToString());
                }
                if (maxAttr != null) {
                    tag.MergeAttribute("data-max-y", maxAttr.MaxDate.Year.ToString());
                    tag.MergeAttribute("data-max-m", maxAttr.MaxDate.Month.ToString());
                    tag.MergeAttribute("data-max-d", maxAttr.MaxDate.Day.ToString());
                }

                if (model != null)
                    tag.MergeAttribute("value", Formatting.FormatDateTime((DateTime)model));// shows date using user's timezone
                hb.Append(tag.ToString(YTagRenderMode.SelfClosing));

            hb.Append($"</div>");

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"new YetaWF_ComponentsHTML.DateEditComponent('{ControlId}');");

            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return hb.ToYHtmlString();
        }
        public async Task<string> RenderJavascriptAsync(string gridId, string elemVarName) {
            await IncludeAsync();
            return string.Format($"new YetaWF_ComponentsHTML.DateGridComponent('{gridId}', {elemVarName});");
        }
    }
}
