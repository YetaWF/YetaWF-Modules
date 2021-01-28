/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
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
    /// An instance of the SelectionItem class represents an entry suitable for use with the CheckList template.
    /// </summary>
    public class SelectionCheckListItem {
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
    /// Allows selection of multiple checkboxes from a list using a dropdown menu. The dropdown menu supports optional tooltips.
    /// </summary>
    public class CheckListComponent : YetaWFComponent, IYetaWFComponent<Dictionary<string, bool>> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

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

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(Dictionary<string, bool> model) {

            List<SelectionCheckListItem>? list;
            if (!TryGetSiblingProperty($"{PropertyName}_List", out list))
                list = new List<SelectionCheckListItem>();
            string? svg;
            TryGetSiblingProperty($"{PropertyName}_SVG", out svg);

            HtmlBuilder hb = new HtmlBuilder();
            HtmlBuilder tagHtml = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}'{FieldSetup(Validation ? FieldType.Validated : FieldType.Normal)} class='yt_checklist t_edit{GetClasses()}'>
    <button class='y_buttonlite'>{(svg != null ? SkinSVGs.Get(AreaRegistration.CurrentPackage, svg) : null)}{SkinSVGs.Get(AreaRegistration.CurrentPackage, "fas-caret-down")}</button>");

            int listLength = list!.Count;
            int index = 0;
            foreach (string key in model.Keys) {
                if (index >= listLength)
                    throw new InternalError($"{FieldName}: provided List is smaller than the list of checkboxes");

                SelectionCheckListItem item = list[index];
                bool dis = !item.Enabled;

                hb.Append($@"<input type='hidden' name='{FieldName}[""{key}""]' value='{(model[key] ? "True" : "False")}'>");

                string? t = item.Tooltip?.ToString();
                string desc = string.Empty;
                if (!string.IsNullOrWhiteSpace(t))
                    desc = $" {Basics.CssTooltip}='{HAE(t)}'";

                tagHtml.Append($@"
        <li data-name='{key}'>
            <a href='#' {desc}{(dis ? " class='t_disabled'" : null)}>
                <input type='checkbox' {(model[key] ? " checked" : null)}{(dis ? " disabled='disabled'" : null)}>
                {HE(item.Text.ToString())}
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
