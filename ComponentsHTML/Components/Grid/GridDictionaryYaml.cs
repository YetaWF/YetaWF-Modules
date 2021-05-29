/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YetaWF.Core;
using YetaWF.Core.Addons;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    internal static partial class GridDictionaryInfo {

        public class YamlConfig {
            public bool? ShowHeader { get; set; }
            public bool? ShowFilter { get; set; }
            public bool? ShowPager { get; set; }
            public bool? SupportReload { get; set; } // whether the data can be reloaded by the user (reload button, ajax only)

            public GridDefinition.SizeStyleEnum SizeStyle { get; set; }

            public bool? SaveColumnWidths { get; set; }
            public bool? SaveColumnFilters { get; set; }
            public int? InitialPageSize { get; set; }
            public List<int>? PageSizes { get; set; }

            public bool? Reorderable { get; set; }
            public bool? HighlightOnClick { get; set; }
            public bool? UseSkinFormatting { get; set; }

            public Panel? Panel { get; set; }

            public List<Column>? Columns { get; set; }
        }

        internal class Panel {
            public bool? Show { get; set; }
            public bool? CanMinimize { get; set; }
            public bool? Search { get; set; }
            public int? AutoSearch { get; set; }
            public bool? ColumnSelection { get; set; }
        }
        internal class Column {
            public string Name { get; set; } = null!;
            public int Icons { get; set; }
            public ColumnVisibilityStatus Visibility { get; set; }
            public GridDefinition.SortBy DefaultSort { get; set; }
            public bool Sort { get; set; }
            public bool Filter { get; set; }
            public List<GridColumnInfo.FilterOptionEnum> FilterOptions { get; set; }
            public string Width { get; set; } = null!;
            public GridHAlignmentEnum Align { get; set; }
            public bool Truncate { get; set; }
            public bool Locked { get; set; }
            public bool OnlySubmitWhenChecked { get; set; }
            public bool Internal { get; set; }
            public bool Hidden { get; set; }

            public Column() {
                FilterOptions = new List<GridColumnInfo.FilterOptionEnum>();
                Truncate = true;
            }

            private int PixWidth { get; set; }
            private int ChWidth { get; set; }

            internal void SetRealPixWidth(int value) {
                PixWidth = value;
            }
            internal int GetRealPixWidth() {
                return PixWidth;
            }
            internal void SetRealChWidth(int value) {
                ChWidth = value;
            }
            internal int GetRealChWidth() {
                return ChWidth;
            }
        }

        private static async Task<ReadGridDictionaryInfo> ReadGridDictionaryYamlAsync(Package package, Type recordType, string file) {

            using (ICacheDataProvider cacheDP = YetaWF.Core.IO.Caching.GetStaticSmallObjectCacheProvider()) {

                // Check cache first
                GetObjectInfo<ReadGridDictionaryInfo> info = await cacheDP.GetAsync<ReadGridDictionaryInfo>(file);
                if (info.Success)
                    return info.Data!;

                // Load the file
                Dictionary<string, GridColumnInfo> dict = new Dictionary<string, GridColumnInfo>();

                if (YetaWFManager.DiagnosticsMode) {// to avoid exception spam
                    if (!await FileSystem.FileSystemProvider.FileExistsAsync(file)) {
                        ReadGridDictionaryInfo dictInfo = new ReadGridDictionaryInfo {
                            ColumnInfo = dict,
                            SortColumn = null,
                            SortBy = GridDefinition.SortBy.NotSpecified,
                            Success = false,
                        };
                        await cacheDP.AddAsync<ReadGridDictionaryInfo>(file, dictInfo);// failure also saved in cache
                        return dictInfo;
                    }
                }

                string text;
                try {
                    text = await FileSystem.FileSystemProvider.ReadAllTextAsync(file);
                } catch (Exception) {
                    ReadGridDictionaryInfo dictInfo = new ReadGridDictionaryInfo {
                        ColumnInfo = dict,
                        SortColumn = null,
                        SortBy = GridDefinition.SortBy.NotSpecified,
                        Success = false,
                    };
                    await cacheDP.AddAsync<ReadGridDictionaryInfo>(file, dictInfo);// failure also saved in cache
                    return dictInfo;
                }

                // Parse the file and return as ReadGridDictionaryInfo
                {
                    YamlConfig config = Utility.YamlDeserialize<YamlConfig>(text);

                    if (config == null)
                        throw new InternalError($"Grid definition file {file} is empty");
                    if (config.Columns == null || config.Columns.Count == 0)
                        throw new InternalError($"Column definitions missing in {file}");

                    Column? sortCol = (from c in config.Columns where c.DefaultSort != GridDefinition.SortBy.NotSpecified select c).FirstOrDefault();

                    EvaluateColumnWidths(config.Columns, package, file);

                    List<GridColumnInfo.FilterOptionEnum> allFilters = GetAllFilterOptions();
                    Dictionary<string, GridColumnInfo> colInfo = new Dictionary<string, GridColumnInfo>();
                    foreach (Column col in config.Columns) {
                        GridColumnInfo c = new GridColumnInfo();
                        if (col.Icons > 0) col.Align = GridHAlignmentEnum.Center;
                        if (col.Filter) {
                            if (col.FilterOptions.Count <= 0)
                                col.FilterOptions = allFilters;
                        } else
                            col.FilterOptions = new List<GridColumnInfo.FilterOptionEnum>();
                        c.ChWidth = col.GetRealChWidth();
                        c.PixWidth = col.GetRealPixWidth();
                        ObjectSupport.CopyData(col, c);
                        colInfo.Add(col.Name, c);
                    }

                    ReadGridDictionaryInfo dictInfo = new ReadGridDictionaryInfo {
                        ColumnInfo = colInfo,
                        HighlightOnClick = config.HighlightOnClick,
                        InitialPageSize = config.InitialPageSize,
                        PageSizes = config.PageSizes,
                        PanelCanMinimize = config.Panel?.CanMinimize,
                        PanelHeaderAutoSearch = config.Panel?.AutoSearch,
                        PanelHeaderColumnSelection = config.Panel?.ColumnSelection,
                        PanelHeaderSearch = config.Panel?.Search,
                        Reorderable = config.Reorderable,
                        SaveColumnFilters = config.SaveColumnFilters,
                        SaveColumnWidths = config.SaveColumnWidths,
                        ShowFilter = config.ShowFilter,
                        ShowHeader = config.ShowHeader,
                        ShowPager = config.ShowPager,
                        ShowPanelHeader = config.Panel?.Show,
                        SizeStyle = config.SizeStyle,
                        SortBy = sortCol?.DefaultSort ?? GridDefinition.SortBy.NotSpecified,
                        SortColumn = sortCol?.Name,
                        SupportReload = config.SupportReload,
                        UseSkinFormatting = config.UseSkinFormatting,

                        Success = true,
                    };

                    // save in cache
                    await cacheDP.AddAsync<ReadGridDictionaryInfo>(file, dictInfo);

                    return dictInfo;
                }
            }
        }

        private static void EvaluateColumnWidths(List<Column> columns, Package package, string file) {
            foreach (Column col in columns) {
                string part = col.Width;
                if (!string.IsNullOrWhiteSpace(part)) {
                    if (part.EndsWith("pix")) {
                        string valueText = part.Substring(0, part.Length - 3);
                        int value;
                        try {
                            value = Convert.ToInt32(valueText);
                        } catch (Exception) {
                            throw new InternalError($"Invalid value {valueText} in file {file}");
                        }
                        col.SetRealPixWidth(value);
                    } else {
                        string valueText = GetPart(part, package, file, col.Name);
                        int value;
                        try {
                            value = Convert.ToInt32(valueText);
                        } catch (Exception) {
                            throw new InternalError($"Invalid value {valueText} in file {file}");
                        }
                        col.SetRealChWidth(value);
                    }
                }
            }
        }

        private static async Task SaveGridDictionaryYamlAsync(GridDictionaryInfo.ReadGridDictionaryInfo dictInfo, string file) {

            List<GridColumnInfo.FilterOptionEnum> allFilters = GetAllFilterOptions();

            List<Column> columns = new List<Column>();
            foreach (string colName in dictInfo.ColumnInfo.Keys) {

                GridColumnInfo c = dictInfo.ColumnInfo[colName];
                Column col = new Column();
                col.Name = colName;
                ObjectSupport.CopyData(c, col);
                if (dictInfo.SortColumn == col.Name)
                    col.DefaultSort = dictInfo.SortBy;

                if (col.FilterOptions.Count > 0) col.Filter = true;
                if (SameFilters(col.FilterOptions, allFilters))
                    col.FilterOptions = new List<GridColumnInfo.FilterOptionEnum>(); 

                if (!string.IsNullOrWhiteSpace(c.OriginalWidthCh))
                    col.Width = c.OriginalWidthCh.Replace("[","<").Replace("]",">");
                else {
                    if (c.ChWidth > 0)
                        col.Width = c.ChWidth.ToString();
                    else if (c.PixWidth > 0)
                        col.Width = $"{c.PixWidth}pix";
                }
                columns.Add(col);
            }

            YamlConfig config = new YamlConfig {
                Columns = columns,
                HighlightOnClick = dictInfo.HighlightOnClick,
                InitialPageSize = dictInfo.InitialPageSize,
                PageSizes = dictInfo.PageSizes,
                Panel = new Panel {
                    AutoSearch = dictInfo.PanelHeaderAutoSearch,
                    CanMinimize = dictInfo.PanelCanMinimize,
                    ColumnSelection = dictInfo.PanelHeaderColumnSelection,
                    Search = dictInfo.PanelHeaderSearch,
                    Show = dictInfo.ShowPanelHeader,
                },
                Reorderable = dictInfo.Reorderable,
                SaveColumnFilters = dictInfo.SaveColumnFilters,
                SaveColumnWidths = dictInfo.SaveColumnWidths,
                ShowFilter = dictInfo.ShowFilter,
                ShowHeader = dictInfo.ShowHeader,
                ShowPager = dictInfo.ShowPager,
                SizeStyle = dictInfo.SizeStyle ?? GridDefinition.SizeStyleEnum.SizeToFit,
                SupportReload = dictInfo.SupportReload,
                UseSkinFormatting = dictInfo.UseSkinFormatting,
            };

            string text = Utility.YamlSerialize(config);
            await FileSystem.FileSystemProvider.WriteAllTextAsync(file, $"# Generated from flat grid definition file on {DateTime.Now}\r\n\r\n");
            await FileSystem.FileSystemProvider.AppendAllTextAsync(file, text);
        }

        private static bool SameFilters(List<GridColumnInfo.FilterOptionEnum> filterOptions, List<GridColumnInfo.FilterOptionEnum> allFilters) {
            return !allFilters.Except(filterOptions).Any();
        }
    }
}
