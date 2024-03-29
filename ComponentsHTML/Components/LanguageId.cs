/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Allows selection of an available language using a dropdown list. The model represents the language ID.
    /// </summary>
    /// <example>
    /// [Caption("Id"), Description("The language id - this is the same as the culture name used throughout .NET")]
    /// [UIHint("LanguageId"), StringLength(LanguageData.MaxId), AdditionalMetadata("NoDefault", true), AdditionalMetadata("AllLanguages", true), Required, Trim]
    /// public string Id { get; set; }
    /// </example>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    [UsesAdditional("AllLanguages", "bool", "false", "Defines whether all system-defined languages are displayed or whether the languages defined for this YetaWF instance are shown. For details see National Language Support.")]
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
