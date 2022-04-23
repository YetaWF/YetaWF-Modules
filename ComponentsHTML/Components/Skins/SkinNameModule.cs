/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
    /// Base class for the SkinNameModule component implementation.
    /// </summary>
    public abstract class SkinNameModuleComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SkinNameModuleComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "SkinNameModule";

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
    /// Internal component used by the Skin component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    public class SkinNameModuleDisplayComponent : SkinNameModuleComponentBase, IYetaWFComponent<string> {

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

            // get all available module skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string? collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            if (collection == null) throw new InternalError($"No value found for property {PropertyName}_Collection");
            ModuleSkinList skinList = skinAccess.GetAllModuleSkins(collection);

            ModuleSkinEntry? entry = (from skin in skinList where skin.CSS == model select skin).FirstOrDefault();
            string? name = null;
            if (entry == null)
                name = skinList.First().Name;
            else
                name = entry.Name;
            return Task.FromResult(HE(name));
        }
    }

    /// <summary>
    /// Internal component used by the Skin component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    [UsesAdditional("NoDefault", "bool", "true", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    public class SkinNameModuleEditComponent : SkinNameModuleComponentBase, IYetaWFComponent<string> {

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

            // get all available module skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string? collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            if (collection == null) throw new InternalError($"No value found for property {PropertyName}_Collection");
            ModuleSkinList skinList = skinAccess.GetAllModuleSkins(collection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.CSS,
            }).ToList();

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinnamemodule");
        }

        internal static string RenderReplacementSkinsForCollection(string skinCollection) {
            SkinAccess skinAccess = new SkinAccess();
            ModuleSkinList skinList = skinAccess.GetAllModuleSkins(skinCollection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.CSS,
            }).ToList();
            // render a new dropdown list
            return DropDownListEditComponentBase<string>.GetOptionsHTML(list);
        }
    }
}
