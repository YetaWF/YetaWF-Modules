/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// An instance of this class contains markdown text and HTML used with the Markdown component.
    /// </summary>
    [TypeConverter(typeof(MarkdownStringBaseConv))]
    [JsonConverter(typeof(NoTypeConverterJsonConverter<MarkdownStringBase>))]
    public class MarkdownStringBase {
        /// <summary>
        /// The markdown text.
        /// </summary>
        [Caption("Markdown Text"), HelpLink("https://www.markdownguide.org/cheat-sheet/")]
        [UIHint("TextAreaSourceOnly")]
        public virtual string? Text { get; set; } = null!;
        /// <summary>
        /// The HTML rendering of the markdown text.
        /// </summary>
        public virtual string? HTML { get; set; } = null!;
        /// <summary>
        /// Implicit conversion to string.
        /// </summary>
        /// <param name="md">The MarkdownStringBase object to convert.</param>
        public static implicit operator string?(MarkdownStringBase? md) {
            return md?.Text;
        }
    }
    /// <summary>
    /// Type converted implementation for MarkdownStringBase mainly to support conversion to type System.String for validation.
    /// </summary>
    public class MarkdownStringBaseConv : TypeConverter {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A System.Type that represents the type you want to convert to.</param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
            if (destinationType == typeof(string))
                return true;
            //else if (destinationType == typeof(EditorDefinition))
            //    return true;
            return base.CanConvertTo(context, destinationType);
        }
        /// <summary>
        /// Converts the given value object to the specified type, using the arguments.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns> An System.Object that represents the converted value.</returns>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
            if (destinationType == typeof(string))
                return ((string?)(MarkdownStringBase?)value) ?? string.Empty;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Base class for the Markdown component implementation.
    /// </summary>
    public abstract class MarkdownComponentBase : YetaWFComponent {

        internal static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(MarkdownComponentBase), name, defaultValue, parms); }

        internal const string TemplateName = "Markdown";

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
    }

    /// <summary>
    /// Displays the HTML contents of the model. If the model is null, nothing is rendered.
    /// </summary>
    /// <example>
    /// [Caption("Contents"), Description("Shows the contents")]
    /// [UIHint("Markdown"), ReadOnly]
    /// public MarkdownStringBase Contents { get; set; }
    /// </example>
    [UsesAdditional("EmHeight", "int", "10", "Defines the height of the markdown component in lines of text.")]
    public class MarkdownDisplayComponent : MarkdownComponentBase, IYetaWFComponent<MarkdownStringBase> {

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
        public Task<string> RenderAsync(MarkdownStringBase model) {

            HtmlBuilder hb = new HtmlBuilder();

            if (model != null && !string.IsNullOrWhiteSpace(model.HTML)) {
                hb.Append($@"
<div id='{ControlId}' class='yt_markdown t_display'>
    {model.HTML}
</div>");
            }
            return Task.FromResult(hb.ToString());
        }
    }

    /// <summary>
    /// Allows entry of markdown text with preview.  The model defines various attributes of the component.
    /// </summary>
    /// <example>
    /// [Category("General"), Caption("Contents"), Description("The text contents")]
    /// [UIHint("Markdown"), AdditionalMetadata("EmHeight", 10)]
    /// public MarkdownString Contents { get; set; }
    ///
    /// public class MarkdownString : MarkdownStringBase {
    ///     [StringLength(0), AdditionalMetadata("EmHeight", 10)]
    ///     public override string Text { get { return base.Text; } set { base.Text = value; } }
    ///     [StringLength(0)]
    ///     public override string HTML { get { return base.HTML; } set { base.HTML = value; } }
    /// }
    /// </example>
    public class MarkdownEditComponent : MarkdownComponentBase, IYetaWFComponent<MarkdownStringBase> {

        /// <summary>
        /// Returns the component type (edit/display).
        /// </summary>
        /// <returns>Returns the component type.</returns>
        public override ComponentType GetComponentType() { return ComponentType.Edit; }

        /// <summary>
        /// Called by the framework when the component is used so the component can add component specific addons.
        /// </summary>
        public override async Task IncludeAsync() {
            await base.IncludeAsync();
        }

        internal class UI {
            [UIHint("Tabs")]
            public TabsDefinition TabsDef { get; set; } = null!;
        }

        /// <summary>
        /// Called by the framework when the component needs to be rendered as HTML.
        /// </summary>
        /// <param name="model">The model being rendered by the component.</param>
        /// <returns>The component rendered as HTML.</returns>
        public async Task<string> RenderAsync(MarkdownStringBase model) {

            await Manager.AddOnManager.AddAddOnNamedAsync(Package.AreaName, "showdownjs.com");

            int emHeight = PropData.GetAdditionalAttributeValue("EmHeight", 10);

            UI ui = new UI {
                TabsDef = new TabsDefinition {
                    Tabs = new List<TabEntry> {
                        new TabEntry {
                            Caption = __ResStr("edit", "Edit"),
                            ToolTip = null,
                            PaneCssClasses = "t_edit",
                            RenderPaneAsync = async (int tabIndex) => {
                                HtmlBuilder hbt = new HtmlBuilder();
                                using (Manager.StartNestedComponent(FieldName)) {
                                    hbt.Append($@"
        {await HtmlHelper.ForLabelAsync(model, nameof(model.Text))}
        {await HtmlHelper.ForEditAsync(model, nameof(model.Text))}");
                                }
                                return hbt.ToString();
                            }
                        },
                        new TabEntry {
                            Caption = __ResStr("preview", "Preview"),
                            ToolTip = null,
                            PaneCssClasses = "t_preview",
                            RenderPaneAsync = async (int tabIndex) => {
                                HtmlBuilder hbt = new HtmlBuilder();
                                using (Manager.StartNestedComponent(FieldName)) {
                                    hbt.Append($@"
        {await HtmlHelper.ForDisplayComponentAsync(model, nameof(model.HTML), model.HTML, "Hidden", HtmlAttributes: new { __NoTemplate = true, @class = "t_html" })}
        <div class='t_previewpane' style='min-height:{emHeight}em'>{model.HTML}</div>");
                                }
                                return hbt.ToString();
                            }
                        }
                    }
                }
            };

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
<div id='{ControlId}' class='yt_markdown t_edit'>
    {await HtmlHelper.ForDisplayAsync(ui, nameof(ui.TabsDef), HtmlAttributes: new { __NoTemplate = true })}
</div>");

            using (Manager.StartNestedComponent(FieldName)) {
                hb.Append($@"
{ValidationMessage(nameof(model.Text))}");// Validatation is on field.Text not main div
            }

            Manager.ScriptManager.AddLast($@"new YetaWF_ComponentsHTML.MarkdownEditComponent('{ControlId}');");

            return hb.ToString();
        }
    }
}
