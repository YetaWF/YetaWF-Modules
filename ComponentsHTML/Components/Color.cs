/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Color component implementation.
    /// </summary>
    public abstract class ColorComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ColorComponentBase), name, defaultValue, parms); }

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
        public override string GetTemplateName() { return "Color"; }
    }

    ///// <summary>
    ///// Base class for the Color display component implementation.
    ///// </summary>
    //public class ColorDisplayComponent : ColorComponentBase, IYetaWFComponent<string> {

    //    /// <summary>
    //    /// Returns the component type (edit/display).
    //    /// </summary>
    //    /// <returns>Returns the component type.</returns>
    //    public override ComponentType GetComponentType() { return ComponentType.Display; }

    //    /// <summary>
    //    /// Constructor.
    //    /// </summary>
    //    public ColorDisplayComponent() { }

    //    /// <summary>
    //    /// Called by the framework when the component needs to be rendered as HTML.
    //    /// </summary>
    //    /// <param name="model">The model being rendered by the component.</param>
    //    /// <returns>The component rendered as HTML.</returns>
    //    public async Task<string> RenderAsync(string model) {

    //        HtmlBuilder hb = new HtmlBuilder();

    //        return hb.ToString();
    //    }
    //}

    /// <summary>
    /// Base class for the Color edit component implementation.
    /// </summary>
    public class ColorEditComponent : ColorComponentBase, IYetaWFComponent<string> {

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

            HtmlBuilder hb = new HtmlBuilder();

            model ??= string.Empty;
            string selModel = "#FFFFFF";
            if (model.StartsWith("#"))
                selModel = model;

            hb.Append($@"

<div class='yt_color t_edit' id='{DivId}'>
    {await TextEditComponent.RenderTextAsync(this, model, null)}
    <input type='color' value='{HAE(selModel)}' class='t_selector'>
</div>");
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.ColorEditComponent('{DivId}');");// , {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }
    }
}
