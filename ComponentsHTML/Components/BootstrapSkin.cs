/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// Base class for the BootstrapSkin component implementation.
    /// </summary>
    public abstract class BootstrapSkinComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(BootstrapSkinComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "BootstrapSkin";

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
    /// Displays a Bootswatch theme name. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Default Bootstrap Skin"), Description("The default skin for overall page appearance and Bootstrap elements (only supported for skins that support Bootswatch) - individual pages can override the default skin")]
    /// [UIHint("BootstrapSkin"), ReadOnly]
    /// public string BootstrapSkin { get; set; }
    /// </example>
    public class BootstrapSkinDisplayComponent : BootstrapSkinComponentBase, IYetaWFComponent<string> {

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

            if (string.IsNullOrWhiteSpace(model))
                return Task.FromResult<string>(null);

            hb.Append($@"<div class='yt_bootstrapskin t_display'>
                {HE(model)}
            </div>");

            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows selection of an installed Bootswatch theme using a dropdown list.
    /// </summary>
    /// <example>
    /// [Category("Skin"), Caption("Default Bootstrap Skin"), Description("The default skin for overall page appearance and Bootstrap elements (only supported for skins that support Bootswatch) - individual pages can override the default skin")]
    /// [HelpLink("https://www.bootstrapcdn.com/bootswatch/")]
    /// [UIHint("BootstrapSkin"), StringLength(SkinDefinition.MaxName), AdditionalMetadata("NoDefault", true), Trim]
    /// public string BootstrapSkin { get; set; }
    /// </example>
    [UsesAdditional("NoDefault", "bool", "false", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    public class BootstrapSkinEditComponent : BootstrapSkinComponentBase, IYetaWFComponent<string> {

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
            List<SelectionItem<string>> list = (from theme in await skinAccess.GetBootstrapThemeListAsync() select new SelectionItem<string>() {
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
                model = await SkinAccess.GetBootstrapDefaultSkinAsync();

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_bootstrapskin");
        }
    }
}
