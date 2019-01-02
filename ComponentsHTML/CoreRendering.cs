/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering : IInitializeApplicationStartup, IYetaWFCoreRendering {

        public Task InitializeApplicationStartupAsync() {
            // install our core rendering support
            YetaWF.Core.Components.YetaWFCoreRendering.Render = this;
            return Task.CompletedTask;
        }
    }
}
