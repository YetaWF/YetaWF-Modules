/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyLanguage#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.TinyLanguage.Addons {

    public class Info : IAddOnSupport {

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}