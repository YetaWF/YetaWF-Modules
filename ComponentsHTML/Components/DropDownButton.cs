/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Displays a dropdown button with an optional attached menu.
    /// </summary>
    public class DropDownButtonComponent : YetaWFComponent, IYetaWFComponent<DropDownButtonComponent.Model>, IYetaWFContainer<DropDownButtonComponent.Model> {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(DropDownButtonComponent), name, defaultValue, parms); }

        /// <summary>
        /// Defines the component's name.
        /// </summary>
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

        /// <summary>
        /// Defines the model used to render the dropdown button.
        /// </summary>
        public class Model {
            /// <summary>
            /// The text of the dropdown button.
            /// </summary>
            public string Text { get; set; } = null!;
            /// <summary>
            /// The optional tooltip of the dropdown button.
            /// </summary>
            public string? Tooltip { get; set; }
            /// <summary>
            /// The HTML Id of the dropdown button.
            /// </summary>
            public string ButtonId { get; set; } = null!;
            /// <summary>
            /// The HTML representing the dropdown menu (a &lt;ul&gt; tag).
            /// </summary>
            public string MenuHTML { get; set; } = null!;
            /// <summary>
            /// Defines whether a small dropdown button is used. A small dropdown button doesn't display the button text.
            /// </summary>
            public bool Mini { get; set; }
            /// <summary>
            /// The Optional CSS class added to the dropdown button tag.
            /// </summary>
            public string? CssClass { get; set; }
        }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            // Add required menu support
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "MenuUL", ComponentType.Display);
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Model model) {

            HtmlBuilder hb = new HtmlBuilder();

            string? cssStyle = null;
            if (model.CssClass != null)
                cssStyle += $" {model.CssClass}";
            if (model.Mini)
                cssStyle += $" t_mini";

            hb.Append($@"
<button id='{model.ButtonId}' class='yt_dropdownbutton{cssStyle}' {Basics.CssTooltip}='{HAE(model.Tooltip)}'>");

            if (!model.Mini) {
                hb.Append($@"
    {HE(model.Text)}");
            }

            hb.Append($@"
        {SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-caret-down")}
    {model.MenuHTML}
</button>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDownButtonComponent('{model.ButtonId}');");

            return Task.FromResult(hb.ToString());
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderContainerAsync(Model model) {
            return RenderAsync(model);
        }
    }
}
