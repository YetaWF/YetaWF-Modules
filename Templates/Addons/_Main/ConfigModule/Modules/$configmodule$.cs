using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using $companynamespace$.Modules.$projectnamespace$.DataProvider;

namespace $companynamespace$.Modules.$projectnamespace$.Modules;

public class $configmodule$ModuleDataProvider : ModuleDefinitionDataProvider<Guid, $configmodule$Module>, IInstallableModel { }

[ModuleGuid("{$configmoduleguid$}")]
[UniqueModule(UniqueModuleStyle.UniqueOnly)]
[ModuleCategory("Configuration")]
public class $configmodule$Module : ModuleDefinition {

    public $configmodule$Module() {
        Title = this.__ResStr("modTitle", "$objectname$");
        Name = this.__ResStr("modName", "$objectname$");
        Description = this.__ResStr("modSummary", "Edits a site's $objectnamelower$");
        ShowHelp = true;
        DefaultViewName = StandardViews.Config;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new $configmodule$ModuleDataProvider(); }
    public override DataProviderImpl GetConfigDataProvider() { return new $modelname$DataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    [Trim]
    public class Model {

        [Caption("A Property"), Description("A Property")]
        [UIHint("String"), StringLength(DataProvider.$modelname$.MaxProp1), Required]
        public string? Prop1 { get; set; }

        [Caption("A Property"), Description("A Property")]
        [UIHint("String"), StringLength(DataProvider.$modelname$.MaxProp2), Required]
        public string? Prop2 { get; set; }

        public $modelname$ GetData($modelname$ config) {
            ObjectSupport.CopyData(this, config);
            return config;
        }
        public void SetData($modelname$ config) {
            ObjectSupport.CopyData(config, this);
        }
        public Model() { }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using ($modelname$DataProvider $modelnamelower$DP = new $modelname$DataProvider()) {
            Model model = new Model { };
            $modelname$? config = await $modelnamelower$DP.GetItemAsync();
            if (config == null)
                throw new InternalError(this.__ResStr("notFound", "The $objectnamelower$ could not be found"));
            model.SetData(config);
            return await RenderAsync(model);
        }
    }

    [ExcludeDemoMode]
    public async Task<IResult> UpdateModuleAsync(Model model) {
        using ($modelname$DataProvider $modelnamelower$DP = new $modelname$DataProvider()) {
                    $modelname$ config = await $modelnamelower$DP.GetItemAsync();// get the original item
            if (!ModelState.IsValid)
                return await PartialViewAsync(model);
            config = model.GetData(config); // merge new config into original
            model.SetData(config); // and all the config back into model for final display
            await $modelnamelower$DP.UpdateConfigAsync(config);
            return await FormProcessedAsync(model, this.__ResStr("okSaved", "$objectname$ saved"));
        }
    }
}
