/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Search.Addons {

    public class Info : IAddOnSupport {

        public const string UrlArg = "!YetaWF_Search_SearchControl";

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddConfigOption(areaName, "UrlArg", UrlArg);

            return Task.CompletedTask;
        }
    }
}