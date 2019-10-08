/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
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

    /// <summary>
    /// Base class for the Tree component implementation.
    /// </summary>
    /// <remarks>TODO: This needs to be revisited. Generating the tree hierarchy is quite expensive for large trees. This needs to lean more on the client side to do some of the work.
    /// Since most trees are initially shown collapsed, a "render/populate as you go" approach would be better.</remarks>
    public abstract class TreeComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TreeComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Tree";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return Controllers.AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }
    }

    internal class TreeSetup {

        [JsonConverter(typeof(TreeDisplayComponent.StaticDataConverter))]
        public List<object> StaticData { get; internal set; }

        public bool DragDrop { get; set; }

        public string HoverCss { get; set; }
        public string HighlightCss { get; set; }
        public string DisabledCss { get; set; }
        public string RowHighlightCss { get; set; }
        public string RowDragDropHighlightCss { get; set; }
        public string SelectedCss { get; set; }

        public TreeSetup() {
            StaticData = new List<object>();
        }
    }

    /// <summary>
    /// Implementation of the Tree display component.
    /// </summary>
    public partial class TreeDisplayComponent : TreeComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Defines the link type represented by a tree item.
        /// </summary>
        public enum LinkTypeEnum {
            /// <summary>
            /// An entry which is handle using client-side processing. The target is not visible in the generated HTML for the tree item.
            /// </summary>
            Local = 0,
            /// <summary>
            /// A link to an external URL.
            /// An &lt;a&gt; tag is generated. The target is visible in the generated HTML for the tree item.
            /// </summary>
            External = 1,
        }

        /// <summary>
        /// The name of the property in the Tree component's data model that is used as the item ID.
        /// </summary>
        public const string IdProperty = "Id";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used as the item's link type.
        /// </summary>
        public const string LinkTypeProperty = "LinkType";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used as the item's target URL when the link type is set to LinkTypeEnum.External.
        /// </summary>
        public const string UrlProperty = "Url";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used as the item's text when the link type is set to LinkTypeEnum.Local.
        /// </summary>
        public const string DisplayProperty = "Text";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used as an item's collection of subitems.
        /// </summary>
        public const string SubEntriesProperty = "SubEntries";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used to determine whether the item should be rendered collapsed (true) or expanded (false).
        /// </summary>
        public const string CollapsedProperty = "Collapsed";
        /// <summary>
        /// The name of the property in the Tree component's data model that is used to determine whether the item should be rendered as initially selected.
        /// </summary>
        public const string SelectedProperty = "Selected";

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await JqueryUICore.UseAsync();// needed for css
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {

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
                SelectedCss = treeModel.UseSkinFormatting ? "ui-state-active" : "t_select",
            };

            PropertyData linkTypeProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, LinkTypeProperty);
            PropertyData urlProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, UrlProperty);
            PropertyData prop = ObjectSupport.GetPropertyData(treeModel.RecordType, DisplayProperty);
            PropertyData subEntriesProp = ObjectSupport.GetPropertyData(treeModel.RecordType, SubEntriesProperty);
            PropertyData collapsedProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, CollapsedProperty);
            PropertyData selectedProp = ObjectSupport.TryGetPropertyData(treeModel.RecordType, SelectedProperty);

            // Headers
            string headerHTML = await GetHeaderAsync(treeModel, data, setup, linkTypeProp, prop, urlProp, subEntriesProp);
            string html = await RenderHTML(HtmlHelper, treeModel, data, setup, linkTypeProp, prop, urlProp, subEntriesProp, collapsedProp, selectedProp, FieldName);

            string dd = "";
            if (treeModel.DragDrop)
                dd = " ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragend='YetaWF_ComponentsHTML.TreeComponent.onDragEnd(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)'";

            hb.Append($@"
<div id='{treeModel.Id}' class='yt_tree t_display {(treeModel.UseSkinFormatting ? "tg_skin ui-corner-top ui-widget ui-widget-content" : "tg_noskin")}'{dd}>
    {headerHTML}
    {html}
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TreeComponent('{treeModel.Id}', {JsonConvert.SerializeObject(setup, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml })});");

            return hb.ToString();
        }

        private Task<string> GetHeaderAsync(TreeDefinition treeDef, List<object> data, TreeSetup setup, PropertyData linkTypeProp, PropertyData prop, PropertyData urlProp, PropertyData subEntriesProp) {

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

        internal static async Task<string> RenderHTML(YHtmlHelper htmlHelper,
                TreeDefinition treeModel, List<object> data, TreeSetup setup, PropertyData linkTypeProp, PropertyData prop, PropertyData urlProp, PropertyData subEntriesProp, PropertyData collapsedProp, PropertyData selectedProp, string fieldPrefix) {

            HtmlBuilder hb = new HtmlBuilder();

            string styleCss = "";
            if (data != null && data.Count > 0)
                styleCss = " style='display:none'";

            hb.Append($@"
<div class='tg_emptytr'{styleCss}>
    <div class='tg_emptydiv'>
        {HE(treeModel.NoRecordsText)}
    </div>
</div>");

            if (data != null && data.Count > 0) {

                hb.Append($@"
<ul class='tg_root'>");

                foreach (object record in data) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, linkTypeProp, prop, urlProp, subEntriesProp, collapsedProp, selectedProp, record, 1, fieldPrefix));
                }

                hb.Append($@"
</ul>");
            } else {
                // when initially rendering a tree with 0 records, we have to prepare for all templates
                await YetaWFComponentExtender.AddComponentForType(treeModel.RecordType);
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordHTMLAsync(YHtmlHelper htmlHelper,
                TreeDefinition treeModel, TreeSetup setup, PropertyData linkTypeProp, PropertyData prop, PropertyData urlProp, PropertyData subEntriesProp, PropertyData collapsedProp, PropertyData selectedProp, object record, int level, string fieldPrefix) {

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
                    selectedCss = $" {setup.SelectedCss}";

                string caret;
                string icon;
                if (ienum != null)
                {
                    if (collapsed)
                        caret = "<i class='t_icright'></i>";
                    else
                        caret = "<i class='t_icdown'></i>";
                    if (urlProp != null && urlProp.GetPropertyValue<string>(record) != null)
                        icon = "<i class='t_icfile'></i>";
                    else
                        icon = "<i class='t_icfolder'></i>";
                } else {
                    caret = "<i class='t_icempty'></i>";
                    icon = "<i class='t_icfile'></i>";
                }

                string dd = "";
                if (treeModel.DragDrop)
                    dd = " draggable='true'";

                // entry
                LinkTypeEnum dt = LinkTypeEnum.Local;
                if (linkTypeProp != null)
                    dt = linkTypeProp.GetPropertyValue<LinkTypeEnum>(record);

                string output;
                switch (dt) {
                    default:
                    case LinkTypeEnum.Local:
                        string t = await htmlHelper.ForDisplayComponentAsync(record, DisplayProperty, value, prop.UIHint);
                        if (!string.IsNullOrWhiteSpace(t))
                            t = t.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
                        if (string.IsNullOrWhiteSpace(t))
                            t = "&nbsp;";
                        output = $"<a class='t_entry{selectedCss}' href='#'{dd}>{t}</a>";
                        break;
                    case LinkTypeEnum.External:
                        string url = urlProp.GetPropertyValue<string>(record);
                        output = $"<a href='{Utility.UrlEncodePath(url)}' target='_blank' class=''>{await htmlHelper.ForDisplayComponentAsync(record, DisplayProperty, value, prop.UIHint)}</a>";
                        break;
                }

                hb.Append($@"
 <li class='{lightCss}' data-record='{recordCount}'>
  {caret}
  {icon}{output}");

                // sub entries
                if (ienum != null) {

                    string collapsedStyle = "";
                    if (collapsed)
                        collapsedStyle = " style='display:none'";

                    IEnumerator<object> ienumerator = ienum.GetEnumerator();
                    hb.Append($@"
 <ul{collapsedStyle}>");

                    for (int i = 0; ienumerator.MoveNext(); i++) {
                        hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, linkTypeProp, prop, urlProp, subEntriesProp, collapsedProp, selectedProp, ienumerator.Current, level+1, fieldPrefix));
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

        internal class TreeEntryContractResolver : DefaultContractResolver {

            public TreeEntryContractResolver() { }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (type != typeof(object)) {
                    List<string> propList = new List<string>();
                    List<PropertyData> props = ObjectSupport.GetPropertyData(type);
                    foreach (PropertyData prop in props) {
                        if (prop.Name.StartsWith("__") || (prop.PropInfo.CanRead && prop.PropInfo.CanWrite && prop.Name != SubEntriesProperty && prop.Name != IdProperty && prop.Name != CollapsedProperty)) {
                            propList.Add(prop.Name);
                        }
                    }
                    properties = (from p in properties where propList.Contains(p.PropertyName) select p).ToList();
                }
                return properties;
            }
        }
        internal class StaticDataConverter : JsonConverter {
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

