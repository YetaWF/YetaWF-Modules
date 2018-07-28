/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public partial class GridDisplayComponent {

        /* protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridComponentBase), name, defaultValue, parms); } */

        internal async Task<YHtmlString> RenderColNamesAsync(GridDefinition gridDef) {

            ScriptBuilder sb = new ScriptBuilder();

            ObjectSupport.ReadGridDictionaryInfo info = await Grid.LoadGridColumnDefinitionsAsync(gridDef);

            foreach (var d in info.ColumnInfo) {
                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;

                var prop = ObjectSupport.GetPropertyData(gridDef.RecordType, propName);

                string caption = prop.GetCaption(gridDef.ResourceRedirect);
                if (!gridCol.Hidden && gridDef.ResourceRedirect != null && string.IsNullOrWhiteSpace(caption))
                    continue;// we need a caption if we're using resource redirects

                string description = prop.GetDescription(gridDef.ResourceRedirect);
                if (string.IsNullOrWhiteSpace(description))
                    sb.Append("'<span>{0}</span>',", YetaWFManager.HtmlEncode(caption));
                else
                    sb.Append("'<span {0}=\"{1}\">{2}</span>',", Basics.CssTooltip, YetaWFManager.HtmlEncode(description), YetaWFManager.HtmlEncode(caption));
            }
            sb.RemoveLast(); // remove last comma
            return sb.ToYHtmlString();
        }

        public class GetColModelInfo {
            public YHtmlString Data { get; set; }
            public bool HasFilters { get; set; }
        }

        private async Task<GetColModelInfo> GetColModelAsync(Grid.GridSavedSettings gridSavedSettings, GridDefinition gridDef) {

            ScriptBuilder sb = new ScriptBuilder();

            ObjectSupport.ReadGridDictionaryInfo info = await Grid.LoadGridColumnDefinitionsAsync(gridDef);

            bool hasFilters = false;

            foreach (var d in info.ColumnInfo) {
                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;

                PropertyData prop = ObjectSupport.GetPropertyData(gridDef.RecordType, propName);

                string caption = prop.GetCaption(gridDef.ResourceRedirect);
                if (!gridCol.Hidden && gridDef.ResourceRedirect != null && string.IsNullOrWhiteSpace(caption))
                    continue;// we need a caption if we're using resource redirects

                sb.Append("{");
                sb.Append("name:{0},index:{0},", YetaWFManager.JsonSerialize(prop.Name));

                int width = 0, charWidth = 0;
                if (gridCol.Icons != 0) {
                    gridCol.Sortable = false;
                    Grid.GridActionsEnum actionStyle = Grid.GridActionsEnum.Icons;
                    if (gridCol.Icons > 1)
                        actionStyle = UserSettings.GetProperty<Grid.GridActionsEnum>("GridActions");
                    gridCol.ChWidth = gridCol.PixWidth = 0;
                    gridCol.Alignment = GridHAlignmentEnum.Center;
                    if (actionStyle == Grid.GridActionsEnum.DropdownMenu) {
                        charWidth = gridDef.DropdownActionWidth ?? 12;
                    } else {
                        width = 10 + (Math.Abs(gridCol.Icons) * (16 + 4) + 10);
                        charWidth = 0;
                    }
                }
                if (gridCol.ChWidth != 0) {
                    charWidth = gridCol.ChWidth;
                } else if (gridCol.PixWidth != 0)
                    width = gridCol.PixWidth;

                if (gridSavedSettings != null && gridSavedSettings.Columns.ContainsKey(prop.Name)) {
                    GridDefinition.ColumnInfo columnInfo = gridSavedSettings.Columns[prop.Name];
                    if (columnInfo.Width >= 0)
                        width = columnInfo.Width; // override calculated width
                }
                sb.Append("has_form_data:{0},", YetaWFManager.JsonSerialize(!prop.ReadOnly));
                if (!prop.ReadOnly)
                    sb.Append("no_sub_if_notchecked:{0},", YetaWFManager.JsonSerialize(gridCol.OnlySubmitWhenChecked));

                sb.Append("width:{0},", width);
                sb.Append("__charWidth:{0},", charWidth);
                sb.Append("title: false,");

                sb.Append("classes:'t_cell t_{0}',", prop.Name.ToLower());
                switch (gridCol.Alignment) {
                    case GridHAlignmentEnum.Unspecified:
                    case GridHAlignmentEnum.Left:
                        break;
                    case GridHAlignmentEnum.Center:
                        sb.Append("align:'center',");
                        break;
                    case GridHAlignmentEnum.Right:
                        sb.Append("align:'right',");
                        break;
                }
                if (!gridCol.Sortable)
                    sb.Append("sortable:false,");
                if (gridCol.Hidden)
                    sb.Append("hidden:true,");
                if (!gridCol.Locked)
                    sb.Append("resizable:true,");
                if (gridCol.FilterOptions.Count > 0 && gridDef.ShowFilter != false) {
                    hasFilters = true;
                    sb.Append("search:true,");
                    if (prop.PropInfo.PropertyType == typeof(Boolean) || prop.PropInfo.PropertyType == typeof(Boolean?)) {
                        sb.Append("stype:'select',searchoptions:{value:'");
                        sb.Append(":All;True:Yes;False:No");
                        sb.Append("'},");
                    } else if (prop.PropInfo.PropertyType == typeof(int) || prop.PropInfo.PropertyType == typeof(int?) || prop.PropInfo.PropertyType == typeof(long) || prop.PropInfo.PropertyType == typeof(long?)) {
                        List<SelectionItem<int>> entries = await YetaWFComponentExtender.GetValueListFromUIHintAsync(prop.UIHint);
                        if (entries == null) {
                            // regular int/long
                            sb.Append("stype:'text',searchoptions:{sopt:['ge','gt','le','lt','eq','ne']},searchrules:{integer:true},");
                        } else {
                            // this is a dynamic enumerated value
                            sb.Append($"stype:'select',searchoptions:{{sopt:['eq','ne'],value:':{__ResStr("noSel", "(no selection)")}");
                            foreach (SelectionItem<int> entry in entries) {
                                string capt = YetaWFManager.JsonSerialize(entry.Text.ToString());
                                capt = capt.Substring(1, capt.Length - 2);
                                sb.Append(";{0}:{1}", (int)entry.Value, capt);
                            }
                            sb.Append("'},");
                        }
                    } else if (prop.PropInfo.PropertyType == typeof(decimal) || prop.PropInfo.PropertyType == typeof(decimal?)) {
                        sb.Append("stype:'text',searchoptions:{sopt:['ge','gt','le','lt','eq','ne']},searchrules:{integer:true},");
                    } else if (prop.PropInfo.PropertyType == typeof(DateTime) || prop.PropInfo.PropertyType == typeof(DateTime?)) {
                        sb.Append("searchoptions:{sopt:['ge','le'],dataInit: function(elem) {");
                        if (prop.UIHint == "DateTime") {
                            DateTimeEditComponent dateTimeComp = new Components.DateTimeEditComponent();
                            sb.Append(await dateTimeComp.RenderJavascriptAsync(gridDef.Id, "elem"));
                        } else if (prop.UIHint == "Date") {
                            DateEditComponent dateTimeComp = new Components.DateEditComponent();
                            sb.Append(await dateTimeComp.RenderJavascriptAsync(gridDef.Id, "elem"));
                        } else {
                            throw new InternalError("Need DateTime or Date UIHint for DateTime data");
                        }
                        sb.Append(" },},");
                    } else if (prop.PropInfo.PropertyType == typeof(Guid) || prop.PropInfo.PropertyType == typeof(Guid?)) {
                        sb.Append("stype:'text',searchoptions:{sopt:['cn','bw','ew']},");
                    } else if (prop.PropInfo.PropertyType.IsEnum) {
                        sb.Append($"stype:'select',searchoptions:{{sopt:['eq','ne'],value:':{__ResStr("noSel", "(no selection)")}");
                        EnumData enumData = ObjectSupport.GetEnumData(prop.PropInfo.PropertyType);
                        foreach (EnumDataEntry entry in enumData.Entries) {
                            string capt = YetaWFManager.JsonSerialize(entry.Caption);
                            capt = capt.Substring(1, capt.Length - 2);
                            sb.Append(";{0}:{1}", (int)entry.Value, capt);
                        }
                        sb.Append("'},");
                    } else {
                        sb.Append("stype:'text',searchoptions:{");
                        AddFilterOptions(sb, gridCol);
                        sb.Append("},");
                    }
                } else {
                    sb.Append("search:false,");
                }
                sb.Append("},");

                // get the uihint to add the template
                if (prop.UIHint != null)
                    await Manager.AddOnManager.AddTemplateFromUIHintAsync(prop.UIHint);
            }
            sb.RemoveLast(); // remove last comma
            return new GetColModelInfo {
                HasFilters = hasFilters,
                Data = sb.ToYHtmlString()
            };
        }

        private void AddFilterOptions(ScriptBuilder sb, GridColumnInfo gridCol) {
            sb.Append("sopt:[");
            foreach (GridColumnInfo.FilterOptionEnum f in gridCol.FilterOptions) {
                sb.Append("'");
                switch (f) {
                    case GridColumnInfo.FilterOptionEnum.Equal: sb.Append("eq"); break;
                    case GridColumnInfo.FilterOptionEnum.GreaterEqual: sb.Append("ge"); break;
                    case GridColumnInfo.FilterOptionEnum.GreaterThan: sb.Append("gt"); break;
                    case GridColumnInfo.FilterOptionEnum.LessEqual: sb.Append("le"); break;
                    case GridColumnInfo.FilterOptionEnum.LessThan: sb.Append("lt"); break;
                    case GridColumnInfo.FilterOptionEnum.NotEqual: sb.Append("ne"); break;
                    case GridColumnInfo.FilterOptionEnum.StartsWith: sb.Append("bw"); break;
                    case GridColumnInfo.FilterOptionEnum.NotStartsWith: sb.Append("bn"); break;
                    case GridColumnInfo.FilterOptionEnum.Contains: sb.Append("cn"); break;
                    case GridColumnInfo.FilterOptionEnum.NotContains: sb.Append("nc"); break;
                    case GridColumnInfo.FilterOptionEnum.Endswith: sb.Append("ew"); break;
                    case GridColumnInfo.FilterOptionEnum.NotEndswith: sb.Append("en"); break;
                    default:
                        throw new InternalError("Unexpected filter option {0}", f);
                }
                sb.Append("',");
            }
            sb.Append("],");
        }

        internal YHtmlString RenderGridSortOrder(GridDefinition gridDef, ObjectSupport.ReadGridDictionaryInfo dictInfo, Grid.GridSavedSettings gridSavedSettings) {
            Dictionary<string, GridDefinition.ColumnInfo> columns = null;

            ScriptBuilder sb = new ScriptBuilder();
            if (gridSavedSettings != null && gridSavedSettings.Columns.Count > 0) { // use the saved sort order
                columns = gridSavedSettings.Columns;
                foreach (var col in columns) {
                    bool found = false;
                    switch (col.Value.Sort) {
                        default:
                        case GridDefinition.SortBy.NotSpecified:
                            break;
                        case GridDefinition.SortBy.Ascending:
                            sb.Append("sortname:'{0}',sortorder:'{1}',", col.Key, "asc");
                            found = true;
                            break;
                        case GridDefinition.SortBy.Descending:
                            sb.Append("sortname:'{0}',sortorder:'{1}',", col.Key, "desc");
                            found = true;
                            break;
                    }
                    if (found) break;// only one column supported in jqgrid
                }
            } else if (dictInfo != null && !string.IsNullOrWhiteSpace(dictInfo.SortColumn))  {
                sb.Append($"sortname:'{dictInfo.SortColumn}',sortorder:'{(dictInfo.SortBy == GridDefinition.SortBy.Ascending ? "asc" : "desc")}',");
            }
            return sb.ToYHtmlString();
        }
    }
}
