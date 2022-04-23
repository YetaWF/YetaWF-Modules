/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the DateTime component type.
    /// The AddSupportAsync method is called so DateTime component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class DateTimeEdit : IAddOnSupport {

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string area = package.AreaName;

            scripts.AddConfigOption(area, "SVG_fas_caret_left", SkinSVGs.Get(package, "fas-caret-left"));
            scripts.AddConfigOption(area, "SVG_fas_caret_right", SkinSVGs.Get(package, "fas-caret-right"));

            scripts.AddLocalization(area, "DateFormat", UserSettings.GetProperty<Formatting.DateFormatEnum>("DateFormat"));
            scripts.AddLocalization(area, "TimeFormat", UserSettings.GetProperty<Formatting.TimeFormatEnum>("TimeFormat"));

            scripts.AddLocalization(area, "WeekDays2", Formatting.GetDayName2CharsArr());
            scripts.AddLocalization(area, "WeekDays", Formatting.GetDayNamesArr());
            scripts.AddLocalization(area, "MonthNames", Formatting.GetMonthNamesArr());
            return Task.CompletedTask;
        }
    }
}
