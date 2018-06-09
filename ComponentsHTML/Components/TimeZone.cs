using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TimeZoneComponentBase : YetaWFComponent {

        public const string TemplateName = "TimeZone";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class TimeZoneDisplayComponent : TimeZoneComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public Task<YHtmlString> RenderAsync(string model) {

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_timezone");
            tag.AddCssClass("t_display");

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(model);
            if (tzi == null) {
                tag.SetInnerText(this.__ResStr("unknown", "(unknown)"));
            } else {
                tag.SetInnerText(tzi.DisplayName);
                tag.Attributes.Add("title", tzi.IsDaylightSavingTime(DateTime.Now/*need local time*/) ? tzi.DaylightName : tzi.StandardName);
            }
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
        }
    }
    public class TimeZoneEditComponent : TimeZoneComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(string model) {

            List<TimeZoneInfo> tzis = TimeZoneInfo.GetSystemTimeZones().ToList();
            DateTime dt = DateTime.Now;// Need local time

            bool showDefault = PropData.GetAdditionalAttributeValue("ShowDefault", true);
            List<SelectionItem<string>> list;
            list = (
                from tzi in tzis orderby tzi.DisplayName
                orderby tzi.DisplayName
                select
                    new SelectionItem<string> {
                        Text = tzi.DisplayName,
                        Value = tzi.Id,
                        Tooltip = tzi.IsDaylightSavingTime(dt) ? tzi.DaylightName : tzi.StandardName,
                    }).ToList<SelectionItem<string>>();
            if (showDefault) {
                if (string.IsNullOrWhiteSpace(model))
                    model = TimeZoneInfo.Local.Id;
            } else
                list.Insert(0, new SelectionItem<string> { Text = this.__ResStr("select", "(select)"), Value = "" });

            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_timezone");
        }
    }
}
