/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SkinNamePopup component implementation.
    /// </summary>
    public abstract class SkinNamePopupComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SkinNamePopupComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "SkinNamePopup";

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
    public class SkinNamePopupDisplayComponent : SkinNamePopupComponentBase, IYetaWFComponent<string> {

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

            // get all available popup skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPopupSkins(collection);

            string desc = (from skin in skinList where skin.ViewName == model select skin.Name).FirstOrDefault();
            if (desc == null)
                desc = skinList.First().Name;
            if (string.IsNullOrWhiteSpace(desc))
                return Task.FromResult<string>(null);
            return Task.FromResult(HE(desc));
        }
    }

    /// <summary>
    /// Internal component used by the Skin component. Not intended for application use.
    /// </summary>
    [PrivateComponent]
    [UsesAdditional("NoDefault", "bool", "true", "Defines whether a \"(Site Default)\" entry is automatically added as the first entry, with a value of null")]
    public class SkinNamePopupEditComponent : SkinNamePopupComponentBase, IYetaWFComponent<string> {

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

            // get all available popup skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPopupSkins(collection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.ViewName,
            }).ToList();

            bool useDefault = !PropData.GetAdditionalAttributeValue<bool>("NoDefault");
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("default", "(Site Default)"),
                    Tooltip = __ResStr("defaultTT", "Use the site defined default"),
                    Value = "",
                });

            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinnamepopup");
        }

        internal static string RenderReplacementSkinsForCollection(string skinCollection) {
            SkinAccess skinAccess = new SkinAccess();
            PageSkinList skinList = skinAccess.GetAllPopupSkins(skinCollection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.ViewName,
            }).ToList();
            // render a new dropdown list
            return DropDownListEditComponentBase<string>.GetOptionsHTML(list);
        }
    }
}
