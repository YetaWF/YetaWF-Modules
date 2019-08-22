/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the DayOfMonth component implementation.
    /// </summary>
    public abstract class DayOfMonthComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DayOfMonthComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "DayOfMonth";

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
    /// Implementation of the DayOfMonth display component.
    /// </summary>
    public class DayOfMonthDisplayComponent : DayOfMonthComponentBase, IYetaWFComponent<int?> {

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
        public Task<string> RenderAsync(int model) {
            return RenderAsync((int?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(int? model) {
            if (model == null) return Task.FromResult<string>(null);
            return Task.FromResult(HE(model.ToString()));
        }
    }

    /// <summary>
    /// Implementation of the DayOfMonth edit component.
    /// </summary>
    public class DayOfMonthEditComponent : DayOfMonthComponentBase, IYetaWFComponent<int?> {

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
        public Task<string> RenderAsync(int model) {
            return RenderAsync((int?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(int? model) {

            List<SelectionItem<int?>> list = new List<SelectionItem<int?>>();
            for (int i = 1; i <= 31; ++i) {
                list.Add(new SelectionItem<int?> { Text = i.ToString(), Value = i });
            }
            list.Insert(0, new SelectionItem<int?> {
                Text = __ResStr("default", "(select)"),
                Value = null,
            });
            return await DropDownListIntNullComponent.RenderDropDownListAsync(this, (int?)model, list, "yt_dayofmonth");
        }
    }
}
