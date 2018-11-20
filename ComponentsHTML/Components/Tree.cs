/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#if MVC6
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ComponentsHTML.Components {

    public abstract class TreeComponentBase : YetaWFComponent {

        public const int MIN_COL_WIDTH = 12;

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TreeComponentBase), name, defaultValue, parms); }

        public const string TemplateName = "Tree";

        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        public override string GetTemplateName() { return TemplateName; }
    }

    public class TreeSetup {

        [JsonConverter(typeof(TreeDisplayComponent.StaticDataConverter))]
        public List<object> StaticData { get; internal set; }

        public bool DragDrop { get; set; }

        public string HoverCss { get; set; }
        public string HighlightCss { get; set; }
        public string DisabledCss { get; set; }
        public string RowHighlightCss { get; set; }
        public string RowDragDropHighlightCss { get; set; }

        public TreeSetup() {
            StaticData = new List<object>();
        }
    }

    public partial class TreeDisplayComponent : TreeComponentBase, IYetaWFComponent<object> {

        public const string DisplayProperty = "Text";
        public const string SubEntriesProperty = "SubEntries";
        public const string CollapsedProperty = "Collapsed";
        public const string SelectedProperty = "Selected";

        public override ComponentType GetComponentType() { return ComponentType.Display; }

        public override async Task IncludeAsync() {
            await base.IncludeAsync();
        }

        public async Task<YHtmlString> RenderAsync(object model) {

            HtmlBuilder hb = new HtmlBuilder();

            TreeDefinition treeModel = GetSiblingProperty<TreeDefinition>($"{FieldName}_TreeDefinition");

            List<object> data = new List<object>();
            IEnumerable<object> ienum = model as IEnumerable<object>;
            IEnumerator<object> ienumerator = ienum.GetEnumerator();
            while (ienumerator.MoveNext())
                data.Add(ienumerator.Current);

            string idEmpty = UniqueId();

            TreeSetup setup = new TreeSetup() {
                DragDrop = treeModel.DragDrop,
                HoverCss = treeModel.UseSkinFormatting ? "ui-state-hover" : "tg_hover",
                HighlightCss = treeModel.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                DisabledCss = treeModel.UseSkinFormatting ? "ui-state-disabled" : "tg_disabled",
                RowHighlightCss = treeModel.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                RowDragDropHighlightCss = treeModel.UseSkinFormatting ? "ui-state-active" : "tg_dragdrophighlight",
            };

            PropertyData prop = ObjectSupport.GetPropertyData(treeModel.RecordType, DisplayProperty);
            PropertyData subEntriesProp = ObjectSupport.GetPropertyData(treeModel.RecordType, SubEntriesProperty);
            PropertyData collapsedProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, CollapsedProperty);
            PropertyData selectedProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, SelectedProperty);

            // Headers
            string headerHTML = await GetHeaderAsync(treeModel, data, setup, prop, subEntriesProp);
            string html = await RenderHTML(HtmlHelper, treeModel, data, setup, prop, subEntriesProp, collapsedProp, selectedProp, FieldName);

            string dd = "";
            if (treeModel.DragDrop)
                dd = " ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragend='YetaWF_ComponentsHTML.TreeComponent.onDragEnd(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)'";

            hb.Append($@"
<div id='{treeModel.Id}' class='yt_tree t_display {(treeModel.UseSkinFormatting ? "tg_skin ui-corner-top ui-widget ui-widget-content" : "tg_noskin")}'{dd}>
    {headerHTML}
    {html}
</div>
<script>
    new YetaWF_ComponentsHTML.TreeComponent('{treeModel.Id}', {JsonConvert.SerializeObject(setup, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml })});
</script>");

            return hb.ToYHtmlString();
        }

        private Task<string> GetHeaderAsync(TreeDefinition treeDef, List<object> data, TreeSetup setup, PropertyData prop, PropertyData subEntriesProp) {

            HtmlBuilder hb = new HtmlBuilder();

            if (treeDef.ShowHeader) {

                // Caption
                string caption = prop.GetCaption(null);
                // Description
                string description = prop.GetDescription(null);

                string alignCss = "tg_left";

                // Render column header
                hb.Append($@"
    <div class='{alignCss} tg_header{(treeDef.UseSkinFormatting ? " ui-state-default" : "")}' {Basics.CssTooltip}='{HAE(description ?? "")}'>
        <span>{HE(caption)}</span>
    </div>");

            }
            return Task.FromResult(hb.ToString());
        }

        internal static async Task<string> RenderHTML(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                TreeDefinition treeModel, List<object> data, TreeSetup setup, PropertyData prop, PropertyData subEntriesProp, PropertyData collapsedProp, PropertyData selectedProp, string fieldPrefix) {

            HtmlBuilder hb = new HtmlBuilder();

            string styleCss = "";
            if (data != null && data.Count > 0)
                styleCss = " style='display:none'";

            hb.Append($@"
<div class='tg_emptytr'{styleCss}'>
    <div class='tg_emptydiv'>
        {HE(treeModel.NoRecordsText)}
    </div>
</div>");

            if (data != null && data.Count > 0) {

                hb.Append($@"
<ul class='tg_root'>");

                foreach (object record in data) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, prop, subEntriesProp, collapsedProp, selectedProp, record, 1, fieldPrefix));
                }

                hb.Append($@"
</ul>");
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordHTMLAsync(
#if MVC6
            IHtmlHelper htmlHelper,
#else
            HtmlHelper htmlHelper,
#endif
                TreeDefinition treeModel, TreeSetup setup, PropertyData prop, PropertyData subEntriesProp, PropertyData collapsedProp, PropertyData selectedProp, object record, int level, string fieldPrefix) {

            HtmlBuilder hb = new HtmlBuilder();

            int recordCount = setup.StaticData.Count;
            using (Manager.StartNestedComponent($"{fieldPrefix}[{recordCount}]")) {

                Type recordType = record.GetType();

                setup.StaticData.Add(record);

                //bool highlight;
                //ObjectSupport.TryGetPropertyValue<bool>(record, "__highlight", out highlight, false);
                //bool lowlight;
                //ObjectSupport.TryGetPropertyValue<bool>(record, "__lowlight", out lowlight, false);

                string lightCss = "";
                //if (highlight)
                //    lightCss = "tg_highlight";
                //else if (lowlight)
                //    lightCss = "tg_lowlight";

                object value = prop.GetPropertyValue<object>(record);

                // check for SubEntriesProperty
                bool collapsed = collapsedProp != null ? collapsedProp.GetPropertyValue<bool>(record) : false;
                object o = subEntriesProp.GetPropertyValue<object>(record);
                IEnumerable<object> ienum = o as IEnumerable<object>;
                if (ienum != null) {
                    IEnumerator<object> ienumerator = ienum.GetEnumerator();
                    if (!ienumerator.MoveNext()) {
                        collapsed = false;
                        ienum = null;
                    }
                }

                // selected
                bool selected = selectedProp != null ? selectedProp.GetPropertyValue<bool>(record) : false;
                string selectedCss = "";
                if (selected)
                    selectedCss = " t_select";

                // entry
                string output = (await htmlHelper.ForDisplayComponentAsync(record, DisplayProperty, value, prop.UIHint)).ToString();
                if (!string.IsNullOrWhiteSpace(output))
                    output = output.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                if (string.IsNullOrWhiteSpace(output))
                    output = "&nbsp;";

                string caret;

                string icon;
                if (ienum != null) {
                    if (collapsed)
                        caret = "<i class='t_icright'></i>";
                    else
                        caret = "<i class='t_icdown'></i>";
                    icon = "<i class='t_icfolder'></i>";
                } else {
                    caret = "<i class='t_icempty'></i>";
                    icon = "<i class='t_icfile'></i>";
                }

                string dd = "";
                if (treeModel.DragDrop)
                    dd = " draggable='true'";

                hb.Append($@"
 <li class='{lightCss}' data-record='{recordCount}'>
  {caret}
  {icon}<a class='t_entry{selectedCss}' href='#'{dd}>{output}</a>");

                // sub entries
                if (ienum != null) {

                    string collapsedStyle = "";
                    if (collapsed)
                        collapsedStyle = " style='display:none'";

                    IEnumerator<object> ienumerator = ienum.GetEnumerator();
                    hb.Append($@"
 <ul{collapsedStyle}>");

                    for (int i = 0; ienumerator.MoveNext(); i++) {
                        hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, prop, subEntriesProp, collapsedProp, selectedProp, ienumerator.Current, level+1, fieldPrefix));
                    }

                    hb.Append($@"
 </ul>");
                }

                hb.Append($@"
 </li>");
                return hb.ToString();
            }
        }

        // Custom serializer to minimize static data being transferred

        public class TreeEntryContractResolver : DefaultContractResolver {

            public TreeEntryContractResolver() { }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (type != typeof(object)) {
                    List<string> propList = new List<string>();
                    List<PropertyData> props = ObjectSupport.GetPropertyData(type);
                    foreach (PropertyData prop in props) {
                        if (prop.Name.StartsWith("__") || (prop.PropInfo.CanRead && prop.PropInfo.CanWrite && prop.Name != "SubEntries" && prop.Name != "Id" && prop.Name != "Collapsed")) {
                            propList.Add(prop.Name);
                        }
                    }
                    properties = (from p in properties where propList.Contains(p.PropertyName) select p).ToList();
                }
                return properties;
            }
        }
        public class StaticDataConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return true;
            }
            public override bool CanRead {
                get { return false; }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                throw new NotImplementedException();
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                string array = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ContractResolver = new TreeEntryContractResolver() });
                writer.WriteRawValue(array);
            }
        }
    }
}

