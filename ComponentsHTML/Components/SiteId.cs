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

    public abstract class SiteIdComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SiteIdComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "SiteId";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        protected static Dictionary<int, StringTT> Sites = null;
    }

    public class SiteIdDisplayComponent : SiteIdComponentBase, IYetaWFComponent<int> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public async Task<YHtmlString> RenderAsync(int model) {

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
