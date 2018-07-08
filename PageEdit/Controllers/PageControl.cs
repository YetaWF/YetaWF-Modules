/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Upload;
using YetaWF.Modules.PageEdit.DataProvider;
using YetaWF.Modules.PageEdit.Modules;
using YetaWF.Core.IO;
using YetaWF.Core.Components;
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
            public async Task AddDataAsync(PageDefinition page) {
                if (page != null) {
                    SelectedPane_List = await page.GetPanesAsync();
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

            public async Task AddDataAsync(PageDefinition page) {
                if (page != null) {
                    ExistingModulePane_List = await page.GetPanesAsync();
                }
            }
        }

        [Trim]
        public class ImportPageModel {

            [UIHint("Hidden")]
            public Guid CurrentPageGuid { get; set; }

            [Caption("ZIP File"), Description("The ZIP file containing the page data to be imported - The page cannot already exist - Modules that are already present are not updated while importing the page")]
            [UIHint("FileUpload1"), Required]
            public FileUpload1 UploadFile { get; set; }

            public void AddData(PageDefinition page, PageControlModule mod) {
                UploadFile = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnImportPage", "Import Page Data..."),
                    SaveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ImportPage), new { __ModuleGuid = mod.ModuleGuid }),
                    RemoveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.RemovePage), new { __ModuleGuid = mod.ModuleGuid }),
                    SerializeForm = true,
                };
            }
        }

        [Trim]
        public class ImportModuleModel {

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

            public async Task AddDataAsync(PageDefinition page, PageControlModule mod) {
                UploadFile = new FileUpload1 {
                    SelectButtonText = this.__ResStr("btnImport", "Import Module Data..."),
                    SaveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.ImportModule), new { __ModuleGuid = mod.ModuleGuid }),
                    RemoveURL = YetaWFManager.UrlFor(typeof(PageControlModuleController), nameof(PageControlModuleController.RemoveModule), new { __ModuleGuid = mod.ModuleGuid }),
                    SerializeForm = true,
                };
                if (page != null) {
                    ModulePane_List = await page.GetPanesAsync();
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
            [UIHint("Text40"), StringLength(Globals.MaxUrl), UrlValidation(urlType: UrlTypeEnum.New), Required, Trim]
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
        public class SkinSelectionModel {
            [Category("Skin"), Caption("Default Bootstrap Skin"), Description("The default skin for overall page appearance and Bootstrap elements - individual pages can override the default skin")]
            [HelpLink("https://www.bootstrapcdn.com/bootswatch/")]
            [UIHint("BootstrapSkin"), StringLength(SkinDefinition.MaxName), AdditionalMetadata("NoDefault", true), Trim]
            public string BootstrapSkin { get; set; }

            [Category("Skin"), Caption("Default jQuery UI Skin"), Description("The default skin for jQuery-UI elements (buttons, modal dialogs, etc.) - individual pages can override the default skin")]
            [HelpLink("http://jqueryui.com/themeroller/")]
            [UIHint("jQueryUISkin"), StringLength(SkinDefinition.MaxName), AdditionalMetadata("NoDefault", true), Trim]
            public string jQueryUISkin { get; set; }

            [Category("Skin"), Caption("Default Kendo UI Skin"), Description("The default skin for Kendo UI elements (buttons, modal dialogs, etc.) - individual pages can override the default skin")]
            [HelpLink("http://demos.telerik.com/kendo-ui/themebuilder/")]
            [UIHint("KendoUISkin"), StringLength(SkinDefinition.MaxName), AdditionalMetadata("NoDefault", true), Trim]
            public string KendoUISkin { get; set; }

            public SkinSelectionModel() {
                BootstrapSkin = Manager.CurrentSite.BootstrapSkin;
                jQueryUISkin = Manager.CurrentSite.jQueryUISkin;
                KendoUISkin = Manager.CurrentSite.KendoUISkin;
            }
        }
        [Trim]
        public class LoginSiteSelectionModel {

            [Caption("Active Site"), Description("List of sites that can be accessed - select an entry to visit the site")]
            [UIHint("DropDownList"), SubmitFormOnChange]
            public string SiteDomain { get; set; }

            public List<SelectionItem<string>> SiteDomain_List { get; set; }

            [Caption("Active User"), Description("List of user accounts that can be used to quickly log into the site - select an entry to log in as that user")]
            [UIHint("YetaWF_Identity_LoginUsers"), SubmitFormOnChange]
            public int UserId { get; set; }

            [Caption("Superuser"), Description("If a superuser was signed on previously in this session, the superuser status remains even if logged in as another user - Uncheck to turn off superuser mode for this session")]
            [UIHint("Boolean"), SuppressIfNotEqual("SuperuserStillActive", true), SubmitFormOnChange]
            public bool? SuperuserStillActive { get; set; }

            [Caption("Superuser"), Description("The currently logged on user is a superuser")]
            [UIHint("Boolean"), SuppressIfEqual("SuperuserCurrent", false), ReadOnly]
            public bool SuperuserCurrent { get; set; }

            public SerializableList<User> UserId_List { get; set; }

            public LoginSiteSelectionModel() { }

            public async Task AddDataAsync() {

                DataProviderGetRecords<SiteDefinition> info = await SiteDefinition.GetSitesAsync(0, 0, null, null);
                SiteDomain_List = (from s in info.Data orderby s.SiteDomain select new SelectionItem<string>() {
                    Text = s.SiteDomain,
                    Value = s.SiteDomain,
                    Tooltip = this.__ResStr("switchSite", "Switch to site \"{0}\"", s.SiteDomain),
                }).ToList();
                SiteDomain = Manager.CurrentSite.SiteDomain;

                ControlPanelConfigData config = await ControlPanelConfigDataProvider.GetConfigAsync();
                UserId_List = config.Users;
                UserId = Manager.UserId;

                List<int> list = Manager.UserRoles;
                int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
                if (Manager.UserRoles != null && Manager.UserRoles.Contains(superuserRole))
                    SuperuserCurrent = true;// the current user is a superuser
                else if (Manager.HasSuperUserRole)
                    SuperuserStillActive = true;
                else
                    SuperuserStillActive = false;
            }
        }

        public class PageControlModel {
            public bool EditAuthorized { get; set; }
            public AddNewModuleModel AddNewModel { get; set; }
            public AddExistingModel AddExistingModel { get; set; }
            public ImportPageModel ImportPageModel { get; set; }
            public ImportModuleModel ImportModuleModel { get; set; }
            public AddNewPageModel AddNewPageModel { get; set; }
            public SkinSelectionModel SkinSelectionModel { get; set; }
            public LoginSiteSelectionModel LoginSiteSelectionModel { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> PageControl() {
            Guid pageGuid = Guid.Empty;
            if (pageGuid == Guid.Empty) {
                if (Manager.CurrentPage == null)
                    pageGuid = new Guid(); // we're not on a designed page
                else
                    pageGuid = Manager.CurrentPage.PageGuid;
            }

            PageDefinition page = await PageDefinition.LoadAsync(pageGuid);
            bool editAuthorized = false;
            if (page.IsAuthorized_Edit())
                editAuthorized = true;

            PageControlModel model = new PageControlModel() {
                EditAuthorized = editAuthorized,
                AddNewPageModel = new AddNewPageModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                AddNewModel = new AddNewModuleModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                AddExistingModel = new AddExistingModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                ImportPageModel = new ImportPageModel(),
                ImportModuleModel = new ImportModuleModel() {
                    CurrentPageGuid = Manager.CurrentPage.PageGuid,
                },
                SkinSelectionModel = new SkinSelectionModel(),
                LoginSiteSelectionModel = new LoginSiteSelectionModel(),
            };
            await model.AddNewModel.AddDataAsync(page);
            await model.AddExistingModel.AddDataAsync(page);
            model.ImportPageModel.AddData(page, Module);
            await model.ImportModuleModel.AddDataAsync(page, Module);
            await model.LoginSiteSelectionModel.AddDataAsync();
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddNewPage_Partial(AddNewPageModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);

            PageDefinition basePage = null;
            if (model.CopyPage)
                basePage = await PageDefinition.LoadAsync(model.CurrentPageGuid);

            PageDefinition.NewPageInfo newPage = await PageDefinition.CreateNewPageAsync(model.Title, model.Description, model.Url, basePage, model.CopyModules);
            PageDefinition page = newPage.Page;
            if (page == null) {
                ModelState.AddModelError("Url", newPage.Message);
                return PartialView(model);
            }
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();

            await page.SaveAsync();
            return FormProcessed(model, this.__ResStr("okNewPage", "New page created"), NextPage: page.EvaluatedCanonicalUrl);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddNewModule_Partial(AddNewModuleModel model) {
            PageDefinition page = await PageDefinition.LoadAsync(model.CurrentPageGuid);
            if (page == null)
                throw new Error("Can't edit this page");
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            await model.AddDataAsync(page);
            if (!ModelState.IsValid)
                return PartialView(model);

            ModuleDefinition module = ModuleDefinition.CreateNewDesignedModule(model.SelectedModule, model.ModuleName, model.ModuleTitle);
            if (!module.IsModuleUnique)
                module.ModuleGuid = Guid.NewGuid();
            page.AddModule(model.SelectedPane, module, model.ModuleLocation == Location.Top);
            await page.SaveAsync();
            return Reload(model, PopupText: this.__ResStr("okNew", "New module added"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddExistingModule_Partial(AddExistingModel model) {
            PageDefinition page = await PageDefinition.LoadAsync(model.CurrentPageGuid);
            if (page == null)
                throw new Error("Can't edit this page");
            if (!page.IsAuthorized_Edit())
                return NotAuthorized();
            await model.AddDataAsync(page);

            if (!ModelState.IsValid)
                return PartialView(model);

            ModuleDefinition module = await ModuleDefinition.LoadAsync(model.ExistingModule);
            page.AddModule(model.ExistingModulePane, module, model.ModuleLocation == Location.Top);
            await page.SaveAsync();
            return Reload(model, PopupText: this.__ResStr("okExisting", "Module added"));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SkinSelection_Partial(SkinSelectionModel model) {

            if (!ModelState.IsValid)
                return PartialView(model);

            SiteDefinition origSite = new SiteDefinition();
            ObjectSupport.CopyData(Manager.CurrentSite, origSite);// make a copy of original site
            SiteDefinition site = Manager.CurrentSite;// update new settings
            site.BootstrapSkin = model.BootstrapSkin;
            site.jQueryUISkin = model.jQueryUISkin;
            site.KendoUISkin = model.KendoUISkin;
            ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origSite, site);
            switch (modelDisp) {
                default:
                case ObjectSupport.ModelDisposition.None:
                    return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"));
                case ObjectSupport.ModelDisposition.PageReload:
                    await site.SaveAsync();
                    return FormProcessed(model, this.__ResStr("okSaved", "Site settings updated"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect: true);
                case ObjectSupport.ModelDisposition.SiteRestart:
                    await site.SaveAsync();
                    return FormProcessed(model, this.__ResStr("okSavedRestart", "Site settings updated - These settings won't take effect until the site is restarted"));
            }
        }

        [AllowPost]
        [ExcludeDemoMode]
#if MVC6
        public async Task<ActionResult> ImportModule(IFormFile __filename, ImportModuleModel model)
#else
        public async Task<ActionResult> ImportModule(HttpPostedFileBase __filename, ImportModuleModel model)
#endif
        {
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            bool success = await ModuleDefinition.ImportAsync(tempName, model.CurrentPageGuid, true, model.ModulePane, model.ModuleLocation == Location.Top, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            ScriptBuilder sb = new ScriptBuilder();
            if (success) {
                // Upload control considers Json result a success
                sb.Append("{{ \"result\": \"YetaWF_Basics.Y_Confirm(\\\"{0}\\\", null, function() {{ YetaWF_Basics.reloadPage(true); }} ); \" }}",
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
        public ActionResult RemoveModule(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }
        [AllowPost]
        [ExcludeDemoMode]
#if MVC6
        public async Task<ActionResult> ImportPage(IFormFile __filename, ImportPageModel model)
#else
        public async Task<ActionResult> ImportPage(HttpPostedFileBase __filename, ImportPageModel model)
#endif
        {
            FileUpload upload = new FileUpload();
            string tempName = await upload.StoreTempPackageFileAsync(__filename);

            List<string> errorList = new List<string>();
            PageDefinition.ImportInfo info = await PageDefinition.ImportAsync(tempName, errorList);
            await FileSystem.TempFileSystemProvider.DeleteFileAsync(tempName);

            string errs = "";
            if (errorList.Count > 0) {
                ScriptBuilder sbErr = new ScriptBuilder();
                sbErr.Append(errorList, LeadingNL: true);
                errs = sbErr.ToString();
            }
            ScriptBuilder sb = new ScriptBuilder();
            if (info.Success) {
                // Upload control considers Json result a success
                sb.Append("{{ \"result\": \"YetaWF_Basics.Y_Confirm(\\\"{0}\\\", null, function() {{ window.location.assign(\\\"{1}\\\"); }} ); \" }}",
                    YetaWFManager.JserEncode(YetaWFManager.JserEncode(this.__ResStr("imported", "\"{0}\" successfully imported(+nl)", __filename.FileName) + errs)),
                    YetaWFManager.JserEncode(info.Url)
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
        public ActionResult RemovePage(string filename) {
            // there is nothing to remove because we already imported the file
            return new EmptyResult();
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> LoginSiteSelection_Partial(LoginSiteSelectionModel model) {
            if (!ModelState.IsValid) {
                await model.AddDataAsync();
                return PartialView(model);
            }

            string nextPage;
            if (Manager.CurrentSite.SiteDomain != model.SiteDomain) {
                nextPage = Manager.CurrentSite.MakeFullUrl(RealDomain: model.SiteDomain, SecurityType: Core.Pages.PageDefinition.PageSecurityType.httpOnly);
            } else { /* if (model.UserId != Manager.UserId) */
                int userId = model.UserId;
                if (userId == 0)
                    await Resource.ResourceAccess.LogoffAsync();
                else
                    await Resource.ResourceAccess.LoginAsAsync(userId);
                if (model.SuperuserStillActive != null && !(bool)model.SuperuserStillActive)
                    Manager.SetSuperUserRole(false);
                nextPage = Manager.ReturnToUrl;
            }
            Manager.PageControlShown = false;
            return Redirect(nextPage, ForceRedirect: true, SetCurrentControlPanelMode: true);
        }

        // if you have permission to view the pagecontrol module, you can switch modes
        public ActionResult SwitchToEdit() {
            Manager.PageControlShown = false;
            Manager.EditMode = true;
            return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true, SetCurrentControlPanelMode: true);
        }

        public ActionResult SwitchToView() {
            Manager.PageControlShown = false;
            Manager.EditMode = false;
            return Redirect(Manager.ReturnToUrl, SetCurrentEditMode: true, SetCurrentControlPanelMode: true);
        }
        public async Task<ActionResult> ExportPage(Guid pageGuid, long cookieToReturn) {
            PageDefinition page = await PageDefinition.LoadAsync(pageGuid);
            YetaWFZipFile zipFile = await page.ExportAsync();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }
    }
}