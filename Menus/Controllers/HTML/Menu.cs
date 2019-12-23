/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Core.Pages;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Controllers {

    public class MainMenuModuleController : MenuModuleController {

        [AllowGet]
        public async Task<ActionResult> MainMenu() {
            // add some bootstrap specific classes
            if (Manager.SkinInfo.UsingBootstrap)
                Module.CssClass = CssManager.CombineCss(Module.CssClass, "navbar-collapse collapse");
            MenuModel model = new MenuModel {
                Menu = new MenuComponentBase.MenuData {
                    MenuList = await GetEditMenu(Module),
                    Direction = Module.Direction,
                    Orientation = Module.Orientation,
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    ShowPath = Module.ShowPath,
                },
            };
            model.Menu.MenuList.LICssClass = Module.LICssClass;
            return View(model);
        }
    }

    public class MenuModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuModule> {

        public class MenuModel {
            [UIHint("Menu"), AdditionalMetadata("Style", MenuComponentBase.MenuStyleEnum.Automatic)]
            public MenuComponentBase.MenuData Menu { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> Menu() {
            MenuModel model = new MenuModel {
                Menu = new MenuComponentBase.MenuData {
                    MenuList = await GetEditMenu(Module),
                    Direction = Module.Direction,
                    Orientation = Module.Orientation,
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    ShowPath = Module.ShowPath,
                },
            };
            model.Menu.MenuList.LICssClass = Module.LICssClass;
            return View(model);
        }

        // Even with caching the main menu is a performance hit due to deserialization (turns out our Simple format is faster than JSON).
        // Researched caching HTML which doesn't work due to missing side effect like CSS loading (megamenu) when menu is not rendered.
        // Fortunately there is no performance hit when navigating within a UPS (SPA) or for static pages.
        // The first full page load is also not a problem. It's mainly an issue when opening an additional tab in the browser (e.g., duplicate tab)
        // or stress testing the site with full page loads.

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
        protected async Task<MenuList> GetEditMenu(MenuModule module) {
            MenuList.SavedCacheInfo info = MenuList.GetCache(module.ModuleGuid);
            if (info == null || info.EditMode != Manager.EditMode || info.UserId != Manager.UserId || info.MenuVersion != Module.MenuVersion) {
                info = new MenuList.SavedCacheInfo {
                    EditMode = Manager.EditMode,
                    UserId = Manager.UserId,
                    Menu = await (await Module.GetMenuAsync()).GetUserMenuAsync(),
                    MenuVersion = Module.MenuVersion,
                };
                MenuList.SetCache(module.ModuleGuid, info);
            }
            return info.Menu;
        }
    }
}