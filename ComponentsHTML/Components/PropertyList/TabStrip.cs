﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Models;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        public static YHtmlString RenderTabStripStart(string controlId) {
            return new YHtmlString("<ul class='t_tabstrip'>");
        }
        public static YHtmlString RenderTabStripEnd(string controlId) {
            return new YHtmlString("</ul>");
        }
        public static YHtmlString RenderTabPaneStart(string controlId, int panel, string cssClass = "") {
            if (!string.IsNullOrWhiteSpace(cssClass)) cssClass = " " + cssClass;
            return new YHtmlString($"<div class='t_table t_cat t_tabpanel{cssClass}' data-tab='{panel}' id='{controlId}_tab{panel}'>");
        }
        public static YHtmlString RenderTabPaneEnd(string controlId, int panel) {
            return new YHtmlString("</div>");
        }
        public static YHtmlString RenderTabEntry(string controlId, string label, string tooltip, int count) {
            HtmlBuilder hb = new HtmlBuilder();
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {
                string tabId = controlId + "_tab" + count.ToString();
                hb.Append("<li data-tab='{0}'><a href='#{1}' {2}='{3}'>{4}</a></li>", count, tabId, Basics.CssTooltip, YetaWFManager.HtmlAttributeEncode(tooltip), YetaWFManager.HtmlEncode(label));
            } else {
                hb.Append("<li data-tab='{0}' {1}='{2}'>{3}</li>", count, Basics.CssTooltip, YetaWFManager.HtmlAttributeEncode(tooltip), YetaWFManager.HtmlEncode(label));
            }
            return hb.ToYHtmlString();
        }
        public static async Task<YHtmlString> RenderTabInitAsync(string controlId, object model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append($@"
<script>");

            // About tab switching and registerPanelSwitched/processPanelSwitched
            // This event occurs even for the first tab, but event handlers may not yet be attached.
            // This event is only intended to notify you that an OTHER tab is now active, which may have updated div dimensions because they're now
            // visible. Divs on the first tab are already visible.  DO NOT use this event for initialization purposes.

            int activeTab = 0;
            string activeTabId = null;
            if (model != null) {
                // check if the model has an _ActiveTab property in which case we'll activate the tab and keep track of the active tab so it can be returned on submit
                if (ObjectSupport.TryGetPropertyValue<int>(model, "_ActiveTab", out activeTab, 0)) {
                    // add a hidden field for _ActiveTab property
                    activeTabId = Manager.UniqueId();
                    hb.Append($@"
    $('#{controlId}').append(""<input name='_ActiveTab' type='hidden' value='{activeTab}' id='{activeTabId}' >"");");
                }
            }
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {

                hb.Append($@"
    YetaWF_ComponentsHTML.PropertyListComponent.tabInitjQuery('{controlId}', {activeTab}, '{activeTabId}');");

            } else if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.Kendo) {

                await KendoUICore.AddFileAsync("kendo.data.min.js");
                await KendoUICore.AddFileAsync("kendo.tabstrip.min.js");
                hb.Append($@"
    YetaWF_ComponentsHTML.PropertyListComponent.tabInitKendo('{controlId}', {activeTab}, '{activeTabId}');");

            } else
                throw new InternalError("Unknown tab control style");

            hb.Append($@"
</script>");

            return hb.ToYHtmlString();
        }
    }
}
