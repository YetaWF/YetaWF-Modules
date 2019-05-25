/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the TimeOfDay component implementation.
    /// </summary>
    public abstract class TimeOfDayComponentBase : YetaWFComponent {

        internal const string TemplateName = "TimeOfDay";

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
    /// Implementation of the TimeOfDay display component.
    /// </summary>
    public class TimeOfDayDisplayComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay> {

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
        public Task<string> RenderAsync(TimeOfDay model) {
        HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_timeofday");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                DateTime dt = model.AsDateTime();
                tag.SetInnerText(Formatting.FormatTime(dt));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Implementation of the TimeOfDay edit component.
    /// </summary>
    public class TimeOfDayEditComponent : TimeOfDayComponentBase, IYetaWFComponent<TimeOfDay> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.calendar.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.timepicker.min.js");
            await KendoUICore.AddFileAsync("kendo.datetimepicker.min.js");
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(TimeOfDay model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_timeofday t_edit'>");

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation));

            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("name", "dtpicker");

            if (model != null) {
                DateTime dt = model.AsDateTime();
                tag.MergeAttribute("value", Formatting.FormatTime(dt));
            }
            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            hb.Append($"</div>");

            Manager.ScriptManager.AddLast($@"(new YetaWF_ComponentsHTML.TimeComponent()).init('{ControlId}');");

            return hb.ToString();
        }
    }
}
