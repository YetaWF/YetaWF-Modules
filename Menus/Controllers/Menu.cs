/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Views.Shared;

namespace YetaWF.Modules.Menus.Controllers {

    public class MainMenuModuleController : MenuModuleController {
        [HttpGet]
        public ActionResult MainMenu() {
            MenuModel model = new MenuModel {
                Menu = new MenuHelper.MenuData {
                    MenuList = GetEditMenu(Manager, Module.Menu, Module.ModuleGuid),
                    Direction = Module.Direction,
                    Orientation = Module.Orientation,
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    ShowPath = Module.ShowPath,
                },
            };
            return View(model);
        }
    }

    public class MenuModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuModule> {

        public class MenuModel {
            [UIHint("YetaWF_Menus_Menu")]
            public MenuHelper.MenuData Menu { get; set; }
        }

        [HttpGet]
        public ActionResult Menu() {
            MenuModel model = new MenuModel {
                Menu = new MenuHelper.MenuData {
                    MenuList = GetEditMenu(Manager, Module.Menu, Module.ModuleGuid),
                    Direction = Module.Direction,
                    Orientation = Module.Orientation,
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    ShowPath = Module.ShowPath,
                },
            };
            return View(model);
        }

        public static MenuList GetEditMenu(YetaWFManager manager, MenuList menu, Guid moduleGuid) {
            // get the fully evaluated menu for the current user
            // we cache the menu in session settings because it is a costly operation to determine 
            // which entries are authorized
            // when we switch between edit/view mode or when the user logs on/off or when the menu has been changed by an admin we'll re-evaluate
            MenuList.SavedCacheInfo info = MenuList.GetCache(moduleGuid);
            if (info == null || info.EditMode != manager.EditMode || info.UserId != manager.UserId || info.Menu.Version != menu.Version) {
                info = new MenuList.SavedCacheInfo {
                    EditMode = manager.EditMode,
                    UserId = manager.UserId,
                    Menu = menu.GetUserMenu()
                };
                MenuList.SetCache(moduleGuid, info);
            }
            return info.Menu;
        }
    }
}