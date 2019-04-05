/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Models;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract partial class PropertyListComponentBase {

        /// <summary>
        /// Renders the starting portion of a tab strip used by the PropertyList component.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <returns>Returns the starting portion of a tab strip as HTML.</returns>
        public static string RenderTabStripStart(string controlId) {
            return "<ul class='t_tabstrip'>";
        }
        /// <summary>
        /// Renders the ending portion of a tab strip used by the PropertyList component.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <returns>Returns the ending portion of a tab strip as HTML.</returns>
        public static string RenderTabStripEnd(string controlId) {
            return "</ul>";
        }
        /// <summary>
        /// Renders the starting portion of a tab pane used by the PropertyList component.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <param name="panel">The zero-based panel index, one for each tab.</param>
        /// <param name="cssClass">The optional CSS class to add the the pane.</param>
        /// <returns>Returns the starting portion of a tab pane as HTML.</returns>
        public static string RenderTabPaneStart(string controlId, int panel, string cssClass = "") {
            if (!string.IsNullOrWhiteSpace(cssClass)) cssClass = " " + cssClass;
            return $"<div class='t_table t_cat t_tabpanel{cssClass}' data-tab='{panel}' id='{controlId}_tab{panel}'>";
        }
        /// <summary>
        /// Renders the ending portion of a tab pane used by the PropertyList component.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <param name="panel">The zero-based panel index, one for each tab.</param>
        /// <returns>Returns the ending portion of a tab pane as HTML.</returns>
        public static string RenderTabPaneEnd(string controlId, int panel) {
            return "</div>";
        }
        /// <summary>
        /// Renders a tab entry used by the PropertyList component.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <param name="count">The zero-based tab index, one for each tab.</param>
        /// <param name="label">The tab label.</param>
        /// <param name="tooltip">The tooltip for the tab.</param>
        /// <returns>Returns the tab entry as HTML.</returns>
        public static string RenderTabEntry(string controlId, string label, string tooltip, int count) {
            HtmlBuilder hb = new HtmlBuilder();
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {
                string tabId = controlId + "_tab" + count.ToString();
                hb.Append("<li data-tab='{0}'><a href='#{1}' {2}='{3}'>{4}</a></li>", count, tabId, Basics.CssTooltip, YetaWFManager.HtmlAttributeEncode(tooltip), YetaWFManager.HtmlEncode(label));
            } else {
                hb.Append("<li data-tab='{0}' {1}='{2}'>{3}</li>", count, Basics.CssTooltip, YetaWFManager.HtmlAttributeEncode(tooltip), YetaWFManager.HtmlEncode(label));
            }
            return hb.ToString();
        }
        /// <summary>
        /// Renders HTML, JavaScript that initializes a tab control.
        /// </summary>
        /// <param name="controlId">The ID of the &gt;div&lt; owning this tab control.</param>
        /// <param name="model">The model used to render the contents of the tab control.</param>
        /// <returns>Returns HTML, JavaScript.</returns>
        /// <remarks>
        /// If the provided <paramref name="model"/> has a property named _ActiveTab with a valid zero-based tab index, the
        /// specified tab is made the active tab.
        /// </remarks>
        public static async Task<string> RenderTabInitAsync(string controlId, object model) {

            ScriptBuilder sb = new ScriptBuilder();

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
                    sb.Append($@"
$('#{controlId}').append(""<input name='_ActiveTab' type='hidden' value='{activeTab}' id='{activeTabId}' >"");");
                }
            }
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {

                await JqueryUICore.UseAsync();

                sb.Append($@"
YetaWF_ComponentsHTML.PropertyListComponent.tabInitjQuery('{controlId}', {activeTab}, '{activeTabId}');");

            } else if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.Kendo) {

                await KendoUICore.AddFileAsync("kendo.data.min.js");
                await KendoUICore.AddFileAsync("kendo.tabstrip.min.js");
                sb.Append($@"
YetaWF_ComponentsHTML.PropertyListComponent.tabInitKendo('{controlId}', {activeTab}, '{activeTabId}');");

            } else
                throw new InternalError("Unknown tab control style");

            Manager.ScriptManager.AddLast(sb.ToString());

            return null;
        }
    }
}
