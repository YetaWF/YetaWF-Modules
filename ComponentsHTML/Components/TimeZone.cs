/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

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
    /// Implementation of the TimeZone display component.
    /// </summary>
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
        public Task<YHtmlString> RenderAsync(string model) {

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_timezone");
            tag.AddCssClass("t_display");

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(model);
            if (tzi == null) {
                tag.SetInnerText(__ResStr("unknown", "(unknown)"));
            } else {
                tag.SetInnerText(tzi.DisplayName);
                tag.Attributes.Add("title", tzi.IsDaylightSavingTime(DateTime.Now/*need local time*/) ? tzi.DaylightName : tzi.StandardName);
            }
            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.Normal));
        }
    }

    /// <summary>
    /// Implementation of the TimeZone edit component.
    /// </summary>
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
        public async Task<YHtmlString> RenderAsync(string model) {

            List<TimeZoneInfo> tzis = TimeZoneInfo.GetSystemTimeZones().ToList();
            DateTime dt = DateTime.Now;// Need local time

            bool showDefault = PropData.GetAdditionalAttributeValue("ShowDefault", true);
            List<SelectionItem<string>> list;
            list = (
                from tzi in tzis orderby tzi.DisplayName
                orderby tzi.DisplayName
                select
                    new SelectionItem<string> {
                        Text = tzi.DisplayName,
                        Value = tzi.Id,
                        Tooltip = tzi.IsDaylightSavingTime(dt) ? tzi.DaylightName : tzi.StandardName,
                    }).ToList<SelectionItem<string>>();
            if (showDefault) {
                if (string.IsNullOrWhiteSpace(model))
                    model = TimeZoneInfo.Local.Id;
            } else
                list.Insert(0, new SelectionItem<string> { Text = __ResStr("select", "(select)"), Value = "" });

            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_timezone");
        }
    }
}
