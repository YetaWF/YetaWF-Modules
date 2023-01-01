/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    internal class LockSingleProvider : ILockProvider {

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisposableTracker.RemoveObject(this);
            }
        }

        // Implementation

        public LockSingleProvider() {
            DisposableTracker.AddObject(this);
        }

        // API

        private class SingleLockObject : ILockObject {

            private string Key;
            private static List<string> LockKeys = new List<string>(); // single instance locked resources
            private static SemaphoreSlim localLock = new SemaphoreSlim(1, 1);// to protect LockKeys
            private bool LocalLocked = false;
            private bool Locked = false;

            public SingleLockObject(string key) {
                this.Key = key;
                DisposableTracker.AddObject(this);
            }
            internal async Task LockAsync() {
                if (Locked) throw new InternalError($"{nameof(LockAsync)} called while already holding a lock on {Key}");
                for (; !Locked;) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        if (!LockKeys.Contains(Key)) {
                            LockKeys.Add(Key);
                            Locked = true;
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
                for (; Locked;) {
                    if (YetaWFManager.IsSync()) {
                        if (localLock.Wait(10))
                            LocalLocked = true;
                    } else {
                        await localLock.WaitAsync();
                        LocalLocked = true;
                    }
                    if (LocalLocked) {
                        LockKeys.Remove(Key);
                        localLock.Release();
                        LocalLocked = false;
                        Locked = false;
                        break;
                    }
                    if (YetaWFManager.IsSync())
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 25));// wait a while - this is bad, only works because "other" instance has lock
                    else
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 25));// wait a while
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
            SingleLockObject lockObject = new SingleLockObject(key);
            await lockObject.LockAsync();
            return (ILockObject)lockObject;
        }
    }
}
