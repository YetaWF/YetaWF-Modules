/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateCheckListModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateCheckListModule>, IInstallableModel { }

    [ModuleGuid("{bf1751a7-d7bb-4338-8f66-420edc135b8b}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    //[ModuleCategory("...")]
    public class TemplateCheckListModule : ModuleDefinition2 {

        public TemplateCheckListModule() {
            Title = this.__ResStr("modTitle", "CheckList Test Template");
            Name = this.__ResStr("modName", "Template Test - CheckList");
            Description = this.__ResStr("modSummary", "CheckList test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateCheckListModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        const int MaxItems = 100;

        [Trim]
        public class Model {

            [Caption("CheckList Menu"), Description("CheckList Menu")]
            [UIHint("CheckListMenu")]
            public List<SelectionCheckListEntry>? Prop1 { get; set; }
            public string Prop1_DDSVG { get { return "fas-columns"; } }
            public List<SelectionCheckListDetail>? Prop1_List {
                get {
                    return new List<SelectionCheckListDetail> {
                        new SelectionCheckListDetail { Key = "Checkbox1", Text = "Item 1", Description = "Description for item 1", Enabled = false },
                        new SelectionCheckListDetail { Key = "Checkbox2", Text = "Item 2", Description = "Description for item 2", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox3", Text = "Item 3", Description = "Description for item 3", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox4", Text = "Item 4", Description = "Description for item 4", Enabled = true },
                        new SelectionCheckListDetail { Key = "Checkbox5", Text = "Item 5", Description = "Description for item 5", Enabled = true },
                    }; ;
                }
            }

            [Caption("CheckList Panel"), Description("CheckList Panel")]
            [UIHint("CheckListPanel")]
            public List<SelectionCheckListEntry>? Prop2 { get; set; }
            public List<SelectionCheckListDetail> Prop2_List {
                get {
                    List<SelectionCheckListDetail> list = new List<SelectionCheckListDetail>();
                    for (int i = 1; i < MaxItems; ++i) {
                        list.Add(new SelectionCheckListDetail { Key = $"Checkbox{i}", Text = $"Item {i:D4}", Description = $"Description for item {i}", Enabled = true });
                    }
                    list[3].Enabled = false;
                    list[8].Enabled = false;
                    return list;
                }
            }

            public Model() { }
        }

        public async Task<ActionInfo> RenderModuleAsync() {
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
            for (int i = 1; i < MaxItems; ++i) {
                model.Prop2.Add(new SelectionCheckListEntry { Key = $"Checkbox{i}", Value = false });
            }
            return await RenderAsync(model);
        }

        public async Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            return await FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
