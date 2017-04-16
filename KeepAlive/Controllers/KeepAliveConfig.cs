/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.KeepAlive.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.KeepAlive.Controllers {

    public class KeepAliveConfigModuleController : ControllerImpl<YetaWF.Modules.KeepAlive.Modules.KeepAliveConfigModule> {

        public KeepAliveConfigModuleController() { }

        [Trim]
        [Header("Defines how often the site is accessed to keep it alive. This is typically used with shared hosting providers which may unload the site after a period of inactivity. Once the site is unloaded it would perform a restart which is a time consuming operation.")]
        public class Model {

            [Caption("Interval"), Description("The interval in minutes used to access the Url to keep the YetaWF instance alive - 0 to turn off")]
            [UIHint("IntValue4"), Range(0, 9999), Required]
            public int Interval { get; set; }

            [Caption("Page Accessed"), Description("The page to be accessed to keep the site alive - use a fully qualified Url including http://, https:// and domain name")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string Url { get; set; }

            public KeepAliveConfigData GetData(KeepAliveConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(KeepAliveConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult KeepAliveConfig() {
            using (KeepAliveConfigDataProvider dataProvider = new KeepAliveConfigDataProvider()) {
                Model model = new Model { };
                KeepAliveConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The keep alive settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult KeepAliveConfig_Partial(Model model) {
            using (KeepAliveConfigDataProvider dataProvider = new KeepAliveConfigDataProvider()) {
                KeepAliveConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Keep alive settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }

        [HttpPost]
        public ActionResult RunKeepAlive() {
            Scheduler.KeepAlive keepAlive = new Scheduler.KeepAlive();
            keepAlive.RunKeepAlive(false);
            return Reload(PopupText: this.__ResStr("done", "Site accessed"), Reload: ReloadEnum.ModuleParts);
        }
    }
}