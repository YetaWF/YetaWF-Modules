/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Scheduler.Controllers {

    public class SchedulerEditModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerEditModule> {

        public SchedulerEditModuleController() { }

        [Trim]
        public class SchedulerEditModel {

            [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
            [UIHint("Text40"), StringLength(SchedulerItemData.MaxName), Required]
            public string Name { get; set; }

            [Caption("Event"), Description("The event running at the scheduled time")]
            [UIHint("YetaWF_Scheduler_Event"), Required]
            public SchedulerEvent Event { get; set; }

            [Caption("Description"), Description("The description of this scheduler item")]
            [UIHint("TextAreaSourceOnly"), StringLength(SchedulerItemData.MaxDescription), Required]
            public string Description { get; set; }

            [Caption("Enabled"), Description("Defines whether the scheduler item is enabled")]
            [UIHint("Boolean")]
            public bool Enabled { get; set; }

            [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
            [UIHint("Boolean")]
            public bool EnableOnStartup { get; set; }

            [Caption("Run Once"), Description("Defines whether the scheduler item is run just once. Once it completes, it is disabled")]
            [UIHint("Boolean")]
            public bool RunOnce { get; set; }

            [Caption("Startup"), Description("Defines whether the scheduler item runs at website startup")]
            [UIHint("Boolean")]
            public bool Startup { get; set; }

            [Caption("Site Specific"), Description("Defines whether the scheduler item runs for each site")]
            [UIHint("Boolean")]
            public bool SiteSpecific { get; set; }

            [Caption("Interval"), Description("The scheduler item's frequency")]
            [UIHint("YetaWF_Scheduler_Frequency"), Required]
            public SchedulerFrequency Frequency { get; set; }

            [UIHint("Hidden")]
            public string OriginalName { get; set; }

            public SchedulerEditModel() {
                Event = new SchedulerEvent();
                Frequency = new SchedulerFrequency();
            }

            public SchedulerItemData GetEvent() {
                SchedulerItemData evnt = new SchedulerItemData();
                ObjectSupport.CopyData(this, evnt);
                return evnt;
            }

            public void SetEvent(SchedulerItemData evnt)
            {
                ObjectSupport.CopyData(evnt, this);
                OriginalName = Name;
            }
        }

        [AllowGet]
        public async Task<ActionResult> SchedulerEdit(string eventName) {
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                SchedulerEditModel model = new SchedulerEditModel { };
                SchedulerItemData evnt = await dataProvider.GetItemAsync(eventName);
                if (evnt == null)
                    throw new Error(this.__ResStr("notFound", "Scheduler item \"{0}\" not found."), eventName);
                model.SetEvent(evnt);
                Module.Title = this.__ResStr("modTitle", "Scheduler Item \"{0}\"", evnt.Name);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SchedulerEdit_Partial(SchedulerEditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                switch (await dataProvider.UpdateItemAsync(model.OriginalName, model.GetEvent())) {
                    default:
                    case UpdateStatusEnum.RecordDeleted:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyDeleted", "The scheduler item named \"{0}\" has been removed and can no longer be updated.", model.Name));
                        return PartialView(model);
                    case UpdateStatusEnum.NewKeyExists:
                        ModelState.AddModelError("Name", this.__ResStr("alreadyExists", "A scheduler item named \"{0}\" already exists.", model.Name));
                        return PartialView(model);
                    case UpdateStatusEnum.OK:
                        break;
                }
                return FormProcessed(model, this.__ResStr("okSaved", "Scheduler item saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}