/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class GridComponentBase : YetaWFComponent {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(GridComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Grid";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public partial class GridDisplayComponent : GridComponentBase, IYetaWFComponent<GridDefinition> {

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public override async Task IncludeAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "github.com.free-jqgrid.jqgrid");
            await base.IncludeAsync();
        }

        public async Task<YHtmlString> RenderAsync(GridDefinition model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model.ShowFilter == null)
                model.ShowFilter = YetaWF.Core.Localize.UserSettings.GetProperty<bool>("ShowGridSearchToolbar");
            if (model.DropdownActionWidth == null)
                model.DropdownActionWidth = GetDropdownActionWidthInChars();

            string dataDelete = "", dataDisplay = "", dataExtra = "";
            if (model.CanAddOrDelete) {
                if (string.IsNullOrWhiteSpace(model.DeleteProperty)) { throw new YetaWF.Core.Support.InternalError("Must provide DeleteProperty in GridDefinition when using CanAddOrDelete"); }
                dataDelete = " data-deleteproperty=\"" + model.DeleteProperty + "\"";
                if (string.IsNullOrWhiteSpace(model.DisplayProperty)) { throw new YetaWF.Core.Support.InternalError("Must provide DisplayProperty in GridDefinition when using CanAddOrDelete"); }
                dataDisplay = " data-displayproperty=\"" + model.DisplayProperty + "\"";
            }
            string idEmpty = UniqueId();
            if (model.ExtraData != null)
                dataExtra = " data-extraproperty=\"" + YetaWFManager.HtmlAttributeEncode(YetaWFManager.JsonSerialize(model.ExtraData)) + "\"";

            hb.Append($@"
<table id='{model.Id}' class='yt_grid {YetaWF.Core.Addons.Forms.CssFormNoSubmitContents} t_display' 
       data-fieldprefix='{FieldName}' {dataDelete} {dataDisplay} {dataExtra}
       data-charavgw={Manager.CharWidthAvg}></table>
<div id='{model.Id}_Pager'></div>");

            Grid.GridSavedSettings gridSavedSettings = null;
            int pageSize = model.InitialPageSize;
            int initialPage = 1;
            if (model.SettingsModuleGuid != Guid.Empty) {
                gridSavedSettings = Grid.LoadModuleSettings(model.SettingsModuleGuid, initialPage, pageSize);
                pageSize = gridSavedSettings.PageSize;
                initialPage = gridSavedSettings.CurrentPage;
            }
            bool isAjax = !string.IsNullOrWhiteSpace(model.AjaxUrl);
            if (isAjax && model.Data != null) { throw new YetaWF.Core.Support.InternalError("Can't use Data with an Ajax grid"); }
            if (!isAjax && model.Data == null) { throw new YetaWF.Core.Support.InternalError("Neither AjaxUrl nor local Data specified"); }

            string dataId = @UniqueId("griddata");

            ObjectSupport.ReadGridDictionaryInfo dictInfo = await Grid.LoadGridColumnDefinitionsAsync(model);
            GetColModelInfo colModelInfo = await GetColModelAsync(gridSavedSettings, model);

            hb.Append($@"
<script>
    function InitGrid_{model.Id} () {{");

            if (!isAjax) {
                hb.Append($@"
        var {dataId} = [
            {await RenderRecordsAsync(model, dictInfo)}
        ];");
            } else {
                // when we're rendering data during ajax call (POST) we can't add javascript files, so we add them now in preparation
                await AddTemplatesForType(model.RecordType);
            }

            hb.Append($@"
        var $grid = $('#{model.Id}');
        var emptyDiv = {YetaWFManager.JsonSerialize($"<div id='{idEmpty}' class='t_emptydiv'>{HE(model.NoRecordsText)}</div>")};
        var options = {{
            colNames: [ {await RenderColNamesAsync(model)} ],
            colModel: [ {colModelInfo.Data} ],
            shrinkToFit: {YetaWFManager.JsonSerialize(model.SizeToFit)},");

            if (pageSize > 0 && (isAjax || model.CanAddOrDelete || pageSize < model.Data.Data.Count)) {

                hb.Append($@"
            pager : '#{model.Id}_Pager',
            // pagerpos: 'left',
            pgButtons: {(model.PagerButtons > 0 ? "true" : "false")},
            rowNum: {pageSize},");

                if (model.PageSizes.Count > 0) {
                    hb.Append($@"
            rowList: {YetaWFManager.JsonSerialize(model.PageSizes)},");
                }

                hb.Append($@"
            viewrecords: true,");
            }

            hb.Append($@"
            {RenderGridSortOrder(model, dictInfo, gridSavedSettings)}
            page: {initialPage},");

            if (isAjax) {
                hb.Append($@"
            url:{YetaWFManager.JsonSerialize(model.AjaxUrl)},
            mtype:'POST',
            datatype: 'json',
            ajaxGridOptions: {{'Accept-Encoding': 'gzip'}},

            loadBeforeSend: function(xhr,settings) {{
                    return YetaWF_Grid.modifySend($grid, {YetaWFManager.JsonSerialize(model.SettingsModuleGuid.ToString())}, options, xhr, settings);
            }},
            beforeProcessing: function(data,status,xhr) {{
                return YetaWF_Grid.modifyReceive($grid, options, data,status,xhr);
            }},
            loadError: function(xhr,status,error) {{
                YetaWF_Grid.loadError($grid,xhr,status,error)
            }},
            jsonReader: {{
                repeatitems: false,
            }},");

            } else {

                hb.Append($@"
            data: {dataId},
            datatype: 'local',");
            }

            if (model.SettingsModuleGuid != Guid.Empty) {
                hb.Append($@"
            resizeStop: function(newwidth, index) {{
                YetaWF_Grid.SaveSettingsColumnWidths($grid, {YetaWFManager.JsonSerialize(GetSettingsSaveColumnWidthsUrl())}, {YetaWFManager.JsonSerialize(model.SettingsModuleGuid)}, options, newwidth, index);
            }},");
            }

            hb.Append($@"
            gridComplete: function() {{
                YetaWF_Grid.gridComplete($grid, '{model.Id}'); // make sure we execute any javascript embedded in the grid (which arrived via data)");

            if (model.ShowHeader) {
                hb.Append($@"
                YetaWF_Grid.gridComplete_NoRecords($grid, '{idEmpty}', emptyDiv); // handle 'No Records' condition");
            }

            hb.Append($@"
            }},
        }};
        YetaWF_Grid.setColumnWidths($grid, options);
        $grid.jqGrid(options);

        $grid.jqGrid('filterToolbar',{{searchOperators : true}});");

            if (pageSize > 0 && (isAjax || model.CanAddOrDelete || pageSize < model.Data.Data.Count)) {
                hb.Append($@"
        $grid.jqGrid('navGrid','#{model.Id}_Pager', {{}});");
            }

            if (isAjax) {
                hb.Append($@"
        // Custom reload button so we can reload the grid without clearing search filters
        $grid.jqGrid('navButtonAdd', '#{model.Id}_Pager', {{
            caption: {YetaWFManager.JsonSerialize(__ResStr("refreshtext", ""))},
            title: {YetaWFManager.JsonSerialize(__ResStr("refreshtitle", "Reload data"))},
            buttonicon: 'ui-icon-refresh',
            onClickButton: function () {{
                $grid.trigger('reloadGrid');
            }}
        }});");
            }

            // http://www.trirand.com/blog/?page_id=393/help/search-toolbar-hide-by-default
            if (colModelInfo.HasFilters) {
                hb.Append($@"
        $grid.jqGrid('navButtonAdd', '#{model.Id}_Pager', {{
            caption: '',
            title: YLocs.YetaWF_ComponentsHTML.pgsearchTB,
            buttonicon: 'ui-icon-search',
            onClickButton: function () {{
                this.toggleToolbar();
                if ($.isFunction(this.p._complete)) {{
                    if ($('.ui-search-toolbar', this.grid.hDiv).is(':visible')) {{
                        $('.ui-search-toolbar', this.grid.fhDiv).show();
                    }} else {{
                        $('.ui-search-toolbar', this.grid.fhDiv).hide();
                    }}
                    this.p._complete.call(this);
                    fixPositionsOfFrozenDivs.call(this);
                }}
            }}
        }});
        YetaWF_Grid.toggleSearchToolbar($grid, {YetaWFManager.JsonSerialize(model.ShowFilter == true)});");
            }

            if (model.SupportReload) {
                hb.Append($@"
        var $mod = $grid.closest('.yModule');
        if ($mod.length != 1) throw 'Can\'t find containing module';
        $YetaWF.registerModuleRefresh($mod[0], function() {{
            $grid.trigger('reloadGrid');
        }});");
            }

            if (!isAjax) {
                hb.Append($@"
        if (typeof $YetaWF.Forms === 'undefined' || $YetaWF.Forms == undefined) throw 'Can\'t use local data outside of a form or partial form';/*DEBUG*/
        var $form = $grid.closest('form');
        if ($form.length == 0) throw 'Can\'t find containing form';");

                if (model.HandleLocalInput) {
                    hb.Append($@"
        YetaWF_Grid.HandleInputUpdates($grid, true);// handle input in grid for local data
        $YetaWF.Forms.addPreSubmitHandler({(Manager.InPartialView ? 1 : 0)}, {{
            form: $form[0],
            callback: function(entry) {{
                YetaWF_Grid.HandleSubmitLocalData($grid, $form);
            }},
            userdata_grid: $grid,
        }});
        YetaWF_Grid.ShowPager($grid); // show/hide pager");
                } else {
                    hb.Append($@"
        YetaWF_Grid.HandleInputUpdates($grid, false);
        $YetaWF.Forms.addPreSubmitHandler({(Manager.InPartialView ? 1 : 0)}, {{
            form: $form[0],
            callback: function(entry) {{
                YetaWF_Grid.HandleSubmitFields($grid, $form);
            }},
            userdata_grid: $grid,
        }});");
                }
            }

            hb.Append($@"
        YetaWF_Grid.fixHeaders($grid);
    }}
    InitGrid_{model.Id}();
</script>");

            return hb.ToYHtmlString();
        }

        private async Task AddTemplatesForType(Type type) {
            List<PropertyData> propData = ObjectSupport.GetPropertyData(type);
            foreach (PropertyData prop in propData) {
                if (prop.UIHint != null) {
                    if (prop.ReadOnly)
                        await YetaWFComponentExtender.MarkUsedDisplayAsync(prop.UIHint);
                    else
                        await YetaWFComponentExtender.MarkUsedEditAsync(prop.UIHint);
                    if (prop.PropInfo.PropertyType.IsClass)
                        await AddTemplatesForType(prop.PropInfo.PropertyType);
                }
            }
        }

        internal int GetDropdownActionWidthInChars() {
            string s = __ResStr("dropdownWidth", "11");
            return Convert.ToInt32(s);
        }
    }
}
