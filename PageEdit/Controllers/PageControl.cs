/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Upload;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.PageEdit.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEdit.Controllers {

    public class PageControlModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.PageControlModule> {

        public enum Location {
            [EnumDescription("Top", "The new module is added at the beginning of the pane")]
            Top,
            [EnumDescription("Bottom", "The new module is added at the end of the pane")]
            Bottom
        }

        [Trim]
        public class AddNewModuleModel {

            [UIHint("Hidden")]
            public Guid CurrentPageGuid { get; set; }

            [Caption("Name"), Description("The name of the new module to be added - the module name uniquely identifies a module")]
            [UIHint("Text40"), StringLength(ModuleDefinition.MaxName), Required, Trim]
            public string ModuleName { get; set; }

            [Caption("Title"), Description("The module title, which appears at the top of the module as its title")]
            [UIHint("MultiString40"), StringLength(ModuleDefinition.MaxTitle), Required, Trim]
            public MultiString ModuleTitle { get; set; }

            [Caption("New Module"), Description("The new module to be added")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", true), Required]
            public Guid SelectedModule { get; set; }

            [Caption("Pane"), Description("The pane where the new module is added - in Edit Mode, all panes are visible, even empty panes")]
            [UIHint("PaneSelection"), Required]
            public string SelectedPane { get; set; }
            public List<string> SelectedPane_List { get; set; }

            [Caption("Location"), Description("The location within the selected pane where the new module is added")]
            [UIHint("Enum"), Required, Trim]
            public Location ModuleLocation { get; set; }

            public AddNewModuleModel() {
                ModuleTitle = new MultiString();
            }
            public void AddData(PageDefinition page) {
                if (page != null) {
                    SelectedPane_List = page.Panes;
                }
            }
        }

        [Trim]
        public class AddExistingModel {

            [UIHint("Hidden")]
            public Guid CurrentPageGuid { get; set; }

            [Caption("Existing Module"), Description("The name of the existing module to be added")]
            [UIHint("ModuleSelection"), AdditionalMetadata("New", false), Required]
            public Guid ExistingModule { get; set; }

            [Caption("Pane"), Description("The pane where the existing module is added - in Edit Mode, all panes are visible, even empty panes")]
            [UIHint("PaneSelection"), Required]
            public string ExistingModulePane { get; set; }
            public List<string> ExistingModulePane_List { get; set; }

            [Caption("Location"), Description("The location within the selected pane where the module is added")]
            [UIHint("Enum"), Required]
            public Location ModuleLocation { get; set; }

            public void AddData(PageDefinition page) {
                if (page != null) {
                    ExistingModulePane_List = page.Panes;
                }
            }
        }

        [Trim]
        public class ImportModel {

            [UIHint("Hidden")]
            public Guid CurrentPageGuid { get; set; }

            [Caption("Pane"), Description("The pane where the new module is added - in Edit Mode, all panes are visible, even empty panes")]
            [UIHint("PaneSelection"), Required]
            public string ModulePane { get; set; }
            public List<string> ModulePane_List { get; set; }

            [Caption("Location"), Description("The location within the selected pane where the module is added")]
            [UIHint("Enum"), Required]
            public Location ModuleLocation { get; set; }

            [Caption("ZIP File"), Description("The ZIP file containing the module data to be imported (creates a new module) ")]
            [UIHint("FileUpload1"), Required]
            public FileUpload1 UploadFile { get; set; }

            public void AddData(PageDefinition page, PageControlModule mod) {
                UploadFile = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnImport", "Import Module Data..."),
                    SaveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), "ImportPackage", new { __ModuleGuid = mod.ModuleGuid }),
                    RemoveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), "RemovePackage", new { __ModuleGuid = mod.ModuleGuid }),
                    SerializeForm = true,
                };
                if (page != null) {
                    ModulePane_List = page.Panes;
                }
            }
        }

        public class AddNewPageModel {

            [UIHint("Hidden")]
            public Guid CurrentPageGuid { get; set; }

            [Caption("Copy Current Page"), Description("Use the current page settings for the new page - Leave Title and Description blank to copy those settings from the current page")]
            [UIHint("Boolean")]
            public bool CopyPage { get; set; }

            [Caption("Copy All Modules"), Description("If selected, copies all modules from the current page and adds them to the new page - Modules are copied by reference, meaning any modules on the new page are the same modules used on the original page and share the same contents - Modules on the template page portion of the current page are never copied")]
            [UIHint("Boolean")]
            public bool CopyModules { get; set; }

            [Caption("Url"), Description("The Url of the new page - local Urls start with / and do not include http:// or https://")]
            [UIHint("Text40"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlHelperEx.UrlTypeEnum.New), Required, Trim]
            public string Url { get; set; }

            [Caption("Title"), Description("The title of the new page - the page title is displayed by the web browser in its header")]
            [UIHint("MultiString"), StringLength(PageDefinition.MaxTitle), RequiredIf("CopyPage", false), Trim]
            public MultiString Title { get; set; }

            [Caption("Description"), Description("The page description - this description is used to document the purpose of the page")]
            [UIHint("MultiString"), StringLength(PageDefinition.MaxDescription), RequiredIf("CopyPage", false), Trim]
            public MultiString Description { get; set; }

            public AddNewPageModel() {
                Title = new MultiString();
                Description = new MultiString();
            }
        }

        public class PageControlModel {
            public AddNewModuleModel AddNewModel { get; set; }
            public AddExistingModel AddExistingModel { get; set; }
            public ImportModel ImportModel { get; set; }
            public AddNewPageModel AddNewPageModel { get; set; }
        }

        [AllowGet]
        public ActionResult PageControl() {
            Guid pageGuid = Guid.Empty;
            if (pageGuid == Guid.Empty) {
                if (Manager.CurrentPage == null)
                    pageGuid = new Guid(); // we're not on a designed page
                else
                    pageGuid = Manager.CurrentPage.PageGuid;
            }
            PageDefinition page = PageDefinition.Load(pageGuid);
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();

            PageControlModel model = new PageControlModel() {
                AddNewPageModel = new AddNewPageModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                AddNewModel = new AddNewModuleModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                AddExistingModel = new AddExistingModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                ImportModel = new ImportModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                }
            };
            model.AddNewModel.AddData(page);
            model.AddExistingModel.AddData(page);
            model.ImportModel.AddData(page, Module);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddNewPage_Partial(AddNewPageModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            PageDefinition basePage = null;
            if (model.CopyPage)
                basePage = PageDefinition.Load(model.CurrentPageGuid);

            string message;
            PageDefinition page = PageDefinition.CreateNewPage(model.Title, model.Description, model.Url, basePage, model.CopyModules, out message);
            if (page == null) {
                ModelState.AddModelError("Url", message);
                return PartialView(model);
            }
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();

            page.Save();
            return FormProcessed(model, this.__ResStr("okNewPage", "New page created"), NextPage: page.EvaluatedCanonicalUrl);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddNewModule_Partial(AddNewModuleModel model) {
            PageDefinition page = PageDefinition.Load(model.CurrentPageGuid);
            if (page == null)
                throw new Error("Can't edit this page");
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            model.AddData(page);
            if (!ModelState.IsValid)
                return PartialView(model);

            ModuleDefinition module = ModuleDefinition.CreateNewDesignedModule(model.SelectedModule, model.ModuleName, model.ModuleTitle);
            if (!module.IsModuleUnique)
                module.ModuleGuid = Guid.NewGuid();
            page.AddModule(model.SelectedPane, module, model.ModuleLocation == Location.Top);
            page.Save();
            return Reload(model, this.__ResStr("okNew", "New module added"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddExistingModule_Partial(AddExistingModel model) {
            PageDefinition page = PageDefinition.Load(model.CurrentPageGuid);
            if (page == null)
                throw new Error("Can't edit this page");
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            model.AddData(page);

            if (!ModelState.IsValid)
                return PartialView(model);

            ModuleDefinition module = ModuleDefinition.Load(model.ExistingModule);
            page.AddModule(model.ExistingModulePane, module, model.ModuleLocation == Location.Top);
            page.Save();
            return Reload(model, this.__ResStr("okExisting", "Module added"));
        }

        [AllowPost]
        [ExcludeDemoMode]
#if MVC6
        public ActionResult ImportPackage(IFormFile __filename, ImportModel model)
#else
        public ActionResult ImportPackage(HttpPostedFileBase __filename, ImportModel model)
#endif
        {
            FileUpload upload = new FileUpload();
            string tempName = upload.StoreTempPackageFile(__filename);

            List<string> errorList = new List<string>();
            bool success = ModuleDefinition.Import(tempName, model.CurrentPageGuid, true, model.ModulePane, model.ModuleLocation == Location.Top, errorList);
            upload.RemoveTempFile(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            ScriptBuilder sb = new ScriptBuilder();
            if (success) {
                // Upload control considers Json result a success
                sb.Append("{{ \"result\": \"Y_Confirm(\\\"{0}\\\", null, function() {{ window.location.reload(); }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(this.__ResStr("imported", "\"{0}\" successfully imported(+nl)", __filename.FileName) + errs))
                );
                return new YJsonResult { Data = sb.ToString() };
            } else {
                // Anything else is a failure
                sb.Append(this.__ResStr("cantImport", "Can't import {0}:(+nl)"), __filename.FileName);
                sb.Append(errs);
                throw new Error(sb.ToString());
            }
        }
        [AllowPost]
        [ExcludeDemoMode]
        public ActionResult RemovePackage(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }

        // if you have permission to view the pagecontrol module, you can switch modes
        public ActionResult SwitchToEdit() {
            Manager.EditMode = true;
            return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true);
        }

        public ActionResult SwitchToView() {
            Manager.EditMode = false;
            return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true);
        }
    }
}