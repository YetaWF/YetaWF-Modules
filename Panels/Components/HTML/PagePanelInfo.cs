using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.Panels.Models;
using YetaWF.Modules.Panels.Modules;

namespace YetaWF.Modules.Panels.Components {

    public abstract class PagePanelInfoComponentBase : YetaWFComponent {

        public const string TemplateName = "PagePanelInfo";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class PagePanelInfoDisplayComponent : PagePanelInfoComponentBase, IYetaWFComponent<PagePanelInfo> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(PagePanelInfo model) {
            HtmlBuilder hb = new HtmlBuilder();

            string styleCss;
            switch (model.Style) {
                default:
                case PagePanelModule.PanelStyleEnum.Default:
                    styleCss = "t_styledefault";
                    break;
                case PagePanelModule.PanelStyleEnum.SmallVertical:
                    styleCss = "t_stylesmallvert";
                    break;
                case PagePanelModule.PanelStyleEnum.SmallTable:
                    styleCss = "t_stylesmalltable";
                    break;
            }

            hb.Append($@"
<div class='yt_panels_pagepanelinfo t_display {styleCss}' id='{ControlId}'>");


            foreach (PagePanelInfo.PanelEntry entry in model.Panels) {
                string caption = entry.Caption.ToString();
                if (string.IsNullOrWhiteSpace(caption))
                    caption = this.__ResStr("emptyCaption", "(no caption - Page Title)");
                string actionLinkClass = "yaction-link";
                if (entry.Popup) {
                    actionLinkClass += string.Format(" {0}", Basics.CssPopupLink);
                    await Manager.AddOnManager.AddAddOnNamedAsync("YetaWF", "Core", "Popups");
                }

                hb.Append($@"
    <div class='t_entry'>
        <div class='t_image'>
            <a class='{actionLinkClass}' {Basics.CssSaveReturnUrl}='' href='{YetaWFManager.HtmlAttributeEncode(entry.Url)}' data-tooltip='{YetaWFManager.HtmlAttributeEncode(entry.ToolTip.ToString())}'>
                <img src='{YetaWFManager.HtmlAttributeEncode(entry.ImageUrl)}' alt='{YetaWFManager.HtmlAttributeEncode(caption)}' title='{YetaWFManager.HtmlAttributeEncode(entry.ToolTip.ToString())}' />
            </a>
        </div>
        <div class='t_link'>
            <a class='{actionLinkClass}' {Basics.CssSaveReturnUrl}='' href='{YetaWFManager.HtmlAttributeEncode(entry.Url)}' data-tooltip='{YetaWFManager.HtmlAttributeEncode(entry.ToolTip.ToString())}'>{YetaWFManager.HtmlEncode(caption)}</a>
        </div>");

                if (model.Style == PagePanelModule.PanelStyleEnum.SmallTable) {
                    hb.Append($@"<div class='t_desc'>{YetaWFManager.HtmlEncode(entry.ToolTip.ToString())}</div>");
                }
                hb.Append($@"
    </div>");

            }
            hb.Append($@"
</div>");
            return hb.ToYHtmlString();
        }
    }
}
