/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
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

namespace YetaWF.Modules.Dashboard.Modules;

public class AddonDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddonDisplayModule>, IInstallableModel { }

[ModuleGuid("{df3df0e1-f88b-45e1-a04e-864748166a21}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class AddonDisplayModule : ModuleDefinition {

    public AddonDisplayModule() {
        Title = this.__ResStr("modTitle", "AddOn Details");
        Name = this.__ResStr("modName", "AddOn Details");
        Description = this.__ResStr("modSummary", "Displays detailed information about one AddOn. Used by the AddOn Info Module.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new AddonDisplayModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction GetAction_Display(string? url, string addonKey) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { Key = addonKey },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display AddOn information"),
            Legend = this.__ResStr("displayLegend", "Displays AddOn information"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,

        };
    }

    public class DisplayModel {

        [Caption("Type"), Description("The AddOn type")]
        [UIHint("Enum"), ReadOnly]
        public Package.AddOnType Type { get; set; }
        [Caption("Domain"), Description("The domain owning this AddOn")]
        [UIHint("String"), ReadOnly]
        public string Domain { get; set; } = null!;
        [Caption("Product"), Description("The AddOn's product name")]
        [UIHint("String"), ReadOnly]
        public string Product { get; set; } = null!;
        [Caption("Version"), Description("The AddOn's version")]
        [UIHint("String"), ReadOnly]
        public string Version { get; set; } = null!;
        [Caption("Name"), Description("The AddOn's internal name")]
        [UIHint("String"), ReadOnly]
        public string Name { get; set; } = null!;
        [Caption("Url"), Description("The AddOn's Url where its files are located")]
        [UIHint("String"), ReadOnly]
        public string Url { get; set; } = null!;

        [Caption("JavaScript Files"), Description("List of JavaScript files for this AddOn (filelistJS.txt file contents)")]
        [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br>"), ReadOnly]
        public List<string> JsFiles { get; set; } = null!;
        [Caption("JavaScript Path"), Description("The AddOn's location for JavaScript files (overrides Url) - only used if a Folder directive was found in filelistJS.txt")]
        [UIHint("String"), ReadOnly]
        public string? JsPathUrl { get; set; }
        [Caption("Css Files"), Description("List of Css files for this AddOn (filelistCSS.txt file contents)")]
        [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br>"), ReadOnly]
        public List<string>? CssFiles { get; set; }
        [Caption("Css Path"), Description("The AddOn's location for Css files (overrides Url) - only used if a Folder directive was found in filelistCSS.txt")]
        [UIHint("String"), ReadOnly]
        public string CssPathUrl { get; set; } = null!;

        [Caption("Support Types"), Description("List of types that define the IAddOnSupport interface for this AddOn, used to add Localization and Config information for client-side JavaScript use")]
        [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br>"), ReadOnly]
        public List<string> SupportTypesStrings { get; set; } = null!;

        [Caption("Skin Definition Path"), Description("The location where the Skin.txt file is located defining the Skin attributes - only used for Skin AddOns")]
        [UIHint("String"), ReadOnly]
        public string? SkinFilePath { get; set; }

        public void SetData(Package.AddOnProduct data) {
            ObjectSupport.CopyData(data, this);
            JsPathUrl = Utility.PhysicalToUrl(data.JsPath);
            CssPathUrl = Utility.PhysicalToUrl(data.CssPath);
            SupportTypesStrings = new List<string>();
            foreach (Type t in data.SupportTypes) {
                SupportTypesStrings.Add(t.FullName!);
            }
            if (data.SkinInfo != null)
                SkinFilePath = data.SkinInfo.Folder;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync(string key) {
        List<Package.AddOnProduct> list = Package.GetAvailableAddOns();
        Package.AddOnProduct? data = (from l in list where l.AddonKey == key select l).FirstOrDefault();
        if (data == null)
            throw new Error(this.__ResStr("notFound", "AddOn Info for key \"{0}\" not found"), key);
        DisplayModel model = new DisplayModel();
        model.SetData(data);
        return await RenderAsync(model);
    }
}