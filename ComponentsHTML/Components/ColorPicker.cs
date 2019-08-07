/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// Base class for the ColorPicker component implementation.
    /// </summary>
    public abstract class ColorPickerComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ColorPickerComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "ColorPicker";

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

    /// <summary>
    /// Implementation of the ColorPicker display component.
    /// </summary>
    public class ColorPickerDisplayComponent : ColorPickerComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Display; }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(string model) {

            HtmlBuilder hb = new HtmlBuilder();

            int tileSize = PropData.GetAdditionalAttributeValue("TileSize", 24);
            string palette = PropData.GetAdditionalAttributeValue("Palette", "basic");
            int columns = PropData.GetAdditionalAttributeValue("Columns", 6);
            bool preview = PropData.GetAdditionalAttributeValue("Preview", true);

            YTagBuilder tag = new YTagBuilder("div");
            tag.AddCssClass("yt_colorpicker");
            tag.AddCssClass("t_display");
            FieldSetup(tag, FieldType.Anonymous);
            string style = $"width:{tileSize}px;height:{tileSize}px";
            if (model != null)
                style += $";background-color:{model}";
            tag.MergeAttribute("style", style);
            hb.Append(tag.ToString(YTagRenderMode.Normal));

            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Implementation of the ColorPicker edit component.
    /// </summary>
    public class ColorPickerEditComponent : ColorPickerComponentBase, IYetaWFComponent<string> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await KendoUICore.AddFileAsync("kendo.color.min.js");
            //await KendoUICore.AddFileAsync("kendo.popup.min.js"); // is now a prereq of kendo.window (2017.2.621)
            await KendoUICore.AddFileAsync("kendo.slider.min.js");
            await KendoUICore.AddFileAsync("kendo.button.min.js");
            await KendoUICore.AddFileAsync("kendo.colorpicker.min.js");
            await base.IncludeAsync();
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public Task<string> RenderAsync(string model) {
            HtmlBuilder hb = new HtmlBuilder();

            int tileSize = PropData.GetAdditionalAttributeValue("TileSize", 16);
            string palette = PropData.GetAdditionalAttributeValue("Palette", "basic");
            int columns = PropData.GetAdditionalAttributeValue("Columns", 6);
            bool preview = PropData.GetAdditionalAttributeValue("Preview", true);

            YTagBuilder tag = new YTagBuilder("input");
            tag.AddCssClass("yt_colorpicker");
            tag.AddCssClass("t_edit");
            FieldSetup(tag, Validation ? FieldType.Validated : FieldType.Normal);
            tag.MergeAttribute("id", ControlId);
            if (model != null)
                tag.MergeAttribute("value", model);
            hb.Append(tag.ToString(YTagRenderMode.StartTag));

            ScriptBuilder sb = new ScriptBuilder();
            sb.Append($@"(new YetaWF_ComponentsHTML.ColorPickerEditComponent('{ControlId}', {{
                palette: '{JE(palette)}',
                tileSize: {tileSize},
                preview: {JE(preview)},
                messages: {{
                    previewInput: '{JE(__ResStr("editColor", "Edit Color using hex values or names"))}',
                    cancel: '{JE(__ResStr("cancel", "Cancel"))}',
                    apply: '{JE(__ResStr("apply", "Apply"))}'
                }}
            }}));");

            Manager.ScriptManager.AddLast(sb.ToString());

            return Task.FromResult(hb.ToString());
        }
    }
}
