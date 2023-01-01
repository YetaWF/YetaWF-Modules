/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Models.Attributes;

namespace YetaWF.Modules.Caching.DataProvider {

    /// <summary>
    /// Represents the object stored in cache containing just the version information.
    /// </summary>
    public class SharedCacheVersion {

        /// <summary>
        /// The maximum key length.
        /// </summary>
        public const int MaxKey = 200;

        /// <summary>
        /// The key.
        /// </summary>
        [Data_PrimaryKey, Data_Index, StringLength(MaxKey)]
        public string Key { get; set; } = null!;
        /// <summary>
        /// The timestamp when the version information was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SharedCacheVersion() { }
    }

    /// <summary>
    /// The object as persisted in shared cache.
    /// </summary>
    internal class SharedCacheObject : SharedCacheVersion {

        [Data_Binary]
        public byte[] Value { get; set; } = null!;

        public SharedCacheObject() { }
    }

    /// <summary>
    /// The object as persisted in local cache, representing a local copy of the object in shared cache
    /// </summary>
    internal class LocalSharedCacheObject {

        public string Key { get; set; } = null!;
        public byte[]? Value { get; set; }
        public DateTime Created { get; set; }

        public LocalSharedCacheObject() { }
    }
}
