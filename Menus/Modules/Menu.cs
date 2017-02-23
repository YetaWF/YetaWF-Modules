/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.Menus.DataProvider;
using YetaWF.Modules.Menus.Views.Shared;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Modules {

    public class MenuModuleDataProvider : ModuleDefinitionDataProvider<Guid, MenuModule>, IInstallableModel { }

    [ModuleGuid("{51E5EB91-56CF-4ad6-A0D9-5C084FFD5D3F}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class MenuModule : ModuleDefinition, IModuleMenu {

        public MenuModule() {
            Name = this.__ResStr("modName", "Menu");
            Description = this.__ResStr("modSummary", "Simple and main page menus");
#pragma warning disable 0618 // Type or member is obsolete
            Menu = null;
#pragma warning restore 0618 // Type or member is obsolete
            MenuVersion = 0;
            Direction = MenuHelper.DirectionEnum.Bottom;
            Orientation = MenuHelper.OrientationEnum.Horizontal;
            HoverDelay = 500;
            ShowTitle = false;
            WantSearch = false;
            WantFocus = false;
            ShowPath = false;
            //UseAnimation = true;
            //OpenAnimation = AnimationEnum.ExpandsDown;
            //OpenDuration = 300;
            //CloseAnimation = AnimationEnum.ExpandsUp;
            //CloseDuration = 300;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new MenuModuleDataProvider(); }

        [Data_Binary, CopyAttribute]
        [Obsolete("Do not use directly - use GetMenu()/SaveMenu() instead - preserved for data conversion (pre 1.1.1)")]
        public MenuList Menu { get; set; }

        [Data_NewValue("(0)")]
        public long MenuVersion { get; set; }

        public void NewMenuVersion() { MenuVersion = MenuVersion+1; }

        public MenuList GetMenu() {
            using (MenuInfoDataProvider menuInfoDP = new MenuInfoDataProvider()) {
#pragma warning disable 0618 // Type or member is obsolete
                MenuList menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
                if (menu != null) {
                    SaveMenu(menu); // Legacy: the menu was saved as part of module definition, move it to MenuInfoDataProvider
                } else {
                    MenuInfo menuInfo = menuInfoDP.GetItem(ModuleGuid);
                    if (menuInfo != null)
                        menu = menuInfo.Menu;
                    else
                        menu = new MenuList();
                }
                return menu;
            }
        }
        public void SaveMenu(MenuList newMenu) {
            using (MenuInfoDataProvider menuInfoDP = new MenuInfoDataProvider()) {
#pragma warning disable 0618 // Type or member is obsolete
                MenuList menu = Menu;
#pragma warning restore 0618 // Type or member is obsolete
                menuInfoDP.ReplaceItem(new MenuInfo {
                    ModuleGuid = ModuleGuid,
                    Menu = newMenu,
                });
                // get a fresh copy of the module definitions
                MenuModule menuMod = (MenuModule)ModuleDefinition.Load(ModuleGuid);
                if (menuMod == null)
                    throw new InternalError("Menu module {0} was deleted", ModuleGuid);
                menuMod.NewMenuVersion();
                this.MenuVersion = menuMod.MenuVersion;
#pragma warning disable 0618 // Type or member is obsolete
                // the menu was saved as part of module definition, move it to MenuInfoDataProvider
                this.Menu = menuMod.Menu = null;// Legacy: clear menu saved as part of module definition
#pragma warning restore 0618 // Type or member is obsolete
                menuMod.Save();// save module definition (without menu)
            }
        }

        [Category("General"), Caption("Edit Url"), Description("The Url used to edit this menu - If omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        [Category("General"), Caption("Opening Direction"), Description("The direction in which submenus open - Ignored for Bootstrap menus")]
        [UIHint("Enum")]
        public MenuHelper.DirectionEnum Direction { get; set; }

        [Category("General"), Caption("Orientation"), Description("The basic orientation of the menu - Ignored for Bootstrap menus")]
        [UIHint("Enum")]
        public MenuHelper.OrientationEnum Orientation { get; set; }

        [Category("General"), Caption("Hover Delay"), Description("Specifies the delay (in milliseconds) before the menu is opened/closed - Used to avoid accidental closure on leaving - Ignored for Bootstrap menus")]
        [UIHint("IntValue4"), Required, Range(0, 10000)]
        public int HoverDelay { get; set; }

        [Category("General"), Caption("Show Path"), Description("Mark entries that partially or completely match the current page's Url as active (highlight)")]
        [UIHint("Boolean")]
        public bool ShowPath { get; set; }

        //[Category("General"), Caption("Use Animations"), Description("Specifies whether open/close animations are used - if this is not selected, animation properties are ignored")]
        //[UIHint("Boolean")]
        //public bool UseAnimation { get; set; }

        //[Category("General"), Caption("Open Animation"), Description("The animation used to open a submenu")]
        //[UIHint("Enum")]
        //public AnimationEnum OpenAnimation { get; set; }

        //[Category("General"), Caption("Open Effect Duration"), Description("Specifies the duration of the open effect (in milliseconds) as a submenu is opened")]
        //[UIHint("IntValue4"), Required, Range(0, 1000)]
        //public int OpenDuration { get; set; }

        //[Category("General"), Caption("Close Animation"), Description("The animation used to close a submenu")]
        //[UIHint("Enum")]
        //public AnimationEnum CloseAnimation { get; set; }

        //[Category("General"), Caption("Close Effect Duration"), Description("Specifies the duration of the close effect (in milliseconds) as a submenu is closed")]
        //[UIHint("IntValue4"), Required, Range(0, 1000)]
        //public int CloseDuration { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            MenuEditModule mod = new MenuEditModule();
            menuList.New(mod.GetAction_Edit(EditUrl, ModuleGuid), location);
            return menuList;
        }
    }
}