/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the Tree component implementation.
    /// </summary>
    public abstract class TreeComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(TreeComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Tree";

        /// <summary>
        /// Returns the package implementing the component.
        /// </summary>
        /// <returns>Returns the package implementing the component.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the component name.
        /// </summary>
        /// <returns>Returns the component name.</returns>
        /// <remarks>Components in packages whose product name starts with "Component" use the exact name returned by GetTemplateName when used in UIHint attributes. These are considered core components.
        /// Components in other packages use the package's area name as a prefix. E.g., the UserId component in the YetaWF.Identity package is named "YetaWF_Identity_UserId" when used in UIHint attributes.
        ///
        /// The GetTemplateName method returns the component name without area name prefix in all cases.</remarks>
        public override string GetTemplateName() { return TemplateName; }

        internal static TreeSetup GetTreeSetup(TreeDefinition treeModel) {
            TreeSetup setup = new TreeSetup() {
                DragDrop = treeModel.DragDrop,
                ContextMenu = treeModel.ContextMenu,
                HighlightCss = "tg_highlight",
                DisabledCss = "tg_disabled",
                RowHighlightCss = "tg_highlight",
                RowDragDropHighlightCss = "tg_dragdrophighlight",
                SelectedCss = "t_select",
                AjaxUrl = treeModel.AjaxUrl,
            };
            return setup;
        }
    }

    internal class TreeSetup {

        public bool DragDrop { get; set; } // Supports drag & drop
        public bool ContextMenu { get; set; } // Supports context menu

        public string HoverCss { get; set; } = null!;
        public string HighlightCss { get; set; } = null!;
        public string DisabledCss { get; set; } = null!;
        public string RowHighlightCss { get; set; } = null!;
        public string RowDragDropHighlightCss { get; set; } = null!;
        public string SelectedCss { get; set; } = null!;
        public string? AjaxUrl { get; set; } // for dynamic population during expand

        public TreeSetup() { }
    }

    /// <summary>
    /// Displays a tree component. The model defines the tree entries.
    /// </summary>
    /// <example>
    /// [UIHint("Tree"), ReadOnly]
    /// public List&lt;EntryElement&gt; Entries { get; set; }
    /// </example>
    [UsesSibling("_TreeDefinition", "YetaWF.Core.Models.TreeDefinition", "Defines the attributes of the tree component.")]
    public partial class TreeDisplayComponent : TreeComponentBase, IYetaWFComponent<object> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <inheritdoc/>
        public override async Task IncludeStandardDisplayAsync() {
            await Manager.AddOnManager.AddAddOnNamedAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, "github.com.grsmto.simplebar");
            await base.IncludeStandardDisplayAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(object model) {

            HtmlBuilder hb = new HtmlBuilder();

            TreeDefinition? treeModel = GetSiblingProperty<TreeDefinition>($"{FieldName}_TreeDefinition");
            if (treeModel == null) throw new InternalError($"No value provided for {FieldName}_TreeDefinition property");

            IEnumerable<object> ienum = (model as IEnumerable<object>)!;
            List<TreeEntry> data = (from t in ienum select (TreeEntry)(object)t).ToList();


            //IEnumerator<object> ienumerator = ienum.GetEnumerator();

            string idEmpty = UniqueId();

            TreeSetup setup = GetTreeSetup(treeModel);

            // Headers
            string headerHTML = await GetHeaderAsync(treeModel, data, setup);
            string html = await RenderHTML(HtmlHelper, treeModel, data, setup);

            string dd = string.Empty;
            if (treeModel.DragDrop)
                dd = " ondragstart='YetaWF_ComponentsHTML.TreeComponent.onDragStart(event)' ondrop='YetaWF_ComponentsHTML.TreeComponent.onDrop(event)' ondragend='YetaWF_ComponentsHTML.TreeComponent.onDragEnd(event)' ondragover='YetaWF_ComponentsHTML.TreeComponent.onDragOver(event)'";

            string scroll = string.Empty;
            if (treeModel.MiniScroll)
                scroll = " data-simplebar data-simplebar-auto-hide='false'";

            hb.Append($@"
<div id='{treeModel.Id}' class='yt_tree t_display'{dd}{scroll}>
    {headerHTML}
    {html}
</div>");

            Manager.ScriptManager.AddLast($"new YetaWF_ComponentsHTML.TreeComponent('{treeModel.Id}', {Utility.JsonSerialize(setup)});");

            return hb.ToString();
        }

        private Task<string> GetHeaderAsync(TreeDefinition treeModel, List<TreeEntry> data, TreeSetup setup) {

            HtmlBuilder hb = new HtmlBuilder();

            if (treeModel.ShowHeader && treeModel.Header != null) {

                PropertyData prop = ObjectSupport.GetPropertyData(treeModel.RecordType, nameof(TreeEntry.Text));
                // Caption
                string? caption = treeModel.Header.ToString();
                if (string.IsNullOrWhiteSpace(caption))
                    caption = prop.GetCaption(null);
                // Description
                string? description = null;
                if (treeModel.HeaderTooltip != null) {
                    description = treeModel.HeaderTooltip.ToString();
                    if (string.IsNullOrWhiteSpace(description))
                        description = prop.GetDescription(null);
                }

                string alignCss = "tg_left";

                // Render column header
                hb.Append($@"
    <div class='{alignCss} tg_header'>
        <span {Basics.CssTooltipSpan}='{HAE(description ?? "")}'>{HE(caption)}</span>
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

            hb.Append($@"
<ul class='tg_root t_sub'>");

            if (data != null && data.Count > 0) {
                foreach (TreeEntry record in data) {
                    hb.Append(await RenderRecordHTMLAsync(htmlHelper, treeModel, setup, record));
                }
            }

            hb.Append($@"
</ul>");

            // when initially rendering a tree with 0 records, we have to prepare for all templates
            if (data != null && data.Count > 0) {
                await YetaWFComponentExtender.AddComponentSupportForType(treeModel.RecordType);
            }

            return hb.ToString();
        }

        internal static async Task<string> RenderRecordsHTMLAsync(YHtmlHelper htmlHelper, TreeDefinition treeModel, List<object> records) {

            HtmlBuilder hb = new HtmlBuilder();
            if (records.Count > 0) {

                TreeSetup setup = GetTreeSetup(treeModel);

                hb.Append("<ul class='t_sub'>");
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

            //string lightCss = "";
            //if (highlight)
            //    lightCss = "tg_highlight";
            //else if (lowlight)
            //    lightCss = "tg_lowlight";

            // check for SubEntriesProperty
            bool collapsed = record.Collapsed;
            bool dynSubs = false;
            List<TreeEntry>? items = record.SubEntries;
            if (items == null || items.Count == 0) {
                items = null;
                if (record.DynamicSubEntries) {
                    dynSubs = record.DynamicSubEntries;
                } else {
                    collapsed = false;
                }
            }

            string? urlNew = record.UrlNew;
            string? urlContent = record.UrlContent;

            // selected
            bool selected = record.Selected;
            string selectedCss = "", selectedLICss = "";
            if (selected) {
                selectedCss = $" {setup.SelectedCss}";
                selectedLICss = " class='t_select'";
            }
            string extraCss = "";
            if (record.ExtraCss != null)
                extraCss = $" {record.ExtraCss}";

            string caret;
            string icon;
            if (dynSubs || items != null) {
                if (collapsed)
                    caret = "<i class='t_icright'>&nbsp;</i>";
                else
                    caret = "<i class='t_icdown'>&nbsp;</i>";

                if (string.IsNullOrWhiteSpace(urlNew) && string.IsNullOrWhiteSpace(urlContent))
                    icon = "<i class='t_icfolder'>&nbsp;</i>";
                else
                    icon = "<i class='t_icfile'>&nbsp;</i>";
            } else {
                caret = "<i class='t_icempty'>&nbsp;</i>";
                icon = "<i class='t_icfile'>&nbsp;</i>";
            }

            string dd = "";
            if (treeModel.DragDrop)
                dd = " draggable='true'";

            // entry

            string text = await htmlHelper.ForDisplayAsync(record, nameof(TreeEntry.Text));
            string? beforeText = null;
            if (record.BeforeText != null)
                beforeText = await htmlHelper.ForDisplayAsync(record, nameof(TreeEntry.BeforeText));
            string? afterText = null;
            if (record.AfterText != null)
                afterText = await htmlHelper.ForDisplayAsync(record, nameof(TreeEntry.AfterText));

            if (!string.IsNullOrWhiteSpace(text))
                text = text.Trim(new char[] { '\r', '\n' }); // templates can generate a lot of extra \r\n which breaks filtering
            if (string.IsNullOrWhiteSpace(text))
                text = "&nbsp;";

            string output;
            if (!string.IsNullOrWhiteSpace(urlNew))
                output = $"<a class='t_entry{selectedCss}{extraCss} yaction-link' target='_blank' href='{HAE(urlNew)}'{dd}>{text}</a>";
            else if (!string.IsNullOrWhiteSpace(urlContent))
                output = $"<a class='t_entry{selectedCss}{extraCss} yaction-link' href='{HAE(urlContent)}'{dd}>{text}</a>";
            else
                output = $"<a class='t_entry yaction-link{selectedCss}{extraCss}' data-nohref='true' href='#'{dd}>{text}</a>";

            string recData = "";
            if (treeModel.JSONData) {
                string json = Utility.JsonSerialize(record, Utility._JsonSettingsGetSet);
                recData = $" data-record='{HAE(json)}'";
            }

            hb.Append($@"
 <li {recData}{selectedLICss}>
  {caret}
  {icon}{beforeText}{output}{afterText}");

            // sub entries
            if (items != null) {

                string collapsedStyle = "";
                if (collapsed)
                    collapsedStyle = " style='display:none'";

                hb.Append($@"
 <ul{collapsedStyle} class='t_sub'>");

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
    }
}


