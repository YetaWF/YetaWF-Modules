/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Endpoints.Support;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Modules;

public class SiteTemplateProcessModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteTemplateProcessModule>, IInstallableModel { }

[ModuleGuid("{8a375e4b-f0ee-4d95-8c18-4e51e83c06e5}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class SiteTemplateProcessModule : ModuleDefinition2 {

    public SiteTemplateProcessModule() {
        Title = this.__ResStr("modTitle", "Process Site Template");
        Name = this.__ResStr("modName", "Process Site Template");
        Description = this.__ResStr("modSummary", "Processes a Site Template and adds all defined pages to the current site. The Process Site Template Module is accessible using Admin > Panel > Packages (standard YetaWF site).");
        Print = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new SiteTemplateProcessModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public async Task<ModuleAction> GetAction_ProcessSiteTemplateAsync(string url, string fileName) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { FileName = fileName },
            Image = await CustomIconAsync("SiteTemplateProcess.png"),
            LinkText = this.__ResStr("processLink", "Site Template"),
            MenuText = this.__ResStr("processText", "Site Template"),
            Tooltip = this.__ResStr("processTooltip", "Process a site template and add all defined pages to the current site"),
            Legend = this.__ResStr("processLegend", "Processes a site template and adds all defined pages to the current site"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    [Trim]
    [Footer("The selected template is used to configure the current site - Once completed the page has to be manually refreshed (deliberately not automatic)")]
    public class EditModel {

        [Caption("Name"), Description("List of site templates (located in the SiteTemplates folder)")]
        [UIHint("DropDownList")]
        public string? SiteTemplate { get; set; }

        public List<SelectionItem<string>>? SiteTemplate_List { get; set; }

        public async Task UpdateDataAsync() {
            PackagesDataProvider packagesDP = new PackagesDataProvider();
            SiteTemplate_List = (from f in await FileSystem.FileSystemProvider.GetFilesAsync(packagesDP.TemplateFolder, "*.txt") orderby f select new SelectionItem<string>() {
                Text = Path.GetFileName(f),
                Value = Path.GetFileName(f),
            }).ToList();
        }
        public EditModel() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        PackagesDataProvider packagesDP = new PackagesDataProvider();
        if (!await FileSystem.FileSystemProvider.DirectoryExistsAsync(packagesDP.TemplateFolder))
            return ActionInfo.Empty;
        EditModel model = new EditModel { };
        await model.UpdateDataAsync();
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(EditModel model) {
        await model.UpdateDataAsync();
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Site template processing is not possible when distributed caching is enabled");

        PackagesDataProvider packagesDP = new PackagesDataProvider();
        await packagesDP.BuildSiteUsingTemplateAsync(model.SiteTemplate!);
        return await FormProcessedAsync(model);
    }
}