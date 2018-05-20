using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class DateTime : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            scripts.AddVolatileOption("YetaWF_ComponentsHTML", "DateTimeFormat", YetaWF.Core.Localize.Formatting.GetFormatDateTimeFormat());

            return Task.CompletedTask;
        }
    }
}
