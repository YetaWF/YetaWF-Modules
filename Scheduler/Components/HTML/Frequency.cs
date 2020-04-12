/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

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

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(FrequencyComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Frequency";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Scheduler package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class FrequencyDisplayComponent : FrequencyComponentBase, IYetaWFComponent<SchedulerFrequency> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(SchedulerFrequency model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_scheduler_frequency t_display'>
    {__ResStr("every", "Every ")}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.Value))}
    {await HtmlHelper.ForDisplayAsync(model, nameof(model.TimeUnits))}
    {__ResStr("everyPost", "")}
</div>");

            return hb.ToString();
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.Scheduler package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class FrequencyEditComponent : FrequencyComponentBase, IYetaWFComponent<SchedulerFrequency> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public async Task<string> RenderAsync(SchedulerFrequency model) {

            HtmlBuilder hb = new HtmlBuilder();

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div class='yt_yetawf_scheduler_frequency t_edit'>
    {__ResStr("every", "Every ")}
    {await HtmlHelper.ForEditAsync(model, nameof(model.Value))}{ValidationMessage(nameof(model.Value))}
    {await HtmlHelper.ForEditAsync(model, nameof(model.TimeUnits))}{ValidationMessage(nameof(model.TimeUnits))}
    {__ResStr("everyPost", "")}
</div>");
            }
            return hb.ToString();
        }
    }
}
