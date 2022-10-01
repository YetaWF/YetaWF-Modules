/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the GridDeleteEntry component implementation.
    /// </summary>
    public abstract class GridDeleteEntryComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridDeleteEntryComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "GridDeleteEntry";

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
    /// Renders a Delete icon in a grid entry. The model value is not used.
    /// </summary>
    /// <example>
    /// [Caption("Delete"), Description("Click to delete this URL path")]
    /// [UIHint("GridDeleteEntry"), ReadOnly]
    /// public int Delete { get; set; }
    /// </example>
    public class GridDeleteEntryDisplayComponent : GridDeleteEntryComponentBase, IYetaWFComponent<object> {

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
        public Task<string> RenderAsync(object model) {
            return Task.FromResult($"<span{FieldSetup(FieldType.Anonymous)}{HtmlBuilder.GetClassAttribute(HtmlAttributes)}>{ImageHTML.BuildKnownIcon("#RemoveLight", title: __ResStr("altRemove", "Remove"), name: "DeleteAction")}</span>");
        }
    }
}
