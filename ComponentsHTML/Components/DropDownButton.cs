/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays a dropdown button with an optional attached menu.
    /// </summary>
    public class DropDownButtonComponent : YetaWFComponent, IYetaWFComponent<DropDownButtonComponent.Model>, IYetaWFContainer<DropDownButtonComponent.Model> {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DropDownButtonComponent), name, defaultValue, parms); }

        public const string TemplateName = "DropDownButton";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }
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

        public class Model {
            public string Text { get; set; }
            public string Tooltip { get; set; }
            public string ButtonId { get; set; }
            public string MenuHTML { get; set; }
            public bool Mini { get; set; }
            public string CssClass { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.button.min.js");
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            string cssStyle = null;
            if (model.CssClass != null)
                cssStyle = $" {model.CssClass}";

            if (!model.Mini) {

                hb.Append($@"
<button id='{model.ButtonId}' type='button' class='yt_dropdownbutton{cssStyle}' {Basics.CssTooltip}='{HAE(model.Tooltip)}'>
    {HE(model.Text)}<span class='k-icon k-i-arrow-60-down'></span>
    {model.MenuHTML}
</button>");

            } else {

                hb.Append($@"
<button id='{model.ButtonId}' href='#' class='yt_dropdownbutton t_mini{cssStyle}'><span class='k-icon k-i-arrow-60-down'></span>
    {model.MenuHTML}
</button>");

            }

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDownButtonComponent('{model.ButtonId}');");

            return Task.FromResult(hb.ToString());
        }

        public Task<string> RenderContainerAsync(Model model) {
            return RenderAsync(model);
        }
    }
}
