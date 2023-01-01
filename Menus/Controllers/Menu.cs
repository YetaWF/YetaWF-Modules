/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menus.Controllers {

    public class MainMenuModuleController : MenuModuleController {

        [AllowGet]
        public async Task<ActionResult> MainMenu() {
            MenuModel model = new MenuModel {
                Menu = new MenuComponentBase.MenuData {
                    MenuList = await GetMenu(Module, External: true),
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    SmallMenuMaxWidth = Module.SmallMenuMaxWidth
                },
            };
            model.Menu.MenuList.LICssClass = Module.LICssClass;
            return View(model);
        }
    }

    public class MenuModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuModule> {

        public class MenuModel {
            [UIHint("Menu")]
            public MenuComponentBase.MenuData Menu { get; set; } = null!;
        }

        [AllowGet]
        public async Task<ActionResult> Menu() {
            MenuModel model = new MenuModel {
                Menu = new MenuComponentBase.MenuData {
                    MenuList = await GetMenu(Module),
                    HoverDelay = Module.HoverDelay,
                    CssClass = Module.CssClass,
                    SmallMenuMaxWidth = Module.SmallMenuMaxWidth
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
        protected async Task<MenuList> GetMenu(MenuModule module, bool External = false) {
            MenuList.SavedCacheInfo? info = MenuList.GetCache(module.ModuleGuid);
            if (info == null || info.EditMode != Manager.EditMode || info.UserId != Manager.UserId || info.MenuVersion != Module.MenuVersion) {
                info = new MenuList.SavedCacheInfo {
                    EditMode = Manager.EditMode,
                    UserId = Manager.UserId,
                    Menu = await (await Module.GetMenuAsync()).GetUserMenuAsync(),
                    MenuVersion = Module.MenuVersion,
                };

                // Add optional external links (this is a custom hack, not an official feature)
                if (External)
                    await AddExternalLinksAsync(info.Menu);

                MenuList.SetCache(module.ModuleGuid, info);
            }
            return info.Menu;
        }
        private async Task AddExternalLinksAsync(List<ModuleAction> origList) {
            if (ExternalList == null) {
                List<ModuleAction> list = new List<ModuleAction>();
                string? listFile = WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "ExternalList");
                if (!string.IsNullOrWhiteSpace(listFile)) {
                    List<string> lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(listFile);
                    foreach (string line in lines) {
                        if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#")) {
                            string[] parts = line.Trim().Split(new char[] { ',' }, 2);
                            if (parts.Length != 2)
                                throw new InternalError($"File {listFile} has an invalid menu entry - {line}");
                            string menu = parts[0].Trim();
                            string url = parts[1].Trim();
                            list.Add(new ModuleAction {
                                LinkText = menu,
                                MenuText = menu,
                                Mode = ModuleAction.ActionModeEnum.Any,
                                Url = url,
                                Style = ModuleAction.ActionStyleEnum.NewWindow,
                            });
                        }
                    }
                }
                ExternalList = list;
            }
            origList.AddRange(ExternalList);
        }
        private static List<ModuleAction>? ExternalList { get; set; }
    }
}