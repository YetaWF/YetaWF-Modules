﻿/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;
using YetaWF.Modules.Menus.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Controllers {

    public class MainMenuModuleController : MenuModuleController {

        [HttpGet]
        public ActionResult MainMenu() {
            // add some bootstrap specific classes
            if (Manager.UsingBootstrap)
                Module.CssClass = YetaWFManager.CombineCss(Module.CssClass, "navbar-collapse collapse");
            MenuModel model = new MenuModel {
                Menu = new MenuHelper.MenuData {
                    MenuList = GetEditMenu(Module),
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
            [UIHint("YetaWF_Menus_Menu"), AdditionalMetadata("Style", MenuHelper.MenuStyleEnum.Automatic)]
            public MenuHelper.MenuData Menu { get; set; }
        }

        [HttpGet]
        public ActionResult Menu() {
            MenuModel model = new MenuModel {
                Menu = new MenuHelper.MenuData {
                    MenuList = GetEditMenu(Module),
                    Direction = Module.Direction,
                    Orientation = Module.Orientation,
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    ShowPath = Module.ShowPath,
                },
            };
            return View(model);
        }

        /// <summary>
        /// Builds the menu for the current user based on all available authorizations.
        /// </summary>
        /// <param name="menu">The entire menu (includes entries that will be removed if they're not available/permitted for the current user.</param>
        /// <param name="moduleGuid">The module that owns the menu. Used as cache key.</param>
        /// <returns>A copy of the menu reduced to just the entries that are available/permitted for the current user.</returns>
        /// <remarks>
        /// The menu is cached in session settings because it is a costly operation to determine permissions for all entries.
        /// The full menu is only evaluated when switching between edit/view mode, when the user logs on/off or when the menu contents have changed.
        /// </remarks>
        protected MenuList GetEditMenu(MenuModule module) {
            MenuList.SavedCacheInfo info = MenuList.GetCache(module.ModuleGuid);
            if (info == null || info.EditMode != Manager.EditMode || info.UserId != Manager.UserId || info.MenuVersion != Module.MenuVersion) {
                info = new MenuList.SavedCacheInfo {
                    EditMode = Manager.EditMode,
                    UserId = Manager.UserId,
                    Menu = Module.GetMenu().GetUserMenu(),
                    MenuVersion = Module.MenuVersion,
                };
                MenuList.SetCache(module.ModuleGuid, info);
            }
            return info.Menu;
        }
    }
}