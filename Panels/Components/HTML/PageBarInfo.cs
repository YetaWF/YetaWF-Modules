/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Controllers;
using YetaWF.Modules.Panels.DataProvider;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Components {

    public abstract class PageBarInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "PageBarInfo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    /// <summary>
    /// This component is used by the YetaWF.Panels package and is not intended for use by an application.
    /// </summary>
    [PrivateComponent]
    public class PageBarInfoComponent : PageBarInfoComponentBase, IYetaWFComponent<PageBarInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public class Setup {
            public bool Resize { get; set; }
            public string ActiveCss { get; set; } = null!;
            public string ExpandCollapseUrl { get; set; } = null!;
        }

        public async Task<string> RenderAsync(PageBarInfo model) {
            HtmlBuilder hb = new HtmlBuilder();

            string? pane = model.ContentPane;

            string styleCss;
            string styleListCss = "";
            string activeCss;
            switch (model.Style) {
                default:
                case PageBarModule.PanelStyleEnum.Vertical:
                    styleCss = "t_stylesvert";
                    break;
                case PageBarModule.PanelStyleEnum.Horizontal:
                    styleCss = "t_styleshorz";
                    break;
            }
            activeCss = " t_active";

            string paneContents = "";
            if (model.ContentPage != null)
                paneContents = await model.ContentPage.RenderPaneAsync(HtmlHelper, string.IsNullOrWhiteSpace(pane) ? Globals.MainPane : pane, PaneDiv: false);

            if (PageBarDataProvider.GetExpanded())
                styleCss += " t_expanded";

            string pageUrl = Manager.CurrentPage.EvaluatedCanonicalUrl!;
            string pageUrlOnly;
            QueryHelper qh = QueryHelper.FromUrl(pageUrl, out pageUrlOnly);

            hb.Append($@"
<div class='yt_panels_pagebarinfo t_display {styleCss}' id='{ControlId}'>
    <div class='yt_panels_pagebarinfo_list{styleListCss}'>
        <div class='t_expcoll'></div>");

            foreach (PageBarInfo.PanelEntry entry in model.Panels) {

                string caption = entry.Caption.ToString();
                if (string.IsNullOrWhiteSpace(caption))
                    caption = this.__ResStr("emptyCaption", "(no caption - Page Title)");

                string active = "";
                if (model.ContentUri != null) {
                    Uri uriLink = new Uri(entry.Url);
                    if (uriLink.AbsolutePath.ToLower() == model.ContentUri.AbsolutePath.ToLower()) {
                        active = activeCss;
                    }
                }

                qh.Add("!ContentUrl", entry.Url, Replace: true);
                string anchor = $"";

                hb.Append($@"
        <a class='t_entry{active} yaction-link' data-contenttarget='{DivId}' data-contentpane='{HAE(pane == "" ? "MainPane" : pane)}' {Basics.CssSaveReturnUrl}='' href='{HAE(qh.ToUrl(pageUrlOnly))}' data-tooltip='{HAE(entry.ToolTip.ToString())}'>");

                if (entry.ImageSVG != null) {
                    hb.Append($@"
            {entry.ImageSVG}");
                } else {
                    hb.Append($@"
            <img src='{HAE(entry.ImageUrl)}' alt='{HAE(caption)}' title='{HAE(entry.ToolTip.ToString())}' />");
                }

                hb.Append($@"
            <div class='t_caption'>
                {HE(caption)}
            </div>
        </a>");

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
                ExpandCollapseUrl = Utility.UrlFor(typeof(PageBarInfoController), nameof(PageBarInfoController.SaveExpandCollapse)),
            };

            Manager.ScriptManager.AddLast($@"new YetaWF_Panels.PageBarInfoComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
