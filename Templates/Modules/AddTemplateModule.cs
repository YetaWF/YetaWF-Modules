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

public class AddTemplateModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddTemplateModule>, IInstallableModel { }

[ModuleGuid("{F5F0CA65-0B1D-4dcd-83F9-AF2B1A26E51F}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class AddTemplateModule : ModuleDefinition {

    public AddTemplateModule() {
        Title = this.__ResStr("modTitle", "Add Template Test Module");
        Name = this.__ResStr("modName", "Add Template Test Module");
        Description = this.__ResStr("modSummary", "Adds a template test module");
        DefaultViewName = StandardViews.Add;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddTemplateModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add Template Test Module"),
            MenuText = this.__ResStr("addText", "Add Template Test Module"),
            Tooltip = this.__ResStr("addTooltip", "Add a template test module"),
            Legend = this.__ResStr("addLegend", "Adds a template test module"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    [Trim]
    [Header("Add a template test module to your project to test a template. This is normally used during development of a new template.")]
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

        [Caption("Template Name"), Description("Enter the desired template test module name")]
        [TextBelow("Example: ModuleSelection")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxModuleName), Required, Trim]
        public string? TemplateModuleName { get; set; }

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

            variables.Add("$templatetest$", model.TemplateModuleName!);
            variables.Add("$templatetestmoduleguid$", Guid.NewGuid().ToString());

            // copy project files
            string sourceUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName) + "TemplateModule";
            string sourcePath = Utility.UrlToPhysical(sourceUrl);

            Process process = new Process(variables);
            await process.CopyFilesAsync(sourcePath, path);

            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New template test module {0} added to project {1}/{2}.", model.TemplateModuleName, model.Domain, model.Project, path), OnClose: OnCloseEnum.Return);
        }
    }
}