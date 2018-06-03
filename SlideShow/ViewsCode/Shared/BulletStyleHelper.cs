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
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.SlideShow.Views.Shared {

    public class BulletStyle<TModel> : RazorTemplate<TModel> { }

    public static class BulletStyleHelper {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(BulletStyleHelper), name, defaultValue, parms); }
#if MVC6
        public static async Task<HtmlString> RenderBulletStyleAsync(this IHtmlHelper htmlHelper, string name, string selection, object HtmlAttributes = null)
#else
        public static async Task<HtmlString> RenderBulletStyleAsync(this HtmlHelper htmlHelper, string name, string selection, object HtmlAttributes = null)
#endif
        {

            List<SelectionItem<string>> list = (from a in await GetBulletsAsync() select new SelectionItem<string>() {
                Text = a.Name,
                Value = a.Value,
            }).ToList();
            return new HtmlString("");
            //$$$return await htmlHelper.RenderDropDownSelectionListAsync(name, selection, list, HtmlAttributes: HtmlAttributes);
        }
        public class Bullet {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public static async Task<List<Bullet>> GetBulletsAsync() {
            List<Bullet> list = new List<Bullet>();
            Package package = YetaWF.Modules.SlideShow.Controllers.AreaRegistration.CurrentPackage;
            string rootUrl = VersionManager.GetAddOnPackageUrl(package.Domain, package.Product);
            List<string> files = await FileSystem.FileSystemProvider.GetFilesAsync(YetaWFManager.UrlToPhysical(rootUrl + "jssor/skins/bullet"), "*.css");
            foreach (var file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                list.Add(new Bullet {
                    Name = string.Format("Bullet {0}", name.Substring(1)),
                    Value = name,
                });
            }
            return list;
        }
    }
}
