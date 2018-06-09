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
                    Text = this.__ResStr("none", "(none)"),
                    Tooltip = this.__ResStr("noneTT", "")
                };
            }
            return await StringTTDisplayComponent.RenderStringTTAsync(this, stringTT, "yt_siteid");
        }
    }
}
