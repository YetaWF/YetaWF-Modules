/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TimeZone component implementation.
    /// </summary>
    public abstract class TimeZoneComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TimeZoneComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "TimeZone";

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
    /// Displays the model (a time zone ID) rendered using time zone information.
    /// </summary>
    /// <example>
    /// [Caption("Time Zone"), Description("The time zone in which the domain is located ")]
    /// [UIHint("TimeZone"), ReadOnly]
    /// public string ScanTimeZone { get; set; }
    /// </example>
    public class TimeZoneDisplayComponent : TimeZoneComponentBase, IYetaWFComponent<string> {

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
        public Task<string> RenderAsync(string model) {

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_timezone");
            tag.AddCssClass("t_display");

            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                try {
                    model = TZConvert.WindowsToIana(model);
                } catch (Exception) { }
            }
            TimeZoneInfo tzi = null;
            try {
                tzi = TimeZoneInfo.FindSystemTimeZoneById(model);
            } catch (Exception) { }
            if (tzi == null) {
                tag.SetInnerText(__ResStr("unknown", "(unknown)"));
            } else {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    tag.SetInnerText(tzi.DisplayName);
                else
                    tag.SetInnerText(tzi.Id);
                tag.Attributes.Add("title", tzi.IsDaylightSavingTime(DateTime.Now/*need local time*/) ? tzi.DaylightName : tzi.StandardName);
            }
            return Task.FromResult(tag.ToString(YTagRenderMode.Normal));
        }
    }

    /// <summary>
    /// Allows selection of a time zone.
    /// </summary>
    /// <example>
    /// [Caption("Time Zone"), Description("Your time zone - all dates/times within this web site will be adjusted for the specified time zone")]
    /// [UIHint("TimeZone"), StringLength(UserData.MaxTimeZone), Required]
    /// public string TimeZone { get; set; }
    /// </example>
    [UsesAdditional("ShowDefault", "bool", "true", "Defines whether the server's time zone is added to the dropdown list as the default, if the model is null. Otherwise, the server's time zone is not added.")]
    public class TimeZoneEditComponent : TimeZoneComponentBase, IYetaWFComponent<string> {

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

            List<TimeZoneInfo> tzis = TimeZoneInfo.GetSystemTimeZones().ToList();
            DateTime dt = DateTime.Now;// Need local time

            bool showDefault = PropData.GetAdditionalAttributeValue("ShowDefault", true);

            List<SelectionItem<string>> list;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                list = (
                    from tzi in tzis orderby tzi.DisplayName
                    orderby tzi.DisplayName
                    select
                        new SelectionItem<string> {
                            Text = tzi.DisplayName,
                            Value = tzi.Id,
                            Tooltip = tzi.IsDaylightSavingTime(dt) ? tzi.DaylightName : tzi.StandardName,
                        }).ToList<SelectionItem<string>>();
            } else {

                try {
                    model = TZConvert.WindowsToIana(model);
                } catch (Exception) {
                    model = null;
                }
                tzis = (from tzi in tzis orderby tzi.BaseUtcOffset, tzi.Id select tzi).ToList();

                list = new List<SelectionItem<string>>();
                foreach (TimeZoneInfo tzi in tzis) {
                    TimeSpan ts = tzi.BaseUtcOffset;
                    int h = ts.Hours;
                    string disp = $"(UTC{(h > 0 ? "+" : "-")}{Math.Abs(h):00}:{ts.Minutes:00}) {tzi.Id}";
                    list.Add(new SelectionItem<string> {
                        Text = disp,
                        Value = tzi.Id,
                        Tooltip = tzi.IsDaylightSavingTime(dt) ? tzi.DaylightName : tzi.StandardName,
                    });
                }
            }

            if (showDefault) {
                if (string.IsNullOrWhiteSpace(model))
                    model = TimeZoneInfo.Local.Id;
            } else
                list.Insert(0, new SelectionItem<string> { Text = __ResStr("select", "(select)"), Value = "" });

            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_timezone");
        }

        /// <summary>
        /// Called before action runs.
        /// </summary>
        /// <remarks>Used to normalize timezones.</remarks>
        public static Task<string> ControllerPreprocessActionAsync(string propName, string model, ModelStateDictionary modelState) {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                // On linux we need to translate the timezone id to a "windows" timezone id
                if (!string.IsNullOrWhiteSpace(model)) {
                    try {
                        model = TZConvert.IanaToWindows(model);
                    } catch (Exception) {
                        model = null;
                    }
                }
            }
            return Task.FromResult(model);
        }
    }
}
