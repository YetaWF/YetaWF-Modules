/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.TawkTo.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.TawkTo.Controllers {

    public class ConfigModuleController : ControllerImpl<YetaWF.Modules.TawkTo.Modules.ConfigModule> {

        public ConfigModuleController() { }

        [Trim]
        public class Model {

            [TextBelow("The Site ID can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
            [Caption("Site ID"), Description("Defines the account used for the chat window - The Site ID can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
            [UIHint("Text80"), StringLength(ConfigData.MaxAccount), Required]
            [HelpLink("https://dashboard.tawk.to")]
            public string Account { get; set; }

            [TextBelow("The API Key can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
            [Caption("API Key"), Description("Defines the API Key used for the chat window - The API Key can be obtained from your Tawk.to dashboard (Administration > Property Settings)")]
            [UIHint("Text80"), StringLength(ConfigData.MaxAPIKey), Required]
            [HelpLink("https://dashboard.tawk.to")]
            public string APIKey { get; set; }

            [Caption(" "), Description(" ")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Page Css (Exclude)"), Description("Defines the Css classes (space separated) for pages where the Tawk.to chat window is not shown - Pages can be prevented from showing the chat invitation by specifying their Css class found on the <body> tag - If no Css class is specified, all pages show the chat invitation")]
            [UIHint("Text80"), StringLength(ConfigData.MaxCss)]
            public string ExcludedPagesCss { get; set; }

            [Caption("Page Css (Include)"), Description("Defines the Css classes (space separated) for pages where the Tawk.to chat window is shown - Only pages with one of the defined Css classes will display the chat invitation - If no Css class is specified, all pages show the chat invitation")]
            [UIHint("Text80"), StringLength(ConfigData.MaxCss)]
            public string IncludedPagesCss { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() {
                Description = this.__ResStr("desc", "The Tawk.to (Skin) module is referenced site wide (in Site Settings), in which case all pages show the chat invitation. By using the fields Page Css (Exclude and Include) below, the pages where the chat invitation is shown can be limited. If a chat is already in progress, the chat window is unaffected and is always shown.");
            }
        }

        [AllowGet]
        public ActionResult Config() {
            using (ConfigDataDataProvider dataProvider = new ConfigDataDataProvider()) {
                Model model = new Model { };
                ConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The Tawk.to settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult Config_Partial(Model model) {
            using (ConfigDataDataProvider dataProvider = new ConfigDataDataProvider()) {
                ConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Tawk.To Settings saved"));
            }
        }
    }
}
