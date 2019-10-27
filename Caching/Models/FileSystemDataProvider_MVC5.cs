/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

#if MVC6
#else

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class FileSystemDataProvider : FileSystemDataProviderBase, IFileSystem {

        public FileSystemDataProvider(string rootFolder, bool Permanent) : base(rootFolder, Permanent) { }

    }
}

#endif