﻿/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SkinNamePage component implementation.
    /// </summary>
    public abstract class SkinNamePageComponentBase : YetaWFComponent {

        internal const string TemplateName = "SkinNamePage";

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
    /// Implementation of the SkinNamePage display component.
    /// </summary>
    public class SkinNamePageDisplayComponent : SkinNamePageComponentBase, IYetaWFComponent<string> {

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
        public Task<YHtmlString> RenderAsync(string model) {

            // get all available page skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPageSkins(collection);

            string desc = (from skin in skinList where skin.PageViewName == model select skin.Name).FirstOrDefault();
            if (desc == null)
                desc = skinList.First().Description;
            if (string.IsNullOrWhiteSpace(desc))
                return Task.FromResult(new YHtmlString());
            return Task.FromResult(new YHtmlString(HE(desc)));
        }
    }

    /// <summary>
    /// Implementation of the SkinNamePage edit component.
    /// </summary>
    public class SkinNamePageEditComponent : SkinNamePageComponentBase, IYetaWFComponent<string> {

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
        public async Task<YHtmlString> RenderAsync(string model) {

            // get all available page skins for this collection
            SkinAccess skinAccess = new SkinAccess();
            string collection = GetSiblingProperty<string>($"{PropertyName}_Collection");
            PageSkinList skinList = skinAccess.GetAllPageSkins(collection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.PageViewName,
            }).ToList();
            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinname");
        }
        internal static YHtmlString RenderReplacementSkinsForCollection(string skinCollection) {
            SkinAccess skinAccess = new SkinAccess();
            PageSkinList skinList = skinAccess.GetAllPageSkins(skinCollection);
            List<SelectionItem<string>> list = (from skin in skinList orderby skin.Description select new SelectionItem<string>() {
                Text = skin.Name,
                Tooltip = skin.Description,
                Value = skin.PageViewName,
            }).ToList();
            // render a new dropdown list
            return DropDownListEditComponentBase<string>.RenderDataSource(list, null);
        }
    }
}
