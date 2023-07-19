using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class $editmodule$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, $editmodule$Module>, IInstallableModel { }

[ModuleGuid("{$editmoduleguid$}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class $editmodule$Module : ModuleDefinition {

    public $editmodule$Module() {
        Title = this.__ResStr("modTitle", "$objectname$");
        Name = this.__ResStr("modName", "Edit $objectname$");
        Description = this.__ResStr("modSummary", "Edits an existing $objectnamelower$");
        DefaultViewName = StandardViews.Edit;
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new $editmodule$ModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Edit(string? url, $modelkey$ $modelkeynamelower$) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { $modelkeyname$ = $modelkeynamelower$ },
            Image = "#Edit",
            LinkText = this.__ResStr("editLink", "Edit"),
            MenuText = this.__ResStr("editText", "Edit"),
            Tooltip = this.__ResStr("editTooltip", "Edit $objectnamelower$"),
            Legend = this.__ResStr("editLegend", "Edits an existing $objectnamelower$"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Update,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    [Trim]
    public class Model {
$iff $dp$ == SQLFile
        [Caption("$modelkeyname$"), Description("The $modelkeynamelower$ of this $objectnamelower$")]
        [UIHint("String"), StringLength($modelname$.Max$modelkeyname$), Required, Trim]
        public $modelkey$ $modelkeyname$ { get; set; }
$end
        [Caption("Field 1"), Description("Field1 description")]
        [UIHint("Text40"), StringLength($modelname$.MaxField1), Required]
        public string? Field1 { get; set; }
$iff $dp$ == SQLIdentity
        [Caption("Id"), Description("The internal id")]
        [UIHint("IntValue"), ReadOnly]
        public $modelkey$ $modelkeyname$ { get; set; }
$end
        [UIHint("Hidden"), ReadOnly]
        public $modelkey$ Original$modelkeyname$ { get; set; } = null!;

        public $modelname$ GetData($modelname$ $modelnamelower$) {
            ObjectSupport.CopyData(this, $modelnamelower$);
            return $modelnamelower$;
        }

        public void SetData($modelname$ $modelnamelower$) {
            ObjectSupport.CopyData($modelnamelower$, this);
            Original$modelkeyname$ = $modelkeyname$;
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync($modelkey$ $modelkeynamelower$) {
        using ($modelname$DataProvider $modelnamelower$DP = new $modelname$DataProvider()) {
            Model model = new Model {};
            $modelname$? $modelnamelower$ = await $modelnamelower$DP.GetItemAsync($modelkeynamelower$);
            if ($modelnamelower$ == null)
                throw new Error(this.__ResStr("notFound", "$objectname$ \"{0}\" not found"), $modelkeynamelower$);
            model.SetData($modelnamelower$);
            Title = this.__ResStr("title", "$objectname$ \"{0}\"", $modelkeynamelower$);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        $modelkey$? original$modelkeyname$ = model.Original$modelkeyname$;

        using ($modelname$DataProvider $modelnamelower$DP = new $modelname$DataProvider()) {
            $modelname$? $modelnamelower$ = await $modelnamelower$DP.GetItemAsync(original$modelkeyname$);// get the original item
            if ($modelnamelower$ == null)
                throw new Error(this.__ResStr("alreadyDeleted", "The $objectname$ {0} has been removed and can no longer be updated", original$modelkeyname$));
            ObjectSupport.CopyData($modelnamelower$, model, ReadOnly: true); // update read only properties in model in case there is an error
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);

            $modelnamelower$ = model.GetData($modelnamelower$); // merge new $modelnamelower$ into original
            model.SetData($modelnamelower$); // and all the $modelnamelower$ back into model for final display

$iff $dp$ == SQLFile
            switch (await $modelnamelower$DP.UpdateItemAsync(original$modelkeyname$, $modelnamelower$)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The $objectname$ {0} has been removed and can no longer be updated", original$modelkeyname$));
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError(nameof(model.$modelkeyname$), this.__ResStr("alreadyExists", "An $objectname$ {0} already exists", model.$modelkeyname$));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
$else
            switch (await $modelnamelower$DP.UpdateItemAsync($modelnamelower$)) {
                default:
                case UpdateStatusEnum.RecordDeleted:
                    throw new Error(this.__ResStr("alreadyDeleted", "The $objectname$ {0} has been removed and can no longer be updated", original$modelkeyname$));
                case UpdateStatusEnum.NewKeyExists:
                    ModelState.AddModelError(nameof(model.$modelkeyname$), this.__ResStr("alreadyExists", "An $objectname$ {0} already exists", model.$modelkeyname$));
                    return await PartialViewAsync(model);
                case UpdateStatusEnum.OK:
                    break;
            }
$end
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "$objectname$ saved"), OnPopupClose: OnPopupCloseEnum.ReloadModule);
        }
    }
}
