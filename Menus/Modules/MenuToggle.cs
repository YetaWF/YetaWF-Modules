/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Menus.Modules {

    public class MenuToggleModuleDataProvider : ModuleDefinitionDataProvider<Guid, MenuToggleModule>, IInstallableModel { }

    [ModuleGuid("{FA60EF56-2FF1-41f1-BD9C-6DAFCC21169B}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class MenuToggleModule : ModuleDefinition {

        public MenuToggleModule() : base() {
            Title = this.__ResStr("modTitle", "Menu Toggle");
            Name = this.__ResStr("modName", "Menu Toggle");
            Description = this.__ResStr("modSummary", "Used to display a menu toggle button on small screens.");
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MenuToggleModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

        [Category("General"), Caption("Target Element"), Description("Defines the element containing the menu controlled by this toggle button using CSS selectors.")]
        [UIHint("Text80"), StringLength(80), Required]
        public string Target { get; set; }
    }
}