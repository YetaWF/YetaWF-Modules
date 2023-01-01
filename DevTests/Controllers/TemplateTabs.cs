/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Models;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTabsModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTabsModule> {

        public TemplateTabsModuleController() { }

        [Trim]
        public class Model {

            [Caption("Tabs"), Description("Tabs")]
            [UIHint("Tabs"), ReadOnly]
            public TabsDefinition Prop1 { get; set; }

            public int _ActiveTab { get; set; }

            public Model() {

                Prop1 = new TabsDefinition();
                for (int i = 0; i < 10; ++i) {
                    Prop1.Tabs.Add(new TabEntry {
                        Caption = $"Tab {i}",
                        ToolTip = $"Tooltip for tab {i}",
                        PaneCssClasses = $"tttt{i}",
                        RenderPaneAsync = (int tabIndex) => { return Task.FromResult($"<div>Pane {tabIndex}</div>"); },
                    });
                }
            
            }
        }

        [AllowGet]
        public ActionResult TemplateTabs() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTabs_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            model.Prop1.ActiveTabIndex = model._ActiveTab;
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
