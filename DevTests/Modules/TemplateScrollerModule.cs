/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateScrollerModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateScrollerModule>, IInstallableModel { }

    [ModuleGuid("{4f4d9110-369c-441d-bf55-adc210fa4bc0}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateScrollerModule : ModuleDefinition {

        public TemplateScrollerModule() {
            Title = this.__ResStr("modTitle", "Scroller Test Component");
            Name = this.__ResStr("modName", "Component Test - Scroller");
            Description = this.__ResStr("modSummary", "Test module for the Scroller component. A test page for this module can be found at Tests > Templates > Scroller (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateScrollerModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction() {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Scroller"),
                MenuText = this.__ResStr("displayText", "Scroller"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Scroller test"),
                Legend = this.__ResStr("displayLegend", "Displays the Scroller test"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,

            };
        }

        public class ScrollerItem {
            [UIHint("Image")]
            public string Image { get; set; } = null!;
            [UIHint("String")]
            public string Title { get; set; } = null!;
            [UIHint("String")]
            public string Summary { get; set; } = null!;
        }

        [Trim]
        public class Model {
            // The Scroller component is a core component implementing the overall Scroller
            // The AdditionalMetadata describes the user-defined component used for each item in the Scroller
            [UIHint("Scroller"), ReadOnly, AdditionalMetadata("Template", "YetaWF_DevTests_ScrollerItem")]
            public List<ScrollerItem> Items { get; set; }

            public Model() {
                Items = new List<ScrollerItem>();
            }
        }

        public Task<ActionInfo> RenderModuleAsync() {
            Model model = new Model { };
            string addonUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName);
            // generate some random data for the scroller items
            for (int index = 0; index < 12; ++index) {
                model.Items.Add(new ScrollerItem {
                    Image = Manager.GetCDNUrl(string.Format("{0}Images/image{1}.png", addonUrl, index)),
                    Title = this.__ResStr("itemTitle", "Item {0}", index),
                    Summary = this.__ResStr("itemSummary", "Summary for item {0} - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", index),
                });
            }
            return RenderAsync(model);
        }
    }
}
