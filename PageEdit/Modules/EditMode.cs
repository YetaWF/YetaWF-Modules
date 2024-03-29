/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.PageEdit.Modules {

    public class EditModeModuleDataProvider : ModuleDefinitionDataProvider<Guid, EditModeModule>, IInstallableModel { }

    [ModuleGuid("{267f00cc-c619-4854-baed-9e4b812d7e95}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class EditModeModule : ModuleDefinition {

        public EditModeModule() {
            Title = this.__ResStr("modTitle", "Page Edit Mode Selector");
            Name = this.__ResStr("modName", "Page Edit Mode Selector (Skin)");
            Description = this.__ResStr("modSummary", "Displays an icon which allows switching between Site Edit Mode and Site View Mode. This module is typically added to every page (as a skin module) so authorized users can switch to Site Edit Mode and back to Site View Mode.");
            WantFocus = false;
            WantSearch = false;
            ShowTitle = false;
            Print = false;
            Invokable = true;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new EditModeModuleDataProvider(); }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction? GetAction_SwitchToEdit() {
            PageControlModule mod = new PageControlModule();
            return mod.GetAction_SwitchToEdit();
        }
        public ModuleAction? GetAction_SwitchToView() {
            PageControlModule mod = new PageControlModule();
            return mod.GetAction_SwitchToView();
        }
    }
}