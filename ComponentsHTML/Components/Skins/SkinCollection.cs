/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the SkinCollection component implementation.
    /// </summary>
    public abstract class SkinCollectionComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(SkinCollectionComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "SkinCollection";

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
    /// Implementation of the SkinCollection display component.
    /// </summary>
    public class SkinCollectionDisplayComponent : SkinCollectionComponentBase, IYetaWFComponent<string> {

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

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();

            string desc = (from skinColl in skinAccess.GetAllSkinCollections() where model == skinColl.CollectionName select skinColl.CollectionDescription).FirstOrDefault();
            if (desc == null) {
                bool useDefault = !PropData.GetAdditionalAttributeValue("NoDefault", false);
                if (useDefault)
                    desc = __ResStr("siteDef", "(Site Default)");
            }
            if (string.IsNullOrWhiteSpace(desc))
                return Task.FromResult<string>(null);
            return Task.FromResult(HE(desc));
        }
    }

    /// <summary>
    /// Implementation of the SkinCollection edit component.
    /// </summary>
    public class SkinCollectionEditComponent : SkinCollectionComponentBase, IYetaWFComponent<string> {

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

            // get all available skins
            SkinAccess skinAccess = new SkinAccess();
            List<SelectionItem<string>> list = (from skinColl in skinAccess.GetAllSkinCollections() orderby skinColl.CollectionDescription select new SelectionItem<string>() {
                Text = skinColl.CollectionDescription,
                Value = skinColl.CollectionName,
            }).ToList();
            bool useDefault = !PropData.GetAdditionalAttributeValue("NoDefault", false);
            if (useDefault)
                list.Insert(0, new SelectionItem<string> {
                    Text = __ResStr("siteDef", "(Site Default)"),
                    Tooltip = __ResStr("siteDefTT", "Use the site defined default skin"),
                    Value = "",
                });
            // display the skins in a drop down
            return await DropDownListComponent.RenderDropDownListAsync(this, model, list, "yt_skinselection");
        }
    }
}
