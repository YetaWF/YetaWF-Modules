/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the KendoUISkin component implementation.
    /// </summary>
    public abstract class KendoUISkinComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(KendoUISkinComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "KendoUISkin";

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
    /// Displays the model as a Kendo UI skin name. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Kendo UI Skin"), Description("The skin for Kendo UI elements (buttons, modal dialogs, etc.) used on this page")]
    /// [UIHint("KendoUISkin"), ReadOnly]
    /// public string KendoUISkin { get; set; }
    /// </example>
    public class KendoUISkinDisplayComponent : KendoUISkinComponentBase, IYetaWFComponent<string> {

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

            if (!string.IsNullOrWhiteSpace(model)) {
                hb.Append($@"<div class='yt_kendouiskin t_display'>
                    {HE(model)}
                </div>");
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows selection of a Kendo UI skin from the list of all installed Kendo UI skins using a dropdown list.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Kendo UI Skin"), Description("The skin for Kendo UI elements (buttons, modal dialogs, etc.) used on this page")]
    /// [UIHint("KendoUISkin"), StringLength(PageDefinition.MaxKendoUISkin), Trim]
    /// public string KendoUISkin { get; set; }
    /// </example>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    public class KendoUISkinEditComponent : KendoUISkinComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from theme in await skinAccess.GetKendoThemeListAsync() select new SelectionItem<string>() {
                Text = theme.Name,
                Tooltip = theme.Description,
                Value = theme.Name,
            }).ToList();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("default", "(Site Default)"),
                    Tooltip = __ResStr("defaultTT", "Use the site defined default theme"),
                    Value = "",
                });
            else if (model == null)
                model = await SkinAccess.GetKendoUIDefaultSkinAsync();

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_kendouiskin");
        }
    }
}
