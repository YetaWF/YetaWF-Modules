/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.SyntaxHighlighter.Controllers;

namespace YetaWF.Modules.SyntaxHighlighter.Support {

    public partial class SkinAccess : IInitializeApplicationStartup {

        private const string SyntaxHighlighterThemeFileMVC5 = "ThemelistMVC5.txt";
        private const string SyntaxHighlighterThemeFileMVC6 = "ThemelistMVC6.txt";

        public class SyntaxHighlighterTheme {
            public string Name { get; set; }
            public string File { get; set; }
            public string Description { get; set; }
        }

        public List<SyntaxHighlighterTheme> GetSyntaxHighlighterThemeList() {
            return SyntaxHighlighterThemeList;
        }
        private static List<SyntaxHighlighterTheme> SyntaxHighlighterThemeList { get; set; }
        private static SyntaxHighlighterTheme _syntaxHighlighterThemeDefault;

        private async Task LoadSyntaxHighlighterThemesAsync() {
            Package package = AreaRegistration.CurrentPackage;
            string url = VersionManager.GetAddOnNamedUrl(package.AreaName, "SkinSyntaxHighlighter");
            string customUrl = VersionManager.GetCustomUrlFromUrl(url);
            string path = YetaWFManager.UrlToPhysical(url);
            string customPath = YetaWFManager.UrlToPhysical(customUrl);

            // use custom or default theme list
            string themeFile;
            if (YetaWFManager.AspNetMvc == YetaWFManager.AspNetMvcVersion.MVC5)
                themeFile = SyntaxHighlighterThemeFileMVC5;
            else
                themeFile = SyntaxHighlighterThemeFileMVC6;
            string filename = Path.Combine(customPath, themeFile);
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                filename = Path.Combine(path, themeFile);

            List<string> lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(filename);
            List<SyntaxHighlighterTheme> syntaxHighlighterList = new List<SyntaxHighlighterTheme>();

            foreach (string line in lines) {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] s = line.Split(new char[] { ',' }, 3);
                string name = s[0].Trim();
                if (string.IsNullOrWhiteSpace(name)) throw new InternalError("Invalid/empty SyntaxHighlighter theme name");
                if (s.Length < 2)
                    throw new InternalError("Invalid SyntaxHighlighter theme entry: {0}", line);
                string file = s[1].Trim();
#if DEBUG // only validate files in debug builds
                if (file.StartsWith("\\")) {
                    string f = Path.Combine(YetaWFManager.RootFolder, file.Substring(1));
                    if (!await FileSystem.FileSystemProvider.FileExistsAsync(f))
                        throw new InternalError("SyntaxHighlighter theme file not found: {0} - {1}", line, f);
                } else {
                    string f = Path.Combine(path, file);
                    if (!await FileSystem.FileSystemProvider.FileExistsAsync(f))
                        throw new InternalError("SyntaxHighlighter theme file not found: {0} - {1}", line, f);
                }
#endif
                string description = null;
                if (s.Length > 2)
                    description = s[2].Trim();
                if (string.IsNullOrWhiteSpace(description))
                    description = null;
                syntaxHighlighterList.Add(new SyntaxHighlighterTheme {
                    Name = name,
                    Description = description,
                    File = file,
                });
            }
            if (syntaxHighlighterList.Count == 0)
                throw new InternalError("No SyntaxHighlighter themes found");

            _syntaxHighlighterThemeDefault = syntaxHighlighterList[0];
            SyntaxHighlighterThemeList = (from theme in syntaxHighlighterList orderby theme.Name select theme).ToList();
        }

        public string FindSyntaxHighlighterSkin(string themeName) {
            string intName = (from th in GetSyntaxHighlighterThemeList() where th.Name == themeName select th.File).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(intName))
                return intName;
            return _syntaxHighlighterThemeDefault.File;
        }
        public static string GetSyntaxHighlighterDefaultSkin() {
            SkinAccess skinAccess = new SkinAccess();
            skinAccess.GetSyntaxHighlighterThemeList();
            return _syntaxHighlighterThemeDefault.Name;
        }
    }
}
