/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
#if MVC6
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

        internal static JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            ContractResolver = new TreeEntryContractResolver(),
            Formatting = Newtonsoft.Json.Formatting.None,
        };

        // Custom serializer to minimize static data being transferred

        internal class TreeEntryContractResolver : DefaultContractResolver {

            public TreeEntryContractResolver() { }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (type != typeof(object)) {
                    List<string> propList = new List<string>();
                    List<PropertyData> props = ObjectSupport.GetPropertyData(type);
                    foreach (PropertyData prop in props) {
                        if (prop.Name.StartsWith("__") || (prop.PropInfo.CanRead && prop.PropInfo.CanWrite)) {
                            propList.Add(prop.Name);
                        }
                    }
                    properties = (from p in properties where propList.Contains(p.PropertyName) select p).ToList();
                }
                return properties;
            }
        }
    }

    internal class TreeSetup {

        public bool DragDrop { get; set; } // Supports drag & drop

        public string HoverCss { get; set; }
        public string HighlightCss { get; set; }
        public string DisabledCss { get; set; }
        public string RowHighlightCss { get; set; }
        public string RowDragDropHighlightCss { get; set; }
        public string SelectedCss { get; set; }

        public string ContentTargetId { get; set; }
        public string ContentTargetPane { get; set; }
        public string AjaxUrl { get; set; } // for dynamic population during expand

        public TreeSetup() { }
    }

    /// <summary>
    /// The type of link used by a menu entry.
    /// </summary>
    public enum LinkTypeEnum {
        /// <summary>
        /// UrlContent property has a local link.
        /// </summary>
        Local = 0,
        /// <summary>
        /// UrlNew has an external link.
        /// </summary>
        External = 1,
    }

    /// <summary>
    /// Base class for all tree entries.
    /// </summary>
    public abstract class TreeEntry {

        /// <summary>
        /// Constructor.
        /// </summary>
        public TreeEntry() {
            SubEntries = new List<TreeEntry>();
        }

        /// <summary>
        /// The type of link used by the entry.
        /// </summary>
        [JsonIgnore]
        public LinkTypeEnum LinkType { get; set; }

        /// <summary>
        /// The item's collection of subitems or null if the item has no subitems.
        /// </summary>
        [JsonIgnore]
        public List<TreeEntry> SubEntries { get; set; }

        /// <summary>
        /// Determines whether an item's subsitems are dynamically added/removed.
        /// </summary>
        public bool DynamicSubEntries { get; set; }

        /// <summary>
        /// Determines whether the item should be rendered collapsed (true) or expanded (false).
        /// </summary>
        [JsonIgnore]
        public virtual bool Collapsed { get; set; }

        /// <summary>
        /// Determines whether the item should be rendered as initially selected.
        /// </summary>
        [JsonIgnore]
        public virtual bool Selected { get; set; }

        /// <summary>
        /// The item's displayed text.
        /// </summary>
        [JsonIgnore]
        public virtual string Text { get; set; }

        /// <summary>
        /// Used as the item's target URL, opened in a new window.
        /// </summary>
        [JsonIgnore]
        public virtual string UrlNew { get; set; }
        /// <summary>
        /// Used as the item's target URL, used to replace a content pane or the entire page if no content information is available.
        /// </summary>
        [JsonIgnore]
        public virtual string UrlContent { get; set; }
    }

    /// <summary>
    /// Implementation of the Tree display component.
    /// </summary>
    public partial class TreeDisplayComponent : TreeComponentBase, IYetaWFComponent<object> {

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

            IEnumerable<object> ienum = model as IEnumerable<object>;
            List<TreeEntry> data = (from t in ienum select (TreeEntry)(object)t).ToList();


            //IEnumerator<object> ienumerator = ienum.GetEnumerator();

            string idEmpty = UniqueId();

            TreeSetup setup = GetTreeSetup(treeModel);

            // Headers
            string headerHTML = await GetHeaderAsync(treeModel, data, setup);
            string html = await RenderHTML(HtmlHelper, treeModel, data, setup);

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

        private Task<string> GetHeaderAsync(TreeDefinition treeModel, List<TreeEntry> data, TreeSetup setup) {

            HtmlBuilder hb = new HtmlBuilder();

            if (treeModel.ShowHeader) {

                PropertyData prop = ObjectSupport.GetPropertyData(treeModel.RecordType, nameof(TreeEntry.Text));
                // Caption
                string caption = prop.GetCaption(null);
                // Description
                string description = prop.GetDescription(null);

                string alignCss = "tg_left";

                // Render column header
                hb.Append($@"
    <div class='{alignCss} tg_header{(treeModel.UseSkinFormatting ? " ui-state-default" : "")}' {Basics.CssTooltip}='{HAE(description ?? "")}'>
        <span>{HE(caption)}</span>
    </div>");

            }
            return Task.FromResult(hb.ToString());
        }

        internal async Task<string> RenderHTML(YHtmlHelper htmlHelper,
                TreeDefinition treeModel, List<TreeEntry> data, TreeSetup setup) {

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

                foreach (TreeEntry record in data) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, record));
                }

                hb.Append($@"
</ul>");
            } else {
                // when initially rendering a tree with 0 records, we have to prepare for all templates
                await YetaWFComponentExtender.AddComponentForType(treeModel.RecordType);
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordsHTMLAsync(YHtmlHelper htmlHelper, TreeDefinition treeModel, List<object> records) {

            HtmlBuilder hb = new HtmlBuilder();
            if (records.Count > 0) {

                TreeSetup setup = GetTreeSetup(treeModel);

                hb.Append("<ul>");
                foreach (TreeEntry record in records) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, record));
                }
                hb.Append("</ul>");
            }
            return hb.ToString();
        }

        internal static async Task<string> RenderRecordHTMLAsync(YHtmlHelper htmlHelper, TreeDefinition treeModel, TreeSetup setup, TreeEntry record) {

            HtmlBuilder hb = new HtmlBuilder();

            //bool highlight;
            //ObjectSupport.TryGetPropertyValue<bool>(record, "__highlight", out highlight, false);
            //bool lowlight;
            //ObjectSupport.TryGetPropertyValue<bool>(record, "__lowlight", out lowlight, false);

            string lightCss = "";
            //if (highlight)
            //    lightCss = "tg_highlight";
            //else if (lowlight)
            //    lightCss = "tg_lowlight";

            // check for SubEntriesProperty
            bool collapsed = record.Collapsed;
            bool dynSubs = false;
            List<TreeEntry> items = record.SubEntries;
            if (items == null || items.Count == 0) {
                collapsed = false;
                items = null;
            } else
                dynSubs = record.DynamicSubEntries;

            string urlNew = record.UrlNew;
            string urlContent = record.UrlContent;

            // selected
            bool selected = record.Selected;
            string selectedCss = "";
            if (selected)
                selectedCss = $" {setup.SelectedCss}";

            string caret;
            string icon;
            if (dynSubs || items != null) {
                if (collapsed)
                    caret = "<i class='t_icright'></i>";
                else
                    caret = "<i class='t_icdown'></i>";

                if (string.IsNullOrWhiteSpace(urlNew) && string.IsNullOrWhiteSpace(urlContent))
                    icon = "<i class='t_icfolder'></i>";
                else
                    icon = "<i class='t_icfile'></i>";
            } else {
                caret = "<i class='t_icempty'></i>";
                icon = "<i class='t_icfile'></i>";
            }

            string dd = "";
            if (treeModel.DragDrop)
                dd = " draggable='true'";

            // entry

            string text = await htmlHelper.ForDisplayAsync(record, nameof(TreeEntry.Text));

            if (!string.IsNullOrWhiteSpace(text))
                text = text.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
            if (string.IsNullOrWhiteSpace(text))
                text = "&nbsp;";

            string output;
            if (!string.IsNullOrWhiteSpace(urlNew))
                output = $"<a class='t_entry{selectedCss} yaction-link' target='_blank' href='{HAE(urlNew)}'{dd}>{text}</a>";
            else if (!string.IsNullOrWhiteSpace(urlContent))
                output = $"<a class='t_entry{selectedCss} yaction-link' data-contenttarget='{treeModel.ContentTargetId}' data-contentpane='{treeModel.ContentTargetPane}' href='{HAE(urlContent)}'{dd}>{text}</a>";
            else
                output = $"<a class='t_entry{selectedCss}' data-nohref='true' href='#'{dd}>{text}</a>";

            string recData = "";
            if (treeModel.JSONData) {
                string json = JsonConvert.SerializeObject(record, JsonSettings);
                recData = $" data-record='{HAE(json)}'";
            }

            hb.Append($@"
 <li {recData}>
  {caret}
  {icon}{output}");

            // sub entries
            if (items != null) {

                string collapsedStyle = "";
                if (collapsed)
                    collapsedStyle = " style='display:none'";

                hb.Append($@"
 <ul{collapsedStyle}>");

                foreach (TreeEntry item in items) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, item));
                }

                hb.Append($@"
 </ul>");
            }

            hb.Append($@"
 </li>");
            return hb.ToString();
        }

        internal static TreeSetup GetTreeSetup(TreeDefinition treeModel) {
            TreeSetup setup = new TreeSetup() {
                DragDrop = treeModel.DragDrop,
                HoverCss = treeModel.UseSkinFormatting ? "ui-state-hover" : "tg_hover",
                HighlightCss = treeModel.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                DisabledCss = treeModel.UseSkinFormatting ? "ui-state-disabled" : "tg_disabled",
                RowHighlightCss = treeModel.UseSkinFormatting ? "ui-state-highlight" : "tg_highlight",
                RowDragDropHighlightCss = treeModel.UseSkinFormatting ? "ui-state-active" : "tg_dragdrophighlight",
                SelectedCss = treeModel.UseSkinFormatting ? "ui-state-active" : "t_select",
                ContentTargetId = treeModel.ContentTargetPane,
                ContentTargetPane = treeModel.ContentTargetPane,
                AjaxUrl = treeModel.AjaxUrl,
            };
            return setup;
        }
    }
}


