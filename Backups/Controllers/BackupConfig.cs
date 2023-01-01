/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Backups.DataProvider;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Backups.Controllers {

    public class BackupConfigModuleController : ControllerImpl<YetaWF.Modules.Backups.Modules.BackupConfigModule> {

        public BackupConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Days"), Description("The number of days a backup is saved - once a backup has been saved for the specified number of days, it is deleted")]
            [UIHint("IntValue"), Range(1, 99999999), Required]
            public int Days { get; set; }

            [Caption("Data Only"), Description("Defines whether only data is backed up (scheduled backups) - Otherwise all source and binaries (complete packages) are backed up - For production sites it is generally sufficient to back up the data")]
            [UIHint("Boolean")]
            public bool DataOnly { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> BackupConfig() {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                Model model = new Model { };
                ConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The backup settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> BackupConfig_Partial(Model model) {
            using (ConfigDataProvider dataProvider = new ConfigDataProvider()) {
                ConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Backup settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}