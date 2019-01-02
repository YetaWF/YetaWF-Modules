/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Dashboard.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AuditDisplayModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AuditDisplayModule> {

        public AuditDisplayModuleController() { }

        public class DisplayModel {

            [Caption("Id"), Description("The internal id")]
            [UIHint("IntValue"), ReadOnly]
            public int Id { get; set; }
            [Caption("Created"), Description("The date/time this record was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Identifier/String"), Description("The identifying string of the record - Identifier String and Type both identify the source of this record")]
            [UIHint("String"), ReadOnly]
            public string IdentifyString { get; set; }
            [Caption("Identifier/Type"), Description("The type of the record - Identifier String and Type both identify the source of this record")]
            [UIHint("Guid"), ReadOnly]
            public Guid IdentifyGuid { get; set; }

            [Caption("Action"), Description("The action that created this record")]
            [UIHint("String"), ReadOnly]
            public string Action { get; set; }
            [Caption("Description"), Description("The description for this record")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }
            [Caption("Changes"), Description("The properties that were changed")]
            [UIHint("String"), ReadOnly]
            public string Changes { get; set; }

            [Caption("Site"), Description("The site that was changed")]
            [UIHint("SiteId"), ReadOnly]
            public int SiteIdentity { get; set; }
            [Caption("User"), Description("The user that made the change")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Restart Pending"), Description("Defines whether this action requires a restart to take effect")]
            [UIHint("Boolean"), ReadOnly]
            public bool RequiresRestart { get; set; }
            [Caption("Expensive Action"), Description("Defines whether this action is an expensive action in a multi-instance site")]
            [UIHint("Boolean"), ReadOnly]
            public bool ExpensiveMultiInstance { get; set; }

            public void SetData(AuditInfo data) {
                ObjectSupport.CopyData(data, this);
                Description = Description?.Truncate(100);
                Changes = Changes?.Replace(",", ", ");
            }
        }

        [AllowGet]
        public async Task<ActionResult> AuditDisplay(int id) {
            using (AuditInfoDataProvider dataProvider = new AuditInfoDataProvider()) {
                AuditInfo data = await dataProvider.GetItemAsync(id);
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "Audit Info \"{0}\" not found"), id);
                DisplayModel model = new DisplayModel();
                model.SetData(data);
                return View(model);
            }
        }
    }
}
