/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders the model as a module action (a button, icon or link). If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("View"), Description("View the complete blog entry")]
    /// [UIHint("ModuleAction"), AdditionalMetadata("RenderAs", ModuleAction.RenderModeEnum.LinksOnly), ReadOnly]
    /// public ModuleAction ViewAction { get; set; }
    /// </example>
    [UsesAdditional("RenderAs", "YetaWF.Core.Modules.ModuleAction.RenderModeEnum", "ModuleAction.RenderModeEnum.Button", "Defines how the module action is rendered.")]
    public class ModuleActionComponent : YetaWFComponent, IYetaWFComponent<ModuleAction> {

        internal const string TemplateName = "ModuleAction";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

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

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(ModuleAction model) {

            using (Manager.StartNestedComponent(FieldName)) {

                HtmlBuilder hb = new HtmlBuilder();

                ModuleAction.RenderModeEnum render = PropData.GetAdditionalAttributeValue("RenderAs", ModuleAction.RenderModeEnum.Button);
                if (model != null) {
                    hb.Append($@"
<div class='yt_moduleaction t_display'>
    {await model.RenderAsync(render)}
</div>");
                }
                return hb.ToString();
            }
        }
    }
}
