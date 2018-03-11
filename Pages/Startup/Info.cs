/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Pages.Addons {

    public class Info : IAddOnSupport {

        public const string Resource_AllowListOfLocalPagesAjax = "YetaWF_Pages-AllowListOfLocalPagesAjax";

        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}