/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the CurrencyISO4217 component implementation.
    /// </summary>
    public abstract class CurrencyISO4217ComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(CurrencyISO4217ComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "CurrencyISO4217";

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

    /// <summary>
    /// Implementation of the CurrencyISO4217 display component.
    /// </summary>
    public class CurrencyISO4217DisplayComponent : CurrencyISO4217ComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {

            string currency = await CurrencyISO4217.IdToCurrencyAsync(model, AllowMismatch: true);
            return HE(currency);
        }
    }

    /// <summary>
    /// Implementation of the CurrencyISO4217 edit component.
    /// </summary>
    public class CurrencyISO4217EditComponent : CurrencyISO4217ComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {

            bool includeSiteCurrency = PropData.GetAdditionalAttributeValue<bool>("SiteCurrency", true);

            List<CurrencyISO4217.Currency> currencies = await CurrencyISO4217.GetCurrenciesAsync(IncludeSiteCurrency: includeSiteCurrency);
            List<SelectionItem<string>> list = (from l in currencies select new SelectionItem<string>() {
                Text = l.Name,
                Value = l.Id,
            }).ToList();
            list.Insert(0, new SelectionItem<string> {
                Text = __ResStr("default", "(select)"),
                Value = "",
            });
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_currencyiso4217");
        }
    }
}
