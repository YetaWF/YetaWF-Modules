/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTabsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTabsModule>, IInstallableModel { }

    [ModuleGuid("{806e260e-608a-4ff1-97cc-58c3f8421ae6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTabsModule : ModuleDefinition2 {

        public TemplateTabsModule() {
            Title = this.__ResStr("modTitle", "Tabs Test Template");
            Name = this.__ResStr("modName", "Template Test - Tabs");
            Description = this.__ResStr("modSummary", "Test module for the Tabs component. A test page for this module can be found at Tests > Templates > Tabs (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTabsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

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

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model();
            return RenderAsync(model);
        }

        public Task<IResult> UpdateModuleAsync(Model model) {
            if (!ModelState.IsValid)
                return PartialViewAsync(model);
            model.Prop1.ActiveTabIndex = model._ActiveTab;
            return FormProcessedAsync(model, this.__ResStr("ok", "OK"));
        }
    }
}
