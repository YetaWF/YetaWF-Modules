/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Panels.Addons {

    public class Info : IAddOnSupport {

        public const string Resource_AllowListOfLocalPagesAjax = "YetaWF_Panels-AllowListOfLocalPagesAjax";

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}