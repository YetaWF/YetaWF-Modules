/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Localize;
using YetaWF.Core.Security;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;

namespace YetaWF.Modules.Blog.Views.Shared {

    public static class GravatarHelper {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GravatarHelper), name, defaultValue, parms); }

        public static MvcHtmlString RenderGravatarDisplay<TModel>(this HtmlHelper<TModel> htmlHelper, string name, string model, object HtmlAttributes = null) {

            BlogConfigData config = BlogConfigDataProvider.GetConfig();

            MD5Crypto md5 = new MD5Crypto();
            string email = md5.StringMD5(model);
            string rating;
            switch (config.GravatarRating) {
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
            switch (config.GravatarDefault) {
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
            TagBuilder tagLink = new TagBuilder("a");

            TagBuilder tagImg = new TagBuilder("img");
            string src = string.Format("//www.gravatar.com/avatar/{0}?s={1}&r={2}&d={3}", email, config.GravatarSize, rating, defaultImage);
            tagImg.Attributes.Add("src", src);
            tagImg.Attributes.Add("alt", __ResStr("altGravatar", "Gravatar image"));

            return MvcHtmlString.Create(tagImg.ToString(TagRenderMode.StartTag));
        }
    }
}