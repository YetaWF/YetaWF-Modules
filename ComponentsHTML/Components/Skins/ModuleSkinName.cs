/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ModuleSkinName component implementation.
    /// </summary>
    public abstract class ModuleSkinNameComponent : YetaWFComponent {

        internal const string TemplateName = "ModuleSkinName";

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
    /// Internal component used by the ModuleSkins component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    public class ModuleSkinNameDisplayComponent : ModuleSkinNameComponent, IYetaWFComponent<string> {

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

            ModuleSkinList modSkins = GetSiblingProperty<ModuleSkinList>($"{PropertyName}_ModuleSkinList");
            string name = (from l in modSkins where l.Name == model select l.Name).FirstOrDefault();
            if (name == null)
                name = modSkins.First().Name;
            return Task.FromResult(HE(name));
        }
    }

    /// <summary>
    /// Internal component used by the ModuleSkins component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    public class ModuleSkinNameEditComponent : ModuleSkinNameComponent, IYetaWFComponent<string> {

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

            ModuleSkinList modSkins = GetSiblingProperty<ModuleSkinList>($"{PropertyName}_ModuleSkinList");
            List<SelectionItem<string>> list = (from l in modSkins select new SelectionItem<string>() {
                Text = l.Name,
                Tooltip = l.Description,
                Value = l.CssClass,
            }).ToList();
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, null);
        }
    }
}
