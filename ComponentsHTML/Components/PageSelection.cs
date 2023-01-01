/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Addons;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the PageSelection component implementation.
    /// </summary>
    public abstract class PageSelectionComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(PageSelectionComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "PageSelection";

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
    /// Displays the page name based on the model's page Guid.
    /// </summary>
    public class PageSelectionComponentDisplay : PageSelectionComponentBase, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

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
        public async Task<string> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {
            HtmlBuilder hb = new HtmlBuilder();

            if (model != null) {
                PageDefinition? page = await PageDefinition.LoadPageDefinitionAsync((Guid)model);
                if (page == null)
                    hb.Append(__ResStr("notFound", "(Page not found - {0})", model.ToString()!));
                else
                    hb.Append(HE(page.Url));
            }
            return hb.ToString();
        }
    }

    /// <summary>
    /// Allows selection of a designed page using a dropdown list. The model contains the page Guid.
    /// </summary>
    /// <remarks>
    /// When a page is selected, a link is shown to open the selected page in a new window.
    /// </remarks>
    /// <example>
    /// [Category("Skin"), Caption("Template"), Description("The local designed page used as a template for this page - All modules from the template are copied and rendered on this page in their defined pane - Modules in panes that are not available are not shown - Any page can be used as a template")]
    /// [UIHint("PageSelection"), Trim]
    /// public Guid? TemplateGuid { get; set; }
    /// </example>
    public class PageSelectionComponentEdit : PageSelectionComponentBase, IYetaWFComponent<Guid>, IYetaWFComponent<Guid?> {

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
        public async Task<string> RenderAsync(Guid model) {
            return await RenderAsync((Guid?)model);
        }
        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(Guid? model) {

            // dropdown
            List<SelectionItem<string>> list;
            list = (
                from p in await PageDefinition.GetDesignedPagesAsync() orderby p.Url select
                    new SelectionItem<string> {
                        Text = p.Url,
                        Value = p.PageGuid.ToString(),
                    }).ToList<SelectionItem<string>>();
            list.Insert(0, new SelectionItem<string> { Text = __ResStr("select", "(select)"), Value = string.Empty });

            string ddList = await DropDownListComponent.RenderDropDownListAsync(this, (model ?? Guid.Empty).ToString(), list, null);

            PageDefinition? page = null;
            if (model != null)
                page = await PageDefinition.LoadAsync((Guid)model);

            // link
            string href = page?.EvaluatedCanonicalUrl ?? string.Empty;
            string tt =__ResStr("linkTT", "Click to preview the page in a new window - not all pages can be displayed correctly and may require additional parameters");

            string tags = $@"
<div id='{DivId}' class='yt_pageselection t_edit'>
    <div class='t_select'>
        {ddList}
    </div>
    <div class='t_link'>
        <a href='{HAE(href)}' target='_blank' rel='nofollow noopener noreferrer' {Basics.CssTooltip}='{HAE(tt)}'>{ImageHTML.BuildKnownIcon("#PagePreview", sprites: Info.PredefSpriteIcons)}</a>
    </div>
    <div class='t_description'>
    </div>
</div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.PageSelectionEditComponent('{DivId}');");

            return tags;
        }
    }
}
