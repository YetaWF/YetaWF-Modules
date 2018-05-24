using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.ComponentsHTML.Components {

    public partial class GridDisplayComponent {

        internal static async Task<List<PropertyListEntry>> GetHiddenGridPropertiesAsync(object obj) {
            ObjectSupport.ReadGridDictionaryInfo info = await GridHelper.LoadGridColumnDefinitionsAsync(obj.GetType());
            return GetGridProperties(obj, info);
        }
        internal static List<PropertyListEntry> GetHiddenGridProperties(object obj, ObjectSupport.ReadGridDictionaryInfo dictInfo) {
            List<PropertyListEntry> list = new List<PropertyListEntry>();
            foreach (var d in dictInfo.ColumnInfo) {
                string propName = d.Key;
                GridColumnInfo gridCol = d.Value;
                if (gridCol.Hidden) {
                    PropertyData prop = ObjectSupport.GetPropertyData(obj.GetType(), propName);
                    list.Add(new PropertyListEntry(prop.Name, prop.GetPropertyValue<object>(obj), "Hidden", false, false, null, null, false, null, SubmitFormOnChangeAttribute.SubmitTypeEnum.None));
                }
            }
            return list;
        }

        //private static async Task<List<PropertyListEntry>> GetGridPropertiesAsync(object obj, GridDefinition gridDef) {
        //    ObjectSupport.ReadGridDictionaryInfo info = await GridHelper.LoadGridColumnDefinitionsAsync(gridDef);
        //    return GetGridProperties(obj, info.ColumnInfo);
        //}
        internal static async Task<List<PropertyListEntry>> GetGridPropertiesAsync(object obj) {
            ObjectSupport.ReadGridDictionaryInfo info = await GridHelper.LoadGridColumnDefinitionsAsync(obj.GetType());
            return GetGridProperties(obj, info);
        }
        internal static List<PropertyListEntry> GetGridProperties(object obj, ObjectSupport.ReadGridDictionaryInfo dictInfo) {
            List<PropertyListEntry> list = new List<PropertyListEntry>();
            foreach (var d in dictInfo.ColumnInfo) {
                string propName = d.Key;
                PropertyData prop = ObjectSupport.GetPropertyData(obj.GetType(), propName);
                list.Add(new PropertyListEntry(prop.Name, prop.GetPropertyValue<object>(obj), prop.UIHint, !prop.ReadOnly, false, null, null, false, null, SubmitFormOnChangeAttribute.SubmitTypeEnum.None));
            }
            return list;
        }


        /// <summary>
        /// Renders the url to save the column widths for a grid
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        private string GetSettingsSaveColumnWidthsUrl() {
            return YetaWFManager.UrlFor(typeof(YetaWF.Core.Controllers.Shared.GridHelperController), nameof(YetaWF.Core.Controllers.Shared.GridHelperController.GridSaveColumnWidths));
        }
    }
}
