/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    public class Currency : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            scripts.AddVolatileOption("YetaWF_ComponentsHTML", "CurrencyFormat", YetaWF.Core.Localize.Formatting.GetFormatCurrencyFormat());

            return Task.CompletedTask;
        }
    }
}
