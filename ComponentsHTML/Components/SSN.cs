/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SSN component implementation.
    /// </summary>
    public abstract class SSNComponentBase : YetaWFComponent {

        internal const string TemplateName = "SSN";

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
    /// Displays the model formatted as a Social Security Number.
    /// </summary>
    public class SSNDisplayComponent : SSNComponentBase, IYetaWFComponent<string> {

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
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && model.Length == 9) {
                hb.Append($@"
<div class='yt_ssn t_display'>
    {HE(SSNValidationAttribute.GetDisplay(model))}
</div>");
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a Social Security Number.
    /// </summary>
    public class SSNEditComponent : SSNComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.maskedtextbox.min.js");
            await base.IncludeAsync();
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

            UseSuppliedIdAsControlId();

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($"<div id='{ControlId}' class='yt_ssn t_edit'>");

            Dictionary<string, object> hiddenAttributes = new Dictionary<string, object>(HtmlAttributes) {
                { "__NoTemplate", true }
            };
            hb.Append(await HtmlHelper.ForEditComponentAsync(Container, PropertyName, null, "Hidden", HtmlAttributes: hiddenAttributes, Validation: Validation));

            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, FieldType.Anonymous);
            tag.Attributes.Add("name", "ssninput");

            if (model != null)
                tag.MergeAttribute("value", model);
            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            hb.Append($"</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.SSNEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}
