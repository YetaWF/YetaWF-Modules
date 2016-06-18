/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.Controllers;

namespace YetaWF.Modules.PageEdit.Modules {

    public class PageControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageControlModule>, IInstallableModel { }

    [ModuleGuid("{466C0CCA-3E63-43f3-8754-F4267767EED1}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class PageControlModule : ModuleDefinition {

        public PageControlModule() {
            Name = this.__ResStr("modName", "Control Panel");
            Title = this.__ResStr("modTitle", "Control Panel");
            Description = this.__ResStr("modSummary", "Control Panel with support for adding modules, new pages, importing modules and editing page settings");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new PageControlModuleDataProvider(); }

        [Category("Variables")]
        [Description("The HTML id used by this module - this id doesn't change for this module and can be used for skinning purposes")]
        [Caption("ModuleHtmlId")]
        public override string ModuleHtmlId { get { return Addons.Info.PageControlMod; } }

        public override bool ShowModuleMenu { get { return false; } }
        public override bool ModuleHasSettings { get { return false; } }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);

            menuList.New(this.GetAction_SwitchToView(), location);
            menuList.New(this.GetAction_SwitchToEdit(), location);

            PageEditModule modEdit = new PageEditModule();
            menuList.New(modEdit.GetAction_Edit(null), location);
            menuList.New(modEdit.GetAction_Remove(null), location);

            return menuList;
        }

        public ModuleAction GetAction_SwitchToEdit() {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PageControlModuleController), "SwitchToEdit"),
                QueryArgs = new { },
                NeedsModuleContext = true,
                Image = "#Edit",
                LinkText = this.__ResStr("modSwitchToEditLink", "Switch To Site Edit Mode"),
                MenuText = this.__ResStr("modSwitchToEditText", "Switch To Site Edit Mode"),
                Tooltip = this.__ResStr("modSwitchToEditTooltip", "Switch to Site Edit Mode"),
                Legend = this.__ResStr("modSwitchToEditLegend", "Switches to Site Edit Mode"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.View,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true
            };

        }
        public ModuleAction GetAction_SwitchToView() {
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PageControlModuleController), "SwitchToView"),
                NeedsModuleContext = true,
                QueryArgs = new { },
                Image = "#Display",
                LinkText = this.__ResStr("modSwitchToViewLink", "Switch To Site View Mode"),
                MenuText = this.__ResStr("modSwitchToViewText", "Switch To Site View Mode"),
                Tooltip = this.__ResStr("modSwitchToViewTooltip", "Switch to Site View Mode"),
                Legend = this.__ResStr("modSwitchToViewLegend", "Switches to Site View Mode"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Edit,
                Location = ModuleAction.ActionLocationEnum.NoAuto |
                            ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                SaveReturnUrl = true
            };
        }
    }
}