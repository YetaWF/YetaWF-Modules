/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Identity;
using YetaWF.Core.Addons;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class ModuleControlModuleController : ControllerImpl<YetaWF.Modules.ModuleEdit.Modules.ModuleControlModule> {

        public class ModuleControlModel { }

        // Move a module up
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MoveUp(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveUp(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload();
        }
        // Move a module down
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MoveDown(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveDown(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload();
        }
        // Move a module to top
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MoveTop(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveTop(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload();
        }
        // Move a module to bottom
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MoveBottom(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveBottom(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload();
        }

        // Move a module to another pane
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> MoveToPane(Guid pageGuid, Guid moduleGuid, string oldPane, string newPane) {
            if (pageGuid == Guid.Empty || moduleGuid == Guid.Empty || oldPane == null || newPane == null)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.MoveToPane(oldPane, moduleGuid, newPane);
            await page.SaveAsync();
            return Reload();
        }

        // Remove a module from a page
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.Remove(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            return Reload();
        }
        // Remove a module from page and permanently
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemovePermanent(Guid pageGuid, Guid moduleGuid, string pane, int moduleIndex = -1) {
            if (pageGuid == Guid.Empty || pane == null || moduleIndex == -1)
                throw new ArgumentException();
            PageDefinition page = await LoadPageAsync(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            page.ModuleDefinitions.Remove(pane, moduleGuid, moduleIndex);
            await page.SaveAsync();
            await YetaWF.Core.IO.Module.RemoveModuleDefinitionAsync(moduleGuid);
            return Reload();
        }

        private async Task<PageDefinition> LoadPageAsync(Guid pageGuid) {
            PageDefinition? page = await PageDefinition.LoadAsync(pageGuid);
            if (page == null)
                throw new Error(this.__ResStr("pageNotFound", "Page {0} doesn't exist"), pageGuid.ToString());
            return page;
        }

        [ResourceAuthorize(CoreInfo.Resource_ModuleExport)]
        public async Task<ActionResult> ExportModuleData(Guid moduleGuid, long cookieToReturn) {
            ModuleDefinition mod = await ModuleDefinition.LoadAsync(moduleGuid) ?? throw new InternalError($"{nameof(ExportModuleData)} called for module {moduleGuid} which doesn't exist");
            YetaWFZipFile zipFile = await mod.ExportDataAsync();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }
    }
}