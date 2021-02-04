/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

#nullable enable

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// An instance of the SelectionCheckListDetail class represents the detail information for a SelectionCheckListEntry for use with the CheckList template and CheckListPanel template.
    /// </summary>
    public class SelectionCheckListDetail {
        /// <summary>
        /// The key value.
        /// </summary>
        public string Key { get; set; } = null!;
        /// <summary>
        /// The text displayed in the dropdown menu for the entry.
        /// </summary> 
        public MultiString Text { get; set; } = null!;
        /// <summary>
        /// The tooltip displayed in the CheckList for the entry.
        /// </summary>
        public MultiString? Tooltip { get; set; }
        /// <summary>
        /// Defines whether the entry is enabled so it can be checked/unchecked.
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// An instance of the SelectionItem class represents an entry suitable for use with the CheckList template and CheckListPanel template.
    /// </summary>
    public class SelectionCheckListEntry {
        /// <summary>
        /// The key value.
        /// </summary>
        public string Key { get; set; } = null!;
        /// <summary>
        /// The selection status (checked/unchecked).
        /// </summary>
        public bool Value { get; set; }
    }

    /// <summary>
    /// Allows selection of multiple checkboxes from a list using a dropdown menu. The dropdown menu supports optional tooltips.
    /// </summary>
    public class CheckListComponent : YetaWFComponent, IYetaWFComponent<List<SelectionCheckListEntry>> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        internal string TemplateName { get { return "CheckList"; } }

        internal class CheckListSetup {
            public string FieldName { get; set; } = null!;
        }

        /// <inheritdoc/>
        public override async Task IncludeAsync() {
            // Add required menu support
            await Manager.AddOnManager.AddTemplateAsync(YetaWF.Modules.ComponentsHTML.AreaRegistration.CurrentPackage.AreaName, "MenuUL", ComponentType.Display);
            await base.IncludeAsync();
        }

        /// <inheritdoc/>
        public Task<string> RenderAsync(List<SelectionCheckListEntry> model) {

            List<SelectionCheckListDetail>? details;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out details))
                details = new List<SelectionCheckListDetail>();

            string? svg;
            TryGetSiblingProperty($"{PropertyName}_DDSVG", out svg);

            HtmlBuilder hb = new HtmlBuilder();
            HtmlBuilder tagHtml = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} class='yt_checklist t_edit{GetClasses()}'>
    <button class='y_buttonlite'>{(svg != null ? SkinSVGs.Get(AreaRegistration.CurrentPackage, svg) : null)}{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-caret-down")}</button>");

            int listLength = details!.Count;
            int index = 0;
            foreach (SelectionCheckListEntry entry in model) {
                if (index >= listLength)
                    throw new InternalError($"{FieldName}: provided List is smaller than the detail list of checkboxes");

                SelectionCheckListDetail? detail = (from l in details where l.Key == entry.Key select l).FirstOrDefault();
                if (detail == null)
                    throw new InternalError($"Missing detail entry in {FieldName} for key {entry.Key}");
                bool dis = !detail.Enabled;

                hb.Append($@"<input type='hidden' name='{FieldName}[{index}].Key' value='{entry.Key}'>");
                hb.Append($@"<input type='hidden' name='{FieldName}[{index}].Value' data-name='{entry.Key}' value='{(entry.Value ? "True" : "False")}'>");

                string? t = detail.Tooltip?.ToString();
                string desc = string.Empty;
                if (!string.IsNullOrWhiteSpace(t))
                    desc = $" {Basics.CssTooltip}='{HAE(t)}'";

                tagHtml.Append($@"
        <li data-name='{entry.Key}'>
            <a href='#' {desc}{(dis ? " class='t_disabled'" : null)}>
                <input type='checkbox' {(entry.Value ? " checked" : null)}{(dis ? " disabled='disabled'" : null)}>
                {HE(detail.Text.ToString())}
            </a>
        </li>");

                ++index;
            }

            hb.Append($@"
    <ul id='{Manager.UniqueId()}' style='display:none'>
        {tagHtml.ToString()}
    </ul>
</div>");

            CheckListSetup setup = new CheckListSetup {
                FieldName = FieldName,
            };
            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.CheckListEditComponent('{ControlId}', {Utility.JsonSerialize(setup)});");

            return Task.FromResult(hb.ToString());
        }
    }
}
