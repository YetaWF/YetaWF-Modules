/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Decimal component implementation.
    /// </summary>
    public abstract class DecimalComponentBase : YetaWFComponent {

        internal const string TemplateName = "Decimal";

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
    /// Displays the model formatted as a decimal number with 2 fractional digits. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Category("General"), Caption("Sales Tax - Rate"), Description("The sales tax rate (in percent) collected on purchases")]
    /// [UIHint("Decimal"), ReadOnly]
    /// public decimal SalesTaxRate { get; set; }
    /// </example>
    public class DecimalDisplayComponent : DecimalComponentBase, IYetaWFComponent<Decimal?> {

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
        public async Task<string> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null && (Decimal)model > Decimal.MinValue && (Decimal)model < Decimal.MaxValue) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("yt_decimal");
                tag.AddCssClass("t_display");
                FieldSetup(tag, FieldType.Anonymous);
                string format = PropData.GetAdditionalAttributeValue("Format", "0.00");
                if (model != null)
                    tag.SetInnerText(((decimal)model).ToString(format));
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of a decimal number with 2 fractional digits.
    /// </summary>
    /// <remarks>
    /// The RangeAttribute can be used to define the lowest and highest allowable values.
    /// </remarks>
    /// <example>
    /// [Category("General"), Caption("Sales Tax - Rate"), Description("The sales tax rate (in percent) collected on purchases")]
    /// [UIHint("Decimal"), Range(0.0, 100.0), Required]
    /// public decimal SalesTaxRate { get; set; }
    /// </example>
    public class DecimalEditComponent : DecimalComponentBase, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.userevents.min.js");
            await KendoUICore.AddFileAsync("kendo.numerictextbox.min.js");
            await base.IncludeAsync();
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_decimal");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            string id = MakeId(tag);

            // handle min/max
            float min = 0, max = 99999999.99F;
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                min = Convert.ToSingle(rangeAttr.Minimum);
                max = Convert.ToSingle(rangeAttr.Maximum);
            }
            string format = PropData.GetAdditionalAttributeValue("Format", "0.00");
            if (model != null)
                tag.MergeAttribute("value", ((decimal)model).ToString(format));

            hb.Append($@"
{tag.ToString(YTagRenderMode.StartTag)}");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DecimalEditComponent('{id}', {{ Min: {min}, Max: {max} }});");

            return Task.FromResult(hb.ToString());
        }
    }
}
