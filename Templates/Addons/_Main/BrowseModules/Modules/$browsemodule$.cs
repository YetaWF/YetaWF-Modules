using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using Softelvdm.Modules.$projectnamespace$.Endpoints;
using Softelvdm.Modules.$projectnamespace$.DataProvider;


namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class $browsemodule$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, $browsemodule$Module>, IInstallableModel { }

[ModuleGuid("{$browsemoduleguid$}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class $browsemodule$Module : ModuleDefinition {

    public $browsemodule$Module() {
        Title = this.__ResStr("modTitle", "$objectname$s");
        Name = this.__ResStr("modName", "$objectname$s");
        Description = this.__ResStr("modSummary", "Displays and manages $objectnamelower$s");
        DefaultViewName = StandardViews.PropertyListEdit;
        UsePartialFormCss = false;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new $browsemodule$ModuleDataProvider(); }

    [Category("General"), Caption("Add Url"), Description("The Url to add a new $objectnamelower$ - If omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? AddUrl { get; set; }
    [Category("General"), Caption("Display Url"), Description("The Url to display a $objectnamelower$ - If omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a $objectnamelower$ - If omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? EditUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } } // TODO: adjust default permissions
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("RemoveItems",
                    this.__ResStr("roleRemItemsC", "Remove $objectname$s"), this.__ResStr("roleRemItems", "The role has permission to remove individual $objectnamelower$s"),
                    this.__ResStr("userRemItemsC", "Remove $objectname$s"), this.__ResStr("userRemItems", "The user has permission to remove individual $objectnamelower$s")),
            };
        }
    }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        $createmodule$Module mod = new $createmodule$Module();
$iff $createmodule$.cs
        menuList.New(mod.GetAction_Add(AddUrl), location);
$end
        return menuList;
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "$objectname$s"),
            MenuText = this.__ResStr("browseText", "$objectname$s"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage $objectnamelower$s"),
            Legend = this.__ResStr("browseLegend", "Displays and manages $objectnamelower$s"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public ModuleAction? GetAction_Remove($modelkey$ $modelkeynamelower$) {
        if (!IsAuthorized("RemoveItems")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor<$browsemodule$ModuleEndpoints>($browsemodule$ModuleEndpoints.Remove),
            QueryArgs = new { $modelkeyname$ = $modelkeynamelower$ },
            Image = "#Remove",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("removeLink", "Remove $objectname$"),
            MenuText = this.__ResStr("removeMenu", "Remove $objectname$"),
            Tooltip = this.__ResStr("removeTT", "Remove the $objectnamelower$"),
            Legend = this.__ResStr("removeLegend", "Removes the $objectnamelower$"),
            Category = ModuleAction.ActionCategoryEnum.Delete,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove $objectnamelower$ \"{0}\"?", $modelkeynamelower$),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();
$iff $displaymodule$.cs
                $displaymodule$Module dispMod = new $displaymodule$Module();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, $modelkeyname$), ModuleAction.ActionLocationEnum.GridLinks);
$end
$iff $editmodule$.cs
                $editmodule$Module editMod = new $editmodule$Module();
                actions.New(editMod.GetAction_Edit(Module.EditUrl, $modelkeyname$), ModuleAction.ActionLocationEnum.GridLinks);
$end
                actions.New(Module.GetAction_Remove($modelkeyname$), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }
        }

$iff $dp$ == SQLFile
        [Caption("$modelkeyname$"), Description("The $modelkeynamelower$ of this $objectnamelower$")]
        [UIHint("String"), ReadOnly]
        public $modelkey$ $modelkeyname$ { get; set; } = null!;
$end
        [Caption("Field 1"), Description("Field1 description")]
        [UIHint("String"), ReadOnly]
        public string? Field1 { get; set; }
$iff $dp$ == SQLIdentity
        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public $modelkey$ $modelkeyname$ { get; set; } = null!;
$end

        private $browsemodule$Module Module { get; set; }

        public BrowseItem($browsemodule$Module module, $modelname$ $modelnamelower$) {
            Module = module;
            ObjectSupport.CopyData($modelnamelower$, this);
        }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<$browsemodule$ModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using ($modelname$DataProvider $modelname$DP = new $modelname$DataProvider()) {
                    DataProviderGetRecords<$modelname$> browseItems = await $modelname$DP.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public class BrowseModel {
        [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
