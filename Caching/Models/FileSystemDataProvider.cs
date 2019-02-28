/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Caching#License */

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

    /// <summary>
    /// An instance of this class is instantiated during application startup and registers low-level data providers
    /// providing file system I/O. All file I/O in YetaWF uses a low-level data provider for file and folder I/O.
    /// Operating system APIs should not be used by applications for file and folder I/O.
    /// </summary>
    /// <remarks>
    /// YetaWF offer a temporary and a permanent file system interface.
    /// Files stored in the temporary file system are lost when the application restarts.
    /// The permanent file system preserves files when the application restarts.
    ///
    /// The temporary and a permanent file systems support single- and multi-instance sites.
    /// All nodes in a multi-instance site must use the same physical file system, shared between sites.
    ///
    /// Applications do not access these low-level data providers directly.
    /// File system services provided by YetaWF.Core.IO.FileSystem, YetaWF.Core.IO.DataFilesProvider and YetaWF.Core.IO.FileIO&lt;TObj&gt; should be used instead.
    /// </remarks>
    public class FileSystemDataProviderStartup : IExternalDataProvider {

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileSystemDataProviderStartup() { }

        // Registration

        /// <summary>
        /// Called by the framework to register external data providers that expose the YetaWF.Core.DataProvider.IExternalDataProvider interface.
        /// </summary>
        public void Register() {

            // used so this is installed immediately
            // permanently created data providers (never disposed)
            Package package = YetaWF.Modules.Caching.Controllers.AreaRegistration.CurrentPackage;
            string permRootFolder = WebConfigHelper.GetValue(package.AreaName, "PermRootFolder", YetaWFManager.RootFolderWebProject);
            string tempRootFolder = WebConfigHelper.GetValue(package.AreaName, "TempRootFolder", YetaWFManager.RootFolderWebProject);

            YetaWF.Core.IO.FileSystem.FileSystemProvider = new FileSystemDataProvider(permRootFolder, Permanent: true);
            YetaWF.Core.IO.FileSystem.TempFileSystemProvider = new FileSystemDataProvider(tempRootFolder, Permanent: false);
        }
    }

    internal class FileSystemDataProvider : IFileSystem {

        public string RootFolder { get; private set; }
        public bool Permanent { get; private set; }

        // Implementation

        public FileSystemDataProvider(string rootFolder, bool Permanent) {
            RootFolder = rootFolder;
            this.Permanent = Permanent;
        }

        // API

        /// <summary>
        /// Lock a file/folder by name until disposed.
        /// </summary>
        public async Task<ILockObject> LockResourceAsync(string fileOrFolder) {
            return await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync(fileOrFolder);
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
                if (YetaWFManager.IsSync())
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50));// wait a while - this is bad, only works because "other" instance has lock
                else
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 50));// wait a while
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
                if (YetaWFManager.IsSync())
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50));// wait a while - this is bad, only works because "other" instance has lock
                else
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 50));// wait a while
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

        // Data Files

        private static string ValidChars = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~`!@#$^&()_-+={}[],.";
        private const string FileExtension = ".dat";

        public string GetDataFileExtension() {
            return FileExtension;
        }

        public string MakeValidDataFileName(string name) {
            StringBuilder sb = new StringBuilder();
            foreach (var c in name) {
                if (ValidChars.Contains(c))
                    sb.Append(c);
                else if (c == '%')
                    sb.Append("%%");
                else
                    sb.Append(string.Format("%{0:x2}", (int)c));
            }
            sb.Append(FileExtension);
            return sb.ToString();
        }

        public string ExtractNameFromDataFileName(string name) {
            StringBuilder sb = new StringBuilder();
            int total = name.Length;
            if (name.EndsWith(FileExtension)) total -= FileExtension.Length;
            for (int i = 0; i < total; ++i) {
                char c = name[i];
                if (c == '%') {
                    if (i + 1 < total && name[i + 1] == '%') {
                        sb.Append('%');
                        i += 1;
                    } else if (i + 2 < total) {
                        string hex = name.Substring(i + 1, 2);
                        int value = (int)'*';
                        try {
                            value = Convert.ToInt32(hex, 16);
                        } catch (Exception) { }
                        sb.Append((char)value);
                        i += 2;
                    }
                } else
                    sb.Append(c);
            }
            return sb.ToString();
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
                    Closed = true;
                }
                return Task.CompletedTask;
            }
            protected void Close() {
                if (!Closed) {
                    Stream.Close();
                    DisposableTracker.RemoveObject(this);
                    Closed = true;
                }
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
