/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/FileSystem#License */

using System.Threading.Tasks;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.Startup {

    public class Application : IInitializeApplicationStartup {

        public Task InitializeApplicationStartupAsync(bool firstNode) {
            return Task.CompletedTask;
        }
    }
}
