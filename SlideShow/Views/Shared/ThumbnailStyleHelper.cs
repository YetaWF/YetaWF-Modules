/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SlideShow#License */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.SlideShow.Views.Shared {

    public class ThumbnailStyle<TModel> : RazorTemplate<TModel> { }

    public static class ThumbnailStyleHelper {

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ThumbnailStyleHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderThumbnailStyle(this HtmlHelper htmlHelper, string name, string selection, object HtmlAttributes = null) {

            List<SelectionItem<string>> list = (from a in Thumbnails select new SelectionItem<string>() {
                Text = a.Name,
                Value = a.Value,
            }).ToList();
            return htmlHelper.RenderDropDownSelectionList(name, selection, list, HtmlAttributes: HtmlAttributes);
        }
        public class Thumbnail {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public static List<Thumbnail> Thumbnails {
            get {
                List<Thumbnail> list = new List<Thumbnail>();
                Package package = YetaWF.Modules.SlideShow.Controllers.AreaRegistration.CurrentPackage;
                string rootUrl = VersionManager.GetAddOnModuleUrl(package.Domain, package.Product);
                string[] files = Directory.GetFiles(YetaWFManager.UrlToPhysical(rootUrl + "jssor/skins/thumb"), "*.css");
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
}
