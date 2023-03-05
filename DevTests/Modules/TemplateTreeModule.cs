/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.DevTests.Endpoints;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTreeModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTreeModule>, IInstallableModel { }

    [ModuleGuid("{A1952569-E77D-40fb-8C4A-2A1412EB03E2}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTreeModule : ModuleDefinition2 {

        public TemplateTreeModule() {
            Title = this.__ResStr("modTitle", "Tree (Static) Test Component");
            Name = this.__ResStr("modName", "Component Test - Tree");
            Description = this.__ResStr("modSummary", "Test module for the Tree component. A test page for this module can be found at Tests > Templates > Tree (standard YetaWF site).");
            DefaultViewName = StandardViews.Display;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTreeModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Tree"),
                MenuText = this.__ResStr("displayText", "Tree"),
                Tooltip = this.__ResStr("displayTooltip", "Display a sample tree"),
                Legend = this.__ResStr("displayLegend", "Displays a sample tree"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true,
            };
        }

        public class EntryElement : TreeEntry {

            [Caption("Text"), Description("Entries")]
            [UIHint("String"), ReadOnly]
            public override string Text { get; set; } = null!;

            public string? OtherData { get; set; }

            public EntryElement() { }
        }

        public class Model {

            [Caption(""), Description("")]
            [UIHint("Tree"), ReadOnly]
            public List<EntryElement> Entries { get; set; } = null!;

            public TreeDefinition Entries_TreeDefinition { get; set; } = null!;

            public Model() { }
        }

        internal static TreeDefinition GetTreeModel() {
            return new TreeDefinition {
                DragDrop = true,
                RecordType = typeof(EntryElement),
                ShowHeader = true,
                JSONData = true,
                AjaxUrl = Utility.UrlFor<TemplateTreeModuleEndpoints>(TemplateTreeModuleEndpoints.GetRecords),
            };
        }

        private static List<EntryElement> GetGeneratedData() {
            List<EntryElement> list = new List<EntryElement>();
            for (int i = 0; i < 3; ++i) {
                list.Add(new EntryElement {
                    Text = $"Entry {i}",
                    OtherData = $"Otherdata {i}",
                    SubEntries = (from s in GetSubEntries(2) select (TreeEntry)s).ToList(),
                });
            }
            return list;
        }

        private static List<EntryElement>? GetSubEntries(int level) {
            if (level == 0) return null;
            --level;

            List<EntryElement> list = new List<EntryElement>();
            for (int i = 0; i < 1 + level; ++i) {
                List<EntryElement>? subs = GetSubEntries(level);
                list.Add(new EntryElement {
                    Text = $"Entry {i}",
                    OtherData = $"Otherdata {i}",
                    SubEntries = (subs != null) ? (from s in subs select (TreeEntry)s).ToList() : null,
                    Collapsed = true,
                    UrlNew = level == 1 ? "https://ubackup.io" : null,
                    UrlContent = level == 0 ? "https://yetawf.com/Admin/Bar/Dashboard" : null,
                    DynamicSubEntries = level == 0,
                });
            }
            return list;
        }
        internal static List<EntryElement> GetDynamicSubEntries() {
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

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model {
                Entries = GetGeneratedData(),
                Entries_TreeDefinition = GetTreeModel(),
            };
            return RenderAsync(model);
        }
    }
}
