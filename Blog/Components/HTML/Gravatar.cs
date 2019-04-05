/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Security;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Blog.Components {

    public abstract class GravatarComponentBase : YetaWFComponent {

        public const string TemplateName = "Gravatar";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

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
            return string.Format("//www.gravatar.com/avatar/{0}?s={1}&r={2}&d={3}", YetaWFManager.UrlEncodeSegment(email), gravatarSize, rating, defaultImage);
        }
    }

    public class GravatarDisplayComponent : GravatarComponentBase, IYetaWFComponent<string> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"<div class='yt_yetawf_blog_gravatar t_display'>");

            YTagBuilder tagLink = new YTagBuilder("a");
            YTagBuilder tagImg = new YTagBuilder("img");
            BlogConfigData config = await BlogConfigDataProvider.GetConfigAsync();
            string url = GravatarUrl(model, config.GravatarSize, config.GravatarRating, config.GravatarDefault);
            tagImg.Attributes.Add("src", url);
            tagImg.Attributes.Add("alt", this.__ResStr("altGravatar", "Gravatar image - {0}", model));
            hb.Append(tagImg.ToString(YTagRenderMode.StartTag));

            hb.Append($@"</div>");

            return hb.ToString();
        }
    }
}
