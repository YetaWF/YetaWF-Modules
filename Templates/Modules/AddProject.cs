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

public class AddProjectModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddProjectModule>, IInstallableModel { }

[ModuleGuid("{a6bb4845-f074-4642-9ee2-b59c692813da}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class AddProjectModule : ModuleDefinition {

    public AddProjectModule() {
        Title = this.__ResStr("modTitle", "Add New Project");
        Name = this.__ResStr("modName", "Add New Project");
        Description = this.__ResStr("modSummary", "Adds a new project");
        DefaultViewName = StandardViews.Add;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddProjectModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add Project"),
            MenuText = this.__ResStr("addText", "Add Project"),
            Tooltip = this.__ResStr("addTooltip", "Add a new project"),
            Legend = this.__ResStr("addLegend", "Adds a new project"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    [Trim]
    [Header("Creates a new project. This new project can be used to develop new YetaWF modules, skins and/or new shared templates (Editor/Display).\r\n\r\nOnce the project has been created, you can add new items (e.g. Browse Module) and other available items.")]
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
        [TextBelow("Example:  MyCompany - This is used as part of the generated namespace and will also be used for all new items added to this project. The identifier can only contain letters and numbers.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxDomain), Required, Trim]
        public string? Domain { get; set; }

        [Caption("Project Name"), Description("Enter the desired project name")]
        [TextBelow("Example:  StoreFont or Blog or Forum - This is used as part of the generated namespace and will also be used for all new items added to this project. The identifier can only contain letters and numbers.")]
        [UIHint("Text40"), StringLength(TemplateDefaults.MaxProject), Required, Trim]
        public string? Project { get; set; }

        [Caption("Namespace"), Description("Shows the generated namespace")]
        [UIHint("Text80"), ReadOnly]
        public string? NameSpace { get; set; }

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
            if (Directory.Exists(path))
                throw new Error(this.__ResStr("exists", "The folder {0} already exists.", path));

            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("$companyname$", model.Company!);
            variables.Add("$companyurl$", model.CompanyUrl!);
            variables.Add("$companynamespace$", model.Domain!);
            variables.Add("$projectname$", model.Project!);
            variables.Add("$projectnamespace$", model.Project!);
            variables.Add("$webprojectfolder$", YetaWFManager.RootFolderWebProject);

            // create all folders typically used (eventually)
            Directory.CreateDirectory(Path.Combine(path, "Addons", "_Addons"));
            Directory.CreateDirectory(Path.Combine(path, "Addons", "_Main"));
            Directory.CreateDirectory(Path.Combine(path, "Addons", "_Templates"));
            Directory.CreateDirectory(Path.Combine(path, "Components", "HTML"));
            Directory.CreateDirectory(Path.Combine(path, "Endpoints"));
            Directory.CreateDirectory(Path.Combine(path, "Models"));
            Directory.CreateDirectory(Path.Combine(path, "Modules"));
            Directory.CreateDirectory(Path.Combine(path, "Views", "HTML"));

            // copy project files
            string sourceUrl = Package.GetAddOnPackageUrl(AreaRegistration.CurrentPackage.AreaName) + "Project";
            string sourcePath = Utility.UrlToPhysical(sourceUrl);

            Process process = new Process(variables);
            await process.CopyFilesAsync(sourcePath, path);

            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New project {0} added at {1}  - Make sure to add the project to your solution, add a project reference in the Website project and rebuild/restart your solution.", model.Project, path), OnClose: OnCloseEnum.Return);
        }
    }
}