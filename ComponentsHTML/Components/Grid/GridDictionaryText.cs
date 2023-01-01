/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    internal static partial class GridDictionaryInfo {

        internal class Config {
            public bool? SaveFilters { get; set; }
            public bool? SaveWidths { get; set; }
            public int? InitialPageSize { get; set; }
            public List<int>? PageSizes { get; set; }
            public bool? ShowHeader { get; set; }
            public bool? ShowFilter { get; set; }
            public bool? ShowPager { get; set; }
            public GridDefinition.SizeStyleEnum? SizeStyle { get; set; }
        }

        private static async Task<ReadGridDictionaryInfo> ReadGridDictionaryTextAsync(Package package, Type recordType, string file) {

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

                List<string> lines;
                try {
                    lines = await FileSystem.FileSystemProvider.ReadAllLinesAsync(file);
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

                // Parse the file
                string? sortCol = null;
                GridDefinition.SortBy sortDir = GridDefinition.SortBy.NotSpecified;

                Config? config = null;

                foreach (string line in lines) {
                    string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    GridColumnInfo gridCol = new GridColumnInfo();
                    int len = parts.Length;
                    if (len > 0) {
                        bool add = true;
                        string name = parts[0];
                        if (name == "Config") {
                            if (config != null)
                                throw new InternalError($"Only one Config statement can be used in {file}");
                            string rest = string.Join(" ", parts.Skip(1));
                            config = Utility.JsonDeserialize<Config>(rest);
                            continue;
                        }
                        for (int i = 1; i < len; ++i) {
                            string part = GetPart(parts[i], package, file, name);
                            if (string.Compare(part, "sort", true) == 0) gridCol.Sort = true;
                            else if (string.Compare(part, "locked", true) == 0) gridCol.Locked = true;
                            else if (string.Compare(part, "left", true) == 0) gridCol.Align = GridHAlignmentEnum.Left;
                            else if (string.Compare(part, "center", true) == 0) gridCol.Align = GridHAlignmentEnum.Center;
                            else if (string.Compare(part, "right", true) == 0) gridCol.Align = GridHAlignmentEnum.Right;
                            else if (string.Compare(part, "hidden", true) == 0) gridCol.Hidden = true;
                            else if (string.Compare(part, "truncate", true) == 0) gridCol.Truncate = true;
                            else if (string.Compare(part, "onlysubmitwhenchecked", true) == 0) gridCol.OnlySubmitWhenChecked = true;
                            else if (string.Compare(part, "icons", true) == 0) {
                                int n = GetNextNumber(parts, i, part, file, name);
                                if (n < 1) throw new InternalError("Icons must be >= 1 for column {0} in {1}", name, file);
                                gridCol.Icons = n;
                                gridCol.Align = GridHAlignmentEnum.Center;// default is centered
                                ++i;
                            } else if (string.Compare(part, "defaultSort", true) == 0) {
                                sortCol = name;
                                part = GetNextPart(parts, i, part, file, name);
                                if (part == "asc") sortDir = GridDefinition.SortBy.Ascending;
                                else if (part == "desc") sortDir = GridDefinition.SortBy.Descending;
                                else throw new InternalError("Missing Asc/Desc following defaultSort for column {1} in {2}", part, name, file);
                                ++i;
                            } else if (string.Compare(part, "internal", true) == 0) {
                                bool showInternals = UserSettings.GetProperty<bool>("ShowInternals");
                                if (!showInternals) {
                                    add = false;
                                    break;
                                }
                            } else if (string.Compare(part, "notshown", true) == 0) {
                                gridCol.Visibility = ColumnVisibilityStatus.NotShown;
                            } else if (string.Compare(part, "alwaysshown", true) == 0) {
                                gridCol.Visibility = ColumnVisibilityStatus.AlwaysShown;
                            } else if (string.Compare(part, "filter", true) == 0) {
                                if (gridCol.FilterOptions.Count > 0) throw new InternalError("Multiple filter options in {0} for {1}", file, name);
                                gridCol.FilterOptions = GetAllFilterOptions();
                            } else if (part.StartsWith("filter(", StringComparison.InvariantCultureIgnoreCase)) {
                                if (gridCol.FilterOptions.Count > 0) throw new InternalError("Multiple filter options in {0} for {1}", file, name);
                                gridCol.FilterOptions = GetFilterOptions(part.Substring(6), file, name);
                            } else if (part.EndsWith("pix", StringComparison.InvariantCultureIgnoreCase)) {
                                if (gridCol.ChWidth != 0) throw new InternalError("Can't use character width and pixel width at the same time in {0} for {1}", file, name);
                                part = part.Substring(0, part.Length - 3);
                                int n = GetNumber(part, file, name);
                                gridCol.PixWidth = n;
                            } else {
                                if (gridCol.PixWidth != 0) throw new InternalError("Can't use character width and pixel width at the same time in {0} for {1}", file, name);
                                int n = GetNumber(part, file, name);
                                gridCol.ChWidth = n;
                                gridCol.OriginalWidthCh = parts[i];// this is saved so we can save the original information in the yaml file
                            }
                        }
                        if (add) {
                            try {
                                dict.Add(name, gridCol);
                            } catch (Exception exc) {
                                throw new InternalError("Can't add {1} in {0} - {2}", file, name, ErrorHandling.FormatExceptionMessage(exc));
                            }
                        }
                    }
                }
                {
                    ReadGridDictionaryInfo dictInfo = new ReadGridDictionaryInfo {
                        ColumnInfo = dict,
                        SortBy = sortDir,
                        SortColumn = sortCol,
                        Success = true,
                        SaveColumnWidths = config?.SaveWidths,
                        SaveColumnFilters = config?.SaveFilters,
                        InitialPageSize = config?.InitialPageSize,
                        PageSizes = config?.PageSizes,
                        ShowHeader = config?.ShowHeader,
                        ShowFilter = config?.ShowFilter,
                        ShowPager = config?.ShowPager,
                        SizeStyle = config?.SizeStyle,
                    };

                    // save in cache
                    await cacheDP.AddAsync<ReadGridDictionaryInfo>(file, dictInfo);

                    return dictInfo;
                }
            }
        }
        internal static List<GridColumnInfo.FilterOptionEnum> GetAllFilterOptions() {
            List<GridColumnInfo.FilterOptionEnum> filterFlags = new List<GridColumnInfo.FilterOptionEnum>() {
                GridColumnInfo.FilterOptionEnum.Contains,
                GridColumnInfo.FilterOptionEnum.NotContains,
                GridColumnInfo.FilterOptionEnum.Equal,
                GridColumnInfo.FilterOptionEnum.NotEqual,
                GridColumnInfo.FilterOptionEnum.LessThan,
                GridColumnInfo.FilterOptionEnum.LessEqual,
                GridColumnInfo.FilterOptionEnum.GreaterThan,
                GridColumnInfo.FilterOptionEnum.GreaterEqual,
                GridColumnInfo.FilterOptionEnum.StartsWith,
                GridColumnInfo.FilterOptionEnum.NotStartsWith,
                GridColumnInfo.FilterOptionEnum.Endswith,
                GridColumnInfo.FilterOptionEnum.NotEndswith,
            };
            return filterFlags;
        }

        private static List<GridColumnInfo.FilterOptionEnum> GetFilterOptions(string part, string file, string name) {
            if (!part.StartsWith("(") || !part.EndsWith(")")) throw new InternalError("Invalid filters() options");
            part = part.Substring(1, part.Length - 2);
            string[] fs = part.Split(new char[] { ',' });
            List<GridColumnInfo.FilterOptionEnum> filterFlags = new List<GridColumnInfo.FilterOptionEnum>();
            foreach (string f in fs) {
                switch (f) {
                    case "==": filterFlags.Add(GridColumnInfo.FilterOptionEnum.Equal); break;
                    case "!=": filterFlags.Add(GridColumnInfo.FilterOptionEnum.NotEqual); break;
                    case "<": filterFlags.Add(GridColumnInfo.FilterOptionEnum.LessThan); break;
                    case "<=": filterFlags.Add(GridColumnInfo.FilterOptionEnum.LessEqual); break;
                    case ">": filterFlags.Add(GridColumnInfo.FilterOptionEnum.GreaterThan); break;
                    case ">=": filterFlags.Add(GridColumnInfo.FilterOptionEnum.GreaterEqual); break;
                    case "x*": filterFlags.Add(GridColumnInfo.FilterOptionEnum.StartsWith); break;
                    case "!x*": filterFlags.Add(GridColumnInfo.FilterOptionEnum.NotStartsWith); break;
                    case "*x": filterFlags.Add(GridColumnInfo.FilterOptionEnum.Endswith); break;
                    case "!*x": filterFlags.Add(GridColumnInfo.FilterOptionEnum.NotEndswith); break;
                    case "*x*": filterFlags.Add(GridColumnInfo.FilterOptionEnum.Contains); break;
                    case "!*x*": filterFlags.Add(GridColumnInfo.FilterOptionEnum.NotContains); break;
                    default:
                        throw new InternalError("Invalid filter option {0} in {1} for {2}", f, file, name);
                }
            }
            filterFlags = filterFlags.Distinct().ToList();
            return filterFlags;
        }

        internal static string GetPart(string part, Package package, string file, string name) {
            if (((part.StartsWith("[") && part.EndsWith("]")) || (part.StartsWith("<") && part.EndsWith(">"))) && part.Length > 2) {
                string[] vars = part.Substring(1, part.Length - 2).Split(new[] { '.' });
                if (vars.Length != 2) throw new InternalError($"Invalid variable {part} for column {name} in {file}");
                if (vars[0] == "Globals") {
                    FieldInfo? fi = typeof(Globals).GetField(vars[1], BindingFlags.Public | BindingFlags.Static);
                    if (fi == null) throw new InternalError($"Globals.{vars[1]} doesn't exist - column {name} in {file}");
                    part = fi.GetValue(null)!.ToString()!;
                } else if (vars[0] == "Package") {
                    Package.AddOnProduct addonVersion = Package.FindPackage(package.AreaName);
                    foreach (var type in addonVersion.SupportTypes) {
                        object? o = Activator.CreateInstance(type);
                        if (o == null)
                            throw new InternalError($"Type {type.Name} can't be created for area {package.AreaName}");
                        FieldInfo? fi = type.GetField(vars[1], BindingFlags.Public | BindingFlags.Static);
                        if (fi != null) {
                            part = fi.GetValue(null)!.ToString()!;
                            break;
                        }
                    }
                } else throw new InternalError($"Unknown variable {part} for column {name} in {file}");
            }
            return part;
        }
        private static int GetNextNumber(string[] parts, int i, string part, string file, string name) {
            part = GetNextPart(parts, i, part, file, name);
            return GetNumber(part, file, name);
        }
        private static string GetNextPart(string[] parts, int i, string part, string file, string name) {
            if (i + 1 >= parts.Length) throw new InternalError("Missing token following {0} column {1} in {2}", part, name, file);
            return parts[i + 1];
        }
        private static int GetNumber(string part, string file, string name) {
            try {
                int val = Convert.ToInt32(part);
                return val;
            } catch (Exception) {
                throw new InternalError("Invalid number for part {0} column {1} in {2}", part, name, file);
            }
        }
    }
}
