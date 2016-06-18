/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Core.Views;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Scheduler.Views.Shared {

    public class SchedulerEvent<TModel> : RazorTemplate<TModel> { }

    public static class SchedulerEventHelper {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SchedulerEventHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderSchedulerEvent(this HtmlHelper htmlHelper, string name, string eventName, object HtmlAttributes = null) {
            List<Type> schedulerEvents = YetaWF.Modules.Scheduler.Support.Scheduler.Instance.SchedulerEvents;

            List<SelectionItem<string>> list = new List<SelectionItem<string>>();
            foreach (Type type in schedulerEvents) {
                IScheduling isched = (IScheduling) Activator.CreateInstance(type);
                SchedulerItemBase[] items = isched.GetItems();
                foreach (var item in items) {
                    list.Add(new SelectionItem<string>() {
                        Text = item.EventName,
                        Tooltip = item.Description,
                        Value = item.EventName + "," + type.FullName + "," + type.Assembly.GetName().Name
                    });
                }
            }
            Package currentPackage = YetaWF.Modules.Scheduler.Controllers.AreaRegistration.CurrentPackage;
            if (list.Count == 0) throw new Error(__ResStr("noEvents", "No events are available"));

            string select = null;
            if (eventName != null)
                select = eventName + "," + htmlHelper.GetModelProperty<string>("ImplementingType") + "," + htmlHelper.GetModelProperty<string>("ImplementingAssembly");
            return htmlHelper.RenderDropDownSelectionList(name, select, list, HtmlAttributes: HtmlAttributes);
        }
    }
}