/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using System.Linq;
using System.Collections.Generic;
using YetaWF.Core.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <inheritdoc/>
    public class DropDownSearchComponent : DropDownSearchEditComponentBase<string> {

        /// <summary>
        /// The template name of the component.
        /// </summary>
        public const string TemplateName = "DropDownSearch";

        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownSearchComponent() : base(TemplateName) { }
    }

    /// <inheritdoc/>
    public class DropDownSearchIntComponent : DropDownSearchEditComponentBase<int?> {

        /// <summary>
        /// The template name of the component.
        /// </summary>
        public const string TemplateName = "DropDownSearchInt";

        /// <summary>
        /// Constructor.
        /// </summary>
        public DropDownSearchIntComponent() : base(TemplateName) { }
    }

    /// <summary>
    /// Allows entry and selection of a value from a list of values using a dropdown. The dropdown supports optional tooltips.
    /// As entries are typed, the dropdown is updated with matching entries, which can be selected from the list.
    /// </summary>
    [UsesSibling("_List", "List<YetaWF.Modules.ComponentsHTML.Components.DropDownSearchComponent.SelectionItem<string>>", "Defines the list of values, displayed text and tooltips shown in the dropdown list.")]
    [UsesSibling("_AjaxUrl", "string", "Defines the URL used to retrieve data for the dropdown search component.")]
    [UsesAdditional("DropDownWidthFactor", "float", "1.0", "Defines the width of the dropdown portion relative to the control width. The control width is multiplied by the factor DropDownWidthFactor to calculate the width. If 1.0 is specified, the dropdown portion is the same size as the control.  If 2.0 is specified, the dropdown portion is the twice the size of the control.")]
    [UsesAdditional("DropDownHeightFactor", "float", "1.0", "Defines the height of the dropdown portion relative to 200 pixels. The control width is multiplied by the factor DropDownHeightFactor to calculate the height. If 1.0 is specified, the dropdown portion is 200 pixels.  If 2.0 is specified, the dropdown portion is the 400 pixels in height.")]
    public abstract class DropDownSearchEditComponentBase<TYPE> : YetaWFComponent, IYetaWFComponent<TYPE> {

        /// <inheritdoc/>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }
        /// <inheritdoc/>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <inheritdoc/>
        public override string GetTemplateName() { return TemplateName; }

        private string TemplateName { get; set; }

        internal class Setup {
            public string AjaxUrl { get; set; } = null!;
            public bool AdjustWidth { get; set; }
            public float DropDownWidthFactor { get; set; }
            public float DropDownHeightFactor { get; set; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        public DropDownSearchEditComponentBase(string templateName) {
            TemplateName = templateName;
        }

        /// <summary>
        /// Adds all addons for the DropDownSearch component to the current page.
        /// </summary>
        public static async Task IncludeExplicitAsync() { // this component is reusable so we need to explicitly include all js/css
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, "Text", ComponentType.Edit);// input is uses Text css
            await Manager.AddOnManager.AddTemplateFromUIHintAsync(null, DropDownSearchComponent.TemplateName, ComponentType.Edit);
        }
        /// <inheritdoc/>
        public Task<string> RenderAsync(TYPE model) {
            if (!TryGetSiblingProperty($"{PropertyName}_AjaxUrl", out string? url))
                throw new InternalError($"No {PropertyName}_AjaxUrl property found");
            if (!TryGetSiblingProperty($"{PropertyName}_Text", out string? text))
                throw new InternalError($"No {PropertyName}_Text property found");
            return RenderDropDownSearchAsync(this, model, text, url!, "yt_dropdownsearch");
        }

        /// <summary>
        /// Renders a DropDownSearch component as HTML.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="model">The model rendered by the component.</param>
        /// <param name="text">The displayable text representing the model's value.</param>
        /// <param name="url">A URL used to retrieve entries matching the current input.</param>
        /// <param name="cssClass">A CSS class to add to the &lt;select&gt; tag. May be null.</param>
        /// <returns></returns>
        public static async Task<string> RenderDropDownSearchAsync(YetaWFComponent component, TYPE model, string? text, string url, string cssClass) {

            await IncludeExplicitAsync();

            if (component.PropData.TryGetAdditionalAttributeValue<string>("PlaceHolder", out string? placeHolder)) {
                placeHolder = $" placeholder='{HAE(placeHolder)}'";
            }

            Setup setup = new Setup {
                AjaxUrl = url,
                AdjustWidth = component.PropData.GetAdditionalAttributeValue("AdjustWidth", true),
                DropDownWidthFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownWidthFactor", 1.0),
                DropDownHeightFactor = (float)component.PropData.GetAdditionalAttributeValue("DropDownHeightFactor", 1.0),
            };

            string tags = $@"
<div id='{component.DivId}' class='yt_dropdownsearch_base t_edit' unselectable='on' role='listbox' aria-haspopup='true' aria-expanded='false' aria-owns='yDDPopup' aria-live='polite' aria-busy='false' aria-activedescendant='{Guid.NewGuid().ToString()}'>
    <input{component.FieldSetup(component.Validation ? FieldType.Validated : FieldType.Normal)} id='{component.ControlId}' type='hidden' value='{HAE(model?.ToString() ?? string.Empty)}'>
    <input type='text' value='{HAE(text ?? string.Empty)}' class='yt_text_base t_edit' autocomplete='off'{placeHolder}>
</div>";

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.DropDownSearchEditComponent('{component.DivId}', {Utility.JsonSerialize(setup)});");

            return tags;
        }
    }

    /// <summary>
    /// Returns a selection list suitable for client-side rendering.
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    public class DropDownSearchResult<TYPE> : YJsonResult {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="list">The list of available selections.</param>
        public DropDownSearchResult(List<SelectionItem<TYPE>> list) {
            Data = (from l in list select new { Text = l.Text?.ToString(), Tooltip = l.Tooltip?.ToString(), Value = l.Value }).ToList();
        }
    }
}
