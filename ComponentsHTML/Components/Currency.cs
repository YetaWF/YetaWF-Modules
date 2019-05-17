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
        public async Task<string> RenderAsync(decimal model) {
            return await RenderAsync((decimal?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Decimal? model) {
            HtmlBuilder hb = new HtmlBuilder();
            if (model != null) {
                hb.Append(HE(Formatting.FormatAmount((decimal)model)));
            }
            return Task.FromResult(hb.ToString());
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

        internal class CurrencySetup {
            public double Min { get; set; }
            public double Max { get; set; }
        }

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

            hb.Append($"<div id='{ControlId}' class='yt_currency t_edit'>");

            YTagBuilder tag = new YTagBuilder("input");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);

            CurrencySetup setup = new CurrencySetup {
                Min = 0,
                Max = 99999999.99,
            };
            // handle min/max
            RangeAttribute rangeAttr = PropData.TryGetAttribute<RangeAttribute>();
            if (rangeAttr != null) {
                setup.Min = (double)rangeAttr.Minimum;
                setup.Max = (double)rangeAttr.Maximum;
            }
            if (model != null)
                tag.MergeAttribute("value", Formatting.FormatAmount((decimal)model));

            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            hb.Append($"</div>");
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.CurrencyEditComponent('{ControlId}', {YetaWFManager.JsonSerialize(setup)});");

            return Task.FromResult(hb.ToString());
        }
    }
}
