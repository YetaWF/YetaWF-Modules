/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Allows selection of a string value from a list of string values using a dropdown list. 
    /// This is used internally for pane selection and is not intended for use by applications.
    /// </summary>
    public class PaneSelectionComponent : YetaWFComponent, IYetaWFComponent<string> {

        internal const string TemplateName = "PaneSelection";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(string model) {

            List<string> list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<string>();
            List<SelectionItem<string>> itemList = new List<SelectionItem<string>>();
            foreach (string l in list) {
                itemList.Add(new SelectionItem<string> {
                    Text = l,
                    Value = l,
                });
            }
            return await DropDownListComponent.RenderDropDownListAsync(this, model, itemList, "yt_paneselection");
        }
    }
}
