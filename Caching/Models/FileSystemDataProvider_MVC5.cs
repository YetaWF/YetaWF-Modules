/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

#if MVC6
#else

using YetaWF.Core.IO;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class FileSystemDataProvider : FileSystemDataProviderBase, IFileSystem {

        public FileSystemDataProvider(string rootFolder, bool Permanent) : base(rootFolder, Permanent) { }

    }
}

#endif