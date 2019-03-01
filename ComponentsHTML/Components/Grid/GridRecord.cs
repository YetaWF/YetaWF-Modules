/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Implementation of the GridRecord display component.
    /// </summary>
    public class GridRecordContainer : YetaWFComponent, IYetaWFContainer<GridRecordData> {

        internal const string TemplateName = "GridRecord";

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

        private class GridRecordResult {
            public string TR { get; set; }
            public object StaticData { get; internal set; }
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<YHtmlString> RenderContainerAsync(GridRecordData model) {

        ScriptBuilder sb = new ScriptBuilder();

            ObjectSupport.ReadGridDictionaryInfo dictInfo = await YetaWF.Core.Components.Grid.LoadGridColumnDefinitionsAsync(model.GridDef.RecordType);

            // render record
            string tr = await GridDisplayComponent.RenderRecordHTMLAsync(HtmlHelper, model.GridDef, dictInfo, model.FieldPrefix, model.Data, 0, 0, false);

            GridRecordResult result = new GridRecordResult {
                TR = tr,
                StaticData = model.Data,
            };
            sb.Append(YetaWFManager.JsonSerialize(result));

            return sb.ToYHtmlString();
        }
    }
}
