/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.SyntaxHighlighter.Controllers;

namespace YetaWF.Modules.SyntaxHighlighter.Support {

    public partial class SkinAccess {

        private const string HighlightJSThemeFile = "Themelist.txt";
        private const string HighlightJSThemeFileMVC6 = "ThemelistMVC6.txt";

        public class HighlightJSTheme {
            public string Name { get; set; }
        }

        public List<HighlightJSTheme> GetHighlightJSThemeList() {
            if (_HighlightJSThemeList == null)
                LoadHighlightJSThemes();
            return _HighlightJSThemeList;
        }
        private static List<HighlightJSTheme> _HighlightJSThemeList;
        private static HighlightJSTheme _HighlightJSThemeDefault;

        private List<HighlightJSTheme> LoadHighlightJSThemes() {
            Package package = AreaRegistration.CurrentPackage;
            string url = VersionManager.GetAddOnNamedUrl(package.Domain, package.Product, "SkinHighlightJS");
            string customUrl = VersionManager.GetCustomUrlFromUrl(url);
            string path = YetaWFManager.UrlToPhysical(url);
            string customPath = YetaWFManager.UrlToPhysical(customUrl);

            // use custom or default theme list
            string themeFile = HighlightJSThemeFile;
            string filename = Path.Combine(customPath, themeFile);
            if (!File.Exists(filename))
                filename = Path.Combine(path, themeFile);

            string[] lines = File.ReadAllLines(filename);
            List<HighlightJSTheme> HighlightJSList = new List<HighlightJSTheme>();

            foreach (string line in lines) {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string name = line.Trim();
#if DEBUG // only validate files in debug builds
                string f = Path.Combine(path, "themes", name) + ".css";
                if (!File.Exists(f))
                    throw new InternalError("HighlightJS theme file not found: {0} - {1}", line, f);
#endif
                HighlightJSList.Add(new HighlightJSTheme {
                    Name = name,
                });
            }
            if (HighlightJSList.Count == 0)
                throw new InternalError("No HighlightJS themes found");

            _HighlightJSThemeDefault = HighlightJSList[0];
            _HighlightJSThemeList = (from theme in HighlightJSList orderby theme.Name select theme).ToList();
            return _HighlightJSThemeList;
        }

        public string FindHighlightJSSkin(string themeName) {
            string intName = (from th in GetHighlightJSThemeList() where th.Name == themeName select th.Name).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(intName))
                return intName;
            return _HighlightJSThemeDefault.Name;
        }
        public static string GetHighlightJSDefaultSkin() {
            SkinAccess skinAccess = new SkinAccess();
            skinAccess.GetHighlightJSThemeList();
            return _HighlightJSThemeDefault.Name;
        }
    }
}
