/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Scheduler.Components {

    public abstract class FrequencyComponentBase : YetaWFComponent {

        public const string TemplateName = "Frequency";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }
    public class FrequencyDisplayComponent : FrequencyComponentBase, IYetaWFComponent<SchedulerFrequency> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(SchedulerFrequency model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_scheduler_frequency t_display'>
    {this.__ResStr("every", "Every ")}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.Value))}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.TimeUnits))}
    {this.__ResStr("everyPost", "")}
</div>");

            return hb.ToYHtmlString();
        }
    }
    public class FrequencyEditComponent : FrequencyComponentBase, IYetaWFComponent<SchedulerFrequency> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<YHtmlString> RenderAsync(SchedulerFrequency model) {

            HtmlBuilder hb = new HtmlBuilder();

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div class='yt_yetawf_scheduler_frequency t_edit'>
    {this.__ResStr("every", "Every ")}
    {await HtmlHelper.ForEditAsync(model, nameof(model.Value))}{ValidationMessage(nameof(model.Value))}
    {await HtmlHelper.ForEditAsync(model, nameof(model.TimeUnits))}{ValidationMessage(nameof(model.TimeUnits))}
    {this.__ResStr("everyPost", "")}
</div>");
            }
            return hb.ToYHtmlString();
        }
    }
}
