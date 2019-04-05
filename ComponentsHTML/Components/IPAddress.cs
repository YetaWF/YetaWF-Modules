/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the IPAddress component implementation.
    /// </summary>
    public abstract class IPAddressComponentBase : YetaWFComponent {

        internal const string TemplateName = "IPAddress";

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
    /// Implementation of the IPAddress display component.
    /// </summary>
    public class IPAddressDisplayComponent : IPAddressComponentBase, IYetaWFComponent<string> {

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
        public async Task<string> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(@"<div class='yt_ipaddress t_display'>");

            if (!string.IsNullOrWhiteSpace(model)) {

                hb.Append(HE(model));

                bool lookup = PropData.GetAdditionalAttributeValue("Lookup", true);
                if (lookup) {
                    ModuleDefinition modDisplay = await ModuleDefinition.LoadAsync(new Guid("{ad95564e-8eb7-4bcb-be64-dc6f1cd6b55d}"), AllowNone: true);
                    if (modDisplay != null) {
                        ModuleAction actionDisplay = await modDisplay.GetModuleActionAsync("DisplayHostName", null, model);
                        if (modDisplay != null)
                            hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
                        actionDisplay = await modDisplay.GetModuleActionAsync("DisplayGeoData", null, model);
                        if (modDisplay != null)
                            hb.Append(await actionDisplay.RenderAsync(ModuleAction.RenderModeEnum.IconsOnly));
                    }
                }
            }
            hb.Append(@"</div>");
            return hb.ToString();
        }
    }
}
