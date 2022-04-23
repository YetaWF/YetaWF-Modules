/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
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

    public class SchedulerAddModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.SchedulerAddModule> {

        public SchedulerAddModuleController() { }

        [Trim]
        public class SchedulerAddModel {

            [Caption("Name"), Description("The name of this scheduler item - the name is used to identify a scheduler item")]
            [UIHint("Text40"), Required, StringLength(SchedulerItemData.MaxName)]
            public string? Name { get; set; }

            [Caption("Event"), Description("The event type of the scheduler item")]
            [UIHint("YetaWF_Scheduler_Event")]
            public SchedulerEvent Event { get; set; }

            [Caption("Description"), Description("The description of this scheduler item")]
            [UIHint("TextAreaSourceOnly"), StringLength(SchedulerItemData.MaxDescription), Required]
            public string? Description { get; set; }

            [Caption("Enabled"), Description("The status of the scheduler item")]
            [UIHint("Boolean")]
            public bool Enabled { get; set; }

            [Caption("Enable On Startup"), Description("Defines whether the scheduler item is enabled every time the website is restarted")]
            [UIHint("Boolean")]
            public bool EnableOnStartup { get; set; }

            [Caption("Run Once"), Description("Defines whether the scheduler item is run just once - once it completes, it is disabled")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(Enabled), true)]
            [ProcessIf(nameof(EnableOnStartup), true)]
            public bool RunOnce { get; set; }

            [Caption("Startup"), Description("Defines whether the scheduler item runs at website startup")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(Enabled), true)]
            [ProcessIf(nameof(EnableOnStartup), true)]
            public bool Startup { get; set; }

            [Caption("Site Specific"), Description("Defines whether the scheduler item runs for each site")]
            [UIHint("Boolean")]
            [ProcessIf(nameof(Enabled), true)]
            [ProcessIf(nameof(EnableOnStartup), true)]
            public bool SiteSpecific { get; set; }

            [Caption("Interval"), Description("The scheduler item's frequency")]
            [UIHint("YetaWF_Scheduler_Frequency"), Required]
            [ProcessIf(nameof(Enabled), true)]
            [ProcessIf(nameof(EnableOnStartup), true)]
            public SchedulerFrequency Frequency { get; set; }

            public SchedulerAddModel() {
                Frequency = new SchedulerFrequency() { TimeUnits = SchedulerFrequency.TimeUnitEnum.Hours, Value = 1 };
                Event = new SchedulerEvent();
            }
            public SchedulerItemData GetEvent() {
                SchedulerItemData evnt = new SchedulerItemData();
                ObjectSupport.CopyData(this, evnt);
                return evnt;
            }
        }

        [AllowGet]
        public ActionResult SchedulerAdd() {
            SchedulerAddModel model = new SchedulerAddModel {};
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SchedulerAdd_Partial(SchedulerAddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (SchedulerDataProvider dataProvider = new SchedulerDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetEvent()))
                    throw new Error(this.__ResStr("alreadyExists", "A scheduler item named \"{0}\" already exists."), model.Name);
                return FormProcessed(model, this.__ResStr("okSaved", "New scheduler item saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
            }
        }
    }
}