/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using System.Collections.Generic;
using YetaWF.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Modules.ComponentsHTML.Components;
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

            public string UrlNew { get; set; }
            public string UrlContent { get; set; }

            public string OtherData { get; set; }

            public int Id { get; set; }
            public bool Collapsed { get; set; }
            public List<EntryElement> SubEntries { get; set; }
            public bool DynamicSubEntries { get; set; }

            public EntryElement() { }
        }

        public class Model {

            [Caption(""), Description("")]
            [UIHint("Tree"), ReadOnly]
            public List<EntryElement> Entries { get; set; }

            public TreeDefinition Entries_TreeDefinition { get; set; }

            public Model() { }
        }

        private TreeDefinition GetTreeModel() {
            return new TreeDefinition {
                DragDrop = true,
                RecordType = typeof(EntryElement),
                ShowHeader = true,
                UseSkinFormatting = true,
                ContentTargetId = null,//$$$
                ContentTargetPane = null,//$$$
                AjaxUrl = GetActionUrl(nameof(TemplateTree_GetRecords)),
            };
        }

        [AllowGet]
        public ActionResult TemplateTree() {
            Model model = new Model {
                Entries = GetGeneratedData(),
                Entries_TreeDefinition = GetTreeModel(),
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
                    Collapsed = true,
                    UrlNew = level == 1 ? "https://ubackup.io" : null,
                    UrlContent = level == 0 ? "https://yetawf.com/Admin/Bar/Dashboard" : null,
                    DynamicSubEntries = level == 0,
                });
            }
            return list;
        }
        private List<EntryElement> GetDynamicSubEntries() {
            List<EntryElement> list = new List<EntryElement>();
            for (int i = 0; i < 10; ++i) {
                list.Add(new EntryElement {
                    Text = $"Entry {i}",
                    OtherData = $"Otherdata {i}",
                    Collapsed = true,
                    UrlContent = "https://yetawf.com/Admin/Bar/Dashboard",
                    DynamicSubEntries = (i % 2) == 0,
                });
            }
            return list;
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public ActionResult TemplateTree_GetRecords(EntryElement data) {
            List<EntryElement> list = GetDynamicSubEntries();
            List<object> d = (from l in list select (object)l).ToList<object>();
            DataSourceResult ds = new DataSourceResult() {
                Data = d,
                Total = d.Count,
            };
            TreePartialData treePartial = new TreePartialData {
                TreeDef = GetTreeModel(),
                Data = ds,
            };
            return PartialView("TreePartialData", treePartial, ContentType: "application/json", PureContent: true, AreaViewName:false,  Gzip: true);
        }
    }
}
;