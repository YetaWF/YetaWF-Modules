/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using System.Collections.Generic;
using YetaWF.Core.Models;
using System.Linq;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class TemplateTreeModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.TemplateTreeModule> {

        public TemplateTreeModuleController() { }

        public class EntryElement {

            [Caption("Text"), Description("Entries")]
            [UIHint("String"), ReadOnly]
            public string Text { get; set; }

            public string OtherData { get; set; }

            public int Id { get; set; }
            public bool Collapsed { get; set; }
            public List<EntryElement> SubEntries { get; set; }

            public EntryElement() { }
        }

        public class Model {

            [Caption(""), Description("")]
            [UIHint("Tree"), ReadOnly]
            public List<EntryElement> Entries { get; set; }

            public TreeDefinition Entries_TreeDefinition { get; set; }

            public Model() {
                Entries_TreeDefinition = new TreeDefinition {
                    DragDrop = true,
                    RecordType = typeof(EntryElement),
                    ShowHeader = true,
                    UseSkinFormatting = true,
                };
            }
        }

        [AllowGet]
        public ActionResult TemplateTree() {
            Model model = new Model {
                Entries = GetGeneratedData(),
            };
            return View(model);
        }

        private List<EntryElement> GetGeneratedData() {
            List<EntryElement> list = new List<EntryElement>();
            for (int i = 0; i < 3; ++i) {
                list.Add(new EntryElement {
                    Text = $"Entry {i}",
                    OtherData = $"Otherdata {i}",
                    SubEntries = GetSubEntries(2),
                });
            }
            return list;
        }

        private List<EntryElement> GetSubEntries(int level) {
            if (level == 0) return null;
            --level;

            List<EntryElement> list = new List<EntryElement>();
            for (int i = 0; i < 1 + level ; ++i) {
                list.Add(new EntryElement {
                    Text = $"Entry {i}",
                    OtherData = $"Otherdata {i}",
                    SubEntries = GetSubEntries(level),
                    Collapsed = level > 0,
                });
            }
            return list;
        }
    }
}
