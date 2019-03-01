/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Currency component implementation.
    /// </summary>
    public abstract class CurrencyComponentBase : YetaWFComponent {

        internal const string TemplateName = "Currency";

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
    /// Implementation of the Currency display component.
    /// </summary>
    public class CurrencyDisplayComponent : CurrencyComponentBase, IYetaWFComponent<decimal?> {

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
        public async Task<YHtmlString> RenderAsync(decimal model) {
            return await RenderAsync((decimal?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<YHtmlString> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(HE(Formatting.FormatAmount((decimal)model)));
            }
            return Task.FromResult(hb.ToYHtmlString());
        }
    }

    /// <summary>
    /// Implementation of the Currency edit component.
    /// </summary>
    public class CurrencyEditComponent : CurrencyComponentBase, IYetaWFComponent<Decimal>, IYetaWFComponent<Decimal?> {

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
        public async Task<YHtmlString> RenderAsync(Decimal model) {
            return await RenderAsync((Decimal?) model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<YHtmlString> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_currency");
            tag.AddCssClass("t_edit");

            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);

            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                tag.MergeAttribute("data-min", ((double)rangeAttr.Minimum).ToString("F3"));
                tag.MergeAttribute("data-max", ((double)rangeAttr.Maximum).ToString("F3"));
            }
            if (model != null)
                tag.MergeAttribute("value", Formatting.FormatAmount((decimal)model));

            return Task.FromResult(tag.ToYHtmlString(YTagRenderMode.StartTag));
        }
    }
}
