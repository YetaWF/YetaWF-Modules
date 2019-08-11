/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Components {

    public abstract class PageBarInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "PageBarInfo";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PageBarInfoComponent : PageBarInfoComponentBase, IYetaWFComponent<PageBarInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Setup {
            public bool Resize { get; set; }
            public string ActiveCss { get; set; }
        }

        public async Task<string> RenderAsync(PageBarInfo model) {
            HtmlBuilder hb = new HtmlBuilder();

            string pane = model.ContentPane;

            string styleCss;
            switch (model.Style) {
                default:
                case PageBarModule.PanelStyleEnum.Vertical:
                    styleCss = "t_stylesvert";
                    break;
                case PageBarModule.PanelStyleEnum.Horizontal:
                    styleCss = "t_styleshorz";
                    break;
            }
            if (model.UseSkinFormatting)
                styleCss += " t_skin";
            else
                styleCss += " t_noskin";

            string styleListCss = "";
            if (model.UseSkinFormatting)
                styleListCss = " ui-widget-content";

            string activeCss;
            if (model.UseSkinFormatting)
                activeCss = " t_active ui-state-active";
            else
                activeCss = " t_active";

            // Current page contents
            string paneContents = "";
            string contentUrl;
            Uri contentUri = null;
            Manager.TryGetUrlArg<string>("!ContentUrl", out contentUrl);
            if (!string.IsNullOrWhiteSpace(contentUrl)) {
                contentUri = new Uri(contentUrl);
            } else {
                if (model.Panels.Count > 0) {
                    contentUri = new Uri(model.Panels[0].Url);
                }
            }
            if (contentUri != null) {
                PageDefinition page = await PageDefinition.LoadFromUrlAsync(contentUri.AbsolutePath);
                if (page != null && page.IsAuthorized_View()) {
                    paneContents = await page.RenderPaneAsync(HtmlHelper, pane == "" ? Globals.MainPane : pane);
                }
            }

            string pageUrl = Manager.CurrentPage.EvaluatedCanonicalUrl;
            string pageUrlOnly;
            QueryHelper qh = QueryHelper.FromUrl(pageUrl, out pageUrlOnly);

            hb.Append($@"
<div class='yt_panels_pagebarinfo t_display {styleCss}' id='{ControlId}'>
    <div class='t_list{styleListCss}'>");

            foreach (PageBarInfo.PanelEntry entry in model.Panels) {

                string caption = entry.Caption.ToString();
                if (string.IsNullOrWhiteSpace(caption))
                    caption = this.__ResStr("emptyCaption", "(no caption - Page Title)");
                string actionLinkClass = "yaction-link";

                string active = "";
                if (contentUri != null) {
                    Uri uriLink = new Uri(entry.Url);
                    if (uriLink.AbsolutePath == contentUri.AbsolutePath) {
                        active = activeCss;
                    }
                }

                qh.Add("!ContentUrl", entry.Url, Replace: true);
                string anchor = $"<a class='{actionLinkClass}' data-contenttarget='{DivId}' data-contentpane='{HAE(pane == "" ? "MainPane" : pane)}' {Basics.CssSaveReturnUrl}='' href='{HAE(qh.ToUrl(pageUrlOnly))}' data-tooltip='{HAE(entry.ToolTip.ToString())}'>";

                hb.Append($@"
        <div class='t_entry{active}'>
            <div class='t_image'>
                {anchor}
                    <img src='{HAE(entry.ImageUrl)}' alt='{HAE(caption)}' title='{HAE(entry.ToolTip.ToString())}' />
                </a>
            </div>
            <div class='t_link'>
                {anchor}
                {HE(caption)}</a>
            </div>
        </div>");

            }

            hb.Append($@"
    </div>
    <div class='t_area' id='{DivId}'>
        {paneContents}
    </div>
</div>");

            Setup setup = new Setup {
                Resize = model.Style == PageBarModule.PanelStyleEnum.Vertical,
                ActiveCss = activeCss,
            };

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.PageBarInfoComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
