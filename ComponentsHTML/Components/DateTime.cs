/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class DateTimeComponentBase : YetaWFComponent {

        public const string TemplateName = "DateTime";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class DateTimeDisplayComponent : DateTimeComponentBase, IYetaWFComponent<DateTime?> {

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
    public class DateTimeEditComponent : DateTimeComponentBase, IYetaWFComponent<DateTime>, IYetaWFComponent<DateTime?> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class DateTimeSetup {
            public DateTime Min { get; set; }
            public DateTime Max { get; set; }
        }

        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.datepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.timepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.datetimepicker.min.js");
            await base.IncludeAsync();
        }
        public async Task<YHtmlString> RenderAsync(DateTime model) {
            return await RenderAsync((DateTime?) model);
        }
        public async Task<YHtmlString> RenderAsync(DateTime? model) {

            UseSuppliedIdAsControlId();

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_datetime t_edit'>");

            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: HtmlAttributes, Validation: Validation));

            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("name", "dtpicker");

            // handle min/max date
            DateTimeSetup setup = new DateTimeSetup {
                Min = new DateTime(1900, 1, 1),
                Max = new DateTime(2199, 12, 31),
            };
            MinimumDateAttribute minAttr = PropData.TryGetAttribute<MinimumDateAttribute>();
            if (minAttr != null)
                setup.Min = minAttr.MinDate;
            MaximumDateAttribute maxAttr = PropData.TryGetAttribute<MaximumDateAttribute>();
            if (maxAttr != null)
                setup.Max = maxAttr.MaxDate;

            if (model != null)
                tag.MergeAttribute("value", Formatting.FormatDateTime((DateTime)model));// shows date using user's timezone
            hb.Append(tag.ToString(YTagRenderMode.SelfClosing));

            hb.Append($"</div>");

            hb.Append($@"
<script>
new YetaWF_ComponentsHTML.DateTimeEditComponent('{ControlId}', {YetaWFManager.JsonSerialize(setup)});
</script>");

            return hb.ToYHtmlString();
        }
    }
}
