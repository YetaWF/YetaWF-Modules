/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Scroller component implementation.
    /// </summary>
    public abstract class ScrollerComponentBase : YetaWFComponent {

        internal const string TemplateName = "Scroller";

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
    /// Renders a list of items in a horizontal or vertical display. This is typically used for a product scroller or to provide a scrollable rendering of objects.
    /// </summary>
    /// <remarks>
    /// The Scroller component is provided with a list of objects, each is rendered with the specified component (similar to how Property Lists are rendered), as each object is rendered using its own properties and the respective UIHint() definition.
    ///
    /// A sample scroller implementation is available at Tests > Templates > Scroller (standard YetaWF site).
    /// </remarks>
    /// <example>
    /// [UIHint("Scroller"), ReadOnly, AdditionalMetadata("Component", "Softelvdm_StoreCatalog_ScrollerProduct")]
    /// public List&lt;Item&gt; Items { get; set; }
    ///
    /// public class Item {
    ///     [UIHint("Image"), AdditionalMetadata("ImageType", ProductImageSupport.ImageType), AdditionalMetadata("Width", 100), AdditionalMetadata("Height", 100)]
    ///     [AdditionalMetadata("ShowMissing", false)]
    ///     public string Logo { get; set; }
    ///     [UIHint("String")]
    ///     public string Product Name { get; set; }
    /// }
    /// </example>
    [UsesAdditional("Component", "string", null, "Defines the component used to render each object within the specified model ({i}objects{/i}). This attribute is required.")]
    public class ScrollerDisplayComponent : ScrollerComponentBase, IYetaWFComponent<IEnumerable> {

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
        public async Task<string> RenderAsync(IEnumerable model) {
            HtmlBuilder hb = new HtmlBuilder();

            string uiHint = PropData.GetAdditionalAttributeValue<string>("Template");
            if (uiHint == null) throw new InternalError("No UIHint available for scroller");


            hb.Append($@"
<div id='{DivId}' class='yt_scroller t_display'>
    <a class='t_left' href='javascript:void(0)'></a>
    <div class='t_scrollarea'>
        <div class='t_items'>");

            foreach (var item in model) {
                YTagBuilder tag = new YTagBuilder("div");
                tag.AddCssClass("t_item");
                tag.InnerHtml = await HtmlHelper.ForDisplayContainerAsync(item, uiHint);
                hb.Append(tag.ToString(YTagRenderMode.Normal));
            }

            hb.Append($@"
        </div>
    </div>
    <a class='t_right' href='javascript:void(0)'></a>
</div>");

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ScrollerComponent('{DivId}');");

            return hb.ToString();
        }
    }
}
