/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the DayOfWeek component implementation.
    /// </summary>
    public abstract class DayOfWeekComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayOfWeekComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "DayOfWeek";

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
    /// Displays the model formatted as a day of the week (Monday - Saturday, Sunday). If the model value is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Day of the Week"), Description("Shows the day of the week")]
    /// [UIHint("DayOfWeek"), ReadOnly]
    /// public DayOfWeek? Day { get; set; }
    /// </example>
    public class DayOfWeekDisplayComponent : DayOfWeekComponentBase, IYetaWFComponent<DayOfWeek?> {

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
        public Task<string> RenderAsync(DayOfWeek model) {
            return RenderAsync((DayOfWeek?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(DayOfWeek? model) {
            if (model == null) return Task.FromResult<string>(null);
            return Task.FromResult(HE(Formatting.GetDayName((DayOfWeek)model)));
        }
    }

    /// <summary>
    /// Allows selection of a day of the week (Monday - Saturday, Sunday) using a dropdown list. The model specifies the selected day.
    /// A "(select)" entry is added as the first entry with a value of null.
    /// </summary>
    /// <example>
    /// [Caption("Day of the Week"), Description("Select the day of the week")]
    /// [UIHint("DayOfWeek")]
    /// public DayOfWeek? Day { get; set; }
    /// </example>
    public class DayOfWeekEditComponent : DayOfWeekComponentBase, IYetaWFComponent<DayOfWeek?> {

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
        public Task<string> RenderAsync(DayOfWeek model) {
            return RenderAsync((DayOfWeek?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(DayOfWeek? model) {

            List<SelectionItem<int?>> list = new List<SelectionItem<int?>>();

            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Monday), Value = (int)DayOfWeek.Monday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Tuesday), Value = (int)DayOfWeek.Tuesday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Wednesday), Value = (int)DayOfWeek.Wednesday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Thursday), Value = (int)DayOfWeek.Thursday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Friday), Value = (int)DayOfWeek.Friday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Saturday), Value = (int)DayOfWeek.Saturday });
            list.Add(new SelectionItem<int?> { Text = Formatting.GetDayName(DayOfWeek.Sunday), Value = (int)DayOfWeek.Sunday });

            list.Insert(0, new SelectionItem<int?> {
                Text = __ResStr("default", "(select)"),
                Value = null,
            });
            return await DropDownListIntNullComponent.RenderDropDownListAsync(this, (int?)model, list, "yt_dayofweek");
        }
    }
}
