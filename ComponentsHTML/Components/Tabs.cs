/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Newtonsoft.Json;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Tabs component implementation.
    /// </summary>
    public abstract class TabsComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TabsComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Tabs";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    internal class TabsSetup {
        public TabStyleEnum TabStyle { get; set; }
        public int ActiveTabIndex { get; set; }
        public string ActiveTabHiddenId { get; set; }
        public TabsSetup() { }
    }

    /// <summary>
    /// Displays a tab component. The model defines various attributes of the tab component.
    /// </summary>
    /// <example>
    /// [UIHint("Tabs")]
    /// public TabsDefinition TabsDef { get; set; }
    /// </example>
    public partial class TabsDisplayComponent : TabsComponentBase, IYetaWFComponent<TabsDefinition> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {
                await JqueryUICore.UseAsync();
            } else if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.Kendo) {
                await KendoUICore.AddFileAsync("kendo.data.min.js");
                await KendoUICore.AddFileAsync("kendo.tabstrip.min.js");
            } else
                throw new InternalError("Unknown tab control style");
            await base.IncludeAsync();
        }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(TabsDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            string tabsCss = Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery ? "t_jquery ui-tabs ui-corner-all ui-widget ui-widget-content" : "t_kendo k-widget k-header k-tabstrip k-floatwrap k-tabstrip-top";
            string stripCss = Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery ? " ui-tabs-nav ui-corner-all ui-helper-reset ui-helper-clearfix ui-widget-header" : "k-tabstrip-items k-reset";
            string tabIndex = Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery ? "" : " tabindex='0'";
            string areaDesc = Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery ? "" : $" aria-activedescendant='{model.Id}_tab_active'";

            hb.Append($@"
<div id='{model.Id}'{tabIndex} class='yt_tabs t_display {tabsCss}' data-role='tabstrip' role='tablist'{areaDesc}>
    <ul class='t_tabstrip {stripCss}' role='tablist'>");

            // Render tabs
            int count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {
                bool active = model.ActiveTabIndex == count;
                string tabId = $"{model.Id}_tab{count}";

                string tabCss = null;
                tabCss = CssManager.CombineCss(tabCss, tabEntry.TabCssClasses);

                if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {

                    tabCss = CssManager.CombineCss(tabCss, active ? "ui-tabs-active ui-state-active" : "");

                    hb.Append($@"
        <li data-tab='{count}' role='tab' tabindex='{(active ? "0" : "-1")}' class='ui-tabs-tab ui-corner-top ui-tab ui-state-default {tabCss}' aria-controls='{tabId}' aria-labelledby='{tabId}_lb' aria-selected='{(active ? "true" : "false")}' aria-expanded='{(active ? "true" : "false")}'>
            <a href='#{tabId}' {Basics.CssTooltip}='{Utility.HtmlAttributeEncode(tabEntry.ToolTip?.ToString())}' role='presentation' tabindex='-1' class='ui-tabs-anchor' id='{tabId}_lb'>
                {Utility.HtmlEncode(tabEntry.Caption?.ToString())}
            </a>
        </li>");

                } else {

                    tabCss = CssManager.CombineCss(tabCss, active ? "k-state-active k-tab-on-top" : "");
                    string id = active ? $" id='{model.Id}_tab_active'" : "";

                    hb.Append(@$"
        <li{id} data-tab='{count}' class='k-item k-state-default {tabCss}{(count==0 ? " k-first":"")}' {Basics.CssTooltip}='{Utility.HtmlAttributeEncode(tabEntry.ToolTip?.ToString())}'{tabCss} role='tab' aria-selected='{(active ? "true" : "false")}' aria-controls='{tabId}'>
            <span class='k-loading k-complete'></span>
            <span unselectable='on' class='k-link'>{Utility.HtmlEncode(tabEntry.Caption?.ToString())}</span>
        </li>");

                }
                ++count;
            }
            hb.Append($@"
    </ul>");

            // Render panes
            count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {
                bool active = model.ActiveTabIndex == count;
                string tabId = $"{model.Id}_tab{count}";

                string cssClass = tabEntry.PaneCssClasses;

                if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {

                    hb.Append($@"
     <div class='t_proptable t_cat t_tabpanel ui-tabs-panel ui-corner-bottom ui-widget-content {cssClass}' data-tab='{count}' id='{tabId}' aria-labelledby='{tabId}_lb' role='tabpanel' {(active? "" : "style='display:none'")} aria-hidden='{(active ? "false" : "true")}'>");

                } else {

                    cssClass = CssManager.CombineCss(cssClass, "k-content");
                    cssClass = CssManager.CombineCss(cssClass, active ? "k-state-active" : "");

                    hb.Append($@"
     <div class='t_proptable t_cat t_tabpanel {cssClass}' data-tab='{count}' id='{tabId}' role='tabpanel' style='display:{(active ? "block" : "none")}' aria-hidden='{(active ? "false" : "true")}' aria-expanded='{(active ? "true" : "false")}'>");

                }

                if (tabEntry.RenderPaneAsync != null)
                    hb.Append(await tabEntry.RenderPaneAsync(count));

                hb.Append($@"
     </div>");
                ++count;
            }

            TabsSetup setup = GetTabsSetup(model);
            hb.Append($@"
    <input name='_ActiveTab' type='hidden' value='{model.ActiveTabIndex}' id='{setup.ActiveTabHiddenId}'>");


            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TabsComponent('{model.Id}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        private static TabsSetup GetTabsSetup(TabsDefinition tabsModel) {
            TabsSetup setup = new TabsSetup() {
                TabStyle = Manager.CurrentSite.TabStyle,
                ActiveTabIndex = tabsModel.ActiveTabIndex,
                ActiveTabHiddenId = Manager.UniqueId(),
        };
            return setup;
        }
    }
}


