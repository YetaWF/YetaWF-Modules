﻿using System;
using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Components;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class DateTimeComponent : YetaWFComponent {

        public const string TemplateName = "DateTime";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
        // or $"{Package.Domain}_{Package.Product}_{TemplateName}";//$$
    }

    public class DateTimeDisplayComponent : DateTimeComponent, IYetaWFComponent<DateTime?> {

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
                tag.SetInnerText(YetaWF.Core.Localize.Formatting.FormatDateTime(model));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }
    public class DateTimeEditComponent : DateTimeComponent, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public override async Task IncludeAsync() {
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.calendar.min.js");
            //await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.datepicker.min.js");
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.timepicker.min.js");
            await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.datetimepicker.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        public async Task<YHtmlString> RenderAsync(DateTime? model) {
            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_datetime t_edit'>");

            hb.Append(await HtmlHelper.ForEditAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: HtmlAttributes, Validation: Validation));

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
            sb.Append($@"(new YetaWF_ComponentsHTML.DateTimeComponent()).init('{ControlId}');");

            hb.Append(Manager.ScriptManager.AddNow(sb.ToString()).ToString());

            return hb.ToYHtmlString();
        }
    }
}