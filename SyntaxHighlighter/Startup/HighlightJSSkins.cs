/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Support;

namespace YetaWF.Modules.SyntaxHighlighter.Startup {

    public partial class SkinAccess : IInitializeApplicationStartup {

        /// <summary>
        /// Called when any node of a (single- or multi-instance) site is starting up.
        /// </summary>
        public async Task InitializeApplicationStartupAsync() {
            await LoadHighlightJSThemesAsync();
        }

        public class HighlightJSTheme {
            public string Name { get; set; } = null!;
            public string File { get; set; } = null!;
        }

        public List<HighlightJSTheme> GetHighlightJSThemeList() {
            return _HighlightJSThemeList;
        }
        private static List<HighlightJSTheme> _HighlightJSThemeList = null!;
        private static string _HighlightJSThemeDefault = "default";

        private async Task LoadHighlightJSThemesAsync() {
            string themeUrl = "/node_modules/@highlightjs/cdn-assets/styles";
            string themePath = Utility.UrlToPhysical(themeUrl);
            List<string> files = await FileSystem.FileSystemProvider.GetFilesAsync(themePath, "*.css");
            files = files.Where((f) => f.EndsWith(".min.css")).ToList();// remove non-minified versions

            _HighlightJSThemeList = (from f in files select new HighlightJSTheme { Name = Path.GetFileNameWithoutExtension(f).TrimEnd(".min"), File = f }).ToList();
            if (_HighlightJSThemeList.Count == 0)
                throw new InternalError("No HighlightJS themes found");
        }

        public static string GetHighlightJSDefaultTheme() {
            return _HighlightJSThemeDefault;
        }
    }
}
