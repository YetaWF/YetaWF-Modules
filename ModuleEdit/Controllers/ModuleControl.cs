/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class ModuleControlModuleController : ControllerImpl<YetaWF.Modules.ModuleEdit.Modules.ModuleControlModule> {

        public class ModuleControlModel { }

        // Move a module up
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MoveUp(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveUp(pane, moduleGuid, moduleIndex);
            page.Save();
            return Reload();
        }
        // Move a module down
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MoveDown(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveDown(pane, moduleGuid, moduleIndex);
            page.Save();
            return Reload();
        }
        // Move a module to top
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MoveTop(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveTop(pane, moduleGuid, moduleIndex);
            page.Save();
            return Reload();
        }
        // Move a module to bottom
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MoveBottom(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveBottom(pane, moduleGuid, moduleIndex);
            page.Save();
            return Reload();
        }

        // Move a module to another pane
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult MoveToPane(Guid pageGuid, Guid moduleGuid, string oldPane, string newPane) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || oldPane == null || newPane == null)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveToPane(oldPane, moduleGuid, newPane);
            page.Save();
            return Reload();
        }

        // Remove a module from a page
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult Remove(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = LoadPage(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.Remove(pane, moduleGuid, moduleIndex);
            page.Save();
            return Reload();
        }

        private PageDefinition LoadPage(Guid pageGuid) {
            PageDefinition page = PageDefinition.Load(pageGuid);
            if (page == null)
                throw new Error(this.__ResStr("pageNotFound", "Page {0} doesn't exist"), pageGuid.ToString());
            return page;
        }

        [Permission("Exports")]
        public ActionResult ExportModuleData(Guid moduleGuid, long cookieToReturn) {
            ModuleDefinition mod = ModuleDefinition.Load(moduleGuid);
            YetaWFZipFile zipFile = mod.ExportData();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }
    }
}