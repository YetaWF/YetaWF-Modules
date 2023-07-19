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

public class AddConfigModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddConfigModule>, IInstallableModel { }

[ModuleGuid("{9854BE8C-005E-4670-B34F-43E74DD1F274}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class AddConfigModule : ModuleDefinition {

    public AddConfigModule() {
        Title = this.__ResStr("modTitle", "Add Configuration Module");
        Name = this.__ResStr("modName", "Add Configuration Module");
        Description = this.__ResStr("modSummary", "Adds a configuration module");
        DefaultViewName = StandardViews.Add;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddConfigModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add Configuration Module"),
            MenuText = this.__ResStr("addText", "Add Configuration Module"),
            Tooltip = this.__ResStr("addTooltip", "Add a configuration module"),
            Legend = this.__ResStr("addLegend", "Adds a configuration module"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    [Trim]
    [Header("Add a configuration module to your project to define configuration settings for an entire package, which can be referenced by all modules in your package.\r\n\r\nAfter the modules are created, you can fully customize the data model used and add code to access external data (SQL tables, files, etc.).\r\n\r\nThe generated modules support popups and page-based modules, include property pages and are localizable, with full data annotations support.")]
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

        [Caption("Module Name"), Description("Enter the desired configuration module name")]
        [TextBelow("Example: Settings")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxModuleName), Required, Trim]
        public string? ConfigModuleName { get; set; }

        [Caption("Data Model Name"), Description("Enter the desired data model name")]
        [TextBelow("Example: ConfigData, RegistrationData")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxModelName), Required, Trim]
        public string? ConfigModelName { get; set; }

        [Caption("Object Name"), Description("Enter the desired user-displayable object name for the data model name")]
        [TextBelow("Example: Configuration Settings")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxObjectName), Required, Trim]
        public string? ConfigObjectName { get; set; }

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

            variables.Add("$configmodule$", model.ConfigModuleName!);
            variables.Add("$configmoduleguid$", Guid.NewGuid().ToString());

            variables.Add("$modelname$", model.ConfigModelName!);
            variables.Add("$modelnamelower$", model.ConfigModelName!.ToLower());
            variables.Add("$objectname$", model.ConfigObjectName!);
            variables.Add("$objectnamelower$", model.ConfigObjectName!.ToLower());

            // copy project files
            string sourceUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName) + "ConfigModule";
            string sourcePath = Utility.UrlToPhysical(sourceUrl);

            Process process = new Process(variables);
            await process.CopyFilesAsync(sourcePath, path);

            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New configuration module {0} added to project {1}/{2}.", model.ConfigModuleName, model.Domain, model.Project, path), OnClose: OnCloseEnum.Return);
        }
    }
}