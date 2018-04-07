/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SlideShow#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using System.Threading.Tasks;
using YetaWF.Core.IO;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SlideShow.Views.Shared {

    public class ThumbnailStyle<TModel> : RazorTemplate<TModel> { }

    public static class ThumbnailStyleHelper {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ThumbnailStyleHelper), name, defaultValue, parms); }
#if MVC6
        public static async Task<HtmlString> RenderThumbnailStyleAsync(this IHtmlHelper htmlHelper, string name, string selection, object HtmlAttributes = null)
#else
        public static async Task<HtmlString> RenderThumbnailStyleAsync(this HtmlHelper htmlHelper, string name, string selection, object HtmlAttributes = null)
#endif
        {

            List<SelectionItem<string>> list = (from a in await GetThumbnailsAsync() select new SelectionItem<string>() {
                Text = a.Name,
                Value = a.Value,
            }).ToList();
            return await htmlHelper.RenderDropDownSelectionListAsync(name, selection, list, HtmlAttributes: HtmlAttributes);
        }
        public class Thumbnail {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        private static async Task<List<Thumbnail>> GetThumbnailsAsync() {
            List<Thumbnail> list = new List<Thumbnail>();
            Package package = YetaWF.Modules.SlideShow.Controllers.AreaRegistration.CurrentPackage;
            string rootUrl = VersionManager.GetAddOnPackageUrl(package.Domain, package.Product);
            List<string> files = await FileSystem.FileSystemProvider.GetFilesAsync(YetaWFManager.UrlToPhysical(rootUrl + "jssor/skins/thumb"), "*.css");
            foreach (var file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                list.Add(new Thumbnail {
                    Name = string.Format("Thumbnail {0}", name.Substring(1)),
                    Value = name,
                });
            }
            return list;
        }
    }
}
