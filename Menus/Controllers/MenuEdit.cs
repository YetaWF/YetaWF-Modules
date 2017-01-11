/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Menus.Modules;

namespace YetaWF.Modules.Menus.Controllers {

    public class MenuEditModuleController : ControllerImpl<YetaWF.Modules.Menus.Modules.MenuEditModule> {

        public class MenuEditModel {
            public string NewEntryJSON { get; set; } // JSON for one new menu entry (used client-side)
            public string MenuJSON { get; set; } // this is the JSON string we send to the client for the entire menu

            [UIHint("Hidden")]
            public Guid MenuGuid { get; set; }
            [UIHint("Hidden")]
            public Guid MenuVersion { get; set; }
            [UIHint("Hidden")]
            public int ActiveEntry { get; set; } // this is the active entry being edited (0 if new)
            [UIHint("Hidden")]
            public int NewAfter { get; set; } // new entry is added after this Id as child item (0 if not a new item)

            [UIHint("PropertyList")]
            public ModuleAction ModAction { get; set; }
        }

        [HttpGet]
        public ActionResult MenuEdit(Guid menuGuid) {
            MenuModule modMenu = (MenuModule) ModuleDefinition.Load(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);

            MenuList newMenu = new MenuList {
                new ModuleAction(Module) {
                     MenuText = this.__ResStr("menuRoot", "Menu"),
                     SubMenu =  modMenu.Menu,
                }
            };
            MenuEditModel model = new MenuEditModel {
                MenuJSON = newMenu.SerializeToJSON(),
                NewEntryJSON = YetaWFManager.Jser.Serialize(new ModuleAction(Module) { Url = this.__ResStr("newUrl", "(new)") }),

                MenuGuid = menuGuid,
                ModAction = new ModuleAction(Module),
                MenuVersion = modMenu.Menu.Version,
                ActiveEntry = 0,
                NewAfter = 0,
            };
            return View(model);
        }

        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MenuEdit_Partial([Bind(Include = "MenuGuid,ModAction,MenuVersion,ActiveEntry,NewAfter")]MenuEditModel model, bool ValidateCurrent) {
            MenuModule modMenu = (MenuModule) ModuleDefinition.Load(model.MenuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", model.MenuGuid);

            if (model.MenuVersion != modMenu.Menu.Version)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved"));

            if (!ValidateCurrent)
                ModelState.Clear();
            if (!ModelState.IsValid)
                return PartialView(model);

            MenuList menu = modMenu.Menu;
            menu.MergeNewAction(model.ActiveEntry, model.NewAfter, model.ModAction);
            model.MenuVersion = Guid.NewGuid();// force a new version
            modMenu.Menu.Version = model.MenuVersion;
            modMenu.Save();

            return PartialView(model);
        }

        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult EntireMenu(string entireMenu, Guid menuGuid, Guid menuVersion) {
            MenuModule modMenu = (MenuModule) ModuleDefinition.Load(menuGuid);
            if (modMenu == null)
                throw new InternalError("Can't find menu module {0}", menuGuid);

            if (menuVersion != modMenu.Menu.Version)
                throw new Error(this.__ResStr("menuChanged", "The menu has been changed by someone else - Your changes can't be saved"));

            MenuList menu = MenuList.DeserializeFromJSON(entireMenu, Original: modMenu.Menu);
            modMenu.Menu = new MenuList(menu[0].SubMenu);
            modMenu.Menu.NewVersion();
            modMenu.Save();

            return new JsonResult() { Data = modMenu.Menu.Version };
        }
    }
}