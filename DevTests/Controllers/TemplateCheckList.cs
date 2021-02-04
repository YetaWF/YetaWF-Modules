using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Core.Support;

#nullable enable

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateCheckListModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateCheckListModule> {

        public TemplateCheckListModuleController() { }

        [Trim]
        public class Model {

            [Caption("CheckList"), Description("CheckList")]
            [UIHint("CheckList")]
            public List<SelectionCheckListEntry> Prop1 { get; set; } = null!;
            public string Prop1_DDSVG { get { return "fas-columns"; } }
            public List<SelectionCheckListDetail> Prop1_List {
                get {
                    return new List<SelectionCheckListDetail> {
                        new SelectionCheckListDetail { Key = "Checkbox1", Text = "Item 1", Tooltip = "Description for item 1", Enabled = false },
                        new SelectionCheckListDetail { Key = "Checkbox2", Text = "Item 2", Tooltip = "Description for item 2", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox3", Text = "Item 3", Tooltip = "Description for item 3", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox4", Text = "Item 4", Tooltip = "Description for item 4", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox5", Text = "Item 5", Tooltip = "Description for item 5", Enabled = true },
                    };;
                }
            }

            [Caption("CheckList Panel"), Description("CheckList Panel")]
            [UIHint("CheckListPanel")]
            public List<SelectionCheckListEntry> Prop2 { get; set; } = null!;
            public List<SelectionCheckListDetail> Prop2_List {
                get {
                    List<SelectionCheckListDetail> list = new List<SelectionCheckListDetail>();
                    for (int i = 1; i < 100; ++i) {
                        list.Add(new SelectionCheckListDetail { Key = $"Checkbox{i}", Text = $"Item {i:D4}", Tooltip = $"Description for item {i}", Enabled = true });
                    }
                    list[3].Enabled = false;
                    list[8].Enabled = false;
                    return list;
                }
            }

            public Model() { }
        }

        [AllowGet]
        public ActionResult TemplateCheckList() {
            Model model = new Model {
                Prop1 = new List<SelectionCheckListEntry> {
                    new SelectionCheckListEntry { Key = "Checkbox1", Value = true },
                    new SelectionCheckListEntry { Key = "Checkbox2", Value = true },
                    new SelectionCheckListEntry { Key = "Checkbox3", Value = true },
                    new SelectionCheckListEntry { Key = "Checkbox4", Value = true },
                    new SelectionCheckListEntry { Key = "Checkbox5", Value = true },
                },
                Prop2 = new List<SelectionCheckListEntry>(),
            };
            for (int i = 1; i < 100; ++i) {
                model.Prop2.Add(new SelectionCheckListEntry { Key = $"Checkbox{i}", Value = false });
            }
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
