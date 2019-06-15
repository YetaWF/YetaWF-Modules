/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feed#License */

using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Feed.Controllers;
using YetaWF.Modules.Feed.Modules;

namespace YetaWF.Modules.Feed.Views {

    public class FeedView : YetaWFView, IYetaWFView<FeedModule, FeedModuleController.DisplayModel> {

        public const string ViewName = "Feed";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public class Setup {
            public int Interval { get; set; }
        }

        public Task<string> RenderViewAsync(FeedModule module, FeedModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{DivId}' class='t_news'>
    <div class='t_header'>");

            if (model.Url != null) {

                hb.Append($@"
        <a class='t_title' href='{HAE(model.Url)}' data-tooltip='{HAE(model.Description)}' target='_blank' rel='noopener noreferrer'>
            {HE(model.Title)}
        </a>");

            } else {

                hb.Append($@"
        <span class='t_title' title='{HAE(model.Description)}>{HE(model.Title)}</span>");

            }

            hb.Append($@"
    </div>
    <div class='t_headerentry'></div>");

            foreach (FeedModuleController.Entry entry in model.Entries) {
                string author = null;
                foreach (FeedModuleController.Author a in entry.Authors) {
                    if (!string.IsNullOrWhiteSpace(a.Email)) {
                        if (string.IsNullOrWhiteSpace(author)) {
                            author += this.__ResStr("sep", " | ");
                        }
                        author += string.Format("<a href='mailto:{0}'>{1}</a>", a.Email, string.IsNullOrWhiteSpace(a.Name) ? a.Email : a.Name);
                    } else if (string.IsNullOrWhiteSpace(a.Url)) {
                        if (string.IsNullOrWhiteSpace(author)) {
                            author += this.__ResStr("sep", " | ");
                        }
                        author += string.Format("<a href='{0}'>{1}</a>", a.Url, string.IsNullOrWhiteSpace(a.Name) ? a.Email : a.Url);
                    } else if (string.IsNullOrWhiteSpace(a.Name)) {
                        author += a.Name;
                    }
                }

                hb.Append($@"
        <div class='t_entry'>
            <a class='t_title' href='{HAE(entry.Links.First())}' target='_blank' rel='noopener noreferrer' data-text='{HAE(entry.Description)}' data-publishedDate='{HAE(Formatting.FormatDate(entry.PublishDate))}' data-author='{HAE(author)}'>
                {HE(entry.Title)}
            </a>
        </div>");
            }

            Setup setup = new Setup {
                Interval = module.Interval * 1000,
            };

            hb.Append($@"
</div>");
            Manager.ScriptManager.AddLast($@"new YetaWF_Feed.Feed('{DivId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(hb.ToString());
        }
    }
}
