/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using YetaWF.Core.Localize;
using YetaWF.Core.Security;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Blog.Views.Shared {

    public static class GravatarHelper {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GravatarHelper), name, defaultValue, parms); }
#if MVC6
        public static MvcHtmlString RenderGravatarDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#else
        public static MvcHtmlString RenderGravatarDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null)
#endif
        {
            TagBuilder tagLink = new TagBuilder("a");
            TagBuilder tagImg = new TagBuilder("img");
            BlogConfigData config = BlogConfigDataProvider.GetConfig();
            string url = GravatarUrl(model, config.GravatarSize, config.GravatarRating, config.GravatarDefault);
            tagImg.Attributes.Add("src", url);
            tagImg.Attributes.Add("alt", __ResStr("altGravatar", "Gravatar image - {0}", model));
            return MvcHtmlString.Create(tagImg.ToString(TagRenderMode.StartTag));
        }

        public static string GravatarUrl(string email, int gravatarSize, Gravatar.GravatarRatingEnum gravatarRating, Gravatar.GravatarEnum gravatarDefault) {
            MD5Crypto md5 = new MD5Crypto();
            email = md5.StringMD5(email);
            string rating;
            switch (gravatarRating) {
                default:
                case Core.Support.Gravatar.GravatarRatingEnum.G:
                    rating = "g";
                    break;
                case Core.Support.Gravatar.GravatarRatingEnum.PG:
                    rating = "pg";
                    break;
                case Core.Support.Gravatar.GravatarRatingEnum.R:
                    rating = "r";
                    break;
                case Core.Support.Gravatar.GravatarRatingEnum.X:
                    rating = "x";
                    break;
            }
            string defaultImage;
            switch (gravatarDefault) {
                case Core.Support.Gravatar.GravatarEnum.None:
                    defaultImage = "404"; break;
                case Core.Support.Gravatar.GravatarEnum.mm:
                    defaultImage = "mm"; break;
                case Core.Support.Gravatar.GravatarEnum.identicon:
                    defaultImage = "identicon"; break;
                case Core.Support.Gravatar.GravatarEnum.monsterid:
                    defaultImage = "monsterid"; break;
                default:
                case Core.Support.Gravatar.GravatarEnum.wavatar:
                    defaultImage = "wavatar"; break;
                case Core.Support.Gravatar.GravatarEnum.retro:
                    defaultImage = "retro"; break;
                case Core.Support.Gravatar.GravatarEnum.blank:
                    defaultImage = "blank"; break;
            }
            return string.Format("//www.gravatar.com/avatar/{0}?s={1}&r={2}&d={3}", email, gravatarSize, rating, defaultImage);
        }
    }
}