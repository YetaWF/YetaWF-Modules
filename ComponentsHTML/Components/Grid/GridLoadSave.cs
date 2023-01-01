/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Repository;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// This static class defines basic services offered by the Grid component used to save the current state of a grid, like column widths, sort order, visible columns, etc.
    /// </summary>
    internal static class GridLoadSave {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        /// <summary>
        /// This class implements grid layout, sorting and filtering information, so this information can be saved when it is updated by user actions, so it can be restored later (for example, if a page is reloaded).
        /// Applications should not manipulate these settings directly.
        /// </summary>
        public class GridSavedSettings {

            /// <summary>
            /// Defines the columns and their current settings.
            /// </summary>
            public GridDefinition.ColumnDictionary Columns { get; set; }

            /// <summary>
            /// Defines the current grid page size, i.e., the maximum number of records shown per page.
            /// </summary>
            public int PageSize { get; set; }

            /// <summary>
            /// Defines the current page number shown. Page numbers are 1 based. These should be 0 based as they are 0 based in other classes, but oh well. Someday....
            /// </summary>
            public int CurrentPage { get; set; }

            /// <summary>
            /// Defines whether the current grid is collapsed (only applies to grids with a panel header).
            /// </summary>
            public bool Collapsed { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public GridSavedSettings() {
                Columns = new GridDefinition.ColumnDictionary();
                PageSize = 10;
                CurrentPage = 1;
            }

            /// <summary>
            /// Returns the current sort order for columns.
            /// </summary>
            /// <returns>A list of columns that have a defined sort order.</returns>
            public List<DataProviderSortInfo>? GetSortInfo() {
                foreach (var keyVal in Columns) {
                    string colName = keyVal.Key;
                    GridDefinition.ColumnInfo col = keyVal.Value;
                    if (col.Sort != GridDefinition.SortBy.NotSpecified) {
                        return new List<DataProviderSortInfo>() {
                            new DataProviderSortInfo {
                                Field = colName,
                                Order = col.Sort == GridDefinition.SortBy.Descending ? DataProviderSortInfo.SortDirection.Descending : DataProviderSortInfo.SortDirection.Ascending ,
                            },
                        };
                    }
                }
                return null;
            }
            /// <summary>
            /// Returns the current filter settings for columns.
            /// </summary>
            /// <returns>A list of columns that have a defined filter setting.</returns>
            public List<DataProviderFilterInfo>? GetFilterInfo() {
                List<DataProviderFilterInfo> list = new List<DataProviderFilterInfo>();
                foreach (var keyVal in Columns) {
                    string colName = keyVal.Key;
                    GridDefinition.ColumnInfo col = keyVal.Value;
                    if (!string.IsNullOrWhiteSpace(col.FilterOperator)) {
                        list.Add(new DataProviderFilterInfo {
                            Field = colName,
                            Operator = col.FilterOperator,
                            ValueAsString = col.FilterValue,
                        });
                    }
                }
                return list.Count > 0 ? list : null;
            }
        }

        public static bool UseGridSettings(Guid? moduleGuid) {
            return moduleGuid != null && moduleGuid != Guid.Empty;
        }

        /// <summary>
        /// Loads grid settings that have been previously saved for a specific module.
        /// If no saved settings are available, default settings are returned.
        /// </summary>
        /// <param name="moduleGuid">The module Guid of the module for which grid settings have been saved.</param>
        /// <param name="defaultInitialPage">Defines the default initial page within the grid. This page number is 1 based.</param>
        /// <param name="defaultPageSize">Defines the default initial page size of the grid.</param>
        /// <returns>Returns grid settings for the specified module.</returns>
        /// <remarks>Grid settings that are saved on behalf of modules are used whenever the module is displayed. This means that the same settings apply even if a module is used on several pages.
        ///
        /// This method is not used by applications. It is reserved for component implementation.</remarks>
        public static GridSavedSettings LoadModuleSettings(Guid moduleGuid, int defaultInitialPage = 1, int defaultPageSize = 10) {
            GridSavedSettings? gridSavedSettings;
            if (UseGridSettings(moduleGuid)) {
                SettingsDictionary modSettings = Manager.SessionSettings.GetModuleSettings(moduleGuid);
                gridSavedSettings = modSettings.GetValue<GridSavedSettings>("GridSavedSettings");
                if (gridSavedSettings == null) {
                    gridSavedSettings = new GridSavedSettings() {
                        CurrentPage = defaultInitialPage,
                        PageSize = defaultPageSize,
                    };
                }
            } else
                throw new InternalError($"{nameof(moduleGuid)} not available to load grid settings");
            return gridSavedSettings;
        }
        /// <summary>
        /// Save grid settings for a specific module.
        /// </summary>
        /// <param name="moduleGuid">The module Guid of the module for which grid settings are saved.</param>
        /// <param name="gridSavedSettings">The grid settings to be saved.</param>
        /// <remarks>This method is not used by applications. It is reserved for component implementation.</remarks>
        public static void SaveModuleSettings(Guid moduleGuid, GridSavedSettings gridSavedSettings) {
            if (UseGridSettings(moduleGuid)) {
                SettingsDictionary modSettings = Manager.SessionSettings.GetModuleSettings(moduleGuid);
                if (modSettings != null) {
                    modSettings.SetValue<GridSavedSettings>("GridSavedSettings", gridSavedSettings);
                    modSettings.Save();
                }
            } else
                throw new InternalError($"{nameof(moduleGuid)} not available to save grid settings");
        }

        public static async Task SaveSettingsAsync(GridPartialData gridData) {

            // save the current sort order and page size
            if (UseGridSettings(gridData.GridDef.SettingsModuleGuid)) {
                GridLoadSave.GridSavedSettings? gridSavedSettings = GridLoadSave.LoadModuleSettings((Guid)gridData.GridDef.SettingsModuleGuid!);
                gridSavedSettings.PageSize = gridData.Take;
                if (gridData.Take == 0)
                    gridSavedSettings.CurrentPage = 1;
                else
                    gridSavedSettings.CurrentPage = Math.Max(1, gridData.Skip / gridData.Take + 1);
                foreach (GridDefinition.ColumnInfo col in gridSavedSettings.Columns.Values)
                    col.Sort = GridDefinition.SortBy.NotSpecified;
                if (gridData.Sorts != null) {
                    foreach (var sortCol in gridData.Sorts) {
                        GridDefinition.SortBy sortDir = (sortCol.Order == DataProviderSortInfo.SortDirection.Ascending) ? GridDefinition.SortBy.Ascending : GridDefinition.SortBy.Descending;
                        if (gridSavedSettings.Columns.ContainsKey(sortCol.Field))
                            gridSavedSettings.Columns[sortCol.Field].Sort = sortDir;
                        else
                            gridSavedSettings.Columns.Add(sortCol.Field, new GridDefinition.ColumnInfo { Sort = sortDir });
                    }
                }
                GridDictionaryInfo.ReadGridDictionaryInfo dictInfo = await GridDictionaryInfo.LoadGridColumnDefinitionsAsync(gridData.GridDef);
                foreach (GridDefinition.ColumnInfo col in gridSavedSettings.Columns.Values) {
                    col.FilterOperator = null;
                    col.FilterValue = string.Empty;
                }
                if (!gridData.Search && gridData.Filters != null && dictInfo.SaveColumnFilters != false) {
                    foreach (var filterCol in gridData.Filters) {
                        if (filterCol.Field != null) {
                            if (gridSavedSettings.Columns.ContainsKey(filterCol.Field)) {
                                gridSavedSettings.Columns[filterCol.Field].FilterOperator = filterCol.Operator;
                                gridSavedSettings.Columns[filterCol.Field].FilterValue = filterCol.ValueAsString;
                            } else {
                                gridSavedSettings.Columns.Add(filterCol.Field, new GridDefinition.ColumnInfo {
                                    FilterOperator = filterCol.Operator,
                                    FilterValue = filterCol.ValueAsString,
                                });
                            }
                        }
                    }
                }
                GridLoadSave.SaveModuleSettings((Guid)gridData.GridDef.SettingsModuleGuid, gridSavedSettings);
            }
        }
    }
}