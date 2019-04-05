/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    public abstract class SiteIdComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SiteIdComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "SiteId";

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

        internal static Dictionary<int, StringTT> Sites = null;
    }

    /// <summary>
    /// Implementation of the SiteId display component.
    /// </summary>
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

            if (Sites == null) {
                Dictionary<int, StringTT> sites = new Dictionary<int, StringTT>();
                DataProviderGetRecords<SiteDefinition> recs = await SiteDefinition.GetSitesAsync(0, 0, null, null);
                foreach (SiteDefinition site in recs.Data) {
                    sites.Add(site.Identity,
                        new StringTT {
                            Text = site.Identity.ToString(),
                            Tooltip = site.SiteDomain,
                        }
                    );
                }
                Sites = sites;
            }

            StringTT stringTT;
            if (Sites.ContainsKey(model))
                stringTT = Sites[model];
            else {
                stringTT = new StringTT {
                    Text = __ResStr("none", "(none)"),
                    Tooltip = __ResStr("noneTT", "")
                };
            }
            return await StringTTDisplayComponent.RenderStringTTAsync(this, stringTT, "yt_siteid");
        }
    }
}
