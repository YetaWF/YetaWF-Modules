using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using $companynamespace$.Modules.$projectnamespace$;

namespace $companynamespace$.Modules.$projectnamespace$.Addons;

public class Info : IAddOnSupport {

    public Task AddSupportAsync(YetaWFManager manager) {

        //ScriptManager scripts = manager.ScriptManager;
        //string areaName = AreaRegistration.CurrentPackage.AreaName;

        //scripts.AddConfigOption(areaName, "something", Something);

        //scripts.AddLocalization(areaName, "something", this.__ResStr("something", "something"));

        return Task.CompletedTask;
    }
}
