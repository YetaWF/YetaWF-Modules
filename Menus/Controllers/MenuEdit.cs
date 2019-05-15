/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Menus.Controllers {

    public class MenuEditModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuEditModule> {

        public class MenuEditModel {

            public ModuleAction NewEntry { get; set; } // one new menu entry (used client-side)

            [UIHint("Tree")]
            public MenuList Menu { get; set; } // this is the JSON string we send to the client for the entire menu
            public TreeDefinition Menu_TreeDefinition { get; set; }

            [UIHint("Hidden")]
            public Guid MenuGuid { get; set; }
            [UIHint("Hidden")]
            public long MenuVersion { get; set; }

            [UIHint("PropertyList")]
            public ModuleAction ModAction { get; set; }

            public MenuEditModel() {
                Menu_TreeDefinition = new TreeDefinition {
                    RecordType = typeof(ModuleAction),
                    ShowHeader = false,
                    DragDrop = true,
                    UseSkinFormatting = true,
                };
            }
        }

        [AllowGet]
        public async Task<ActionResult> MenuEdit(Guid menuGuid) {
            MenuModule modMenu = (MenuModule) await ModuleDefinition.LoadAsync(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);

            MenuList origMenu = await modMenu.GetMenuAsync();

            MenuEditModel model = new MenuEditModel {
                Menu = origMenu,
                NewEntry = new ModuleAction(Module) { Url = "" },

                MenuGuid = menuGuid,
                ModAction = new ModuleAction(Module),
                MenuVersion = modMenu.MenuVersion,
            };
            return View(model);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MenuEdit_Partial(MenuEditModel model, bool ValidateCurrent) {

            MenuModule modMenu = (MenuModule) await ModuleDefinition.LoadAsync(model.MenuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", model.MenuGuid);

            if (model.MenuVersion != modMenu.MenuVersion)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved - Please refresh the current page before proceeding"));

            if (!ValidateCurrent)
                ModelState.Clear();
            if (!ModelState.IsValid)
                return PartialView(model);

            return PartialView(model);
        }

        public class EntireMenuResult {
            public long NewVersion { get; set; }
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> EntireMenu(string entireMenu, Guid menuGuid, long menuVersion) {
            MenuModule modMenu = (MenuModule) await ModuleDefinition.LoadAsync(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);
            if (menuVersion != modMenu.MenuVersion)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved - Please refresh the current page before proceeding"));

            MenuList origMenu = await modMenu.GetMenuAsync();
            MenuList menu = MenuList.DeserializeFromJSON(entireMenu, Original: origMenu);
            await modMenu.SaveMenuAsync(menu);

            return new YJsonResult() {
                Data = new EntireMenuResult {
                    NewVersion = modMenu.MenuVersion
                }
            };
        }
    }
}