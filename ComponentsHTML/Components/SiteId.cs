/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SiteId component implementation.
    /// </summary>
    public abstract class SiteIdComponentBase : YetaWFComponent, ISelectionListInt {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SiteIdComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "SiteId";

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

        internal static Dictionary<int, string> Sites = null;

        internal async Task<Dictionary<int, string>> GetSitesAsync() {
            Dictionary<int, string> sites = new Dictionary<int, string>();
            List<DataProviderSortInfo> sorts = null;
            sorts = DataProviderSortInfo.Join(sorts, new DataProviderSortInfo { Field = nameof(SiteDefinition.SiteDomain), Order = DataProviderSortInfo.SortDirection.Ascending });
            DataProviderGetRecords<SiteDefinition> recs = await SiteDefinition.GetSitesAsync(0, 0, null, null);
            foreach (SiteDefinition site in recs.Data)
                sites.Add(site.Identity, site.SiteDomain);
            return sites;
        }

        /// <summary>
        /// Returns a collection of integer/enum values suitable for rendering in a DropDownList component.
        /// </summary>
        /// <param name="showDefault">Set to true to add a "(select)" entry at the top of the list, false otherwise.</param>
        /// <returns>Returns a collection of integer/enum values suitable for rendering in a DropDownList component.</returns>
        public async Task<List<SelectionItem<int>>> GetSelectionListIntAsync(bool showDefault) {

            Sites ??= await GetSitesAsync();

            List<SelectionItem<int>> list = new List<SelectionItem<int>>();
            foreach (KeyValuePair<int, string> site in Sites) {
                list.Add(new SelectionItem<int> {
                    Text = site.Value,
                    Value = site.Key,
                });
            }
            if (showDefault) {
                list.Insert(0, new SelectionItem<int> {
                    Text = __ResStr("select", "(select)"),
                    Value = 0,
                });
            }
            return list;
        }
    }

    /// <summary>
    /// Displays a site ID based on the model, with a tooltip showing the site's domain name.
    /// </summary>
    /// <example>
    /// [Caption("Site"), Description("The site that was changed")]
    /// [UIHint("SiteId"), ReadOnly]
    /// </example>
    public class SiteIdDisplayComponent : SiteIdComponentBase, IYetaWFComponent<int> {

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
        public async Task<string> RenderAsync(int model) {

            Sites ??= await GetSitesAsync();

            StringTT stringTT;
            if (Sites.TryGetValue(model, out string name)) {
                stringTT = new StringTT {
                    Text = name,
                    Tooltip = __ResStr("siteId", "Site Id {0}", model),
                };
            } else {
                stringTT = new StringTT {
                    Text = __ResStr("none", "(none)"),
                    Tooltip = __ResStr("noneTT", "")
                };
            }
            return await StringTTDisplayComponent.RenderStringTTAsync(this, stringTT, "yt_siteid");
        }
    }

    /// <summary>
    /// Allows selection of a site from a dropdown list. The model represents the site ID.
    /// </summary>
    public class SiteIdEditComponent : SiteIdComponentBase, IYetaWFComponent<int> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(int model) {

            HtmlBuilder hb = new HtmlBuilder();

            List<SelectionItem<int>> list = await GetSelectionListIntAsync(true);

            hb.Append($@"
<div class='yt_yetawf_identity_siteid t_edit'>
    {await DropDownListIntComponent.RenderDropDownListAsync(this, model, list, null)}
</div>");
            return hb.ToString();
        }
    }
}
