using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Date : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            scripts.AddVolatileOption("YetaWF_ComponentsHTML", "DateFormat", YetaWF.Core.Localize.Formatting.GetFormatDateFormat());

            return Task.CompletedTask;
        }
    }
}
