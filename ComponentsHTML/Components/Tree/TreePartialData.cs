﻿/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.ResponseFilter;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders partial tree data. This component is used by the tree component and is not directly used by an application.
    /// </summary>
    [PrivateComponent]
    public class TreeDataContainer : YetaWFComponent, IYetaWFContainer<TreePartialData> {

        internal const string TemplateName = "TreePartialData";

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

        private class TreePartialResult {
            public int Records { get; set; }
            public string HTML { get; set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderContainerAsync(TreePartialData model) {

        ScriptBuilder sb = new ScriptBuilder();

            // render all records
            string data = await TreeDisplayComponent.RenderRecordsHTMLAsync(HtmlHelper, model.TreeDef, model.Data.Data);

            // because this is returned as json (ultimately), we need to do whitespace compression here otherwise
            // we get a mismatch between original server generated tree and dynamic updates via client side
            data = WhiteSpaceResponseFilter.Compress(data);

            data += Manager.ScriptManager.RenderEndofPageScripts();// portions generated by components

            TreePartialResult result = new TreePartialResult {
                Records = model.Data.Total,
                HTML = data,
            };

            sb.Append(Utility.JsonSerialize(result));
            return sb.ToString();
        }
    }
}
