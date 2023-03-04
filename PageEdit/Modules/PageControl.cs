/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.PageEdit.DataProvider;
using YetaWF.Modules.PageEdit.Endpoints;
using System.Linq;
using Microsoft.AspNetCore.Http;
using YetaWF.Modules.PageEdit.Views;

namespace YetaWF.Modules.PageEdit.Modules;

public class PageControlModuleDataProvider : ModuleDefinitionDataProvider<Guid, PageControlModule>, IInstallableModel { }

[ModuleGuid("{466C0CCA-3E63-43f3-8754-F4267767EED1}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
public class PageControlModule : ModuleDefinition2 {

    public PageControlModule() {
        Name = this.__ResStr("modName", "Control Panel (Skin)");
        Title = this.__ResStr("modTitle", "Control Panel");
        Description = this.__ResStr("modSummary", "Displays an icon opening/closing the Control Panel which supports adding new and existing modules to a page, supports importing a module and is used to create new pages, change page settings and remove the current page.");
        ShowTitleActions = false;
        ShowTitle = false;
        WantFocus = false;
        WantSearch = false;
        Print = false;
        Invokable = true;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PageControlModuleDataProvider(); }

    public override bool ShowModuleMenu { get { return false; } }
    public override bool ModuleHasSettings { get { return false; } }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> baseMenuList = await base.GetModuleMenuListAsync(renderMode, location);
        List<ModuleAction> menuList = new List<ModuleAction>();

        PageEditModule modEdit = new PageEditModule();
        menuList.New(await modEdit.GetAction_EditAsync(null), location);
        menuList.New(await GetAction_ExportPageAsync(null), location);
        menuList.New(await modEdit.GetAction_RemoveAsync(null), location);

        menuList.New(GetAction_SwitchToView(), location);
        menuList.New(GetAction_SwitchToEdit(), location);

        menuList.New(await GetAction_W3CValidationAsync(), location);
        menuList.New(await GetAction_RestartSite(), location);
        menuList.New(GetAction_ClearJsCssCache(), location);

        menuList.AddRange(baseMenuList);
        return menuList;
    }

    public async Task<ModuleAction?> GetAction_PageControlAsync() {
        return new ModuleAction(this) {
            Category = ModuleAction.ActionCategoryEnum.Significant,
            CssClass = "y_button_outline",
            Image = await new SkinImages().FindIcon_PackageAsync("PageEdit.png", Package.GetCurrentPackage(this)),
            Location = ModuleAction.ActionLocationEnum.Any,
            Mode = ModuleAction.ActionModeEnum.Any,
            Style = ModuleAction.ActionStyleEnum.Nothing,
            LinkText = this.__ResStr("pageControlLink", "Control Panel"),
            MenuText = this.__ResStr("pageControlMenu", "Control Panel"),
            Tooltip = this.__ResStr("pageControlTT", "Control Panel - Add new or existing modules, add new pages, switch to edit mode, access page settings and other site management tasks"),
            Legend = this.__ResStr("pageControlLeg", "Control Panel - Adds new or existing modules, adds new pages, switches to edit mode, accesses page settings and other site management tasks"),
        };
    }

    public ModuleAction? GetAction_SwitchToEdit() {
        if (!Manager.CurrentPage.IsAuthorized_Edit()) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.SwitchToEdit),
            QueryArgs = new { },
            Image = "#Edit",
            LinkText = this.__ResStr("modSwitchToEditLink", "Switch To Site Edit Mode"),
            MenuText = this.__ResStr("modSwitchToEditText", "Switch To Site Edit Mode"),
            Tooltip = this.__ResStr("modSwitchToEditTooltip", "Switch to Site Edit Mode"),
            Legend = this.__ResStr("modSwitchToEditLegend", "Switches to Site Edit Mode"),
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.View,
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                    ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
            DontFollow = true,
        };

    }
    public ModuleAction? GetAction_SwitchToView() {
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.SwitchToView),
            QueryArgs = new { },
            Image = "#Display",
            LinkText = this.__ResStr("modSwitchToViewLink", "Switch To Site View Mode"),
            MenuText = this.__ResStr("modSwitchToViewText", "Switch To Site View Mode"),
            Tooltip = this.__ResStr("modSwitchToViewTooltip", "Switch to Site View Mode"),
            Legend = this.__ResStr("modSwitchToViewLegend", "Switches to Site View Mode"),
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Edit,
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            SaveReturnUrl = true,
            DontFollow = true,
        };
    }
    public async Task<ModuleAction?> GetAction_ExportPageAsync(Guid? pageGuid = null) {
        Guid guid;
        PageDefinition? page;
        if (pageGuid == null) {
            page = Manager.CurrentPage;
            if (page == null) return null;
            guid = page.PageGuid;
        } else {
            guid = (Guid)pageGuid;
            page = await PageDefinition.LoadAsync(guid);
        }
        if (page == null) return null;
        if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_PageExport)) return null;
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.ExportPage),
            QueryArgs = new { PageGuid = guid },
            Image = await CustomIconAsync("ExportPage.png"),
            Name = "ExportPage",
            LinkText = this.__ResStr("modExportLink", "Export"),
            MenuText = this.__ResStr("modExportMenu", "Export Page"),
            Tooltip = this.__ResStr("modExportTT", "Export the page and modules by creating an importable ZIP file (using Control Panel, Import Page)"),
            Legend = this.__ResStr("modExportLegend", "Exports the page and modules by creating an importable ZIP file (using Control Panel, Import Page)"),
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            Mode = ModuleAction.ActionModeEnum.Any,
            CookieAsDoneSignal = true,
            Style = ModuleAction.ActionStyleEnum.Normal,
        };
    }
    public async Task<ModuleAction?> GetAction_W3CValidationAsync() {
        if (Manager.CurrentPage == null) return null;
        if (Manager.IsLocalHost) return null;
        ControlPanelConfigData config = await ControlPanelConfigDataProvider.GetConfigAsync();
        if (string.IsNullOrWhiteSpace(config.W3CUrl)) return null;
        if (!config.W3CUrl.Contains("{0}")) return null;
        return new ModuleAction(this) {
            Url = string.Format(config.W3CUrl, Manager.CurrentPage.EvaluatedCanonicalUrl),
            Image = await CustomIconAsync("W3CValidator.png"),
            Name = "W3CValidate",
            LinkText = this.__ResStr("modW3CValLink", "W3C Validation"),
            MenuText = this.__ResStr("modW3CValText", "W3C Validation"),
            Tooltip = this.__ResStr("modW3CValTooltip", "Use W3C Validation service to validate the current page - The page must be accessible to the remote service as an anonymous user"),
            Legend = this.__ResStr("modW3CValLegend", "Uses the defined W3C Validation service to validate a page - The page must be accessible to the remote service as an anonymous user"),
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            DontFollow = true,
        };
    }
    public async Task<ModuleAction?> GetAction_RestartSite() {
        if (!Manager.HasSuperUserRole) return null;
        if (YetaWF.Core.Support.Startup.MultiInstance) return null;
        return new ModuleAction(this) {
            Url = "/$restart",
            Image = await CustomIconAsync("RestartSite.png"),
            LinkText = this.__ResStr("restartLink", "Restart Site"),
            MenuText = this.__ResStr("restartText", "Restart Site"),
            Tooltip = this.__ResStr("restartTooltip", "Restart the site immediately (IIS restart)"),
            Legend = this.__ResStr("restartLegend", "Restarts the site immediately (IIS restart)"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            DontFollow = true,
        };
    }
    public ModuleAction? GetAction_ClearJsCssCache() {
        if (!Manager.HasSuperUserRole) return null;
        return new ModuleAction(this) {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.ClearJsCss),
            Image = "#Remove",
            LinkText = this.__ResStr("clrCacheLink", "Clear JS/CSS/Statics Cache"),
            MenuText = this.__ResStr("clrCacheText", "Clear JS/CSS/Statics Cache"),
            Tooltip = this.__ResStr("clrCacheTooltip", "Clear the cached JavaScript/CSS bundles and cached static small objects"),
            Legend = this.__ResStr("clrCacheLegend", "Clears the cached JavaScript/CSS bundles and cached static small objects"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            ConfirmationText = this.__ResStr("clrCacheConfirm", "Are you sure you want to clear the JavaScript/CSS bundles and cached static small objects?"),
            Location = ModuleAction.ActionLocationEnum.NoAuto |
                        ModuleAction.ActionLocationEnum.MainMenu | ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
            DontFollow = true,
        };
    }

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

        [Caption("New Module"), Description("The new module to be added")]
        [UIHint("ModuleSelection"), AdditionalMetadata("New", true), Required]
        public Guid SelectedModule { get; set; }

        [Caption("Name"), Description("The name of the new module to be added - the module name uniquely identifies a module")]
        [UIHint("Text40"), StringLength(ModuleDefinition.MaxName), Required, Trim]
        public string? ModuleName { get; set; }

        [Caption("Title"), Description("The module title, which appears at the top of the module as its title")]
        [UIHint("MultiString40"), StringLength(ModuleDefinition.MaxTitle), Required, Trim]
        public MultiString ModuleTitle { get; set; }

        [Caption("Pane"), Description("The pane where the new module is added - in Edit Mode, all panes are visible, even empty panes")]
        [UIHint("PaneSelection"), Required]
        public string? SelectedPane { get; set; }
        public List<string> SelectedPane_List { get; set; }

        [Caption("Location"), Description("The location within the selected pane where the new module is added")]
        [UIHint("Enum"), Required, Trim]
        public Location ModuleLocation { get; set; }

        public AddNewModuleModel() {
            ModuleTitle = new MultiString();
            SelectedPane_List = new List<string>();
        }
        public void AddData(PageDefinition page) {
            if (page != null) {
                SelectedPane_List = page.GetPanes();
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
        public string? ExistingModulePane { get; set; }
        public List<string> ExistingModulePane_List { get; set; }

        [Caption("Location"), Description("The location within the selected pane where the module is added")]
        [UIHint("Enum"), Required]
        public Location ModuleLocation { get; set; }

        public AddExistingModel() {
            ExistingModulePane_List = new List<string>();
        }

        public void AddData(PageDefinition page) {
            if (page != null) {
                ExistingModulePane_List = page.GetPanes();
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

        public ImportPageModel() {
            UploadFile = new FileUpload1();
        }

        public void AddData(PageDefinition page, PageControlModule mod) {
            UploadFile = new FileUpload1 {
                SelectButtonText = this.__ResStr("btnImportPage", "Import Page Data..."),
                SaveURL = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.ImportPage),
                RemoveURL = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.RemovePage),
            };
        }
    }

    [Trim]
    public class ImportModuleModel {

        [UIHint("Hidden")]
        public Guid CurrentPageGuid { get; set; }

        [Caption("Pane"), Description("The pane where the new module is added - in Edit Mode, all panes are visible, even empty panes")]
        [UIHint("PaneSelection"), Required]
        public string? ModulePane { get; set; }
        public List<string> ModulePane_List { get; set; }

        [Caption("Location"), Description("The location within the selected pane where the module is added")]
        [UIHint("Enum"), Required]
        public Location ModuleLocation { get; set; }

        [Caption("ZIP File"), Description("The ZIP file containing the module data to be imported (creates a new module) ")]
        [UIHint("FileUpload1"), Required]
        public FileUpload1 UploadFile { get; set; } = null!;

        public ImportModuleModel() {
            ModulePane_List = new List<string>();
        }

        public void AddData(PageDefinition page, PageControlModule mod) {
            UploadFile = new FileUpload1 {
                SelectButtonText = this.__ResStr("btnImport", "Import Module Data..."),
                SaveURL = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.ImportModule),
                RemoveURL = Utility.UrlFor(typeof(PageControlModuleEndpoints), PageControlModuleEndpoints.RemoveModule),
            };
            if (page != null) {
                ModulePane_List = page.GetPanes();
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
        public string? Url { get; set; }

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

        [Category("Skin"), Caption("Theme"), Description("The theme used for all pages of the site")]
        [UIHint("Theme"), StringLength(SiteDefinition.MaxTheme), SelectionRequired]
        public string? Theme { get; set; }

        public SkinSelectionModel() { }
    }

    [Trim]
    public class LoginSiteSelectionModel {

        [Caption("Active Site"), Description("List of sites that can be accessed - select an entry to visit the site")]
        [UIHint("DropDownList"), SubmitFormOnChange]
        public string? SiteDomain { get; set; }
        public List<SelectionItem<string>> SiteDomain_List { get; set; }

        [Caption("Active User"), Description("List of user accounts that can be used to quickly log into the site - select an entry to log in as that user")]
        [UIHint("YetaWF_Identity_LoginUsers"), SubmitFormOnChange]
        public int UserId { get; set; }
        public SerializableList<User> UserId_List { get; set; }

        [Caption("Superuser"), Description("If a superuser was signed on previously in this session, the superuser status remains even if logged in as another user - Uncheck to turn off superuser mode for this session")]
        [UIHint("Boolean"), SuppressIfNot("SuperuserStillActive", true), SubmitFormOnChange]
        public bool? SuperuserStillActive { get; set; }

        [Caption("Superuser"), Description("The currently logged on user is a superuser")]
        [UIHint("Boolean"), SuppressIf("SuperuserCurrent", false), ReadOnly]
        public bool SuperuserCurrent { get; set; }

        public LoginSiteSelectionModel() {
            SiteDomain_List = new List<SelectionItem<string>>();
            UserId_List = new SerializableList<User>();
        }

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
        public AddNewModuleModel AddNewModel { get; set; } = null!;
        public AddExistingModel AddExistingModel { get; set; } = null!;
        public ImportPageModel ImportPageModel { get; set; } = null!;
        public ImportModuleModel ImportModuleModel { get; set; } = null!;
        public AddNewPageModel AddNewPageModel { get; set; } = null!;
        public SkinSelectionModel SkinSelectionModel { get; set; } = null!;
        public LoginSiteSelectionModel LoginSiteSelectionModel { get; set; } = null!;
        [UIHint("ModuleActions"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.NormalLinks), ReadOnly]
        public List<ModuleAction> Actions { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {

        if (Manager.IsInPopup) return ActionInfo.Empty;
        if (Manager.CurrentPage == null || Manager.CurrentPage.Temporary) return ActionInfo.Empty;
#if DEBUG
        // allow in debug mode without checking unless marked deployed
        if (YetaWFManager.Deployed && !Manager.CurrentPage.IsAuthorized_Edit()) return ActionInfo.Empty;
#else
        if (!Manager.CurrentPage.IsAuthorized_Edit()) return ActionInfo.Empty;
#endif

        Guid pageGuid = Guid.Empty;
        if (pageGuid == Guid.Empty) {
            if (Manager.CurrentPage == null)
                pageGuid = new Guid(); // we're not on a designed page
            else
                pageGuid = Manager.CurrentPage.PageGuid;
        }

        PageDefinition? page = await PageDefinition.LoadAsync(pageGuid) ?? throw new InternalError($"Page {pageGuid} not found");
        bool editAuthorized = false;
        if (page.IsAuthorized_Edit())
            editAuthorized = true;

        PageControlModel model = new PageControlModel() {
            EditAuthorized = editAuthorized,
            AddNewPageModel = new AddNewPageModel() {
                CurrentPageGuid = Manager.CurrentPage!.PageGuid,
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
            SkinSelectionModel = new SkinSelectionModel {
                Theme = Manager.CurrentSite.Theme,
            },
            LoginSiteSelectionModel = new LoginSiteSelectionModel(),
        };

        PageEditModule modEdit = new PageEditModule();
        model.Actions = new List<ModuleAction>();
        model.Actions.New(await modEdit.GetAction_EditAsync(null));
        model.Actions.New(await GetAction_ExportPageAsync(null));
        model.Actions.New(await modEdit.GetAction_RemoveAsync(null));
        model.Actions.New(GetAction_SwitchToView());
        model.Actions.New(GetAction_SwitchToEdit());
        model.Actions.New(await GetAction_W3CValidationAsync());
        model.Actions.New(await GetAction_RestartSite());
        model.Actions.New(GetAction_ClearJsCssCache());

        model.AddNewModel.AddData(page);
        model.AddExistingModel.AddData(page);
        model.ImportPageModel.AddData(page, this);
        model.ImportModuleModel.AddData(page, this);
        await model.LoginSiteSelectionModel.AddDataAsync();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_PageAdd)]
    public async Task<IResult> UpdateAddNewPageAsync(AddNewPageModel model) {

        if (!ModelState.IsValid)
            return await PartialViewAsync(model, ViewName: AddNewPageView.ViewName);

        PageDefinition? basePage = null;
        if (model.CopyPage)
            basePage = await PageDefinition.LoadAsync(model.CurrentPageGuid);

        PageDefinition.NewPageInfo newPage = await PageDefinition.CreateNewPageAsync(model.Title, model.Description, model.Url!, basePage, model.CopyModules);
        PageDefinition page = newPage.Page;
        if (page == null) {
            ModelState.AddModelError(nameof(model.Url), newPage.Message);
            return await PartialViewAsync(model);
        }

        await page.SaveAsync();
        return await FormProcessedAsync(model, this.__ResStr("okNewPage", "New page created"), NextPage: page.EvaluatedCanonicalUrl);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_ModuleNewAdd)]
    public async Task<IResult> UpdateAddNewModuleAsync(AddNewModuleModel model) {
        PageDefinition? page = await PageDefinition.LoadAsync(model.CurrentPageGuid);
        if (page == null)
            throw new Error(this.__ResStr("addNewModCant:", "Can't edit this page"));
        model.AddData(page);
        if (!ModelState.IsValid)
            return await PartialViewAsync(model, ViewName: AddNewModuleView.ViewName);

        ModuleDefinition module = ModuleDefinition.CreateNewDesignedModule(model.SelectedModule, model.ModuleName, model.ModuleTitle);
        if (!module.IsModuleUnique)
            module.ModuleGuid = Guid.NewGuid();
        page.AddModule(model.SelectedPane!, module, model.ModuleLocation == Location.Top);
        await page.SaveAsync();
        return await FormProcessedAsync(model, this.__ResStr("okNew", "New module added"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_ModuleExistingAdd)]
    public async Task<IResult> UpdateAddExistingModuleAsync(AddExistingModel model) {
        PageDefinition? page = await PageDefinition.LoadAsync(model.CurrentPageGuid);
        if (page == null)
            throw new Error("Can't edit this page");
        model.AddData(page);

        if (!ModelState.IsValid)
            return await PartialViewAsync(model, ViewName: AddExistingModuleView.ViewName);

        ModuleDefinition? module = await ModuleDefinition.LoadAsync(model.ExistingModule) ?? throw new InternalError($"Existing module {model.ExistingModule} not found");
        page.AddModule(model.ExistingModulePane!, module, model.ModuleLocation == Location.Top);
        await page.SaveAsync();
        return await FormProcessedAsync(model, this.__ResStr("okExisting", "Module added"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage);
    }

    [ExcludeDemoMode]
    [ResourceAuthorize(CoreInfo.Resource_SiteSkins)]
    public async Task<IResult> UpdateSkinSelectionAsync(SkinSelectionModel model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model, ViewName: SkinSelectionView.ViewName);

        SiteDefinition origSite = new SiteDefinition();
        ObjectSupport.CopyData(Manager.CurrentSite, origSite);// make a copy of original site
        SiteDefinition site = Manager.CurrentSite;// update new settings
        site.Theme = model.Theme!;
        ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origSite, site);
        switch (modelDisp) {
            default:
            case ObjectSupport.ModelDisposition.None:
                return await FormProcessedAsync(model, this.__ResStr("okSaved", "Site settings updated"));
            case ObjectSupport.ModelDisposition.PageReload:
                await site.SaveAsync();
                return await FormProcessedAsync(model, this.__ResStr("okSaved", "Site settings updated"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect: true);
            case ObjectSupport.ModelDisposition.SiteRestart:
                await site.SaveAsync();
                return await FormProcessedAsync(model, this.__ResStr("okSavedRestart", "Site settings updated - These settings won't take effect until the site is restarted"));
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateLoginSiteSelectionAsync(LoginSiteSelectionModel model) {
        if (YetaWFManager.Deployed) {
            if (!await Resource.ResourceAccess.IsResourceAuthorizedAsync(CoreInfo.Resource_OtherUserLogin))
                throw new InternalError("Not Authorized");
        }

        if (!ModelState.IsValid) {
            await model.AddDataAsync();
            return await PartialViewAsync(model, ViewName: LoginSiteSelectionView.ViewName);
        }

        string nextPage;
        if (Manager.CurrentSite.SiteDomain != model.SiteDomain) {
            nextPage = Manager.CurrentSite.MakeFullUrl(RealDomain: model.SiteDomain, SecurityType: Core.Pages.PageDefinition.PageSecurityType.httpOnly);
        } else { /* if (model.UserId != Manager.UserId) */
            //if (model.SuperuserStillActive != null && !(bool)model.SuperuserStillActive)
            Manager.SetSuperUserRole(false);
            int userId = model.UserId;
            if (userId == 0)
                await Resource.ResourceAccess.LogoffAsync();
            else
                await Resource.ResourceAccess.LoginAsAsync(userId);
            //if (model.SuperuserStillActive != null && !(bool)model.SuperuserStillActive)
            //  Manager.SetSuperUserRole(false);
            nextPage = Manager.ReturnToUrl;
        }
        Manager.PageControlShown = false;
        return await FormProcessedAsync(model, NextPage: nextPage, ForceRedirect: true);
    }
}
