/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public class CountryISO3166EditComponent : YetaWFComponent, IYetaWFComponent<string> {

        public const string TemplateName = "CountryISO3166";

        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }

        public async Task<YHtmlString> RenderAsync(string model) {

            bool includeSiteCountry = PropData.GetAdditionalAttributeValue<bool>("SiteCountry", true);
            List<CountryISO3166.Country> countries = CountryISO3166.GetCountries(IncludeSiteCountry: includeSiteCountry);

            List<SelectionItem<string>> list = (from l in countries select new SelectionItem<string>() {
                Text = l.Name,
                Value = l.Name,
            }).ToList();
            list.Insert(0, new SelectionItem<string> {
                Text = this.__ResStr("default", "(select)"),
                Value = "",
            });
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_countryiso3166");
        }
    }
}
