/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyBar#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.TinyBar.Addons;

public class Info : IAddOnSupport {

    public Task AddSupportAsync(YetaWFManager manager) {
        return Task.CompletedTask;
    }
}