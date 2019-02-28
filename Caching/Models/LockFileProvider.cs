/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class LockFileProvider : ILockProvider {

        public static string RootFolder { get; private set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        // Implementation

        public LockFileProvider(string rootFolder) {
            RootFolder = rootFolder;
            DisposableTracker.AddObject(this);
        }

        // API

        private class MultiLockObject : ILockObject {

            private string Key;
            private string LockFile;
            private System.IO.FileStream FileStream;
            private static SemaphoreSlim localLock = new SemaphoreSlim(1, 1);// to protect local instance
            private bool LocalLocked = false;
            private bool Locked = false;

            public MultiLockObject(string key) {
                Key = key;
                LockFile = Path.Combine(RootFolder, "__LOCK__" + key);
                DisposableTracker.AddObject(this);
            }
            internal async Task LockAsync() {
                if (Locked) throw new InternalError($"{nameof(LockAsync)} called while already holding a lock on {Key}");
                for ( ; !Locked; ) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        try {
                            FileStream = new System.IO.FileStream(LockFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                            if (YetaWFManager.IsSync()) {
                                FileStream.Write(new byte[] { 99 }, 0, 1);
                            } else {
                                await FileStream.WriteAsync(new byte[] { 99 }, 0, 1);
                            }
                            Locked = true;
                        } catch (Exception) { }
                        finally {
                            if (!Locked) {
                                if (FileStream != null) {
                                    FileStream.Close();
                                    FileStream = null;
                                }
                            }
                        }
                        localLock.Release();
                        LocalLocked = false;
                        if (Locked)
                            break;
                    }
                    if (YetaWFManager.IsSync())
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 25));// wait a while - this is bad, only works because "other" instance has lock
                    else
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 25));// wait a while
                }
            }
            public async Task UnlockAsync() {
                if (Locked) {
                    FileStream.Close();
                    Locked = false;
                    try {
                        await YetaWF.Core.IO.FileSystem.FileSystemProvider.DeleteFileAsync(LockFile);
                    } catch (Exception) { }// this could fail if another task already obtained the lock
                }
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    DisposableTracker.RemoveObject(this);
                    YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                        await UnlockAsync()
                    );
                }
            }
        }
        /// <summary>
        /// Lock a key by name until disposed.
        /// </summary>
        public async Task<ILockObject> LockResourceAsync(string key) {
            key = key.Replace("\\", "++"); // turn it into a valid file name
            key = key.Replace(":", "++"); // turn it into a valid file name

            // If we're running in a single instance, a simple lock by name is sufficient.
            // On multi instances, we need a locking implementation using a lock file. This is somewhat expensive.
            MultiLockObject lockObject = new MultiLockObject(key);
            await lockObject.LockAsync();
            return (ILockObject)lockObject;
        }
    }
}
