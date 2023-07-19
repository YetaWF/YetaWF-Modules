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

namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class $displaymodule$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, $displaymodule$Module>, IInstallableModel { }

[ModuleGuid("{$displaymoduleguid$}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
//[ModuleCategory("...")]
public class $displaymodule$Module : ModuleDefinition {

    public $displaymodule$Module() {
        Title = this.__ResStr("modTitle", "$objectname$");
        Name = this.__ResStr("modName", "$objectname$");
        Description = this.__ResStr("modSummary", "Displays an existing $objectnamelower$");
        DefaultViewName = StandardViews.Display;
        ShowTitleActions = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new $displaymodule$ModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_Display(string? url, $modelkey$ $modelkeynamelower$) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { $modelkeyname$ = $modelkeynamelower$ },
            Image = "#Display",
            LinkText = this.__ResStr("displayLink", "Display"),
            MenuText = this.__ResStr("displayText", "Display"),
            Tooltip = this.__ResStr("displayTooltip", "Display the $objectnamelower$"),
            Legend = this.__ResStr("displayLegend", "Displays an existing $objectnamelower$"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
        };
    }

    public class Model {
$iff $dp$ == SQLFile
        [Caption("$modelkeyname$"), Description("The $modelkeynamelower$ of this $objectnamelower$")]
        [UIHint("String")]
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

        public void SetData($modelname$ data) {
            ObjectSupport.CopyData(data, this);
        }
    }

    public async Task<ActionInfo> RenderModuleAsync($modelkey$ $modelkeynamelower$) {
        using ($modelname$DataProvider $modelnamelower$DP = new $modelname$DataProvider()) {
            $modelname$? data = await $modelnamelower$DP.GetItemAsync($modelkeynamelower$);
            if (data == null)
                throw new Error(this.__ResStr("notFound", "$objectname$ \"{0}\" not found"), $modelkeynamelower$);
            Model model = new Model();
            model.SetData(data);
            Module.Title = this.__ResStr("title", "$objectname$ \"{0}\"", $modelkeynamelower$);
            return await RenderAsync(model);
        }
    }
}
