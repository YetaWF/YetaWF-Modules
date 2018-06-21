/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
            return new YHtmlString($"<div class='t_table t_cat t_tabpanel{cssClass}' data-tab='{controlId}_tab{panel}' id='{controlId}_tab{panel}'>");
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

            ScriptBuilder sb = new ScriptBuilder();
            // About tab switching and YetaWF_PropertyList_PanelSwitched
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
                    sb.Append(@"$('#{0}').append(""<input name='_ActiveTab' type='hidden' value='{1}' id='{2}'/>"");", controlId, activeTab, activeTabId);
                }
            }
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {
                sb.Append($"YetaWF_PropertyList.tabInitjQuery('{controlId}', {activeTab}, '{activeTabId}');\n");
            } else if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.Kendo) {
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.data.min.js");
                await Manager.ScriptManager.AddKendoUICoreJsFileAsync("kendo.tabstrip.min.js");
                sb.Append($"YetaWF_PropertyList.tabInitKendo('{controlId}', {activeTab}, '{activeTabId}');\n");
            } else
                throw new InternalError("Unknown tab control style");

            if (Manager.CurrentSite.JSLocation == YetaWF.Core.Site.JSLocationEnum.Top)
                return Manager.ScriptManager.AddNow(sb.ToString()).ToYHtmlString();
            else {
                Manager.ScriptManager.AddLast(sb.ToString());
                return new YHtmlString("");
            }
        }
    }
}
