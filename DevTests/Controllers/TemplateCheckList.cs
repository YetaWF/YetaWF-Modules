using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Modules.ComponentsHTML.Components;

#nullable enable

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateCheckListModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateCheckListModule> {

        public TemplateCheckListModuleController() { }

        [Trim]
        public class Model {

            [Caption("CheckList"), Description("CheckList")]
            [UIHint("CheckList"), Trim]
            public Dictionary<string, bool> Prop1 { get; set; } = null!;
            public List<SelectionCheckListItem> Prop1_List {
                get {
                    return new List<SelectionCheckListItem> {
                        new SelectionCheckListItem{ Text = "Item 1", Tooltip = "Tooltip for item 1", Enabled = false },
                        new SelectionCheckListItem{ Text = "Item 2", Tooltip = "Tooltip for item 2", Enabled = true },
                        new SelectionCheckListItem{ Text = "Item 3", Tooltip = "Tooltip for item 3", Enabled = true },
                        new SelectionCheckListItem{ Text = "Item 4", Tooltip = "Tooltip for item 4", Enabled = true },
                        new SelectionCheckListItem{ Text = "Item 5", Tooltip = "Tooltip for item 5", Enabled = true },
                    };
                }
            }
            public string Prop1_SVG { get { return "fas-columns"; } }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateCheckList() {
            Model model = new Model {
                Prop1 = new Dictionary<string, bool> { { "Checkbox1", true }, { "Checkbox2", true }, { "Checkbox3", false }, { "Checkbox4", false }, { "Checkbox5", true } },
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateCheckList_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            return FormProcessed(model, this.__ResStr("ok", "OK"));
        }
    }
}
