/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
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
    /// Allows entry of a currency amount based, formatted using the site's defined default currency.
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
    [UsesSibling("_PlaceHolder", "string", "Defines the placeholder text shown when control contents are empty.")]
    public class CurrencyEditComponent : CurrencyComponentBase, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class CurrencySetup {
            public double Min { get; set; }
            public double Max { get; set; }
            public Boolean ReadOnly { get; set; }
            public string PlaceHolder { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.userevents.min.js");
            await KendoUICore.AddFileAsync("kendo.numerictextbox.min.js");
            await base.IncludeAsync();
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Decimal? model) {

            bool rdonly = PropData.GetAdditionalAttributeValue<bool>("ReadOnly", false);
            bool disabled = PropData.GetAdditionalAttributeValue<bool>("Disabled", false);
            string dis = string.Empty;
            if (disabled)
                dis = " disabled='disabled'";

            TryGetSiblingProperty<string>($"{PropertyName}_PlaceHolder", out string placeHolder);

            string val = string.Empty;
            if (model != null)
                val = $" value='{Formatting.FormatAmount((decimal)model)}'";

            CurrencySetup setup = new CurrencySetup {
                Min = 0,
                Max = 999999999.99,
                ReadOnly = rdonly,
                PlaceHolder = placeHolder,
            };
            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                setup.Min = (double)rangeAttr.Minimum;
                setup.Max = (double)rangeAttr.Maximum;
            }
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.CurrencyEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(
$@"<div id='{ControlId}' class='yt_currency t_edit y_inline'>
    <input{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} class='{GetClasses()}'{val}{dis}>
</div>");

        }
    }
}
