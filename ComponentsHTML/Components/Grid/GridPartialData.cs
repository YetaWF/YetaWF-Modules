/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Renders partial grid data. This component is used by the grid component and is not directly used by an application.
    /// </summary>
    [PrivateComponent]
    public class GridDataContainer : YetaWFComponent, IYetaWFContainer<GridPartialData> {

        internal const string TemplateName = "GridPartialData";

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

        private class GridPartialResult {
            public int Records { get; set; }
            public string TBody { get; set; } = null!;
            public int Pages { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<Boolean> ColumnVisibility { get; set; } = null!;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderContainerAsync(GridPartialData model) {

            ScriptBuilder sb = new ScriptBuilder();

            GridDictionaryInfo.ReadGridDictionaryInfo dictInfo = await GridDictionaryInfo.LoadGridColumnDefinitionsAsync(model.GridDef);

            GridDefinition.ColumnDictionary colDict = new GridDefinition.ColumnDictionary();
            if (GridLoadSave.UseGridSettings(model.GridDef.SettingsModuleGuid)) {
                GridLoadSave.GridSavedSettings gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)model.GridDef.SettingsModuleGuid!, 1, 10);
                colDict = gridSavedSettings.Columns;
            }

            // render all records
            string data = await GridDisplayComponent.RenderTableHTML(HtmlHelper, model.GridDef, model.Data, model.StaticData, dictInfo, colDict, model.FieldPrefix, true, model.Skip, model.Take);
            data += Manager.ScriptManager.RenderEndofPageScripts();// portions generated by components

            int pages = 0;
            int page = 0;
            int pageSize = model.Take;
            if (model.Data.Total > 0) {
                if (model.Take == 0) {
                    pages = 1;
                    pageSize = 0;
                } else {
                    pages = Math.Max(1, model.Data.Total / model.Take + (model.Data.Total % model.Take == 0 ? 0 : 1));
                    page = Math.Max(model.Skip / model.Take, 0);
                }
            }

            List<bool> colVisible = new List<bool>();
            foreach (string colName in dictInfo.ColumnInfo.Keys) {
                GridColumnInfo columnInfo = dictInfo.ColumnInfo[colName];
                if (!columnInfo.Hidden) {
                    colDict.TryGetValue(colName, out GridDefinition.ColumnInfo? colInfo);
                    colVisible.Add(colInfo != null ? colInfo.Visible : dictInfo.GetColumnStatus(colName) != ColumnVisibilityStatus.NotShown);
                }
            }

            GridPartialResult result = new GridPartialResult {
                Records = model.Data.Total,
                TBody = data,
                Pages = pages,
                Page = page,
                PageSize = pageSize,
                ColumnVisibility = colVisible,
            };

            sb.Append(Utility.JsonSerialize(result));

            return sb.ToString();
        }
    }
}
