/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
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
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
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
        public bool ContextMenu { get; set; }
        public int ActiveTabIndex { get; set; }
        public string ActiveTabHiddenId { get; set; } = null!;
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
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(TabsDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{model.Id}' class='yt_tabs t_display' data-role='tabstrip' role='tablist'>
    <ul class='t_tabstrip' role='tablist'>");

            // Render tabs
            int count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {
                bool active = model.ActiveTabIndex == count;
                string tabId = $"{model.Id}_tab{count}";

                string? tabCss = null;
                tabCss = CssManager.CombineCss(tabCss, tabEntry.TabCssClasses);
                if (active)
                    tabCss = CssManager.CombineCss(tabCss, "t_tabactive");

                hb.Append($@"
        <li data-tab='{count}' role='tab' tabindex='{(active ? "0" : "-1")}' class='t_tab {tabCss}' aria-controls='{tabId}' aria-labelledby='{tabId}_lb' aria-selected='{(active ? "true" : "false")}' aria-expanded='{(active ? "true" : "false")}'>
            <a {Basics.CssTooltip}='{Utility.HAE(tabEntry.ToolTip?.ToString())}' role='presentation' tabindex='-1' class='t_tabanchor' id='{tabId}_lb'>
                {Utility.HE(tabEntry.Caption?.ToString())}
            </a>
        </li>");

                ++count;
            }
            hb.Append($@"
    </ul>");

            // Render panes
            count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {
                bool active = model.ActiveTabIndex == count;
                string tabId = $"{model.Id}_tab{count}";
                string? cssClass = tabEntry.PaneCssClasses;

                hb.Append($@"
     <div class='t_proptable t_cat t_tabpanel {cssClass}' data-tab='{count}' id='{tabId}' aria-labelledby='{tabId}_lb' role='tabpanel' {(active ? "" : "style='display:none'")} aria-hidden='{(active ? "false" : "true")}'>");

                if (tabEntry.RenderPaneAsync != null)
                    hb.Append(await tabEntry.RenderPaneAsync(count));

                hb.Append($@"
     </div>");
                ++count;
            }

            TabsSetup setup = GetTabsSetup(model);
            hb.Append($@"
    <input name='_ActiveTab' type='hidden' value='{model.ActiveTabIndex}' id='{setup.ActiveTabHiddenId}' />");

            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TabsComponent('{model.Id}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        private static TabsSetup GetTabsSetup(TabsDefinition tabsModel) {
            TabsSetup setup = new TabsSetup() {
                ContextMenu = tabsModel.ContextMenu,
                ActiveTabIndex = tabsModel.ActiveTabIndex,
                ActiveTabHiddenId = Manager.UniqueId(),
            };
            return setup;
        }
    }
}


