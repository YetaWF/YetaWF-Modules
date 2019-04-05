/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the LanguageId edit component.
    /// </summary>
    public class LanguageIdComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "LanguageId";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            bool allLanguages = PropData.GetAdditionalAttributeValue<bool>("AllLanguages");

            List<SelectionItem<string>> list;
            if (allLanguages) {
                CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures);
                list = (from c in ci orderby c.DisplayName select new SelectionItem<string>() {
                    Text = string.Format("{0} - {1}", c.DisplayName, c.Name),
                    Value = c.Name,
                }).ToList();
            } else {
                list = (from l in MultiString.Languages select new SelectionItem<string>() {
                    Text = l.ShortName,
                    Tooltip = l.Description,
                    Value = l.Id,
                }).ToList();
            }
            if (useDefault) {
                list.Insert(0, new SelectionItem<string> {
                    Text = this.__ResStr("default", "(Site Default)"),
                    Tooltip = this.__ResStr("defaultTT", "Use the site defined default language"),
                    Value = "",
                });
            } else {
                if (string.IsNullOrWhiteSpace(model))
                    model = MultiString.ActiveLanguage;
            }
            // display the languages in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_languageid");
        }
    }
}
