/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Currency component implementation.
    /// </summary>
    public abstract class CurrencyComponentBase : YetaWFComponent {

        internal const string TemplateName = "Currency";

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
    }

    /// <summary>
    /// Displays a currency amount based on the provided model, formatted using the site's defined default currency. If the model is null, nothing is rendered.
    /// </summary>
    /// <remarks>
    /// The currency is based on the site's defined default currency. The default currency can be found at Admin > Settings > Site Settings, Site tab, Currency, Currency Format and Currency Rounding fields.
    /// </remarks>
    /// <example>
    /// [Caption("Hourly Rate"), Description("Shows the hourly rate")]
    /// [UIHint("Currency"), ReadOnly]
    /// public decimal DefaultHourlyRate { get; set; }
    /// </example>
    public class CurrencyDisplayComponent : CurrencyComponentBase, IYetaWFComponent<decimal?> {

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
        public async Task<string> RenderAsync(decimal model) {
            return await RenderAsync((decimal?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(HE(Formatting.FormatAmount((decimal)model)));
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a currency amount, formatted using the site's defined default currency.
    /// </summary>
    /// <remarks>
    /// The currency is based on the site's defined default currency. The default currency can be found at Admin > Settings > Site Settings, Site tab, Currency, Currency Format and Currency Rounding fields.
    ///
    /// The RangeAttribute attribute can be used to set upper and/or lower amount limits.
    /// </remarks>
    /// <example>
    /// [Caption("Hourly Rate"), Description("Enter the hourly rate")]
    /// [UIHint("Currency"), Range(0.0, 999999.0), Required]
    /// public decimal DefaultHourlyRate { get; set; }
    /// </example>
    [UsesAdditional("ReadOnly", "bool", "false", "Defines whether the control is rendered read/only.")]
    [UsesAdditional("Disabled", "bool", "false", "Defines whether the control is disabled.")]
    [UsesAdditional("Plain", "bool", "false", "Defines whether the number is shown using the defined locale.")]
    [UsesSibling("_PlaceHolder", "string", "Defines the placeholder text shown when control contents are empty.")]
    public class CurrencyEditComponent : CurrencyComponentBase, IYetaWFComponent<decimal?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "Number", ComponentType.Edit);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(decimal? model) {

            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);
            bool disabled = PropData.GetAdditionalAttributeValue<bool>("Disabled", false);
            string dis = string.Empty;
            if (disabled)
                dis = " disabled='disabled'";
            string ro = string.Empty;
            if (rdonly)
                ro = " readonly='readonly'";

            bool plain = PropData.GetAdditionalAttributeValue<bool>("Plain", false);
            TryGetSiblingProperty<string>($"{PropertyName}_PlaceHolder", out string? placeHolder);

            CurrencyISO4217.Currency currency = Manager.CurrentSite.CurrencyInfo;

            NumberSetup setup = new NumberSetup {
                Min = 0,
                Max = 999999999.99,
                Step = 1,
                Digits = Manager.CurrentSite.CurrencyDecimals,
                Plain = plain,
                Locale = MultiString.ActiveLanguage,
                Lead = currency.Lead,
                Trail = currency.Trail,
            };
            // handle min/max
            RangeAttribute? rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                setup.Min = (double)rangeAttr.Minimum;
                setup.Max = (double)rangeAttr.Maximum;
            }

            string? internalValue = model?.ToString();
            string displayValue = model != null ? ((decimal)model).ToString(currency.Format) : string.Empty;
            placeHolder = string.IsNullOrWhiteSpace(placeHolder) ? string.Empty : $" placeholder={HAE(placeHolder)}";

            string tags =
$@"<div id='{ControlId}'{GetClassAttribute("yt_number_container yt_currency t_edit")}>
    <input type='hidden'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} value='{HAE(internalValue)}'>
    <input type='text' maxlength='20' {dis}{ro}{placeHolder}{(disabled ? " t_disabled" : "")}{(rdonly ? " t_readonly" : "")}  value='{HAE(displayValue)}'>
</div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.CurrencyEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(tags);
        }
    }
}
