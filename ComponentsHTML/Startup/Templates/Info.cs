using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class DateTime : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            //string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "something", Something);

            //scripts.AddLocalization(areaName, "something", this.__ResStrxxx("something", "something"));

            scripts.AddVolatileOption("YetaWF_ComponentsHTML", "DateTimeFormat", YetaWF.Core.Localize.Formatting.GetFormatDateTimeFormat());

            return Task.CompletedTask;
        }
    }
}
