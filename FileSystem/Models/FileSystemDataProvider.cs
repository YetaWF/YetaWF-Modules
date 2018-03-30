/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/FileSystem#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Caching.DataProvider {

    public class SharedCacheVersion {

        public const int MaxKey = 100;

        [Data_PrimaryKey, Data_Index, StringLength(MaxKey)]
        public string Key { get; set; }
        public DateTime Created { get; set; }

        public SharedCacheVersion() { }
    }

    public class FileSystemDataProviderStartup : IExternalDataProvider {

        public FileSystemDataProviderStartup() { }

        // Registration

        public void Register() {

            // used so this is installed immediately
            // permanently created dataproviders (never disposed)
            Package package = YetaWF.Modules.FileSystem.Controllers.AreaRegistration.CurrentPackage;
            string permRootFolder = WebConfigHelper.GetValue(package.AreaName, "PermRootFolder", YetaWFManager.RootFolderWebProject);
            string tempRootFolder = WebConfigHelper.GetValue(package.AreaName, "TempRootFolder", YetaWFManager.RootFolderWebProject);

            YetaWF.Core.IO.FileSystem.FileSystemProvider = new FileSystemDataProvider(permRootFolder, Permanent: true);
            YetaWF.Core.IO.FileSystem.TempFileSystemProvider = new FileSystemDataProvider(tempRootFolder, Permanent: false);
        }
    }

    public class FileSystemDataProvider : IFileSystem {

        public string RootFolder { get; private set; }
        public bool Permanent { get; private set; }

        public const string LOCKFOLDER = "__LOCKS";

        // Implementation

        public FileSystemDataProvider(string rootFolder, bool Permanent) {
            RootFolder = rootFolder;
            this.Permanent = Permanent;
        }

        // API

        private class FileLockObject : IDisposable, IStaticLockObject {

            private string key;

            public FileLockObject(string key) {
                this.key = key;
                DisposableTracker.AddObject(this);
            }

            public Task UnlockAsync() {
                //$$
                return Task.CompletedTask;
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    if (YetaWF.Core.IO.Caching.MultiInstance) {
                        //if (... != null) {
                        //    DisposableTracker.RemoveObject(this);
                        //    YetaWFManager.Syncify(async () => // Only used if caller forgets to Unlock
                        //        //$$
                        //    );
                        //    ... = null;
                        //}
                    }
                }
            }
        }

        public async Task<IDisposable> LockResourceAsync(string fileOrFolder) {
            // If we're running in a single instance, no need for any locking.
            // On multi instances, we need a locking implementation. This is somewhat expensive. 
            // This is a concept implementation. A better implementation would be a file system SERVER which is shared by all instances.
            if (YetaWF.Core.IO.Caching.MultiInstance) {
                for (;;) {
                    try {
                        //$$$$
                        if (true)
                            break;
                    } catch (Exception) {
                        await Task.Delay(new TimeSpan(0, 0, 0, 50));// wait a while
                    }
                }
            }
            return new FileLockObject(fileOrFolder);
        }

        // Folders

        public async Task DeleteDirectoryAsync(string targetFolder) {
            if (!Directory.Exists(targetFolder)) return;// avoid exception spam

            int retry = 10; // folder occasionally are in use so we'll just wait a bit
            while (retry > 0) {
                try {
                    Directory.Delete(targetFolder, true);// recursive 
                    return;
                } catch (Exception exc) {
                    if (exc is DirectoryNotFoundException)
                        return;// done
                    if (retry <= 1)
                        throw;
                }
                await Task.Delay(new TimeSpan(0, 0, 0, 50));// wait a while
                --retry;
            }
        }

        public async Task CreateDirectoryAsync(string targetFolder) {
            if (Directory.Exists(targetFolder)) return;// avoid exception spam

            int retry = 10; // folder occasionally are in use so we'll just wait a bit
            while (retry > 0) {
                try {
                    Directory.CreateDirectory(targetFolder);
                    return;
                } catch (Exception) {
                    if (retry <= 1)
                        throw;
                }
                await Task.Delay(new TimeSpan(0, 0, 0, 50));// wait a while
                --retry;
            }
        }

        public Task<bool> DirectoryExistsAsync(string targetFolder) {
            if (Directory.Exists(targetFolder))
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task<List<string>> GetDirectoriesAsync(string targetFolder, string pattern = null) {
            List<string> files;
            if (pattern == null)
                files = Directory.GetDirectories(targetFolder).ToList();
            else
                files = Directory.GetDirectories(targetFolder, pattern).ToList();
            return Task.FromResult(files);
        }

        public Task<List<string>> GetFilesAsync(string targetFolder, string pattern = null) {
            List<string> files;
            if (pattern == null)
                files = Directory.GetFiles(targetFolder).ToList();
            else
                files = Directory.GetFiles(targetFolder, pattern).ToList();
            return Task.FromResult(files);
        }

        public Task<DateTime> GetDirectoryCreationTimeUtcAsync(string targetFolder) {
            return Task.FromResult(Directory.GetCreationTimeUtc(targetFolder));
        }

        // Files

        public Task<bool> FileExistsAsync(string filePath) {
            if (File.Exists(filePath))
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task<DateTime> GetCreationTimeUtcAsync(string filePath) {
            return Task.FromResult(File.GetCreationTimeUtc(filePath));
        }

        public Task<DateTime> GetLastWriteTimeUtcAsync(string filePath) {
            return Task.FromResult(File.GetLastWriteTimeUtc(filePath));
        }

        public Task SetLastWriteTimeUtcAsync(string filePath, DateTime timeUtc) {
            File.SetLastWriteTimeUtc(filePath, timeUtc);
            return Task.CompletedTask;
        }

        public Task SetLastWriteTimeLocalAsync(string filePath, DateTime timeLocal) {
            File.SetLastWriteTime(filePath, timeLocal);
            return Task.CompletedTask;
        }

        public Task<List<string>> ReadAllLinesAsync(string filePath) {
            List<string> lines = File.ReadLines(filePath).ToList();
            return Task.FromResult(lines);
        }

        public Task WriteAllLinesAsync(string filePath, List<string> lines) {
            File.WriteAllLines(filePath, lines);
            return Task.CompletedTask;
        }

        public Task<string> ReadAllTextAsync(string filePath) {
            string text = File.ReadAllText(filePath);
            return Task.FromResult(text);
        }

        public Task WriteAllTextAsync(string filePath, string text) {
            File.WriteAllText(filePath, text);
            return Task.CompletedTask;
        }

        public Task AppendAllTextAsync(string filePath, string text) {
            File.AppendAllText(filePath, text);
            return Task.CompletedTask;
        }

        public Task AppendAllLinesAsync(string filePath, List<string> lines) {
            File.AppendAllLines(filePath, lines);
            return Task.CompletedTask;
        }

        public Task<byte[]> ReadAllBytesAsync(string filePath) {
            byte[] data = File.ReadAllBytes(filePath);
            return Task.FromResult(data);
        }

        public Task WriteAllBytesAsync(string filePath, byte[] data) {
            File.WriteAllBytes(filePath, data);
            return Task.CompletedTask;
        }

        public Task MoveFileAsync(string fromPath, string toPath) {
            File.Move(fromPath, toPath);
            return Task.CompletedTask;
        }

        public Task CopyFileAsync(string fromPath, string toPath) {
            File.Copy(fromPath, toPath, true);
            return Task.CompletedTask;
        }

        public Task DeleteFileAsync(string filePath) {
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        // FileStream

        public class FileStream : IDisposable, IFileStream {

            private string FilePath;
            private FileMode Mode;
            private bool Closed;
            private System.IO.FileStream Stream;

            public FileStream(string filePath, FileMode mode) {
                FilePath = filePath;
                Closed = false;
                Mode = mode;
                Stream = new System.IO.FileStream(filePath, Mode);
                DisposableTracker.AddObject(this);
            }
            public System.IO.FileStream GetFileStream() {
                return Stream;
            }
            public long GetLength() {
                return Stream.Length;
            }
            public Task<int> ReadAsync(byte[] btes, int offset, int length) {
                if (YetaWFManager.IsSync())
                    return Task.FromResult(Stream.Read(btes, offset, length));
                else
                    return Stream.ReadAsync(btes, offset, length);
            }
            public Task WriteAsync(byte[] btes, int offset, int length) {
                if (YetaWFManager.IsSync()) {
                    Stream.Write(btes, offset, length);
                    return Task.CompletedTask;
                } else
                    return Stream.ReadAsync(btes, offset, length);
            }
            public Task FlushAsync() {
                if (YetaWFManager.IsSync()) {
                    Stream.Flush();
                    return Task.CompletedTask;
                } else
                    return Stream.FlushAsync();
            }

            public Task CloseAsync() {
                if (!Closed) {
                    Stream.Close();
                    DisposableTracker.RemoveObject(this);
                }
                Closed = true;
                return Task.CompletedTask;
            }
            protected void Close() {
                if (!Closed) {
                    Stream.Close();
                    DisposableTracker.RemoveObject(this);
                }
                Closed = true;
            }

            public void Dispose() { Dispose(true); }
            protected virtual void Dispose(bool disposing) {
                if (disposing) {
                    Close();
                }
            }
        }

        // FileNotFoundException, DirectoryNotFoundException
        public Task<IFileStream> OpenFileStreamAsync(string filePath) {
            return Task.FromResult((IFileStream) new FileStream(filePath, FileMode.Open));
        }
        public Task<IFileStream> CreateFileStreamAsync(string filePath) {
            return Task.FromResult((IFileStream)new FileStream(filePath, FileMode.Create));
        }
    }
}
