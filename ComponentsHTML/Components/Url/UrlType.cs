/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Internal component used by the Url component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    public class UrlTypeComponent : YetaWFComponent, IYetaWFComponent<UrlTypeEnum> {

        internal const string TemplateName = "UrlType";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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
        public async Task<string> RenderAsync(UrlTypeEnum model) {

            List<SelectionItem<string>> items = new List<SelectionItem<string>>();
            if ((model & UrlTypeEnum.Local) != 0) {
                items.Add(new SelectionItem<string> {
                    Text = this.__ResStr("selLocal", "Designed Page"),
                    Value = ((int)UrlTypeEnum.Local).ToString(),
                    Tooltip = this.__ResStr("selLocalTT", "Select for local, designed pages"),
                });
            }
            if ((model & UrlTypeEnum.Remote) != 0) {
                items.Add(new SelectionItem<string> {
                    Text = this.__ResStr("selRemote", "Local/Remote Url"),
                    Value = ((int)UrlTypeEnum.Remote).ToString(),
                    Tooltip = this.__ResStr("selRemoteTT", "Select to enter a Url (local or remote) - Can contain query string arguments - Local Urls start with /, remote Urls with http:// or https://"),
                });
            }
            if ((model & UrlTypeEnum.New) != 0)
                throw new InternalError("New url not supported by this template");

            return await DropDownListComponent.RenderDropDownListAsync(this, model.ToString(), items, "yt_urltype");
        }
    }
}
