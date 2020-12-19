/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Allows selection of a country name from a dropdown list. The model value is the user displayable country name.
    /// The CountryISO3166Id component allows a 2 character country ID as model instead.
    /// </summary>
    /// <remarks>
    /// The list of countries is located at ./CoreComponents/Core/Addons/_Templates/CountryISO3166/Countries.txt.
    ///
    /// For information about ISO 3166 see https://en.wikipedia.org/wiki/ISO_3166.
    /// </remarks>
    /// <example>
    /// [Category("Site"), Caption("Country"), Description("The country where you/your company is located")]
    /// [UIHint("CountryISO3166"), StringLength(MaxCountry), Trim, Required]
    /// public string Country { get; set; }
    /// </example>
    [UsesAdditional("SiteCountry", "bool", "true", "Defines whether the site's defined country is shown in the list of countries. If shown, it will always be shown as the first entry.")]
    public class CountryISO3166EditComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "CountryISO3166";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

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
