using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using $companynamespace$.Modules.$projectnamespace$.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class $createmodule$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, $createmodule$Module>, IInstallableModel { }

[ModuleGuid("{$createmoduleguid$}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class $createmodule$Module : ModuleDefinition {

    public $createmodule$Module() {
        Title = this.__ResStr("modTitle", "Add New $objectname$");
        Name = this.__ResStr("modName", "Add New $objectname$");
        Description = this.__ResStr("modSummary", "Adds a new $objectnamelower$");
        DefaultViewName = StandardViews.Add;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new $createmodule$ModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Add(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Add",
            LinkText = this.__ResStr("addLink", "Add"),
            MenuText = this.__ResStr("addText", "Add"),
            Tooltip = this.__ResStr("addTooltip", "Add a new $objectnamelower$"),
            Legend = this.__ResStr("addLegend", "Adds a new $objectnamelower$"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
        };
    }

    [Trim]
    public class Model {

$iff $dp$ == SQLFile
        [Caption("$modelkeyname$"), Description("The $modelkeynamelower$ of this $objectnamelower$")]
        [UIHint("Text40"), StringLength($modelname$.Max$modelkeyname$), Required, Trim]
        public $modelkey$ $modelkeyname$ { get; set; } = null!;
$end

        [Caption("Field 1"), Description("Field1 description")]
        [UIHint("Text40"), StringLength($modelname$.MaxField1), Required, Trim]
        public string? Field1 { get; set; }

        public Model() { }

        public $modelname$ GetData() {
            $modelname$ $modelnamelower$ = new $modelname$();
            ObjectSupport.CopyData(this, $modelnamelower$);
            return $modelnamelower$;
        }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        Model model = new Model {};
        ObjectSupport.CopyData(new $modelname$(), model);
        return await RenderAsync(model);
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        if (!ModelState.IsValid)
            return await PartialViewAsync(model);

        using ($modelname$DataProvider $modelname$DP = new $modelname$DataProvider()) {
            if (!await $modelname$DP.AddItemAsync(model.GetData())) {
$iff $dp$ == SQLFile
                ModelState.AddModelError(nameof(model.$modelkeyname$), this.__ResStr("alreadyExists", "A $objectnamelower$ named \"{0}\" already exists", model.$modelkeyname$));
                return await PartialViewAsync(model);
$else
                throw new Error(this.__ResStr("alreadyExists", "New $objectnamelower$ couldn't be added"));
$end
            }
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "New $objectnamelower$ saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
