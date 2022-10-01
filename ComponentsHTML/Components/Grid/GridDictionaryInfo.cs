/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    internal static partial class GridDictionaryInfo {

        public class ReadGridDictionaryInfo {

            public Dictionary<string, GridColumnInfo> ColumnInfo { get; set; }
            public string? SortColumn { get; set; }
            public GridDefinition.SortBy SortBy { get; set; }

            public bool? ShowPanelHeader { get; set; }
            public bool? ShowHeader { get; set; }
            public bool? ShowFilter { get; set; }
            public bool? ShowPager { get; set; }
            public bool? SupportReload { get; set; }

            public GridDefinition.SizeStyleEnum? SizeStyle { get; set; }

            public bool? SaveColumnWidths { get; set; }
            public bool? SaveColumnFilters { get; set; }

            public int? InitialPageSize { get; set; }
            public List<int>? PageSizes { get; set; }

            public bool? Reorderable { get; set; }
            public bool? HighlightOnClick { get; set; }
            public bool? UseSkinFormatting { get; set; }

            public bool? PanelCanMinimize { get; set; }
            public bool? PanelHeaderSearch { get; set; }
            public int? PanelHeaderAutoSearch { get; set; }
            public bool? PanelHeaderColumnSelection { get; set; }

            public bool Success { get; set; }

            public int VisibleColumns {
                get {
                    return (from c in ColumnInfo.Values where !c.Hidden select c).Count();
                }
            }

            public ColumnVisibilityStatus GetColumnStatus(string name) {
                if (!ColumnInfo.TryGetValue(name, out GridColumnInfo? colInfo))
                    return ColumnVisibilityStatus.Shown;
                return colInfo.Visibility;
            }

            public ReadGridDictionaryInfo() {
                ColumnInfo = new Dictionary<string, GridColumnInfo>();
            }
        }

        /// <summary>
        /// Loads the grid column definitions for a grid.
        /// </summary>
        /// <param name="gridDef">The GridDefinition object describing the grid.</param>
        /// <returns>A GridDictionaryInfo.ReadGridDictionaryInfo object describing the grid.</returns>
        /// <remarks>This method is not used by applications. It is reserved for component implementation.</remarks>
        public static async Task<GridDictionaryInfo.ReadGridDictionaryInfo> LoadGridColumnDefinitionsAsync(GridDefinition gridDef) {

            GridDictionaryInfo.ReadGridDictionaryInfo? dictInfo = (GridDictionaryInfo.ReadGridDictionaryInfo?)gridDef.CachedData;
            if (dictInfo == null || !YetaWFManager.Deployed) {// don't use cached grid info in development

                gridDef.CachedData = await LoadGridColumnDefinitionsAsync(gridDef.RecordType);
                dictInfo = (GridDictionaryInfo.ReadGridDictionaryInfo)gridDef.CachedData;

                // set grid model from dictionary overrides
                if (dictInfo.ShowPanelHeader != null)
                    gridDef.PanelHeader = (bool)dictInfo.ShowPanelHeader;
                if (dictInfo.ShowHeader != null)
                    gridDef.ShowHeader = (bool)dictInfo.ShowHeader;
                if (dictInfo.ShowFilter != null)
                    gridDef.ShowFilter = (bool)dictInfo.ShowFilter;
                if (dictInfo.ShowPager != null && gridDef.ShowPager)
                    gridDef.ShowPager = (bool)dictInfo.ShowPager;
                if (dictInfo.SupportReload != null)
                    gridDef.SupportReload = (bool)dictInfo.SupportReload;

                if (dictInfo.SizeStyle != null)
                    gridDef.SizeStyle = (GridDefinition.SizeStyleEnum)dictInfo.SizeStyle;

                if (dictInfo.InitialPageSize != null && dictInfo.PageSizes != null) {
                    gridDef.InitialPageSize = (int)dictInfo.InitialPageSize;
                    gridDef.PageSizes = dictInfo.PageSizes;
                }

                if (dictInfo.Reorderable != null)
                    gridDef.Reorderable = (bool)dictInfo.Reorderable;
                if (dictInfo.HighlightOnClick != null)
                    gridDef.HighlightOnClick = (bool)dictInfo.HighlightOnClick;
                if (dictInfo.UseSkinFormatting != null)
                    gridDef.UseSkinFormatting = (bool)dictInfo.UseSkinFormatting;

                if (dictInfo.PanelCanMinimize != null)
                    gridDef.PanelCanMinimize = (bool)dictInfo.PanelCanMinimize;
                if (dictInfo.PanelHeaderSearch != null)
                    gridDef.PanelHeaderSearch = (bool)dictInfo.PanelHeaderSearch;
                if (dictInfo.PanelHeaderAutoSearch != null)
                    gridDef.PanelHeaderAutoSearch = (int)dictInfo.PanelHeaderAutoSearch;
                if (dictInfo.PanelHeaderColumnSelection != null)
                    gridDef.PanelHeaderColumnSelection = (bool)dictInfo.PanelHeaderColumnSelection;

                if (gridDef.Reorderable) {
                    if (gridDef.InitialPageSize != 0 || !gridDef.IsStatic)
                        throw new InternalError("Unsupported options used for reorderable grid");
                }
            }
            return dictInfo;
        }

        /// <summary>
        /// Loads the grid column definitions for a grid.
        /// </summary>
        /// <param name="recordType">The record type for which grid column definitions are to be loaded.</param>
        /// <returns>A GridDictionaryInfo.ReadGridDictionaryInfo object describing the grid.</returns>
        /// <remarks>This method is not used by applications. It is reserved for component implementation.</remarks>
        private static async Task<GridDictionaryInfo.ReadGridDictionaryInfo> LoadGridColumnDefinitionsAsync(Type recordType) {
            string className = recordType.FullName!.Split(new char[] { '.' }).Last();
            string[] s = className.Split(new char[] { '+' });
            int len = s.Length;
            if (len != 2) throw new InternalError("Unexpected class {0} in record type {1}", className, recordType.FullName);
            string controller = s[0];
            string model = s[1];
            string file = controller + "." + model;
            Package package = Package.GetPackageFromType(recordType);
            string predefUrl = Package.GetAddOnPackageUrl(package.AreaName) + "Grids/" + file;
            string customUrl = Package.GetCustomUrlFromUrl(predefUrl);

            GridDictionaryInfo.ReadGridDictionaryInfo? info = null;
            GridDictionaryInfo.ReadGridDictionaryInfo yamlInfo = await GridDictionaryInfo.ReadGridDictionaryYamlAsync(package, Utility.UrlToPhysical(predefUrl + ".yaml"));
            if (yamlInfo.Success) {
                info = yamlInfo;
                GridDictionaryInfo.ReadGridDictionaryInfo customYamlInfo = await GridDictionaryInfo.ReadGridDictionaryYamlAsync(package, Utility.UrlToPhysical(customUrl + ".yaml"));
                if (customYamlInfo.Success)
                    info = customYamlInfo;
            } else {
                GridDictionaryInfo.ReadGridDictionaryInfo predefInfo = await GridDictionaryInfo.ReadGridDictionaryTextAsync(package, recordType, Utility.UrlToPhysical(predefUrl));
                if (predefInfo.Success) {
#if DEBUG
                    await SaveGridDictionaryYamlAsync(predefInfo, Utility.UrlToPhysical(predefUrl + ".yaml"));
#endif
                    info = predefInfo;
                    GridDictionaryInfo.ReadGridDictionaryInfo customInfo = await GridDictionaryInfo.ReadGridDictionaryTextAsync(package, recordType, Utility.UrlToPhysical(customUrl));
                    if (customInfo.Success) {
#if DEBUG
                        await SaveGridDictionaryYamlAsync(predefInfo, Utility.UrlToPhysical(customInfo + ".yaml"));
#endif
                        info = customInfo;
                    }
                }
            }
            if (info == null || info.ColumnInfo.Count == 0)
                throw new InternalError("No grid definition exists for {0}", file);
            return info;
        }
    }
}
