/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Scheduler.Components {

    public abstract class EventComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(EventComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Event";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Scheduler package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class EventDisplayComponent : EventComponentBase, IYetaWFComponent<SchedulerEvent> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(SchedulerEvent model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div class='yt_yetawf_scheduler_event t_display'>
    <div class='t_event'>
        <span class='t_eventname'>{Utility.HE(model.Name)}</span>
    </div>
    <div class='t_info'>
        <div class='t_row t_implementingassembly'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(model, nameof(model.ImplementingAssembly))}
            </div>
            <div class='t_vals'>
                <span class='t_implasm'>{Utility.HE(model.ImplementingAssembly)}</span>
            </div>
        </div>
        <div class='t_row t_implementingtype'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(model, nameof(model.ImplementingType))}
            </div>
            <div class='t_vals'>
                <span class='t_impltype'>{Utility.HE(model.ImplementingType)}</span>
            </div>
        </div>
    </div>
</div>");

            return hb.ToString();
        }
    }

    /// <summary>
    /// This component is used by the YetaWF.Scheduler package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class EventEditComponent : EventComponentBase, IYetaWFComponent<SchedulerEvent> {

        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        public class EventUI {
            [UIHint("DropDownList")]
            public string DropDown { get; set; }
            public List<SelectionItem<string>> DropDown_List { get; set; }
        }

        public async Task<string> RenderAsync(SchedulerEvent model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<Type> schedulerEvents = YetaWF.Modules.Scheduler.Support.Scheduler.Instance.SchedulerEvents;
            List<SelectionItem<string>> list = new List<SelectionItem<string>>();
            foreach (Type type in schedulerEvents) {
                IScheduling isched = (IScheduling)Activator.CreateInstance(type);
                SchedulerItemBase[] items = isched.GetItems();
                foreach (SchedulerItemBase item in items) {
                    list.Add(new SelectionItem<string>() {
                        Text = item.EventName,
                        Tooltip = item.Description,
                        Value = item.EventName + "," + type.FullName + "," + type.Assembly.GetName().Name
                    });
                }
            }
            if (list.Count == 0) throw new Error(__ResStr("noEvents", "No events are available"));

            string select;
            if (string.IsNullOrWhiteSpace(model.Name)) {
                select = list.First().Value;
            } else {
                select = model.Name + "," + model.ImplementingType + "," + model.ImplementingAssembly;
            }

            EventUI eventUI = new EventUI {
                DropDown = select,
                DropDown_List = list,
            };

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div id='{ControlId}' class='yt_yetawf_scheduler_event t_edit'>
    <div class='t_event'>
        {await HtmlHelper.ForEditAsync(eventUI, nameof(eventUI.DropDown))}
        {await HtmlHelper.ForEditAsync(model, nameof(model.Name))}
        {await HtmlHelper.ForEditAsync(model, nameof(model.ImplementingAssembly))}
        {await HtmlHelper.ForEditAsync(model, nameof(model.ImplementingType))}
    </div>
    <div class='t_info'>
        <div class='t_row t_implementingassembly'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(model, nameof(model.ImplementingAssembly))}
            </div>
            <div class='t_vals'>
                <span class='t_implasm'>{Utility.HE(model.ImplementingAssembly)}</span>
            </div>
        </div>
        <div class='t_row t_implementingtype'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(model, nameof(model.ImplementingType))}
            </div>
            <div class='t_vals'>
                <span class='t_impltype'>{Utility.HE(model.ImplementingType)}</span>
            </div>
        </div>
        <div class='t_row t_eventbuiltindescription'>
            <div class='t_labels'>
                {await HtmlHelper.ForLabelAsync(model, nameof(model.EventBuiltinDescription))}
            </div>
            <div class='t_vals'>
                <span class='t_description'>{Utility.HE(model.EventBuiltinDescription)}</span>
            </div>
        </div>
    </div>
</div>");

                Manager.ScriptManager.AddLast($@"new YetaWF_Scheduler.EventEditComponent('{ControlId}');");
            }

            return hb.ToString();
        }
    }
}
