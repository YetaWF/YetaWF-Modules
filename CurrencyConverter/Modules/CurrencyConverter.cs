/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.CurrencyConverter.Modules {

    public class CurrencyConverterModuleDataProvider : ModuleDefinitionDataProvider<Guid, CurrencyConverterModule>, IInstallableModel { }

    [ModuleGuid("{d1b33e01-7acd-4f0e-a128-5101dd59e085}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CurrencyConverterModule : ModuleDefinition {

        public const int MaxCountry = 3;

        public CurrencyConverterModule() {
            Title = this.__ResStr("modTitle", "Currency Conversion");
            Name = this.__ResStr("modName", "Currency Conversion");
            Description = this.__ResStr("modSummary", "Currency conversion for site visitors. A test page for the Currency Converter Module can be accessed using Tests > Currency Converter (standard YetaWF site).");
            FromCountry = "USD"; // US $
            ToCountry = "CAD"; // Canadian $
            DefaultAmount = 100M;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CurrencyConverterModuleDataProvider(); }

        [Category("General"), Caption("From Currency"), Description("Currency for which the amount is entered")]
        [UIHint("YetaWF_CurrencyConverter_Country"), StringLength(MaxCountry), Required]
        public string FromCountry { get; set; }
        [Category("General"), Caption("To Currency"), Description("Currency for which the converted amount is to be calculated")]
        [UIHint("YetaWF_CurrencyConverter_Country"), StringLength(MaxCountry), Required]
        public string ToCountry { get; set; }
        [Category("General"), Caption("Default Amount"), Description("Default amount to be converted")]
        [UIHint("Currency"), Required, Range(0.0, 99999.99)]
        public decimal DefaultAmount { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public async Task<ModuleAction> GetAction_CurrencyConverterAsync(string url, decimal amount = 0.0M) {
            if (amount == 0.0M) amount = DefaultAmount;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Amount = amount.ToString("0.00") },
                Image = await CustomIconAsync("CurrencyConverter.png"),
                LinkText = this.__ResStr("editLink", "Currency Converter"),
                MenuText = this.__ResStr("editText", "Currency Converter"),
                Tooltip = this.__ResStr("editTooltip", "Get currency conversion information"),
                Legend = this.__ResStr("editLegend", "Provides currency conversion information"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}