using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Templates.DataProvider;
using YetaWF.Modules.Templates.Support;

namespace YetaWF.Modules.Templates.Modules;

public class AddBrowseModulesDataProvider : ModuleDefinitionDataProvider<Guid, AddBrowseModule>, IInstallableModel { }

[ModuleGuid("{967DC58D-CC17-4ce5-AF10-3EAC96994882}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class AddBrowseModule : ModuleDefinition {

    public AddBrowseModule() {
        Title = this.__ResStr("modTitle", "Add Browse Modules");
        Name = this.__ResStr("modName", "Add Browse Modules");
        Description = this.__ResStr("modSummary", "Adds browse modules");
        DefaultViewName = StandardViews.Add;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddBrowseModulesDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add Browse Modules"),
            MenuText = this.__ResStr("addText", "Add Browse Modules"),
            Tooltip = this.__ResStr("addTooltip", "Add browse modules"),
            Legend = this.__ResStr("addLegend", "Adds browse modules"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    [Trim]
    [Header("Add modules to your project to browse, add, edit and display items with data storage in files and/or SQL tables. Item removal is supported by the browse, edit and display modules.\r\n\r\nAfter the modules are created, you can fully customize the data model used and add code to access external data (SQL tables, files, etc.).\r\n\r\nThe generated modules use jQuery, support popups and page-based modules, include property pages and are localizable, with full data annotations support.")]
    public class Model {

        [Caption("Company"), Description("Enter your company name")]
        [TextBelow("Example:  My Company Name - This is used to show your company name in displayable form.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxCompany), Required, Trim]
        public string? Company { get; set; }

        [Caption("Company URL"), Description("Enter your company's domain Url")]
        [TextBelow("Example:  MyCompany.com - Your company's domain Url.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxCompanyUrl), Required, Trim]
        public string? CompanyUrl { get; set; }

        [Caption("Domain"), Description("Enter your company's domain name without .com")]
        [TextBelow("Example:  MyCompany - A project must already exist which uses the entered Domain and Project Name. This is used as part of the generated namespace and will also be used for all new items added to this project. The identifier can only contain letters and numbers.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxDomain), Required, Trim]
        public string? Domain { get; set; }

        [Caption("Project Name"), Description("Enter the desired project name")]
        [TextBelow("Example:  StoreFont or Blog or Forum - A project must already exist which uses the entered Domain and Project Name. This is used as part of the generated namespace and will also be used for all new items added to this project. The identifier can only contain letters and numbers.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxProject), Required, Trim]
        public string? Project { get; set; }


        [Caption("Module Name"), Description("Enter the base name for all browse, add, edit, display modules")]
        [TextBelow("Example: Invoice, Product, Employee")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxModuleName), Required, Trim]
        public string? BrowseModuleName { get; set; }

        [Caption("Browse Module"), Description("Select to generate a browse module")]
        [UIHint("Switch")]
        public bool BrowseWanted { get; set; }
        [Caption("Add Module"), Description("Select to generate an add module")]
        [UIHint("Switch")]
        public bool AddWanted { get; set; }
        [Caption("Edit Module"), Description("Select to generate an edit module")]
        [UIHint("Switch")]
        public bool EditWanted { get; set; }
        [Caption("Display Module"), Description("Select to generate a display module")]
        [UIHint("Switch")]
        public bool DisplayWanted { get; set; }

        [Caption("Data Provider"), Description("Enter the desired data provider")]
        [UIHint("Enum")]
        public TemplateDefaults.DataProviderType BrowseDataProvider { get; set; }

        [Caption("Data Model Name"), Description("Enter the desired data model name")]
        [TextBelow("Example: UserSettings, PersonnelData")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxModelName), Required, Trim]
        public string? BrowseModelName { get; set; }

        [Caption("Object Name"), Description("Enter the desired user-displayable object name for the data model name")]
        [TextBelow("Example: User Account, Product Type")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxObjectName), Required, Trim]
        public string? BrowseObjectName { get; set; }

        [Caption("Key Type"), Description("Enter the desired key type for the data model")]
        [TextBelow("Example: Guid, String, int")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxBrowseKeyType), Required, Trim]
        public string? BrowseKeyType { get; set; }

        [Caption("Key Name"), Description("Enter the desired key property name for the data model")]
        [TextBelow("Example: Name, Key, UserName, ProductName")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxBrowseKeyName), Required, Trim]
        public string? BrowseKeyName { get; set; }

        public Model() { }

        public TemplateDefaults GetData() {
            TemplateDefaults template = new TemplateDefaults();
            ObjectSupport.CopyData(this, template);
            return template;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model { };
        using (TemplateDefaultsDataProvider templateDP = new TemplateDefaultsDataProvider()) {
            TemplateDefaults? template = await templateDP.GetItemAsync(Manager.UserId);
            if (template == null)
                template = new TemplateDefaults();
            ObjectSupport.CopyData(template, model);
        }
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (!model.BrowseWanted && !model.AddWanted && !model.EditWanted && !model.DisplayWanted) {
            ModelState.AddModelError(nameof(model.BrowseWanted), this.__ResStr("select", "At least one module type must be selected."));
            return await PartialViewAsync(model);
        }

        using (TemplateDefaultsDataProvider TemplateDefaultsDP = new TemplateDefaultsDataProvider()) {
            if (!await TemplateDefaultsDP.AddItemAsync(model.GetData()))
                throw new Error(this.__ResStr("cantSave", "Unable to save template defaults"));

            // set up all variables
            string path = Path.Combine(YetaWFManager.RootFolderSolution, "Modules", model.Domain!, model.Project!);
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
                throw new Error(this.__ResStr("notExists", "Project {0} not found. The folder {1} doesn't exist. Modules can only be added to existing projects.", model.Project, path));

            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("$companyname$", model.Company!);
            variables.Add("$companyurl$", model.CompanyUrl!);
            variables.Add("$companynamespace$", model.Domain!);
            variables.Add("$projectname$", model.Project!);
            variables.Add("$projectnamespace$", model.Project!);
            variables.Add("$webprojectfolder$", YetaWFManager.RootFolderWebProject);

            variables.Add("$browsemodule$", model.BrowseWanted ? $"{model.BrowseModuleName}Browse" : string.Empty);
            variables.Add("$createmodule$", model.AddWanted ? $"{model.BrowseModuleName}Add" : string.Empty);
            variables.Add("$editmodule$", model.EditWanted ? $"{model.BrowseModuleName}Edit" : string.Empty);
            variables.Add("$displaymodule$", model.DisplayWanted ? $"{model.BrowseModuleName}Display" : string.Empty);

            variables.Add("$browsemoduleguid$", Guid.NewGuid().ToString());
            variables.Add("$createmoduleguid$", Guid.NewGuid().ToString());
            variables.Add("$editmoduleguid$", Guid.NewGuid().ToString());
            variables.Add("$displaymoduleguid$", Guid.NewGuid().ToString());

            variables.Add("$dp$", model.BrowseDataProvider.ToString());
            
            variables.Add("$modelname$", model.BrowseModelName!);
            variables.Add("$modelnamelower$", model.BrowseModelName!.ToLower());
            variables.Add("$objectname$", model.BrowseObjectName!);
            variables.Add("$objectnamelower$", model.BrowseObjectName!.ToLower());
            variables.Add("$modelkey$", model.BrowseKeyType!);
            variables.Add("$modelkeyname$", model.BrowseKeyName!);
            variables.Add("$modelkeynamelower$", char.ToLower(model.BrowseKeyName![0]) + model.BrowseKeyName!.Substring(1));

            // copy project files
            string sourceUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName) + "BrowseModules";
            string sourcePath = Utility.UrlToPhysical(sourceUrl);

            Process process = new Process(variables);
            await process.CopyFilesAsync(sourcePath, path);

            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New module(s) {0} added to project {1}/{2}.", model.BrowseModuleName, model.Domain, model.Project, path), OnClose: OnCloseEnum.Return);
        }
    }
}