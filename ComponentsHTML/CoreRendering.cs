/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
#if MVC6
#else
#endif

namespace YetaWF.Modules.ComponentsHTML {

    /// <summary>
    /// The CoreRendering class implements the YetaWF.Core.Components.IYetaWFCoreRendering interface required by YetaWF for component and view rendering.
    /// </summary>
    /// <remarks>This class installs the component and view rendering support in the framework class YetaWF.Core.Components.YetaWFCoreRendering during application startup.</remarks>
    public partial class CoreRendering : IInitializeApplicationStartup, IYetaWFCoreRendering {

        /// <summary>
        /// Called during application startup.
        /// </summary>
        /// <remarks>Installs the component and view rendering support in the framework class YetaWF.Core.Components.YetaWFCoreRendering.</remarks>
        public Task InitializeApplicationStartupAsync() {
            // install our core rendering support
            if (YetaWFManager.Manager.HostUsed != YetaWFManager.BATCHMODE && YetaWFManager.Manager.HostUsed != YetaWFManager.SERVICEMODE)
                YetaWF.Core.Components.YetaWFCoreRendering.Render = this;
            return Task.CompletedTask;
        }
    }
}
