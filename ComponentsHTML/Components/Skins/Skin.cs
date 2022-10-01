/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Skin component implementation.
    /// </summary>
    public abstract class SkinComponentBase : YetaWFComponent {

        internal const string TemplateName = "Skin";

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
    /// Displays the selected page skin information. The model defines the skin definition and cannot be null.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Skin"), Description("The skin used to display pages/popups")]
    /// [UIHint("Skin"), ReadOnly]
    /// public SkinDefinition SelectedSkin { get; set; }
    /// </example>
    public class SkinDisplayComponent : SkinComponentBase, IYetaWFComponent<SkinDefinition> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        internal class SkinUI {
            [Caption("Skin Collection"), Description("The name of the skin collection")]
            [StringLength(SkinDefinition.MaxCollection)]
            [UIHint("SkinCollection")]
            public string Collection { get; set; } = null!;

            [Caption("Page"), Description("The name of the page skin")]
            [StringLength(SkinDefinition.MaxPageFile)]
            [UIHint("SkinNamePage")]
            public string PageFileName { get; set; } = null!;
            public string PageFileName_Collection { get { return Collection; } }

            [Caption("Popup"), Description("The name of the popup skin")]
            [StringLength(SkinDefinition.MaxPopupFile)]
            [UIHint("SkinNamePopup")]
            public string PopupFileName { get; set; } = null!;
            public string PopupFileName_Collection { get { return Collection; } }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(SkinDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            SkinUI ps = new SkinUI {
                Collection = model.Collection,
                PageFileName = model.PageFileName,
                PopupFileName = model.PopupFileName,
            };

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
<div id='{ControlId}' class='yt_skin t_display'>
    <div class='t_collection'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.Collection))}
        {await HtmlHelper.ForDisplayAsync(ps, nameof(ps.Collection))}
    </div>
    <div class='t_skinpage'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.PageFileName))}
        {await HtmlHelper.ForDisplayAsync(ps, nameof(ps.PageFileName))}
    </div>
    <div class='t_skinpopup'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.PopupFileName))}
        {await HtmlHelper.ForDisplayAsync(ps, nameof(ps.PopupFileName))}
    </div>
</div>");
            }
            return hb.ToString();
        }
    }

    /// <summary>
    /// Allows selection of a page skin from all the available skin collections.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Skin"), Description("The skin used to display pages/popups")]
    /// [UIHint("Skin"), Trim]
    /// public SkinDefinition SelectedSkin { get; set; }
    /// </example>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    public class SkinEditComponent : SkinComponentBase, IYetaWFComponent<SkinDefinition> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        internal class SkinUI {
            [Caption("Skin Collection"), Description("The name of the skin collection")]
            [StringLength(SkinDefinition.MaxCollection)]
            [UIHint("SkinCollection")]
            public string Collection { get; set; } = null!;

            [Caption("Page"), Description("The name of the page skin")]
            [StringLength(SkinDefinition.MaxPageFile)]
            [UIHint("SkinNamePage")]
            public string PageFileName { get; set; } = null!;
            public string PageFileName_Collection { get { return Collection; } }

            [Caption("Popup"), Description("The name of the popup skin")]
            [StringLength(SkinDefinition.MaxPopupFile)]
            [UIHint("SkinNamePopup")]
            public string PopupFileName { get; set; } = null!;
            public string PopupFileName_Collection { get { return Collection; } }
        }
        internal class Setup {
            public string AjaxUrl { get; set; } = null!;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(SkinDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            SkinUI ps = new SkinUI {
                Collection = model.Collection,
                PageFileName = model.PageFileName,
                PopupFileName = model.PopupFileName,
            };

            Setup setup = new Setup {
                AjaxUrl = Utility.UrlFor(typeof(SkinController), nameof(SkinController.GetSkins)),
            };

            // add dummy input field so we can find the property name in this template
            hb.Append($@"
<div id='{ControlId}' class='yt_skin t_edit'>
     {await HtmlHelper.ForEditComponentAsync(Container, PropertyName, "-", "Hidden", HtmlAttributes: new { __NoTemplate = true, @class = Forms.CssFormNoSubmit })}");

            using (Manager.StartNestedComponent(FieldName)) {

                hb.Append($@"
    <div class='t_collection'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.Collection))}
        {await HtmlHelper.ForEditAsync(ps, nameof(ps.Collection))}
    </div>
    <div class='t_skinpage'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.PageFileName))}
        {await HtmlHelper.ForEditAsync(ps, nameof(ps.PageFileName))}
    </div>
    <div class='t_skinpopup'>
        {await HtmlHelper.ForLabelAsync(ps, nameof(ps.PopupFileName))}
        {await HtmlHelper.ForEditAsync(ps, nameof(ps.PopupFileName))}
    </div>
</div>");
                Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.SkinEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            }
            return hb.ToString();
        }
    }
}
