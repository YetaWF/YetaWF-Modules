/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Newtonsoft.Json;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
#if MVC6
#else
using System.Web.Mvc;
#endif

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
    /// Implementation of the Tabs display component.
    /// </summary>
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

            TabsSetup setup = GetTabsSetup(model);

            string tabsCss = Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery ? "t_jquery" : "t_kendo";

            hb.Append($@"
<div id='{model.Id}' class='yt_tabs t_display {tabsCss}'>
    <ul class='t_tabstrip'>");
            // Render tabs
            int count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {
                string tabCss = string.IsNullOrWhiteSpace(tabEntry.TabCssClasses) ? "" : $" class='{tabEntry.TabCssClasses}'";
                if (Manager.CurrentSite.TabStyle == YetaWF.Core.Site.TabStyleEnum.JQuery) {
                    string tabId = $"{model.Id}_tab{count}";
                    hb.Append($"<li data-tab='{count}'{tabCss}><a href='#{tabId}' {Basics.CssTooltip}='{Utility.HtmlAttributeEncode(tabEntry.ToolTip?.ToString())}'>{Utility.HtmlEncode(tabEntry.Caption?.ToString())}</a></li>");
                } else {
                    hb.Append($"<li data-tab='{count}' {Basics.CssTooltip}='{Utility.HtmlAttributeEncode(tabEntry.ToolTip?.ToString())}'{tabCss}>{Utility.HtmlEncode(tabEntry.Caption?.ToString())}</li>");
                }
                ++count;
            }
            hb.Append($@"
    </ul>");

            // Render panes
            count = 0;
            foreach (TabEntry tabEntry in model.Tabs) {

                string cssClass = string.IsNullOrWhiteSpace(tabEntry.PaneCssClasses) ? "" : $" {tabEntry.PaneCssClasses}";

                hb.Append($@"
     <div class='t_proptable t_cat t_tabpanel{cssClass}' data-tab='{count}' id='{model.Id}_tab{count}'>");

                if (tabEntry.RenderPaneAsync != null)
                    hb.Append(await tabEntry.RenderPaneAsync(count));


                hb.Append($@"
     </div>");
                ++count;
            }

            setup.ActiveTabHiddenId = Manager.UniqueId();
            hb.Append($@"
    <input name='_ActiveTab' type='hidden' value='{model.ActiveTabIndex}' id='{setup.ActiveTabHiddenId}'>");


            hb.Append($@"
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TabsComponent('{model.Id}', {JsonConvert.SerializeObject(setup)});");

            return hb.ToString();
        }

        internal static TabsSetup GetTabsSetup(TabsDefinition tabsModel) {
            TabsSetup setup = new TabsSetup() {
                TabStyle = Manager.CurrentSite.TabStyle,
                ActiveTabIndex = tabsModel.ActiveTabIndex,
                ActiveTabHiddenId = null,
            };
            return setup;
        }
    }
}


